import { Component, OnInit, ChangeDetectorRef, Output, EventEmitter } from '@angular/core';
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
  Math = Math;
  posts: any[] = [];
  currentTab: 'all' | 'blurred' = 'all';
  searchTerm: string = '';
  isLoading = false;
  
  unblurredPostsCount = 0;
  blurredPostsCount = 0;
  
  @Output() openImage = new EventEmitter<string>();


  constructor(private authService: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadPosts();
  }

  loadPosts() {
    this.isLoading = true;
    this.authService.getUserPosts('').subscribe({
      next: (res) => {
        this.posts = (res || []).map((p: any) => ({
          ...p,
          displayImage: p.postImage || p.userImage || p.profileimage || null
        }));
        this.updateCounts();
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  updateCounts() {
    this.unblurredPostsCount = this.posts.filter(p => !p.isBlurred).length;
    this.blurredPostsCount = this.posts.filter(p => p.isBlurred).length;
  }

  // Pagination
  currentPage = 1;
  pageSize = 15;

  // --- HTML INTERFACE METHODS ---
  setTab(tab: 'all' | 'blurred') {
    this.currentTab = tab;
    this.currentPage = 1; // Reset to page 1 when tab changes
    this.cdr.detectChanges();
  }

  onSearchChange() {
    this.currentPage = 1;
    this.cdr.detectChanges();
  }

  get paginatedPosts() {
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    return this.filteredPosts.slice(start, end);
  }

  get totalPages() {
    return Math.ceil(this.filteredPosts.length / this.pageSize);
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.cdr.detectChanges();
    }
  }

  prevPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.cdr.detectChanges();
    }
  }

  goToPage(page: number) {
    this.currentPage = page;
    this.cdr.detectChanges();
  }

  get filteredPosts() {
    let list = this.posts;
    if (this.currentTab === 'blurred') {
      list = list.filter(p => p.isBlurred);
    }
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      list = list.filter(p => 
        p.userName?.toLowerCase().includes(term) || 
        p.caption?.toLowerCase().includes(term)
      );
    }
    return list;
  }

  getAuthorAvatar(post: any) {
    const img = post.authorImage || post.userImage || post.profileImage;
    return img ? (img.startsWith('data:image') ? img : 'data:image/jpeg;base64,' + img) : 'https://ui-avatars.com/api/?name=' + post.userName;
  }

  toggleBlur(post: any) {
    if (!post.id) return;
    this.authService.blurPost(post.id).subscribe({
      next: () => {
        post.isBlurred = !post.isBlurred;
        this.updateCounts();
        this.cdr.detectChanges();
      }
    });
  }

  archivePost(post: any) {
    const reason = prompt('Reason for archiving:');
    if (reason === null) return;
    this.authService.deleteAndArchivePost(post.id, reason).subscribe({
      next: () => {
        this.posts = this.posts.filter(p => p.id !== post.id);
        this.updateCounts();
        this.cdr.detectChanges();
      }
    });
  }

  openImagePopup(post: any, event: Event) {
    if (event) event.stopPropagation();
    const imageUrl = 'data:image/webp;base64,' + post.displayImage;
    this.openImage.emit(imageUrl);
  }

}
