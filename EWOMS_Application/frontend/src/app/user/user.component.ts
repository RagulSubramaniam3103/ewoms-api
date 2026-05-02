import { Component, HostListener, OnInit, ChangeDetectorRef, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { UserPostsComponent } from './user-posts/user-posts.component';
import { UserContributionsComponent } from './user-contributions/user-contributions.component';
import { ChatComponent } from '../chat/chat.component';
import { CreatePostComponent } from './create-post/create-post.component';
import { ChangePasswordComponent } from '../admin/change-password/change-password.component';
import { UserEditProfileComponent } from './edit-profile/edit-profile.component';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { AuthService } from '../services/auth.service';
import { CommunityFeedComponent } from '../shared/community-feed.component';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    UserPostsComponent,
    ChatComponent,
    CreatePostComponent,
    ChangePasswordComponent,
    UserEditProfileComponent,
    UserProfileComponent,
    CommunityFeedComponent
  ],
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit {

  currentView: string = 'community-feed';
  sidebarCollapsed: boolean = false;
  mobileSidebarOpen: boolean = false;
  
  @ViewChild('zoomLens') zoomLens!: ElementRef;
  @ViewChild('zoomImage') zoomImage!: ElementRef;
  @ViewChild('zoomResult') zoomResult!: ElementRef;
  profileImage: string | null = null;
  userName: string = '';
  userEmail: string = '';

  notificationCount: number = 0;
  notifications: any[] = [];
  notificationsOpen: boolean = false;
  unreadCount: number = 0; 
  currentPostCount: number = 0;
  today = new Date();

  activeProfileTab: string = 'overview';
  targetUserId: string | null = null;
  targetUser: any = null;
  targetAbout: any = null;
  currentLoggedInUserId: string | null = null;

  // Social System
  followersCount: number = 0;
  followingCount: number = 0;
  isFollowing: boolean = false;
  showFollowListModal: boolean = false;
  followListTitle: string = '';
  followList: any[] = [];

  activePopupImage: string | null = null;

  constructor(
    private router: Router,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  suggestedUsers: any[] = [];

  ngOnInit() {
    this.extractUserId();
    this.loadUserFromToken();
    this.loadProfileImage();
    this.fetchNotifications();
    this.loadUnreadCount();
    this.loadSuggestedUsers();
    if (this.currentLoggedInUserId) {
      this.loadFollowStats(this.currentLoggedInUserId);
    }
  }

  loadSuggestedUsers() {
    const myId = this.currentLoggedInUserId;
    if (!myId) return;

    // First fetch who we are following to ensure we don't suggest them
    this.authService.getFollowing(myId).subscribe({
      next: (following: any[]) => {
        const followingIds = (following || []).map(f => String(f.userId || f.UserId || f.id || f.Id));
        
        this.authService.getSuggestedUsers().subscribe({
          next: (res: any[]) => {
            this.suggestedUsers = (res || [])
              .map(u => ({
                ...u,
                userId: String(u.userId || u.UserId),
                fullName: u.fullName || u.FullName,
                profileImage: u.profileImage || u.ProfileImage,
                role: u.role || u.Role || 'Personnel'
              }))
              .filter(u => !followingIds.includes(u.userId) && u.userId !== String(myId));
            
            this.cdr.detectChanges();
          }
        });
      }
    });
  }

  userRole: string = 'user';

  extractUserId() {
    const token = localStorage.getItem('jwtToken');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        this.currentLoggedInUserId = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || payload.nameid;
        this.userRole = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || payload.role || 'user';
      } catch (e) {}
    }
  }

  loadUnreadCount() {
    this.authService.getTotalUnreadCount().subscribe({
      next: (res: any) => {
        this.unreadCount = res.count || 0;
        this.cdr.detectChanges();
      }
    });
  }

  fetchNotifications() {
    this.authService.getNotifications().subscribe({
      next: (res) => {
        if (res && res.count !== undefined) {
          this.notificationCount = res.count;
          this.notifications = res.data || [];
        }
      }
    });
  }

  loadProfileImage() {
    let img = localStorage.getItem('profileImage');
    if (!img) img = this.authService.getSessionProfileImage();
    if (img) {
      this.profileImage = img.startsWith('data:image') ? img : 'data:image/png;base64,' + img;
    }
  }

  loadUserFromToken() {
    const storedFullName = localStorage.getItem('fullName');
    const storedUserName = localStorage.getItem('userName');
    const storedEmail = localStorage.getItem('userEmail');

    if (storedFullName || storedUserName) {
      this.userName = storedFullName || storedUserName || 'User';
      this.userEmail = storedEmail || 'user@example.com';
      return;
    }

    const token = localStorage.getItem('jwtToken');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        this.userName = payload?.name || payload?.unique_name || payload?.email || 'User';
        this.userEmail = payload?.email || payload?.sub || 'user@example.com';
      } catch (e) {}
    }
  }

  toggleNotifications(event: Event) {
    event.stopPropagation();
    this.notificationsOpen = !this.notificationsOpen;
  }

  closeNotifications() { this.notificationsOpen = false; }

  @HostListener('document:click')
  onDocumentClick() { this.closeNotifications(); }

  setView(view: string) {
    this.currentView = view;
    this.mobileSidebarOpen = false; // Close sidebar on mobile after selection
    this.cdr.detectChanges(); // Instant sidebar update
    if (view === 'profile' || view === 'view-profile') {
      this.activeProfileTab = 'overview';
      if (view === 'profile' && this.currentLoggedInUserId) this.loadFollowStats(this.currentLoggedInUserId);
    }
    if (view !== 'view-profile') { this.targetUserId = null; this.targetUser = null; this.targetAbout = null; }
    this.cdr.detectChanges();
  }

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
    this.cdr.detectChanges();
  }

  setProfileTab(tab: string) {
    this.activeProfileTab = tab;
    this.cdr.detectChanges();
  }

  toggleMobileSidebar() {
    this.mobileSidebarOpen = !this.mobileSidebarOpen;
    this.cdr.detectChanges();
  }

  // 🔥 AMAZON STYLE ZOOM
  toggleZoom(show: boolean) {
    if (!this.zoomLens || !this.zoomResult || !this.activePopupImage) return;
    const display = show ? 'block' : 'none';
    this.zoomLens.nativeElement.style.display = display;
    this.zoomResult.nativeElement.style.display = display;
    
    if (show) {
      this.zoomResult.nativeElement.style.backgroundImage = `url(${this.activePopupImage})`;
    }
  }

  onZoomMove(e: MouseEvent) {
    if (!this.zoomImage || !this.zoomLens || !this.zoomResult) return;
    
    const img = this.zoomImage.nativeElement;
    const lens = this.zoomLens.nativeElement;
    const result = this.zoomResult.nativeElement;
    
    const rect = img.getBoundingClientRect();
    let x = e.clientX - rect.left;
    let y = e.clientY - rect.top;
    
    x = x - (lens.offsetWidth / 2);
    y = y - (lens.offsetHeight / 2);
    
    if (x > img.width - lens.offsetWidth) x = img.width - lens.offsetWidth;
    if (x < 0) x = 0;
    if (y > img.height - lens.offsetHeight) y = img.height - lens.offsetHeight;
    if (y < 0) y = 0;
    
    lens.style.left = x + 'px';
    lens.style.top = y + 'px';
    
    const cx = result.offsetWidth / lens.offsetWidth;
    const cy = result.offsetHeight / lens.offsetHeight;
    
    result.style.backgroundSize = (img.width * cx) + 'px ' + (img.height * cy) + 'px';
    result.style.backgroundPosition = '-' + (x * cx) + 'px -' + (y * cy) + 'px';
  }

  viewPublicProfile(userId: string) {
    if (!userId) return;
    this.targetUserId = userId;
    this.targetUser = null;
    this.targetAbout = null;
    this.currentView = 'view-profile';
    this.activeProfileTab = 'overview';
    this.cdr.detectChanges();
    this.loadTargetUser(userId);
  }

  loadTargetUser(userId: string) {
    this.authService.getUserById(userId).subscribe({
      next: (res: any) => {
        this.targetUser = {
          userId: res.userId || res.UserId,
          fullName: res.fullName || res.FullName,
          email: res.email || res.Email,
          profileImage: res.profileImage || res.ProfileImage || res.profileImge || res.ProfileImge,
          isPrivate: res.isPrivate || res.IsPrivate || false
        };
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.targetUser = { userId, fullName: 'Unknown User', email: 'Not available', isError: true };
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
    if (!this.targetUserId) return;
    this.authService.toggleFollow(this.targetUserId).subscribe({
      next: (res: any) => {
        this.isFollowing = res.status === 'followed';
        this.loadFollowStats(this.targetUserId!);
      }
    });
  }

  toggleFollowInList(user: any) {
    this.authService.toggleFollow(user.userId).subscribe({
      next: (res: any) => {
        user.isFollowing = (res.status === 'followed');
        const refreshId = this.targetUserId || this.currentLoggedInUserId;
        if (refreshId) this.loadFollowStats(refreshId);
        this.loadSuggestedUsers(); // Refresh suggestions to remove the newly followed user
        this.cdr.detectChanges();
      }
    });
  }

  openFollowers(userId?: string) {
    const id = userId || this.targetUserId || this.currentLoggedInUserId;
    if (!id) return;
    this.followListTitle = 'Followers';
    this.authService.getFollowers(id).subscribe({
      next: (res: any[]) => {
        this.followList = (res || []).map(u => ({ ...u, userId: u.userId || u.UserId }));
        this.showFollowListModal = true;
        this.cdr.detectChanges();
      }
    });
  }

  openFollowing(userId?: string) {
    const id = userId || this.targetUserId || this.currentLoggedInUserId;
    if (!id) return;
    this.followListTitle = 'Following';
    this.authService.getFollowing(id).subscribe({
      next: (res: any[]) => {
        this.followList = (res || []).map(u => ({ ...u, userId: u.userId || u.UserId }));
        this.showFollowListModal = true;
        this.cdr.detectChanges();
      }
    });
  }

  closeFollowList() {
    this.showFollowListModal = false;
    this.followList = [];
  }

  getAvatar(u: any) {
    if (!u) return null;
    const img = u.profileImage || u.ProfileImage || u.profileImge || u.ProfileImge;
    if (!img) return null;
    return img.startsWith('data:image') ? img : `data:image/jpeg;base64,${img}`;
  }

  openImagePopup(event: Event, customUrl?: string) {
    if (event) event.stopPropagation();
    this.activePopupImage = customUrl || this.profileImage || `https://ui-avatars.com/api/?name=${this.userName || 'U'}&background=6366f1&color=fff`;
    this.closeNotifications();
  }

  openImageFromPost(imageUrl: string) {
    this.activePopupImage = imageUrl;
    this.cdr.detectChanges();
  }

  closeImagePopup() { this.activePopupImage = null; }

  @HostListener('document:keydown.escape')
  onEscPress() { this.closeImagePopup(); }

  markAllNotificationsRead() {
    this.authService.markAllNotificationsRead().subscribe({
      next: (res) => {
        this.notificationCount = 0;
        this.notifications = [];
        this.cdr.detectChanges();
      }
    });
  }

  logout() {
    localStorage.clear();
    this.router.navigate(['/login']);
  }
}
