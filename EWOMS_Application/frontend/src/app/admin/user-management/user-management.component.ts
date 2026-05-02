import { Component, OnInit, ChangeDetectorRef, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  @Input() defaultTab: string = 'User';
  
  users: any[] = [];
  filteredUsers: any[] = [];
  paginatedUsers: any[] = [];

  selectedRole: string = 'User';
  isLoading: boolean = false;
  errorMessage: string = '';

  // DataGrid State
  searchTerm: string = '';
  currentPage: number = 1;
  pageSize: number = 5;
  totalPages: number = 1;

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.selectedRole = this.defaultTab;
    this.fetchUsers();
  }

  fetchUsers(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    if (this.selectedRole === 'Deleted') {
      this.authService.getDeletedUsers().subscribe({
        next: (response: any) => {
          let data = response?.value || response?.data || response;
          if (data && typeof data === 'object' && data.result) {
            data = data.result;
          }
          this.users = Array.isArray(data) ? data : (data ? [data] : []);
          this.applyFilters();
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.errorMessage = 'Failed to load deleted records.';
          this.isLoading = false;
          this.cdr.detectChanges();
        }
      });
      return;
    }

    this.authService.getUsersByRole(this.selectedRole).subscribe({
      next: (response) => {
        console.log('API Response for GetUsersByRole:', response);
        try {
          // Sometimes wrapped in response.value or response.data depending on interceptors/API
          let data = response?.value || response?.data || response;
          if (data && typeof data === 'object' && data.result) {
            data = data.result;
          }

          this.users = Array.isArray(data) ? data : (data ? [data] : []);
          this.applyFilters();
        } catch (e) {
          console.error("Error processing grid data:", e);
        } finally {
          this.isLoading = false;
          this.cdr.detectChanges(); // Force angular to un-spin!
        }
      },
      error: (err) => {
        this.errorMessage = 'Failed to load users. Please ensure you have appropriate administrative privileges.';
        this.isLoading = false;
        this.cdr.detectChanges();
        console.error(err);
      }
    });
  }

  onRoleChange(newRole: string): void {
    this.selectedRole = newRole;
    this.searchTerm = '';
    this.currentPage = 1;
    this.fetchUsers();
  }

  // --- Grid Logic ---

  applyFilters(): void {
    let temp = this.users;

    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      temp = temp.filter(u =>
        (u.userName && u.userName.toLowerCase().includes(term)) ||
        (u.email && u.email.toLowerCase().includes(term)) ||
        (u.id && u.id.toLowerCase().includes(term))
      );
    }

    this.filteredUsers = temp;
    this.totalPages = Math.ceil(this.filteredUsers.length / this.pageSize) || 1;

    if (this.currentPage > this.totalPages) {
      this.currentPage = this.totalPages;
    }

    this.updatePagination();
  }

  updatePagination(): void {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    this.paginatedUsers = this.filteredUsers.slice(startIndex, startIndex + this.pageSize);
  }

  onSearchChange(): void {
    this.currentPage = 1;
    this.applyFilters();
  }

  onPageSizeChange(event: any): void {
    this.pageSize = Number(event.target.value);
    this.currentPage = 1;
    this.applyFilters();
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.updatePagination();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.updatePagination();
    }
  }

  // --- Update Logic ---
  isUpdateModalOpen = false;
  selectedUserData: any = {
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
    Country: ''
  };
  selectedImage: File | null = null;
  imagePreview: string | null = null;
  isSaving = false;

  openUpdateModal(user: any): void {
    console.log('Opening update for user:', user);
    this.selectedUserData = {
      UserId: user.id || user.UserId || '',
      UserName: user.userName || user.username || '',
      FullName: user.fullName || '',
      Email: user.email || '',
      PhoneNumber: user.phoneNumber || '',
      Address1: user.address1 || '',
      Address2: user.address2 || '',
      City: user.city || '',
      State: user.state || '',
      PostalCode: user.postalCode || '',
      Country: user.country || ''
    };
    this.imagePreview = null; // Reset image preview
    this.selectedImage = null;
    this.isUpdateModalOpen = true;
  }

  closeUpdateModal(): void {
    this.isUpdateModalOpen = false;
  }

  onFileSelected(event: any): void {
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

  submitUpdate(): void {
    this.isSaving = true;
    const formData = new FormData();
    Object.keys(this.selectedUserData).forEach(key => {
      formData.append(key, this.selectedUserData[key]);
    });

    if (this.selectedImage) {
      formData.append('profileImage', this.selectedImage);
    }

    this.authService.updateAdminProfile(formData).subscribe({
      next: (res) => {
        alert(res.Message || 'User updated successfully!');
        this.isSaving = false;
        this.isUpdateModalOpen = false;
        this.fetchUsers(); // Refresh grid
      },
      error: (err) => {
        alert('Update failed. Ensure that this user is eligible for Admin-role updates.');
        this.isSaving = false;
        console.error(err);
      }
    });
  }

  getRolesString(roles: any): string {
    if (!roles) return this.selectedRole;
    if (Array.isArray(roles)) return roles.join(', ');
    return roles;
  }

  deleteUser(userId: string): void {
    const reason = prompt('Please provide a reason for deleting this user (required):');
    if (reason === null) return; // Cancelled
    
    if (!reason.trim()) {
      alert('A reason is required to delete a user.');
      return;
    }

    if (confirm('Are you absolutely sure? This will PERMANENTLY DELETE the user and ALL their data (posts, stories, messages, interactions). A backup will be stored.')) {
      this.isLoading = true;
      this.authService.deleteUserFullData(userId, reason).subscribe({
        next: (res) => {
          alert(res.Message || 'User and all data deleted successfully.');
          this.fetchUsers();
        },
        error: (err) => {
          alert('Failed to delete user. Make sure you have admin rights.');
          this.isLoading = false;
          console.error(err);
        }
      });
    }
  }

  // --- Deleted Posts Expansion ---
  expandedUserId: string | null = null;
  userDeletedPosts: any[] = [];
  isLoadingPosts = false;

  toggleDetails(user: any): void {
    const userId = user.userId || user.id || user.UserId;
    if (this.expandedUserId === userId) {
      this.expandedUserId = null;
      this.userDeletedPosts = [];
    } else {
      this.expandedUserId = userId;
      if (this.selectedRole === 'Deleted') {
        this.isLoadingPosts = true;
        this.authService.getDeletedPosts(userId).subscribe({
          next: (response: any) => {
            let data = response?.value || response?.data || response;
            if (data && typeof data === 'object' && data.result) {
              data = data.result;
            }
            this.userDeletedPosts = Array.isArray(data) ? data : [];
            this.isLoadingPosts = false;
            this.cdr.detectChanges();
          },
          error: (err) => {
            console.error('Error fetching deleted posts:', err);
            this.isLoadingPosts = false;
            this.cdr.detectChanges();
          }
        });
      }
    }
  }

  // --- Image Popup Logic ---
  activePopupImage: string | null = null;
  activePopupUserName: string = '';
  activePopupUserEmail: string = '';

  openImagePopup(user: any, event?: Event): void {
    if (event) event.stopPropagation();
    this.activePopupUserName = user.userName || user.username || user.caption || 'Preview';
    this.activePopupUserEmail = user.email || (user.createdAt ? 'Posted on ' + new Date(user.createdAt).toLocaleDateString() : '');
    const imgStr = user.profileImage || user.ProfileImage || user.image;
    
    if (imgStr) {
      this.activePopupImage = imgStr.startsWith('data:') ? imgStr : 'data:image/webp;base64,' + imgStr;
    } else {
      this.activePopupImage = `https://ui-avatars.com/api/?name=${this.activePopupUserName}&background=6366f1&color=fff`; 
    }
  }

  closeImagePopup(): void {
    this.activePopupImage = null;
  }

  // --- ROLE MANAGEMENT ---
  changeUserRole(userId: string, newRole: string): void {
    if (!confirm(`Are you sure you want to change this user's security level to ${newRole}? This will update their administrative permissions immediately.`)) {
      this.fetchUsers(); // Reset dropdown if cancelled
      return;
    }

    this.isLoading = true;
    this.authService.updateUserRole(userId, newRole).subscribe({
      next: (res) => {
        alert(res.Message || `Personnel security level successfully updated to ${newRole}.`);
        this.fetchUsers(); // Refresh grid
      },
      error: (err) => {
        alert('Failed to update role. This administrative endpoint may requires additional backend implementation.');
        this.isLoading = false;
        this.fetchUsers(); // Reset grid
        console.error(err);
      }
    });
  }
}
