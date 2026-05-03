import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  resetForm: FormGroup;
  email: string | null = null;
  token: string | null = null;
  
  isLoading = false;
  successMessage: string | null = null;
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router
  ) {
    this.resetForm = this.fb.group({
      oldPassword: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    // Read query parameters: ?email=...&token=...
    this.route.queryParamMap.subscribe(params => {
      this.email = params.get('email');
      this.token = params.get('token');

      if (!this.email || !this.token) {
        this.errorMessage = "Invalid or missing reset token in the URL.";
      }
    });
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;
    if (password !== confirmPassword && password && confirmPassword) {
      control.get('confirmPassword')?.setErrors({ 'mismatch': true });
      return { mismatch: true };
    }
    return null;
  }

  onSubmit(): void {
    if (this.resetForm.invalid || !this.email || !this.token) {
      this.resetForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;
    this.successMessage = null;

    const { oldPassword, password, confirmPassword } = this.resetForm.value;

    const apiUrl = `${environment.apiUrl}/ManageUsers/ForgotPassword_User`;
    
    const params = new HttpParams()
      .set('Email', this.email)
      .set('OldPassword', oldPassword)
      .set('Password', password)
      .set('ConfirmPassword', confirmPassword)
      .set('PasswordToken', this.token);

    this.http.post(apiUrl, null, { params }).subscribe({
      next: (response: any) => {
        this.isLoading = false;
        
        const result = response?.getresult || response?.getResult || response;
        const status = result?.status || result?.Status;
        const msg = result?.message || result?.Message || "Password has been successfully reset.";
        
        if (status === 'Success') {
          this.successMessage = msg;
          this.resetForm.reset();
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 2000);
        } else if (status === 'Error') {
          this.errorMessage = msg;
        } else {
          this.successMessage = msg;
          this.resetForm.reset();
        }
      },
      error: (error) => {
        this.isLoading = false;
        
        // Handle token expiry or other errors
        const errMsg = error.error?.message || error.error || error.message;
        
        if (error.status === 400 || error.status === 401 || String(errMsg).toLowerCase().includes('expire')) {
           this.errorMessage = "Token expired or invalid";
        } else {
           this.errorMessage = typeof errMsg === 'string' ? errMsg : "An error occurred while resetting the password. Please try again.";
        }
      }
    });
  }
}
