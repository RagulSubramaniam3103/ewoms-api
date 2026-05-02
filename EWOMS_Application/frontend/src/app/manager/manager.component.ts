import { Component, OnInit, HostListener, ChangeDetectorRef, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ChangePasswordComponent } from '../admin/change-password/change-password.component';
import { UserManagementComponent } from '../admin/user-management/user-management.component';
import { RegisterUserComponent } from '../admin/register-user/register-user.component';
import { LockoutManagementComponent } from '../admin/lockout-management/lockout-management.component';
import { ChatComponent } from '../chat/chat.component';
import { AuthService } from '../services/auth.service';
import { ManagerEditProfileComponent } from './edit-profile/edit-profile.component';
import { UserPostsComponent } from '../user/user-posts/user-posts.component';
import { CreatePostComponent } from '../user/create-post/create-post.component';
import { UserProfileComponent } from '../user/user-profile/user-profile.component';
import { CommunityFeedComponent } from '../shared/community-feed.component';

@Component({
  selector: 'app-manager',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule,
    CommunityFeedComponent,
    UserManagementComponent,
    RegisterUserComponent,
    LockoutManagementComponent,
    ManagerEditProfileComponent,
    CreatePostComponent,
    UserProfileComponent,
    ChatComponent
  ],
  templateUrl: './manager.component.html',
  styleUrls: ['./manager.component.css']
})
export class ManagerComponent implements OnInit {
  sidebarCollapsed = false;
  currentView = 'community-feed';
  activeProfileTab: string = 'overview';
  
  @ViewChild('zoomLens') zoomLens!: ElementRef;
  @ViewChild('zoomImage') zoomImage!: ElementRef;
  @ViewChild('zoomResult') zoomResult!: ElementRef;
  
  dashboardStats: any = {
    totalMembers: 0,
    activeSessions: 0,
    securityAlerts: 0,
    totalPosts: 0
  };
  userName = '';
  userEmail = '';
  profileImage: string | null = null;
  unreadCount: number = 0;
  currentPostCount: number = 0;
  showImagePopup = false;
  selectedImageUrl: string = '';

  onProfileUpdated() {
    this.loadProfileImage();
    this.loadUserFromToken();
  }
  today = new Date();

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
  isProfileModalOpen: boolean = false;

  suggestedUsers: any[] = [];
  
  constructor(
    private router: Router, 
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.extractUserId();
    this.loadUserFromToken();
    this.loadProfileImage();
    this.loadUnreadCount(); 
    this.fetchNotifications();
    this.loadDashboardStats();
    if (this.currentLoggedInUserId) {
      this.loadFollowStats(this.currentLoggedInUserId);
    }
    this.loadSuggestions();
  }

  loadSuggestions() {
    this.authService.getSuggestedUsers().subscribe({
      next: (res) => {
        this.suggestedUsers = (res || []).slice(0, 5);
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Failed to load suggestions', err)
    });
  }

  followUser(userId: string) {
    this.authService.toggleFollow(userId).subscribe({
      next: () => {
        this.loadSuggestions();
      }
    });
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
        this.dashboardStats = res || { totalMembers: 0, activeSessions: 0, securityAlerts: 0, totalPosts: 0 };
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load dashboard stats', err);
        this.dashboardStats = { totalMembers: 0, activeSessions: 0, securityAlerts: 0, totalPosts: 0 };
      }
    });
  }

  loadUnreadCount() {
    this.authService.getTotalUnreadCount().subscribe({
      next: (res: any) => {
        this.unreadCount = res?.count || 0;
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

  markAllNotificationsRead() {
    this.authService.markAllNotificationsRead().subscribe({
      next: (res) => {
        this.notificationCount = 0;
        this.notifications = [];
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
      this.userName = storedFullName || storedUserName || 'Manager';
      this.userEmail = storedEmail || 'manager@company.com';
      return;
    }

    const token = localStorage.getItem('jwtToken');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const nameKey = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
        const emailKey = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
        this.userName = payload[nameKey] || payload.name || 'Manager';
        this.userEmail = payload[emailKey] || payload.email || 'manager@company.com';
      } catch (e) {}
    }
  }

  setView(view: string) {
    this.currentView = view;
    this.cdr.detectChanges(); // Instant sidebar update
    if (view === 'profile' || view === 'view-profile') this.activeProfileTab = 'overview';
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

  toggleNotifications(event: Event) {
    event.stopPropagation();
    this.notificationsOpen = !this.notificationsOpen;
  }

  closeNotifications() { this.notificationsOpen = false; }

  @HostListener('document:click')
  onDocumentClick() { this.closeNotifications(); }

  openImagePopup(event: Event, customUrl?: string) {
    if (event) event.stopPropagation();
    this.activePopupImage = customUrl || this.profileImage || `https://ui-avatars.com/api/?name=${this.userName || 'M'}&background=6366f1&color=fff`;
    this.closeNotifications();
  }

  openImageFromPost(imageUrl: string) {
    this.activePopupImage = imageUrl;
    this.cdr.detectChanges();
  }

  closeImagePopup() { this.activePopupImage = null; }

  @HostListener('document:keydown.escape')
  onEscPress() { this.closeImagePopup(); }

  logout() {
    localStorage.clear();
    this.router.navigate(['/login']);
  }
}
