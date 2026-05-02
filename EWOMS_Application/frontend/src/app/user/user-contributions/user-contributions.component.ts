import { Component, OnInit, Input, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-user-contributions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-contributions.component.html',
  styleUrls: ['./user-contributions.component.css']
})
export class UserContributionsComponent implements OnInit {
  @Input() userId: any = null;
  @Input() viewType: 'grid' | 'list' = 'grid';

  posts: any[] = [];
  isLoading = false;
  selectedPost: any = null;

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadContributions();
  }

  loadContributions() {
    if (!this.userId) return;
    this.isLoading = true;
    this.authService.getUserPosts(this.userId).subscribe({
      next: (res: any) => {
        this.posts = res.map((p: any) => {
          p.displayImage = p.displayImage || p.postImage || p.PostImage || p.image || p.Image || null;
          p.authorImage = p.authorImage || p.userImage || p.UserImage || p.profileImage || p.ProfileImage || null;
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
