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
  searchTerm: string = '';
  lockoutUsers: any[] = [];
  filteredLockoutUsers: any[] = [];
  
  isLoading = false;
  actionLoading: { [key: string]: boolean } = {};
  
  successMessage = '';
  errorMessage = '';

  constructor(
    private authService: AuthService, 
    private datePipe: DatePipe,
    private cdr: ChangeDetectorRef
  ) {
    this.loadAllLockouts();
  }

  loadAllLockouts() {
    const today = this.datePipe.transform(new Date(), 'yyyy-MM-dd') || '';
    this.searchLockedUsers(today);
  }

  onSearchChange() {
    if (!this.searchTerm.trim()) {
      this.filteredLockoutUsers = [...this.lockoutUsers];
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredLockoutUsers = this.lockoutUsers.filter(u => {
        const name = (u.userName || u.UserName || '').toLowerCase();
        const email = (u.email || u.Email || '').toLowerCase();
        return name.includes(term) || email.includes(term);
      });
    }
    this.cdr.detectChanges();
  }

  searchLockedUsers(date: string): void {
    this.isLoading = true;
    this.cdr.detectChanges();

    this.authService.getLockoutUser(date).subscribe({
      next: (res) => {
        let data = res?.value || res?.getresult || res;
        if (data && typeof data === 'object' && data.result) data = data.result;

        if (Array.isArray(data)) {
          this.lockoutUsers = data;
        } else if (data && (data.email || data.userName)) {
          this.lockoutUsers = [data];
        } else {
          this.lockoutUsers = [];
        }

        this.onSearchChange();
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to retrieve locked-out users.';
        this.cdr.detectChanges();
      }
    });
  }

  unlockUser(id: any): void {
    const user = this.lockoutUsers.find(u => u.id === id);
    const email = user?.email;
    if (!email) return;

    if (!confirm(`Restore access for ${email}?`)) return;

    this.actionLoading[email] = true;
    this.cdr.detectChanges();

    this.authService.releaseLockoutUser(email, true).subscribe({
      next: (res) => {
        this.actionLoading[email] = false;
        this.successMessage = res?.Message || 'Access restored.';
        this.loadAllLockouts();
      },
      error: (err) => {
        this.actionLoading[email] = false;
        this.errorMessage = 'Failed to restore access.';
        this.cdr.detectChanges();
      }
    });
  }
}
