import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-lockout-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [DatePipe],
  templateUrl: './lockout-management.component.html',
  styleUrls: ['./lockout-management.component.css']
})
export class LockoutManagementComponent {
  searchDate: string = '';
  lockedUsers: any[] = [];
  
  isLoading = false;
  actionLoading: { [key: string]: boolean } = {};
  
  successMessage = '';
  errorMessage = '';

  constructor(
    private authService: AuthService, 
    private datePipe: DatePipe,
    private cdr: ChangeDetectorRef
  ) {
    // Default to today
    this.searchDate = this.datePipe.transform(new Date(), 'yyyy-MM-dd') || '';
  }

  searchLockedUsers(): void {
    if (!this.searchDate) {
      console.warn('No date selected for lockout search.');
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.cdr.detectChanges();

    console.log('Sending date to API:', this.searchDate);

    this.authService.getLockoutUser(this.searchDate).subscribe({
      next: (res) => {
        console.log('Search response:', res);
        
        let data = res?.value || res?.getresult || res;
        
        if (data && typeof data === 'object' && data.result) {
          data = data.result;
        }

        // If data is an object but not a user (has no Email/UserName), it's likely a message object
        const isUserRecord = (obj: any) => obj && (obj.email || obj.Email || obj.userName || obj.UserName || obj.id || obj.Id);

        if (Array.isArray(data)) {
          this.lockedUsers = data;
        } else if (isUserRecord(data)) {
          this.lockedUsers = [data];
        } else {
          // It's likely an empty result or a message object like { message: "..." }
          this.lockedUsers = [];
          if (data?.message || data?.Message) {
              console.log('API returned message instead of data:', data.message || data.Message);
          }
        }

        console.log('Final mapped locked users:', this.lockedUsers);
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Search error:', err);
        this.isLoading = false;
        this.errorMessage = 'Failed to retrieve locked-out users.';
        this.cdr.detectChanges();
      }
    });
  }

  releaseLockout(email: string): void {
    if (!email) return;

    const confirmRelease = window.confirm(`Are you sure you want to release the lockout for ${email}?`);
    if (!confirmRelease) return;

    console.log('Releasing lockout for:', email);
    this.actionLoading[email] = true;
    this.successMessage = '';
    this.errorMessage = '';
    this.cdr.detectChanges();

    this.authService.releaseLockoutUser(email, true).subscribe({
      next: (res) => {
        console.log('Release response:', res);
        
        const msg = res?.Message || res?.message || res?.getresult?.Message || res?.getresult?.message || `Lockout successfully released for ${email}.`;
        
        this.actionLoading[email] = false;
        this.successMessage = msg;
        
        // Refresh list after a tiny delay to ensure DB propagation
        setTimeout(() => {
          this.searchLockedUsers();
        }, 200);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Release error:', err);
        this.actionLoading[email] = false;
        this.errorMessage = `Failed to release lockout for ${email}.`;
        this.cdr.detectChanges();
      }
    });
  }
}
