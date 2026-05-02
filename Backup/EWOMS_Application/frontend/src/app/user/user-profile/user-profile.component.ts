import { Component, Input, OnInit, ChangeDetectorRef, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { SocialContributionsComponent } from '../../shared/social-contributions.component';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, SocialContributionsComponent],
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {
  @Input() userId: string | null = null;
  @Input() currentLoggedInUserId: string | null = null;
  @Input() isOwn: boolean = false;

  @Output() viewProfile = new EventEmitter<string>();
  @Output() openImage = new EventEmitter<string>();
  @Output() openFollowers = new EventEmitter<string>();
  @Output() openFollowing = new EventEmitter<string>();
  @Output() switchToChat = new EventEmitter<void>();
  @Output() switchToCreatePost = new EventEmitter<void>();

  user: any = null;
  targetAbout: any = null;
  activeProfileTab: string = 'overview';
  currentPostCount: number = 0;
  
  followersCount: number = 0;
  followingCount: number = 0;
  isFollowing: boolean = false;

  constructor(
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    if (this.userId) {
      this.loadUser(this.userId);
    }
  }

  ngOnChanges() {
    if (this.userId) {
      this.loadUser(this.userId);
    }
  }

  loadUser(userId: string) {
    this.authService.getUserById(userId).subscribe({
      next: (res: any) => {
        this.user = {
          userId: res.userId || res.UserId,
          fullName: res.fullName || res.FullName,
          email: res.email || res.Email,
          profileImage: res.profileImage || res.ProfileImage || res.profileImge || res.ProfileImge,
          isPrivate: res.isPrivate || res.IsPrivate || false
        };
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.user = { userId, fullName: 'Unknown User', email: 'Not available', isError: true };
        this.cdr.detectChanges();
      }
    });

    this.authService.getAboutInfo(userId).subscribe({
      next: (res: any) => {
        this.targetAbout = res;
        this.cdr.detectChanges();
      }
    });

    this.loadFollowStats(userId);
  }

  loadFollowStats(userId: string) {
    this.authService.getFollowStats(userId).subscribe({
      next: (res: any) => {
        this.followersCount = res.followersCount;
        this.followingCount = res.followingCount;
        this.isFollowing = res.isFollowing;
        this.cdr.detectChanges();
      }
    });
  }

  toggleFollow() {
    if (!this.userId) return;
    this.authService.toggleFollow(this.userId).subscribe({
      next: (res: any) => {
        this.isFollowing = res.status === 'followed';
        this.loadFollowStats(this.userId!);
      }
    });
  }

  setProfileTab(tab: string) {
    this.activeProfileTab = tab;
    this.cdr.detectChanges();
  }

  getAvatar(u: any) {
    if (!u) return null;
    const img = u.profileImage || u.ProfileImage || u.profileImge || u.ProfileImge;
    if (!img) return null;
    return img.startsWith('data:image') ? img : `data:image/jpeg;base64,${img}`;
  }

  triggerFollowers() { this.openFollowers.emit(this.userId!); }
  triggerFollowing() { this.openFollowing.emit(this.userId!); }
  triggerViewProfile(uid: string) { this.viewProfile.emit(uid); }
  triggerOpenImage(url: string) { this.openImage.emit(url); }
  triggerCreatePost() { this.switchToCreatePost.emit(); }
}
