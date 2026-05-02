import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {
  changeForm!: FormGroup;
  isLoading = false;
  successMessage = '';
  errorMessage = '';

  showOld = false;
  showNew = false;
  showConfirm = false;

  constructor(private fb: FormBuilder, private authService: AuthService) {}

  ngOnInit(): void {
    this.changeForm = this.fb.group(
      {
        Email: ['', [Validators.required, Validators.email]],
        OldPassword: ['', Validators.required],
        Password: ['', [Validators.required, Validators.minLength(6)]],
        ConfirmPassword: ['', Validators.required]
      },
      { validators: this.passwordsMatch }
    );
  }

  passwordsMatch(group: AbstractControl): ValidationErrors | null {
    const pwd = group.get('Password')?.value;
    const confirm = group.get('ConfirmPassword')?.value;
    return pwd && confirm && pwd !== confirm ? { mismatch: true } : null;
  }

  onSubmit(): void {
    if (this.changeForm.invalid) {
      this.changeForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.successMessage = '';
    this.errorMessage = '';

    this.authService.changePassword(this.changeForm.value).subscribe({
      next: (response) => {
        this.isLoading = false;
        console.log('Change password response:', response);

        // Backend returns: { getresult: { Message: "Password Reset Successfully" } }
        const msg =
          response?.getresult?.Message ||
          response?.getresult?.message ||
          response?.Message ||
          response?.message ||
          'Password changed successfully!';

        this.successMessage = msg;
        this.changeForm.reset();
      },
      error: (err) => {
        this.isLoading = false;
        const errMsg =
          err?.error?.getresult?.Message ||
          err?.error?.Message ||
          err?.error?.message ||
          'Failed to change password. Please try again.';
        this.errorMessage = errMsg;
      }
    });
  }

  get f() { return this.changeForm.controls; }
}
