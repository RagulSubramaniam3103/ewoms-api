import { Component, HostListener, OnInit, ChangeDetectorRef, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { UserManagementComponent } from './user-management/user-management.component';
import { RegisterUserComponent } from './register-user/register-user.component';
import { LockoutManagementComponent } from './lockout-management/lockout-management.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { EditProfileComponent } from './edit-profile/edit-profile.component';
import { ChatComponent } from '../chat/chat.component';
import { CreatePostComponent } from '../user/create-post/create-post.component';
import { PostModerationComponent } from './post-moderation/post-moderation.component';
import { UserProfileComponent } from '../user/user-profile/user-profile.component';
import { CommunityFeedComponent } from '../shared/community-feed.component';
import { SocialContributionsComponent } from '../shared/social-contributions.component';
import { ThemeService } from '../services/theme.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    UserManagementComponent,
    RegisterUserComponent,
    LockoutManagementComponent,
    ChangePasswordComponent,
    EditProfileComponent,
    ChatComponent,
    CreatePostComponent,
    PostModerationComponent,
    UserProfileComponent,
    CommunityFeedComponent,
    SocialContributionsComponent
  ],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {

  sidebarCollapsed = false;
  currentView = 'dashboard';
  activeProfileTab = 'overview'; 
  
  @ViewChild('zoomLens') zoomLens!: ElementRef;
  @ViewChild('zoomImage') zoomImage!: ElementRef;
  @ViewChild('zoomResult') zoomResult!: ElementRef;
  profileImage: string | null = null;
  userName: string = '';
  userEmail: string = '';
  unreadCount: number = 0; 
  currentPostCount: number = 0;

  activePopupImage: string | null = null;
  notificationCount: number = 0;
  notifications: any[] = [];
  notificationsOpen = false;

  followersCount: number = 0;
  followingCount: number = 0;
  isFollowing: boolean = false;
  showFollowListModal: boolean = false;
  followListTitle: string = '';
  followList: any[] = [];
  currentLoggedInUserId: string | null = null;
  
  dashboardStats: any = {
    totalMembers: 0,
    activeSessions: 0,
    securityAlerts: 0,
    totalPosts: 0
  };

  constructor(
    private router: Router,
    public themeService: ThemeService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.extractUserId();
    this.loadProfileImage();
    this.loadUserFromToken();
    this.fetchNotifications();
    this.loadDashboardStats();
    this.loadUnreadCount(); 
    if (this.currentLoggedInUserId) {
      this.loadFollowStats(this.currentLoggedInUserId);
    }
  }

  extractUserId() {
    const token = localStorage.getItem('jwtToken');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        this.currentLoggedInUserId = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || payload.nameid;
      } catch (e) {}
    }
  }

  loadDashboardStats() {
    this.authService.getDashboardStats().subscribe({
      next: (res) => {
        this.dashboardStats = res;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Failed to load dashboard stats', err)
    });
  }

  loadUnreadCount() {
    this.authService.getTotalUnreadCount().subscribe({
      next: (res: any) => {
        this.unreadCount = res.count || 0;
        this.cdr.detectChanges();
      }
    });
  }

  targetUserId: string | null = null;
  targetUser: any = null;
  targetAbout: any = null;

  toggleFollowInList(user: any) {
    this.authService.toggleFollow(user.userId).subscribe({
      next: (res: any) => {
        user.isFollowing = (res.status === 'followed');
        const refreshId = this.targetUserId || this.currentLoggedInUserId;
        if (refreshId) this.loadFollowStats(refreshId);
        this.cdr.detectChanges();
      }
    });
  }

  viewPublicProfile(userId: string) {
    if (!userId) {
      alert('Unable to load profile: Missing User ID.');
      return;
    }
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
        console.error('Failed to load user identity:', err);
        this.targetUser = {
          userId: userId,
          fullName: 'Unknown User',
          email: 'Not available',
          isError: true
        };
        this.cdr.detectChanges();
      }
    });

    this.authService.getAboutInfo(userId).subscribe({
      next: (res: any) => {
        this.targetAbout = res;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.targetAbout = null;
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

  openFollowers(userId?: string) {
    const id = userId || this.targetUserId || this.currentLoggedInUserId;
    if (!id) return;
    this.followListTitle = 'Followers';
    this.authService.getFollowers(id).subscribe({
      next: (res: any[]) => {
        this.followList = (res || []).map(u => ({
          ...u,
          userId: u.userId || u.UserId
        }));
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
        this.followList = (res || []).map(u => ({
          ...u,
          userId: u.userId || u.UserId
        }));
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
    if (img.startsWith('data:image')) return img;
    return `data:image/jpeg;base64,${img}`;
  }

  markAllNotificationsRead() {
    this.authService.markAllNotificationsRead().subscribe({
      next: (res) => {
        this.notificationCount = 0;
        this.notifications = [];
        this.cdr.detectChanges();
      },
      error: (err) => console.error('AdminComponent: Error marking notifications as read', err)
    });
  }

  fetchNotifications() {
    this.authService.getNotifications().subscribe({
      next: (res) => {
        if (res && res.count !== undefined) {
          this.notificationCount = res.count;
          this.notifications = res.data || [];
        }
      },
      error: (err) => console.error('Error fetching notifications', err)
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
      this.userName = storedFullName || storedUserName || 'Admin User';
      this.userEmail = storedEmail || 'admin@example.com';
      return;
    }

    const token = localStorage.getItem('jwtToken');
    if (!token) return;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      this.userName = payload?.name || payload?.unique_name || payload?.email || 'Admin User';
      this.userEmail = payload?.email || payload?.sub || 'admin@example.com';
    } catch (error) {
      console.error('Token parse error:', error);
    }
  }

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  setView(view: string) {
    this.currentView = view;
    if (view === 'profile' || view === 'view-profile') {
      this.activeProfileTab = 'overview';
      if (view === 'profile' && this.currentLoggedInUserId) {
        this.loadFollowStats(this.currentLoggedInUserId);
      }
    }
    if (view !== 'view-profile') {
      this.targetUserId = null;
      this.targetUser = null;
      this.targetAbout = null;
    }
    this.cdr.detectChanges();
  }

  setProfileTab(tab: string) {
    this.activeProfileTab = tab;
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
    
    // Calculate cursor position relative to image
    let x = e.clientX - rect.left;
    let y = e.clientY - rect.top;
    
    // Center lens on cursor
    x = x - (lens.offsetWidth / 2);
    y = y - (lens.offsetHeight / 2);
    
    // Constrain lens within image boundaries
    if (x > img.width - lens.offsetWidth) x = img.width - lens.offsetWidth;
    if (x < 0) x = 0;
    if (y > img.height - lens.offsetHeight) y = img.height - lens.offsetHeight;
    if (y < 0) y = 0;
    
    lens.style.left = x + 'px';
    lens.style.top = y + 'px';
    
    // Calculate zoom ratio
    const cx = result.offsetWidth / lens.offsetWidth;
    const cy = result.offsetHeight / lens.offsetHeight;
    
    result.style.backgroundSize = (img.width * cx) + 'px ' + (img.height * cy) + 'px';
    result.style.backgroundPosition = '-' + (x * cx) + 'px -' + (y * cy) + 'px';
  }

  toggleNotifications(event: Event) {
    event.stopPropagation();
    this.notificationsOpen = !this.notificationsOpen;
  }

  closeNotifications() {
    this.notificationsOpen = false;
  }

  @HostListener('document:click')
  onDocumentClick() {
    this.closeNotifications();
  }

  openImagePopup(event: Event, customUrl?: string) {
    if (event) event.stopPropagation();
    this.activePopupImage = customUrl || this.profileImage || `https://ui-avatars.com/api/?name=${this.userName || 'A'}&background=6366f1&color=fff`;
    this.closeNotifications();
  }

  openImageFromPost(imageUrl: string) {
    this.activePopupImage = imageUrl;
    this.cdr.detectChanges();
  }

  closeImagePopup() {
    this.activePopupImage = null;
  }

  @HostListener('document:keydown.escape')
  onEscPress() {
    this.closeImagePopup();
  }

  logout() {
    localStorage.clear();
    this.router.navigate(['/login']);
  }
}