import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface MasterUserLoginDTO {
  email?: string;
  password?: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  // Use environment.apiUrl to get the base URL
  private apiUrl = `${environment.apiUrl}/ManageUsers/Login_MasterUser`;
  
  private _sessionProfileImage: string | null = null;

  setSessionProfileImage(image: string) {
    this._sessionProfileImage = image;
  }

  getSessionProfileImage(): string | null {
    return this._sessionProfileImage;
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('jwtToken');
  }

  constructor(private http: HttpClient) {}

  getUserIdFromToken(): string {
    const token = localStorage.getItem('jwtToken');
    if (!token) return '';

    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      
      const payload = JSON.parse(jsonPayload);
      return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
          || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
          || payload.nameid
          || payload.sub
          || payload.UserId
          || payload.userId
          || payload.id
          || '';
    } catch (e) {
      console.error('Error decoding JWT', e);
      return '';
    }
  }

  loginMaster(credentials: MasterUserLoginDTO): Observable<any> {
    return this.http.get<any>(`${environment.apiUrl}/ManageUsers/Login_MasterUser`, {
      params: {
        Email: credentials.email || '',
        Password: credentials.password || '',
      },
    });
  }

  requestPasswordReset(email: string): Observable<any> {
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/ForgotPassword_EmailSent`, null, {
      params: {
        Email: email
      }
    });
  }

  // Admin Methods
  getUsersByRole(role: string): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/GetUser_RoleWise`, null, {
      params: { UserRole: role },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  registerUser(formData: FormData): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    const headers: any = {};
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/Register_MasterUser`, formData, {
      headers: headers
    });
  }

  getLockoutUser(endDate: string): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    console.log('AuthService: Requesting lockout users for date:', endDate);
    
    return this.http.get<any>(`${environment.apiUrl}/ManageUsers/GetLockout_User`, {
      params: { LockoutEndDate: endDate },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  releaseLockoutUser(email: string, release: boolean): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.get<any>(`${environment.apiUrl}/ManageUsers/ReleaseLockout_User`, {
      params: { Email: email, ReleaseLockout: release },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  changePassword(data: { Email: string; OldPassword: string; Password: string; ConfirmPassword: string }): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.get<any>(`${environment.apiUrl}/ManageUsers/ForgotPasswordChange`, {
      params: {
        Email: data.Email,
        OldPassword: data.OldPassword,
        Password: data.Password,
        ConfirmPassword: data.ConfirmPassword
      },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  getNotifications(): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.get<any>(`${environment.apiUrl}/ManageUsers/GetNewNotificationUserRegister`, {
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  markAllNotificationsRead(): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    console.log('AuthService: MarkAllNotificationsRead request sent');
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/MarkAllNotificationsRead`, {}, {
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  markNotificationRead(notificationId: number): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    console.log('AuthService: MarkNotificationRead request sent for ID:', notificationId);
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/MarkNotificationRead`, null, {
      params: { notificationId: notificationId.toString() },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  updateAdminProfile(formData: FormData): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/Update_Admin`, formData, {
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  updateManagerProfile(formData: FormData): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/Update_Manager`, formData, {
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  // --- USER SPECIFIC METHODS ---
  updateUserProfile(formData: FormData): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/Update_User`, formData, {
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  createPost(formData: FormData): Observable<string> {
    const token = localStorage.getItem('jwtToken');
    return this.http.post(`${environment.apiUrl}/ManageUsers/UserPost`, formData, {
      headers: { Authorization: `Bearer ${token}` },
      responseType: 'text'
    });
  }

  getUserPosts(targetUserId: string | null = null, currentUserId: string | null = null): Observable<any[]> {
    let params: any = {};
    if (targetUserId) params.userId = targetUserId;
    if (currentUserId) params.currentUserId = currentUserId;
    const token = localStorage.getItem('jwtToken');
    const headers: any = {};
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    return this.http.get<any[]>(`${environment.apiUrl}/ManageUsers/GetUserPosts`, { 
      params,
      headers: headers
    });
  }

  togglePostLike(postId: number, userId: string): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/TogglePostLike`, null, {
      params: { postId: postId.toString(), userId: userId },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  toggleSavePost(postId: number, userId: string): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/ToggleSavePost`, null, {
      params: { postId: postId.toString(), userId: userId },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  getSavedPosts(userId: string): Observable<any[]> {
    const token = localStorage.getItem('jwtToken');
    return this.http.get<any[]>(`${environment.apiUrl}/ManageUsers/GetSavedPosts`, {
      params: { userId: userId },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  getDashboardStats(): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.get<any>(`${environment.apiUrl}/ManageUsers/GetDashboardStats`, {
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  addComment(postId: number, userId: string, content: string): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/AddComment`, null, {
      params: { postId: postId.toString(), userId: userId, content: content },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  getComments(postId: number): Observable<any[]> {
    const token = localStorage.getItem('jwtToken');
    return this.http.get<any[]>(`${environment.apiUrl}/ManageUsers/GetComments`, {
      params: { postId: postId.toString() },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  // --- STORY METHODS (NEW TABLE) ---
  shareStory(formData: FormData): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/ShareStory`, formData, {
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  getStories(userId: string): Observable<any[]> {
    const token = localStorage.getItem('jwtToken');
    return this.http.get<any[]>(`${environment.apiUrl}/ManageUsers/GetStories`, {
      params: { currentUserId: userId },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  markStoryAsSeen(storyId: number, userId: string): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/MarkStoryAsSeen`, null, {
      params: { storyId: storyId.toString(), userId: userId },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  // --- POST MODERATION METHODS ---
  deleteAndArchivePost(postId: number, reason: string): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    // Sending parameters as query string to match the [HttpPost] signature with simple types
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/DeleteAndArchivePost`, null, {
      params: { 
        postId: postId.toString(), 
        reason: reason || '' 
      },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  getArchivedPosts(): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.get<any>(`${environment.apiUrl}/ManageUsers/GetArchivedPosts`, {
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  blurPost(postId: number): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/BlurPost`, null, {
      params: { postId: postId },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  deleteUserFullData(userId: string, reason?: string): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/DeleteUserFullData`, null, {
      params: { 
        userId: userId,
        reason: reason || ''
      },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  updateUserRole(userId: string, newRole: string): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.post<any>(`${environment.apiUrl}/ManageUsers/UpdateUserRole`, null, {
      params: { 
        userId: userId,
        newRole: newRole
      },
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  getDeletedUsers(): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.get<any>(`${environment.apiUrl}/ManageUsers/GetDeletedUsers`, {
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  getDeletedPosts(userId?: string): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    let params: any = {};
    if (userId) params.userId = userId;
    return this.http.get<any>(`${environment.apiUrl}/ManageUsers/GetDeletedPosts`, {
      params,
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  getAuditLogs(): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    return this.http.get<any>(`${environment.apiUrl}/ManageUsers/GetAuditLogs`, {
      headers: { Authorization: `Bearer ${token}` }
    });
  }

  // =========================
  // FRIEND SYSTEM & CHAT (ChatUser Controller)
  // =========================

  private get chatUrl() {
    return `${environment.apiUrl}/ChatUser`;
  }

  private getAuthHeaders() {
    const token = localStorage.getItem('jwtToken');
    return {
      headers: {
        Authorization: `Bearer ${token}`
      }
    };
  }

  searchUsers(key: string): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.chatUrl}/SearchUser?key=${key}`,
      this.getAuthHeaders()
    );
  }

  sendFriendRequest(receiverId: string): Observable<any> {
    return this.http.post<any>(
      `${this.chatUrl}/SendFriendRequest`,
      { receiverId },
      this.getAuthHeaders()
    );
  }

  getFriendRequests(): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.chatUrl}/GetFriendRequests`,
      this.getAuthHeaders()
    );
  }

  acceptFriendRequest(requestId: number): Observable<any> {
    return this.http.post<any>(
      `${this.chatUrl}/AcceptFriendRequest/${requestId}`,
      {},
      this.getAuthHeaders()
    );
  }

  declineFriendRequest(requestId: number): Observable<any> {
    return this.http.post<any>(
      `${this.chatUrl}/DeclineFriendRequest/${requestId}`,
      {},
      this.getAuthHeaders()
    );
  }

  getFriends(): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.chatUrl}/GetFriends`,
      this.getAuthHeaders()
    );
  }

  getChatUsers(): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.chatUrl}/GetChatUsers`,
      this.getAuthHeaders()
    );
  }

  getMessages(friendId: string): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.chatUrl}/GetMessages/${friendId}`,
      this.getAuthHeaders()
    );
  }

  sendChatMessage(payload: any): Observable<any> {
    return this.http.post<any>(
      `${this.chatUrl}/SendMessage`,
      payload,
      this.getAuthHeaders()
    );
  }

  isUserOnline(userId: string): Observable<any> {
    return this.http.get<any>(
      `${this.chatUrl}/IsUserOnline/${userId}`,
      this.getAuthHeaders()
    );
  }

  getUserById(userId: string): Observable<any> {
    return this.http.get<any>(
      `${this.chatUrl}/GetUserInfo/${userId}`,
      this.getAuthHeaders()
    );
  }

  getAboutInfo(userId: string) {
    return this.http.get<any>(
      `${this.chatUrl}/GetAboutInfo/${userId}`,
      this.getAuthHeaders()
    );
  }

  // --- SOCIAL SYSTEM ---
  toggleFollow(userId: string): Observable<any> {
    return this.http.post<any>(
      `${this.chatUrl}/ToggleFollow/${userId}`,
      {},
      this.getAuthHeaders()
    );
  }

  getFollowStats(userId: string): Observable<any> {
    return this.http.get<any>(
      `${this.chatUrl}/GetFollowStats/${userId}`,
      this.getAuthHeaders()
    );
  }

  getFollowers(userId: string): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.chatUrl}/GetFollowers/${userId}`,
      this.getAuthHeaders()
    );
  }

  getFollowing(userId: string): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.chatUrl}/GetFollowing/${userId}`,
      this.getAuthHeaders()
    );
  }

  getSuggestedUsers(): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.chatUrl}/GetSuggestedUsers`,
      this.getAuthHeaders()
    );
  }

  getTotalUnreadCount(): Observable<any> {
    return this.http.get<any>(
      `${this.chatUrl}/GetTotalUnreadCount`,
      this.getAuthHeaders()
    );
  }

  // ==================== GROUP CHAT ====================

  createGroup(dto: { name: string; description?: string; memberIds: string[] }): Observable<any> {
    return this.http.post<any>(`${this.chatUrl}/CreateGroup`, dto, this.getAuthHeaders());
  }

  getMyGroups(): Observable<any[]> {
    return this.http.get<any[]>(`${this.chatUrl}/GetMyGroups`, this.getAuthHeaders());
  }

  getGroupMessages(groupId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.chatUrl}/GetGroupMessages/${groupId}`, this.getAuthHeaders());
  }

  getGroupMembers(groupId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.chatUrl}/GetGroupMembers/${groupId}`, this.getAuthHeaders());
  }

  addGroupMember(groupId: number, memberId: string): Observable<any> {
    return this.http.post<any>(`${this.chatUrl}/AddGroupMember/${groupId}/${memberId}`, {}, this.getAuthHeaders());
  }

  leaveGroup(groupId: number): Observable<any> {
    return this.http.delete<any>(`${this.chatUrl}/LeaveGroup/${groupId}`, this.getAuthHeaders());
  }
}




