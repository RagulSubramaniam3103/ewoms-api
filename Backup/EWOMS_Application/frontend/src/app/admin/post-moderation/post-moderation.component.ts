import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-post-moderation',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './post-moderation.component.html',
  styleUrls: ['./post-moderation.component.css']
})
export class PostModerationComponent implements OnInit {
  Math = Math; // Expose Math for template
  activePosts: any[] = [];
  archivedPosts: any[] = [];
  activeTab: 'pending' | 'archived' = 'pending'; // 👈 Matched with HTML
  isLoading = false;
  isArchiving = false;

  // For Archive Reason Modal (Standard EWOMS Modal)
  showDeleteModal = false;
  selectedPostId: number | null = null;
  deleteReason = '';

  // Post Detail Popup (Screening Modal)
  selectedPost: any = null;

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadActivePosts();
    this.loadArchivedPosts();
  }

  loadActivePosts() {
    this.isLoading = true;
    this.authService.getUserPosts('').subscribe({
      next: (res) => {
        this.activePosts = (res || []).map((p: any) => ({
          ...p,
          displayImage: p.postImage || p.userImage || p.profileimage || null,
          isRevealed: false
        }));
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load active posts', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadArchivedPosts() {
    this.authService.getArchivedPosts().subscribe({
      next: (res) => {
        this.archivedPosts = (res || []).map((p: any) => ({
          ...p,
          displayImage: p.profileImage || p.ProfileImage || p.postImage || null,
          id: p.uId || p.UId || p.id,
          userName: p.userId || p.UserId || 'Unknown User',
          caption: p.caption || p.Caption || '',
          createdAt: p.createdAt || p.CreatedAt || null,
          isRevealed: true 
        }));
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Failed to load archived posts', err)
    });
  }

  // Pagination
  currentPage = 1;
  pageSize = 15;

  // --- HTML INTERFACE METHODS ---
  setTab(tab: 'pending' | 'archived') {
    this.activeTab = tab;
    this.currentPage = 1; // reset to first page on tab switch
    this.cdr.detectChanges();
  }

  get totalItems() {
    return this.activeTab === 'pending' ? this.activePosts.length : this.archivedPosts.length;
  }

  get totalPages() {
    return Math.ceil(this.totalItems / this.pageSize);
  }

  get filteredPosts() {
    const allPosts = this.activeTab === 'pending' ? this.activePosts : this.archivedPosts;
    const startIndex = (this.currentPage - 1) * this.pageSize;
    return allPosts.slice(startIndex, startIndex + this.pageSize);
  }

  setPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.cdr.detectChanges();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  get archivedCount() {
    return this.archivedPosts.length;
  }

  openDetail(post: any) {
    this.selectedPost = post;
    this.cdr.detectChanges();
  }

  closeDetail() {
    this.selectedPost = null;
    this.cdr.detectChanges();
  }

  toggleBlur(post: any) {
    if (!post.id) return;
    this.authService.blurPost(post.id).subscribe({
      next: (res) => {
        alert(res.Message || 'Content visibility updated');
        this.loadActivePosts();
      },
      error: (err) => alert(err?.error?.Message || 'Failed to update visibility')
    });
  }

  archivePost(post: any) {
    this.selectedPostId = post.id;
    this.deleteReason = '';
    this.showDeleteModal = true;
    this.cdr.detectChanges();
  }

  confirmArchive() {
    if (this.selectedPostId === null || this.isArchiving) return;
    if (!this.deleteReason.trim()) {
      alert('Please provide a reason for archiving.');
      return;
    }
    
    this.isArchiving = true;
    this.authService.deleteAndArchivePost(this.selectedPostId, this.deleteReason).subscribe({
      next: (res) => {
        alert(res.Message || 'Post archived successfully');
        this.isArchiving = false;
        this.showDeleteModal = false;
        this.selectedPost = null; // Close screening modal if open
        this.loadActivePosts();
        this.loadArchivedPosts();
      },
      error: (err) => {
        this.isArchiving = false;
        alert(err?.error?.Message || 'Archive failed');
      }
    });
  }

  closeArchiveModal() {
    this.showDeleteModal = false;
    this.selectedPostId = null;
  }
}
