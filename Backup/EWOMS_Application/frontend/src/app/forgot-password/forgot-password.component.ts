import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  forgotForm: FormGroup;
  isLoading = false;
  successMessage = '';
  errorMessage = '';

  constructor(private fb: FormBuilder, private authService: AuthService) {
    this.forgotForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit() {
    if (this.forgotForm.invalid) {
      this.forgotForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.successMessage = '';
    this.errorMessage = '';

    const email = this.forgotForm.value.email;

    this.authService.requestPasswordReset(email).subscribe({
      next: (response) => {
        this.isLoading = false;
        // Typically Ok(existinguser) would return true/false, an object, or a message.
        if (response) {
            this.successMessage = 'A password reset link has been formally dispatched to your registered email address.';
            this.forgotForm.reset();
        } else {
            this.errorMessage = 'The email entry corresponds to an inactive or non-existent profile.';
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Error communicating with server. Please try again.';
      }
    });
  }

  get f() { return this.forgotForm.controls; }
}
