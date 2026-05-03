import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  registerForm!: FormGroup;
  isRegisterMode = false;
  selectedFile: File | null = null;
  successMessage = '';
  isLoading = false;
  errorMessage = '';
  warningMessage = '';   // for password expiry warning
  selectedFileName = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    this.registerForm = this.fb.group({
      FullName: ['', Validators.required],
      UserName: ['', Validators.required],
      Email: ['', [Validators.required, Validators.email]],
      Password: ['', [Validators.required, Validators.minLength(6)]],
      UserRole: ['User', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.warningMessage = '';

    const credentials = {
      email: this.loginForm.value.email,
      password: this.loginForm.value.password
    };

    this.authService.loginMaster(credentials).subscribe({
      next: (response) => {
        this.isLoading = false;

        console.log('Raw login response:', response);

        // Backend returns { Token, Message } or { Message } on failure (all HTTP 200)
        const token =
          response?.Token ||
          response?.token ||
          response?.getresult?.Token ||
          response?.getresult?.token;

        // Always grab the backend message (shown on failure)
        // Check both direct property and .value wrapper
        const data = response?.value || response;
        const backendMessage =
          data?.Message ||
          data?.message ||
          data?.getresult?.Message ||
          data?.getresult?.message ||
          '';

        console.log('Extracted backend message:', backendMessage);

        // Password expiry warning (80–90 day zone)
        const expiryWarning =
          data?.PasswordExpiryWarning ||
          data?.getresult?.PasswordExpiryWarning ||
          '';
        const image =
          data?.Image ||
          data?.image ||
          data?.getresult?.Image ||
          data?.getresult?.image ||
          null;

        if (token) {
          localStorage.setItem('jwtToken', token);
          if (image) {
            this.authService.setSessionProfileImage(image);
            try {
              localStorage.setItem('profileImage', image);
            } catch (e) {
              console.warn('Profile image too large for localStorage quota. Skipping local cache.');
            }
          }

          // Save UserId (Required for Update_Admin)
          const userId = response?.userId || response?.getresult?.userId || data?.userId || data?.getresult?.userId;
          if (userId) localStorage.setItem('userId', userId);

          // 🔥 Save explicit user data from the 'Data' field
          const userInfo = data?.getresult?.Data || data?.Data || data?.getresult?.data || data?.data;
          if (userInfo) {
            localStorage.setItem('userData', JSON.stringify(userInfo));
            
            const uName = userInfo.userName || userInfo.UserName;
            const fName = userInfo.fullName || userInfo.FullName;
            const email = userInfo.email || userInfo.Email;

            if (uName) localStorage.setItem('userName', uName);
            if (fName) localStorage.setItem('fullName', fName);
            if (email) localStorage.setItem('userEmail', email);
            
            // Fallback for ID if not found at top level
            if (!userId) {
              const altId = userInfo.id || userInfo.userId || userInfo.UserId;
              if (altId) localStorage.setItem('userId', altId);
            }
          }

          // Show expiry warning if present
          if (expiryWarning) {
            this.warningMessage = expiryWarning;
          }

          try {
            const payloadContent = atob(token.split('.')[1]);
            const tokenPayload = JSON.parse(payloadContent);

            console.log('Decoded JWT payload:', tokenPayload);

            const roleKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
            const role = tokenPayload[roleKey] || tokenPayload.role || '';

            console.log('Extracted role:', role);

            // Role-based redirection
            if (role === 'User') {
              this.router.navigate(['/user']);
            } else if (role === 'Manager') {
              this.router.navigate(['/manager']);
            } else if (role !== '') {
              this.router.navigate(['/admin']);
            } else {
              this.errorMessage = 'Access Denied: No valid role found in your token.';
              localStorage.removeItem('jwtToken');
            }
          } catch (e) {
            console.error('Failed to parse JWT token', e);
            this.errorMessage = 'Invalid token received from the server.';
          }
        } else {
          // No token → show exact backend message (e.g. "Invalid Credentials.", "User is locked out.", etc.)
          this.errorMessage = backendMessage || 'Login failed. No token received.';
          console.log('No token found, error message is now:', this.errorMessage);
        }
        this.cdr.detectChanges(); // Force UI update
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error?.error?.Message || error?.error?.message || 'Login failed. Please check your credentials and try again.';
        console.error('Login error', error);
        this.cdr.detectChanges();
      }
    });
  }

  get f() {
    return this.loginForm.controls;
  }

  get rf() {
    return this.registerForm.controls;
  }

  toggleMode(event: Event): void {
    event.preventDefault();
    this.isRegisterMode = !this.isRegisterMode;
    this.errorMessage = '';
    this.successMessage = '';
    this.warningMessage = '';
    this.loginForm.reset();
    
    if (this.registerForm) {
      this.registerForm.reset();
      this.registerForm.patchValue({ UserRole: 'User' });
    }
    
    this.selectedFile = null;
    this.selectedFileName = '';
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      this.selectedFileName = file.name;
    } else {
      this.selectedFileName = '';
    }
    this.cdr.detectChanges();
  }

  onRegister(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const formData = new FormData();
    formData.append('UserName', this.registerForm.get('UserName')?.value);
    formData.append('FullName', this.registerForm.get('FullName')?.value);
    formData.append('Email', this.registerForm.get('Email')?.value);
    formData.append('Password', this.registerForm.get('Password')?.value);
    
    const role = this.registerForm.get('UserRole')?.value || 'User';
    formData.append('UserRole', role);
    
    if (this.selectedFile) {
      formData.append('profileImage', this.selectedFile);
    }

    this.authService.registerUser(formData).subscribe({
      next: (response) => {
        this.isLoading = false;
        const data = response?.value || response;
        this.successMessage = data?.Message || data?.message || 'Registration successful! You can now login.';
        setTimeout(() => {
          this.isRegisterMode = false;
          this.cdr.detectChanges();
        }, 1500);
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error?.error?.Message || error?.error?.message || 'Registration failed. Please try again.';
        this.cdr.detectChanges();
      }
    });
  }
}
