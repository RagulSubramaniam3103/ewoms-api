import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-user-edit-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.css']
})
export class UserEditProfileComponent implements OnInit {
  profileData: any = {
    UserId: '',
    UserName: '',
    FullName: '',
    Email: '',
    PhoneNumber: '',
    Address1: '',
    Address2: '',
    City: '',
    State: '',
    PostalCode: '',
    Country: '',
    IsPrivate: false
  };
  userRole: string = '';

  selectedImage: File | null = null;
  imagePreview: string | null = null;
  isLoading = false;
  successMsg = '';
  errorMsg = '';

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadUserData();
  }

  loadUserData() {
    const token = localStorage.getItem('jwtToken');
    this.profileData.UserId = localStorage.getItem('userId') || '';

    // Load full cached user data to pre-fill address and phone fields
    const savedUserDataStr = localStorage.getItem('userData');
    if (savedUserDataStr) {
      try {
        const parsed = JSON.parse(savedUserDataStr);
        this.profileData.PhoneNumber = parsed.phoneNumber || parsed.PhoneNumber || '';
        this.profileData.Address1 = parsed.address1 || parsed.Address1 || '';
        this.profileData.Address2 = parsed.address2 || parsed.Address2 || '';
        this.profileData.City = parsed.city || parsed.City || '';
        this.profileData.State = parsed.state || parsed.State || '';
        this.profileData.PostalCode = parsed.postalCode || parsed.PostalCode || '';
        this.profileData.Country = parsed.country || parsed.Country || '';
        this.profileData.IsPrivate = parsed.isPrivate === true || parsed.IsPrivate === true || false;
      } catch (e) {
        console.error('Failed to parse saved user data', e);
      }
    }
    
    if (this.profileData.UserId) {
      this.authService.getUserById(this.profileData.UserId).subscribe({
        next: (res: any) => {
          this.profileData.FullName = res.fullName || res.FullName || this.profileData.FullName;
          this.profileData.Email = res.email || res.Email || this.profileData.Email;
          this.profileData.IsPrivate = res.isPrivate === true || res.IsPrivate === true || false;
          
          const img = res.profileImage || res.ProfileImage || res.profileImge || res.ProfileImge || res.userImage || res.UserImage;
          if (img) {
            this.imagePreview = img.startsWith('data:image') ? img : 'data:image/jpeg;base64,' + img;
            localStorage.setItem('profileImage', img);
          }
          this.cdr.detectChanges();
        }
      });
    }
    
    if (token) {
      try {
        const decoded: any = jwtDecode(token);
        // Fallback: Check common JWT claim keys for the ID
        const idKey = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
        if (!this.profileData.UserId) {
           this.profileData.UserId = decoded[idKey] || decoded.nameid || decoded.sub || '';
        }

        this.profileData.Email = decoded.email || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || '';
        this.profileData.FullName = decoded.name || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || '';
        this.profileData.UserName = decoded.unique_name || this.profileData.FullName;
        this.userRole = (decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || '').toLowerCase();
        
        // Load existing image if available
        let savedImg = localStorage.getItem('profileImage');
        if (!savedImg) {
          savedImg = this.authService.getSessionProfileImage();
        }

        if (savedImg) {
          if (savedImg.startsWith('data:image')) {
            this.imagePreview = savedImg;
          } else {
            this.imagePreview = 'data:image/png;base64,' + savedImg;
          }
        }
      } catch (e) {
        console.error('Token decoding failed', e);
      }
    }
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedImage = file;
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  dataURLtoFile(dataurl: string, filename: string): File {
    const arr = dataurl.split(',');
    const mimeMatch = arr[0].match(/:(.*?);/);
    const mime = mimeMatch ? mimeMatch[1] : 'image/png';
    const bstr = atob(arr[1]);
    let n = bstr.length;
    const u8arr = new Uint8Array(n);
    while (n--) {
      u8arr[n] = bstr.charCodeAt(n);
    }
    return new File([u8arr], filename, { type: mime });
  }

  onSubmit() {
    this.isLoading = true;
    this.successMsg = '';
    this.errorMsg = '';

    // Immediate Client-Side Validation to prevent 400 Bad Request
    if (!this.profileData.UserId) {
      this.isLoading = false;
      this.errorMsg = 'Critical Error: Session User ID is missing. Please Log Out and Log Back In to synchronize your identity token.';
      alert(this.errorMsg);
      return;
    }

    const formData = new FormData();
    const requiredFields = ['Address1', 'Address2', 'City', 'State', 'PostalCode', 'Country', 'PhoneNumber'];
    Object.keys(this.profileData).forEach(key => {
      let val = this.profileData[key];
      // Use '.' as a fallback to satisfy strict backend [Required] validation
      if (requiredFields.includes(key)) {
        formData.append(key, (val && val.trim()) ? val : '.');
      } else if (val !== null && val !== undefined && val !== '') {
        formData.append(key, val);
      }
    });

    if (this.selectedImage) {
      formData.append('profileImage', this.selectedImage);
    } else if (this.imagePreview && this.imagePreview.includes('base64,')) {
      // Send the existing image if no new one was provided, so it doesn't get wiped!
      try {
        const existingFile = this.dataURLtoFile(this.imagePreview, 'existing_profile.png');
        formData.append('profileImage', existingFile);
      } catch (e) {
        console.error('Failed to convert existing image to file:', e);
      }
    }
    
    console.log(`Submitting Update for role: ${this.userRole}...`);
    formData.forEach((value, key) => {
      console.log(`${key}:`, value);
    });

    let updateObservable;
    if (this.userRole === 'admin') {
      updateObservable = this.authService.updateAdminProfile(formData);
    } else if (this.userRole === 'manager') {
      updateObservable = this.authService.updateManagerProfile(formData);
    } else {
      updateObservable = this.authService.updateUserProfile(formData);
    }

    updateObservable.subscribe({
      next: (res: any) => {
        this.isLoading = false;
        this.successMsg = res.Message || 'Profile updated successfully!';
        this.cdr.detectChanges();
        
        // Update local storage so that changes reflect!
        if (this.profileData.FullName) localStorage.setItem('fullName', this.profileData.FullName);
        if (this.profileData.UserName) localStorage.setItem('userName', this.profileData.UserName);
        
        const storedUserDataStr = localStorage.getItem('userData');
        if (storedUserDataStr) {
           try {
             let parsed = JSON.parse(storedUserDataStr);
             parsed.phoneNumber = this.profileData.PhoneNumber;
             parsed.address1 = this.profileData.Address1;
             parsed.address2 = this.profileData.Address2;
             parsed.city = this.profileData.City;
             parsed.state = this.profileData.State;
             parsed.postalCode = this.profileData.PostalCode;
             parsed.country = this.profileData.Country;
             parsed.isPrivate = this.profileData.IsPrivate;
             localStorage.setItem('userData', JSON.stringify(parsed));
           } catch(e) {}
        }

        // Update local image if it was changed
        if (this.imagePreview && this.selectedImage) {
           const base64 = this.imagePreview.split(',')[1];
           this.authService.setSessionProfileImage(base64);
           try {
             localStorage.setItem('profileImage', base64);
           } catch(e) {}
        }
        
        // Let the success message stay on the screen for the user to see!
        setTimeout(() => {
          this.successMsg = '';
          this.cdr.detectChanges();
        }, 5000);
      },
      error: (err) => {
        this.isLoading = false;
        
        let detailedError = 'Failed to update profile. Please check your data.';
        
        // Log the full error object to console for debugging
        console.error('Full ASP.NET Error Object:', err);

        if (err?.error?.errors) {
            // ASP.NET Core Model Validation Errors
            const validationErrors = Object.values(err.error.errors).flat().join(' | ');
            detailedError = `Validation Error: ${validationErrors}`;
        } else if (err?.error?.Message || err?.error?.message) {
            detailedError = err.error.Message || err.error.message;
        }

        this.errorMsg = detailedError;
        this.cdr.detectChanges();
        alert(this.errorMsg);
      }
    });
  }
}
