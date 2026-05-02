import { Component, OnInit, Input, ChangeDetectorRef, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-social-contributions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './social-contributions.component.html',
  styleUrls: ['./social-contributions.component.css']
})
export class SocialContributionsComponent implements OnInit, OnChanges {
  @Input() userId: any = null;
  @Input() viewType: 'grid' | 'list' = 'grid';
  @Input() mode: 'posts' | 'saved' = 'posts';

  posts: any[] = [];
  isLoading = false;
  selectedPost: any = null;

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadContributions();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['mode'] && !changes['mode'].firstChange) {
      this.loadContributions();
    }
  }

  loadContributions() {
    if (!this.userId) return;
    this.isLoading = true;
    const request = this.mode === 'saved' 
      ? this.authService.getSavedPosts(this.userId) 
      : this.authService.getUserPosts(this.userId);
      
    request.subscribe({
      next: (res: any) => {
        this.posts = res.map((p: any) => {
          // Comprehensive mapping for all possible image field names
          p.displayImage = p.displayImage || p.postImage || p.PostImage || p.postImageBase64 || p.image || p.Image || p.imageContent || null;
          p.authorImage = p.authorImage || p.userImage || p.UserImage || p.profileImage || p.ProfileImage || p.authorImageBase64 || null;
          return p;
        });
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openPostDetail(post: any) {
    this.selectedPost = post;
    this.cdr.detectChanges();
  }

  closePostDetail() {
    this.selectedPost = null;
    this.cdr.detectChanges();
  }
}
