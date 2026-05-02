import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register-user',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register-user.component.html',
  styleUrl: './register-user.component.css'
})
export class RegisterUserComponent implements OnInit {
  registerForm!: FormGroup;
  selectedFile: File | null = null;
  imagePreview: string | null = null;
  isLoading = false;
  successMessage = '';
  errorMessage = '';

  roles = ['Admin', 'Manager', 'User'];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      UserName: ['', Validators.required],
      FullName: ['', Validators.required],
      Email: ['', [Validators.required, Validators.email]],
      Password: ['', [Validators.required, Validators.minLength(6)]],
      UserRole: ['User', Validators.required]
    });
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
        this.cdr.detectChanges();
      };
      reader.readAsDataURL(file);
    }
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.successMessage = '';
    this.errorMessage = '';
    this.cdr.detectChanges();

    // Create FormData for multipart upload
    const formData = new FormData();
    
    // Append form fields
    formData.append('UserName', this.registerForm.get('UserName')?.value);
    formData.append('FullName', this.registerForm.get('FullName')?.value);
    formData.append('Email', this.registerForm.get('Email')?.value);
    formData.append('Password', this.registerForm.get('Password')?.value);
    formData.append('UserRole', this.registerForm.get('UserRole')?.value);

    // Append file if selected
    if (this.selectedFile) {
      formData.append('profileImage', this.selectedFile, this.selectedFile.name);
    }

    this.authService.registerUser(formData).subscribe({
      next: (response) => {
        this.isLoading = false;

        const data = response?.value || response?.getresult || response;
        this.successMessage = data?.Message || data?.message || 'User registered successfully!';

        this.registerForm.reset({ UserRole: 'User' });
        this.selectedFile = null;
        this.imagePreview = null;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        const data = err?.error?.value || err?.error?.getresult || err?.error || err;
        this.errorMessage = data?.Message || data?.message || 'Registration failed. Please try again.';
        this.cdr.detectChanges();
      }
    });
  }

  get f() { return this.registerForm.controls; }
}
