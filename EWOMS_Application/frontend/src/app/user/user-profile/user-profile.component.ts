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
  @Input() role: string = 'user';

  @Output() viewProfile = new EventEmitter<string>();
  @Output() openImage = new EventEmitter<string>();
  @Output() openFollowers = new EventEmitter<string>();
  @Output() openFollowing = new EventEmitter<string>();
  @Output() switchToChat = new EventEmitter<void>();
  @Output() switchToCreatePost = new EventEmitter<void>();
  @Output() switchToEditProfile = new EventEmitter<void>();

  user: any = null;
  targetAbout: any = null;
  activeProfileTab: string = 'posts';
  currentPostCount: number = 0;
  
  followersCount: number = 0;
  followingCount: number = 0;
  isFollowing: boolean = false;
  isRequested: boolean = false;
  
  // Stories for Long-Press Popup
  userStories: any[] = [];
  activeStory: any = null;
  storyTimer: any;
  longPressTimer: any;
  isLongPressing: boolean = false;
  
  followersList: any[] = [];
  isLoadingFollowers: boolean = false;

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

    this.authService.getUserPosts(userId).subscribe({
      next: (res: any[]) => {
        this.currentPostCount = res ? res.length : 0;
        this.cdr.detectChanges();
      }
    });

    this.loadFollowStats(userId);
    this.loadUserStories(userId);
  }

  loadUserStories(userId: string) {
    const contextId = this.currentLoggedInUserId || this.authService.getUserIdFromToken();
    if (!contextId) return;

    this.authService.getStories(contextId).subscribe({
      next: (res: any) => {
        // Find stories for the target userId in the feed
        const found = (res || []).find((s: any) => String(s.userId || s.UserId || s.id || s.Id) === String(userId));
        if (found) {
          this.userStories = (found.segments || found.Segments || []).map((seg: any) => ({
            id: seg.id || seg.Id,
            displayImage: seg.displayImage || seg.DisplayImage,
            caption: seg.caption || seg.Caption,
            isSeen: seg.isSeen !== undefined ? seg.isSeen : seg.IsSeen
          }));
        } else {
          this.userStories = [];
        }
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Profile: Story fetch failed', err)
    });
  }

  loadFollowStats(userId: string) {
    this.authService.getFollowStats(userId).subscribe({
      next: (res: any) => {
        this.followersCount = res.followersCount;
        this.followingCount = res.followingCount;
        this.isFollowing = res.isFollowing;
        this.isRequested = res.isRequested || res.IsRequested || false;
        this.cdr.detectChanges();
        
        // Fetch the actual list
        this.loadFollowersList(userId);
      }
    });
  }

  loadFollowersList(userId: string) {
    this.isLoadingFollowers = true;
    this.authService.getFollowers(userId).subscribe({
      next: (res: any[]) => {
        this.followersList = (res || []).map(u => ({
          ...u,
          userId: u.userId || u.UserId || u.id || u.Id,
          fullName: u.fullName || u.FullName,
          profileImage: u.profileImage || u.ProfileImage
        }));
        this.isLoadingFollowers = false;
        this.cdr.detectChanges();
      }
    });
  }

  toggleFollow() {
    if (!this.userId) return;
    this.authService.toggleFollow(this.userId).subscribe({
      next: (res: any) => {
        this.isFollowing = res.status === 'followed';
        this.isRequested = res.status === 'requested';
        this.loadFollowStats(this.userId!);
      }
    });
  }

  toggleFollowInList(user: any) {
    this.authService.toggleFollow(user.userId).subscribe({
      next: (res: any) => {
        user.isFollowing = (res.status === 'followed');
        this.loadFollowStats(this.userId!);
        this.cdr.detectChanges();
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
  triggerEditProfile() { this.switchToEditProfile.emit(); }
  triggerCreatePost() { this.switchToCreatePost.emit(); }

  // AVATAR INTERACTION LOGIC
  onAvatarClick() {
    if (this.isLongPressing) return; // Prevent double trigger if long press already opened it

    if (this.userStories.length > 0) {
      this.openStoryPopup();
    } else {
      // Fallback: Show profile image large
      const avatarUrl = this.getAvatar(this.user);
      if (avatarUrl) {
        this.triggerOpenImage(avatarUrl);
      }
    }
  }

  onAvatarMouseDown(event: MouseEvent) {
    if (event.button !== 0) return; // Only left click
    
    this.isLongPressing = false;
    if (this.longPressTimer) clearTimeout(this.longPressTimer);
    
    this.longPressTimer = setTimeout(() => {
      if (this.userStories.length > 0) {
        this.isLongPressing = true;
        this.openStoryPopup();
      }
    }, 400); 
  }

  onAvatarMouseUp() {
    if (this.longPressTimer) {
      clearTimeout(this.longPressTimer);
    }
  }

  onAvatarMouseLeave() {
    if (this.longPressTimer) {
      clearTimeout(this.longPressTimer);
    }
  }

  openStoryPopup() {
    this.activeStory = {
      segments: this.userStories,
      currentSegmentIndex: 0,
      userName: this.user.fullName,
      userImage: this.getAvatar(this.user)
    };
    this.startStoryTimer();
    this.cdr.detectChanges();
  }

  startStoryTimer() {
    if (this.storyTimer) clearInterval(this.storyTimer);
    this.storyTimer = setInterval(() => {
      this.nextStorySegment();
    }, 4000);
  }

  nextStorySegment() {
    if (!this.activeStory) return;
    if (this.activeStory.currentSegmentIndex < this.activeStory.segments.length - 1) {
      this.activeStory.currentSegmentIndex++;
    } else {
      this.closeStoryPopup();
    }
    this.cdr.detectChanges();
  }

  closeStoryPopup() {
    if (this.storyTimer) clearInterval(this.storyTimer);
    this.activeStory = null;
    this.cdr.detectChanges();
  }
}
