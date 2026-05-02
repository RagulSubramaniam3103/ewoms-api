import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-audit-logs',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './audit-logs.component.html',
  styleUrls: ['./audit-logs.component.css']
})
export class AuditLogsComponent implements OnInit {
  logs: any[] = [];
  filteredLogs: any[] = [];
  paginatedLogs: any[] = [];
  
  isLoading: boolean = false;
  searchTerm: string = '';
  
  currentPage: number = 1;
  pageSize: number = 15;
  totalPages: number = 1;

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.fetchLogs();
  }

  fetchLogs(): void {
    this.isLoading = true;
    this.authService.getAuditLogs().subscribe({
      next: (response) => {
        let data = response?.value || response?.data || response;
        if (data && typeof data === 'object' && data.result) {
          data = data.result;
        }
        this.logs = Array.isArray(data) ? data : [];
        this.applyFilters();
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading audit logs:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  applyFilters(): void {
    let temp = this.logs;
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      temp = temp.filter(l => 
        l.action.toLowerCase().includes(term) ||
        l.adminName.toLowerCase().includes(term) ||
        (l.targetName && l.targetName.toLowerCase().includes(term)) ||
        (l.details && l.details.toLowerCase().includes(term))
      );
    }
    this.filteredLogs = temp;
    this.totalPages = Math.ceil(this.filteredLogs.length / this.pageSize) || 1;
    this.updatePagination();
  }

  updatePagination(): void {
    const start = (this.currentPage - 1) * this.pageSize;
    this.paginatedLogs = this.filteredLogs.slice(start, start + this.pageSize);
  }

  onSearch(): void {
    this.currentPage = 1;
    this.applyFilters();
  }

  changePage(dir: number): void {
    this.currentPage += dir;
    this.updatePagination();
  }
}
