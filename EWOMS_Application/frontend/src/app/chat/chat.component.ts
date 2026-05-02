import { ChangeDetectorRef, Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { ChatServices } from '../services/chat-services';

@Component({
  selector: 'app-chat',
  standalone: true,
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css',
  styles: [`:host { display: flex; flex: 1; flex-direction: column; height: 100%; }`],
  imports: [CommonModule, FormsModule]
})
export class ChatComponent implements OnInit {

  @Output() back = new EventEmitter<void>();
  @Output() viewProfile = new EventEmitter<string>();
  @Output() unreadCountChanged = new EventEmitter<void>();

  selectedFriend: any;
  users: any[] = [];
  messages: any[] = [];
  newMessage: string = ''; // Used by ngModel in HTML
  currentUserId: string = '';
  activeTab: string = 'chats'; // 'chats', 'people', 'requests'
  showDetails: boolean = false;
  showVault: boolean = false;
  activeVaultTab: string = 'assets';
  chatTasks: any[] = [];
  sharedMedia: any[] = [];
  selectedImage: string | null = null;
  imagePreview: string | null = null;
  fullScreenImage: string | null = null;
  selectedVideo: string | null = null;
  selectedDocument: string | null = null;
  selectedFileName: string | null = null;

  // Emoji & GIF State
  showEmojiPicker: boolean = false;
  showGifPicker: boolean = false;
  gifSearchKey: string = '';
  
  commonEmojis: string[] = [
    '😀', '😂', '😍', '😎', '🔥', '✨', '🙌', '👍', '❤️', '🎉',
    '🤔', '😭', '😊', '🤩', '💯', '🚀', '🌈', '🍕', '🐱', '🎮'
  ];

  mockGifs: string[] = [
    'https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExNHJmZzIydW95Nnd4NnN6eXp6eXp6eXp6eXp6eXp6eXp6eXp6ZSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/3o7TKDkDbIDJieKbVm/giphy.gif',
    'https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExNHJmZzIydW95Nnd4NnN6eXp6eXp6eXp6eXp6eXp6eXp6eXp6ZSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/l0HlU7eD9N8tYmH56/giphy.gif',
    'https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExNHJmZzIydW95Nnd4NnN6eXp6eXp6eXp6eXp6eXp6eXp6eXp6ZSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/3o7TKVUn7iM8FMEU24/giphy.gif',
    'https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExNHJmZzIydW95Nnd4NnN6eXp6eXp6eXp6eXp6eXp6eXp6eXp6ZSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/26Ff3FNWp3u964f4Y/giphy.gif'
  ];

  searchResults: any[] = [];
  friendRequests: any[] = [];
  searchKey: string = '';

  onlineUsers: Set<string> = new Set();

  // ==================== GROUP CHAT STATE ====================
  groups: any[] = [];
  selectedGroup: any = null;
  groupMessages: any[] = [];
  groupMembers: any[] = [];
  showCreateGroup: boolean = false;
  showGroupDetails: boolean = false;
  showAddGroupMemberModal: boolean = false;
  friendsToAdd: any[] = [];
  newGroupName: string = '';
  newGroupDescription: string = '';
  newGroupMemberIds: string[] = [];
  friends: any[] = [];
  groupImagePreview: string | null = null;
  groupSelectedImage: string | null = null;
  groupSelectedDocument: string | null = null;
  groupSelectedFileName: string | null = null;
  isGroupMode: boolean = false; // true = viewing a group chat

  constructor(
    private auth: AuthService,
    private cdr: ChangeDetectorRef,
    private chatService: ChatServices
  ) {}

  ngOnInit() {
    this.currentUserId = localStorage.getItem('userId') || '';
    this.loadUsers();
    this.loadRequests();

    const token = localStorage.getItem('jwtToken')!;
    if (token) {
      this.chatService.startConnection(token);
    }

    // 💬 RECEIVE MESSAGE (ONLY SOURCE OF TRUTH)
    this.chatService.onMessage((msg: any) => {
      console.log('Incoming Message:', msg);

      // Robust property check for both camelCase and PascalCase
      const sId = msg.senderId || msg.SenderId;
      const rId = msg.receiverId || msg.ReceiverId;
      const mId = msg.id || msg.Id;

      const isCurrentChat =
        this.selectedFriend &&
        (sId === this.selectedFriend.userId ||
         rId === this.selectedFriend.userId);

      if (isCurrentChat) {
        // If I am the receiver and I am currently viewing this chat, mark it as read immediately
        if (rId === this.currentUserId) {
          this.chatService.markAsRead(sId);
          this.unreadCountChanged.emit();
          // Find the user in the sidebar and ensure their unread count stays 0
          const senderUser = this.users.find(u => u.userId === sId);
          if (senderUser) senderUser.unreadCount = 0;
        }

        // prevent duplicates
        const exists = this.messages.some(m => (m.id || m.Id) === mId);

        if (!exists) {
          // Normalize to camelCase for template consistency if needed
          const normalizedMsg = {
            id: mId,
            senderId: sId,
            receiverId: rId,
            message: msg.message || msg.Message,
            image: msg.image || msg.Image,
            video: msg.video || msg.Video,
            document: msg.document || msg.Document,
            fileName: msg.fileName || msg.FileName,
            sentAt: msg.sentAt || msg.SentAt,
            isRead: msg.isRead || msg.IsRead,
            isDelivered: msg.isDelivered || msg.IsDelivered
          };
          this.messages.push(normalizedMsg);
        } else {
          // update status
          const existing = this.messages.find(m => (m.id || m.Id) === mId);
          if (existing) {
            existing.isDelivered = msg.isDelivered || msg.IsDelivered;
            existing.isRead = msg.isRead || msg.IsRead;
          }
        }

        this.scrollToBottom();
      } else {
        // Chat not open, increment unread count if I am the receiver
        if (rId === this.currentUserId) {
          const senderUser = this.users.find(u => u.userId === sId);
          if (senderUser) {
            senderUser.unreadCount = (senderUser.unreadCount || 0) + 1;
            senderUser.lastMessage = msg.message || msg.Message;
            this.unreadCountChanged.emit();
          } else {
            // New friend or not in current list, reload users
            this.loadUsers();
          }
        }
      }

      this.cdr.detectChanges();
      this.updateSharedCollections();
    });

    // 🔵 ON MESSAGES READ (Blue Ticks)
    this.chatService.onMessagesRead((userId: string) => {
      if (this.selectedFriend && this.selectedFriend.userId === userId) {
        // Mark all my sent messages as read
        this.messages.forEach(m => {
          if (m.senderId === this.currentUserId) {
            m.isRead = true;
          }
        });
        this.cdr.detectChanges();
      }
    });

    // 🟢 USER ONLINE/OFFLINE
    this.chatService.onUserStatus((userId: string, isOnline: boolean) => {
      if (isOnline) {
        this.onlineUsers.add(userId);
      } else {
        this.onlineUsers.delete(userId);
      }
      this.cdr.detectChanges();
    });

    // 📢 GROUP MESSAGE
    this.chatService.onGroupMessage((msg: any) => {
      if (this.selectedGroup && (msg.GroupId === this.selectedGroup.groupId || msg.groupId === this.selectedGroup.groupId)) {
        const exists = this.groupMessages.some(m => (m.id || m.Id) === (msg.id || msg.Id));
        if (!exists) {
          this.groupMessages.push({
            id: msg.id || msg.Id,
            senderId: msg.senderId || msg.SenderId,
            senderName: msg.senderName || msg.SenderName,
            senderImage: msg.senderImage || msg.SenderImage,
            message: msg.message || msg.Message,
            image: msg.image || msg.Image,
            video: msg.video || msg.Video,
            document: msg.document || msg.Document,
            fileName: msg.fileName || msg.FileName,
            sentAt: msg.sentAt || msg.SentAt,
          });
          this.scrollToBottom();
          this.cdr.detectChanges();
        }
      }
    });

    this.loadGroups();
  }

  // 🔍 SEARCH USERS
  searchUsers() {
    if (!this.searchKey.trim()) {
      this.searchResults = [];
      return;
    }
    this.auth.searchUsers(this.searchKey).subscribe(res => {
      this.searchResults = (res || []).map((u: any) => ({
        ...u,
        userId: u.userId || u.UserId,
        fullName: u.fullName || u.FullName,
        profileImage: u.profileImage || u.ProfileImage || u.profileImge || u.ProfileImge,
        requestSent: u.isRequested || u.IsRequested || u.requestSent || false
      }));
    });
  }

  // ➕ SEND FRIEND REQUEST
  sendRequest(userId: string) {
    this.auth.sendFriendRequest(userId).subscribe({
      next: (res: any) => {
        const user = this.searchResults.find(u => u.userId === userId);
        if (user) {
          user.requestSent = true;
          // Handle both plain string and object response
          user.requestMessage = typeof res === 'string' ? res : (res?.message || 'Requested');
        }
        this.cdr.detectChanges();
        // Removed the timeout that was filtering out users, to match 'Following' logic
      },
      error: (err) => {
        console.error('Friend Request Error:', err);
        const errorBody = err.error;
        const errorMessage = typeof errorBody === 'string' 
          ? errorBody 
          : (errorBody?.message || errorBody?.title || errorBody?.error || 'Failed to send request');
        
        // If already sent, update UI accordingly
        if (errorMessage.toLowerCase().includes('already') || errorMessage.toLowerCase().includes('pending')) {
          const user = this.searchResults.find(u => u.userId === userId);
          if (user) {
            user.requestSent = true;
            user.requestMessage = 'Requested';
          }
        } else {
          alert(errorMessage);
        }
        this.cdr.detectChanges();
      }
    });
  }

  // 📩 LOAD REQUESTS
  loadRequests() {
    this.auth.getFriendRequests().subscribe(res => {
      this.friendRequests = (res || []).map((req: any) => ({
        ...req,
        sender: req.sender ? {
          ...req.sender,
          userId: req.sender.userId || req.sender.id || req.sender.Id,
          fullName: req.sender.fullName || req.sender.FullName,
          profileImage: req.sender.profileImage || req.sender.ProfileImage
        } : null
      }));
      this.cdr.detectChanges();
    });
  }

  // ✅ ACCEPT REQUEST
  acceptRequest(requestId: number) {
    // 🚀 OPTIMISTIC UPDATE: Clear from UI immediately
    this.friendRequests = this.friendRequests.filter(r => r.id !== requestId);
    this.cdr.detectChanges();

    this.auth.acceptFriendRequest(requestId).subscribe({
      next: () => {
        // Once confirmed by server, refresh users list to show the new friend
        this.loadUsers();
      },
      error: (err) => {
        console.error('Failed to accept request:', err);
        // If it failed, reload to restore the item
        this.loadRequests();
      }
    });
  }

  switchTab(tab: string) {
    this.activeTab = tab;
    if (tab === 'requests') this.loadRequests();
    if (tab === 'chats') this.loadUsers();
    if (tab === 'groups') this.loadGroups();
    this.cdr.detectChanges();
  }

  declineRequest(requestId: number) {
    this.friendRequests = this.friendRequests.filter(r => r.id !== requestId);
    this.cdr.detectChanges();

    this.auth.declineFriendRequest(requestId).subscribe({
      next: () => {
        console.log('Request declined');
      },
      error: (err) => {
        console.error('Failed to decline request:', err);
        this.loadRequests();
      }
    });
  }

  // 👥 LOAD USERS
  loadUsers() {
    this.auth.getChatUsers().subscribe(res => {
      const rawUsers = res || [];
      const uniqueMap = new Map();
      rawUsers.forEach((u: any) => {
        const userId = u.userId || u.UserId;
        if (!uniqueMap.has(userId)) {
          uniqueMap.set(userId, {
            ...u,
            userId: userId,
            fullName: u.fullName || u.FullName,
            profileImage: u.profileImage || u.ProfileImage || u.profileImge || u.ProfileImge
          });
        }
      });
      this.users = Array.from(uniqueMap.values());
      this.cdr.detectChanges();
    });
  }

  // 👤 SELECT USER
  selectUser(user: any) {
    this.selectedGroup = null;
    this.isGroupMode = false;
    this.selectedFriend = user;
    this.messages = []; // Clear immediately for visual feedback
    this.cdr.detectChanges();
    
    this.loadMessages();
    this.checkOnlineStatus(user.userId);
    this.chatService.markAsRead(user.userId);
    
    // Optimistic UI update: Clear unread count
    user.unreadCount = 0;
    this.unreadCountChanged.emit();
    this.cdr.detectChanges();
  }

  markAllAsRead() {
    this.users.forEach(u => {
      if (u.unreadCount > 0) {
        this.chatService.markAsRead(u.userId);
        u.unreadCount = 0;
      }
    });
    this.unreadCountChanged.emit();
    this.cdr.detectChanges();
  }

  checkOnlineStatus(userId: string) {
    this.auth.isUserOnline(userId).subscribe(res => {
      if (res.isOnline) {
        this.onlineUsers.add(userId);
      } else {
        this.onlineUsers.delete(userId);
      }
      this.cdr.detectChanges();
    });
  }

  // 💬 LOAD MESSAGES FROM DB
  loadMessages() {
    if (!this.selectedFriend?.userId) return;

    this.auth.getMessages(this.selectedFriend.userId)
      .subscribe(res => {
        this.messages = res || [];
        this.updateSharedCollections();
        this.scrollToBottom();
        this.cdr.detectChanges();
      });
  }

  updateSharedCollections() {
    // Collect all media and tasks from the current conversation
    this.sharedMedia = this.messages.filter(m => m.image || m.video || m.document);
    this.chatTasks = this.messages
      .filter(m => m.message?.startsWith('/task'))
      .map(m => ({
        id: m.id,
        senderId: m.senderId,
        text: m.message.replace('/task', '').trim(),
        sentAt: m.sentAt,
        status: m.isRead ? 'Completed' : 'Pending'
      }));
  }

  // ✉ SEND MESSAGE (UNIFIED)
  sendMessage() {
    if (this.selectedGroup) {
      this.sendGroupMsg();
      return;
    }

    if ((!this.newMessage.trim() && !this.selectedImage && !this.selectedVideo && !this.selectedDocument) || !this.selectedFriend?.userId) return;

    const text = this.newMessage.trim();
    const img = this.selectedImage;
    const vid = this.selectedVideo;
    const doc = this.selectedDocument;
    const fileName = this.selectedFileName;

    console.log('✉️ Sending to:', this.selectedFriend.userId, 'Text:', text);

    this.chatService.sendMessage(
      this.selectedFriend.userId,
      text,
      img ?? undefined,
      vid ?? undefined,
      doc ?? undefined,
      fileName ?? undefined
    )?.then(() => {
      console.log('✅ Message sent confirmed by server');
      this.newMessage = '';
      this.selectedImage = null;
      this.imagePreview = null;
      this.selectedVideo = null;
      this.selectedDocument = null;
      this.selectedFileName = null;
      this.cdr.detectChanges();
    }).catch(err => {
      console.error('❌ SignalR Send Error:', err);
    });
  }

  openGroupInfo() {
    this.showGroupDetails = true;
    this.cdr.detectChanges();
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.clearAttachments(); // Clear previous selection
      const type = file.type;
      const reader = new FileReader();

      if (type.startsWith('image/')) {
        reader.onload = (e: any) => {
          this.selectedImage = e.target.result.split(',')[1];
          this.imagePreview = e.target.result;
          this.cdr.detectChanges();
        };
        reader.readAsDataURL(file);
      } else if (type.startsWith('video/')) {
        reader.onload = (e: any) => {
          this.selectedVideo = e.target.result.split(',')[1];
          this.cdr.detectChanges();
        };
        reader.readAsDataURL(file);
      } else {
        // Assume Document for other types
        this.selectedFileName = file.name;
        reader.onload = (e: any) => {
          this.selectedDocument = e.target.result.split(',')[1];
          this.cdr.detectChanges();
        };
        reader.readAsDataURL(file);
      }
    }
  }

  clearAttachments() {
    this.selectedImage = null;
    this.imagePreview = null;
    this.selectedVideo = null;
    this.selectedDocument = null;
    this.selectedFileName = null;
    this.cdr.detectChanges();
  }



  // 📜 SCROLL TO BOTTOM
  scrollToBottom() {
    setTimeout(() => {
      const chatBody = document.querySelector('.message-container');
      if (chatBody) {
        chatBody.scrollTop = chatBody.scrollHeight;
      }
    }, 100);
  }

  // 🟢 ONLINE CHECK
  isOnline(userId: string): boolean {
    return this.onlineUsers.has(userId);
  }

  getAvatar(user: any) {
    if (!user) return null;
    const img = user.profileImage || user.ProfileImage || user.profileImge || user.ProfileImge;
    if (!img) return null;
    if (img.startsWith('data:image')) return img;
    return `data:image/jpeg;base64,${img}`;
  }

  onViewProfile() {
    console.log('ChatComponent: onViewProfile clicked. selectedFriend:', this.selectedFriend);

    if (this.selectedFriend && this.selectedFriend.userId) {
      console.log('ChatComponent: Emitting userId:', this.selectedFriend.userId);
      this.viewProfile.emit(this.selectedFriend.userId);
      this.showDetails = false;
    } else {
      console.error('ChatComponent: selectedFriend or userId is missing!', this.selectedFriend);
    }
  }

  openImagePopup(image: string) {
    this.fullScreenImage = image;
    this.cdr.detectChanges();
  }

  closeImagePopup() {
    this.fullScreenImage = null;
    this.cdr.detectChanges();
  }

  // EMOJI & GIF LOGIC
  addEmoji(emoji: string) {
    this.newMessage += emoji;
    this.showEmojiPicker = false;
  }

  toggleEmojiPicker() {
    this.showEmojiPicker = !this.showEmojiPicker;
    this.showGifPicker = false;
  }

  toggleGifPicker() {
    this.showGifPicker = !this.showGifPicker;
    this.showEmojiPicker = false;
  }

  async selectGif(gifUrl: string) {
    if (!this.selectedFriend?.userId) return;

    // We can send the GIF URL directly or convert to base64 if your backend requires it
    // For simplicity with your current ChatMessage model, we'll send it as the 'message' or 'image'
    // Let's send it as an image message by fetching and converting to base64
    try {
      const response = await fetch(gifUrl);
      const blob = await response.blob();
      const reader = new FileReader();
      reader.onloadend = () => {
        const base64 = (reader.result as string).split(',')[1];
        this.chatService.sendMessage(this.selectedFriend.userId, '', base64);
        this.showGifPicker = false;
        this.cdr.detectChanges();
      };
      reader.readAsDataURL(blob);
    } catch (e) {
      console.error('Error sending GIF:', e);
    }
  }

  goBack() {
    this.back.emit();
  }

  // ==================== GROUP CHAT METHODS ====================

  loadGroups() {
    this.auth.getMyGroups().subscribe({
      next: (res) => { this.groups = res || []; this.cdr.detectChanges(); },
      error: (err) => console.error('Failed to load groups', err)
    });
  }

  loadFriends() {
    this.auth.getFriends().subscribe({
      next: (res: any[]) => { 
        this.friends = (res || []).map(f => ({
          ...f,
          userId: f.userId || f.id || f.Id,
          fullName: f.fullName || f.FullName,
          profileImage: f.profileImage || f.ProfileImage
        })); 
        this.cdr.detectChanges(); 
      },
      error: (err) => console.error('Failed to load friends', err)
    });
  }

  openAddGroupMemberModal() {
    this.showAddGroupMemberModal = true;
    this.auth.getFriends().subscribe({
      next: (res: any[]) => { 
        const allFriends = (res || []).map(f => ({
          ...f,
          userId: f.userId || f.id || f.Id,
          fullName: f.fullName || f.FullName,
          profileImage: f.profileImage || f.ProfileImage
        })); 
        const existingIds = this.groupMembers.map(m => m.userId || m.UserId);
        this.friendsToAdd = allFriends.filter(f => !existingIds.includes(f.userId));
        this.cdr.detectChanges(); 
      },
      error: (err) => console.error('Failed to load friends for group', err)
    });
  }

  closeAddGroupMemberModal() {
    this.showAddGroupMemberModal = false;
  }

  addGroupMember(memberId: string) {
    if (!this.selectedGroup || !memberId) return;
    this.auth.addGroupMember(this.selectedGroup.groupId, memberId).subscribe({
      next: () => {
        // Refresh members
        this.auth.getGroupMembers(this.selectedGroup.groupId).subscribe({
          next: (members) => { 
            this.groupMembers = members || []; 
            this.friendsToAdd = this.friendsToAdd.filter(f => (f.id || f.Id || f.userId || f.UserId) !== memberId);
            this.cdr.detectChanges(); 
          }
        });
      },
      error: (err) => console.error('Failed to add group member', err)
    });
  }

  openGroup(group: any) {
    // Leave previous group if any
    if (this.selectedGroup) {
      this.chatService.leaveGroup(this.selectedGroup.groupId);
    }
    this.selectedFriend = null;
    this.selectedGroup = group;
    this.isGroupMode = true;
    this.groupMessages = []; // Clear immediately
    this.groupMembers = [];
    this.cdr.detectChanges(); // Trigger UI change to group mode immediately

    this.chatService.joinGroup(group.groupId);
    this.auth.getGroupMessages(group.groupId).subscribe({
      next: (msgs) => {
        this.groupMessages = (msgs || []).map((m: any) => ({
          id: m.id || m.Id,
          senderId: m.senderId || m.SenderId,
          senderName: m.senderName || m.SenderName,
          senderImage: m.senderImage || m.SenderImage,
          message: m.message || m.Message,
          image: m.image || m.Image,
          video: m.video || m.Video,
          document: m.document || m.Document,
          fileName: m.fileName || m.FileName,
          sentAt: m.sentAt || m.SentAt,
        }));
        this.scrollToBottom();
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Failed to load group messages', err)
    });
    this.auth.getGroupMembers(group.groupId).subscribe({
      next: (members) => {
        this.groupMembers = members || [];
        // Cache members on the group object so the list can show avatars
        group.members = this.groupMembers;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Failed to load group members', err)
    });
  }

  sendGroupMsg() {
    if (!this.selectedGroup || (!this.newMessage.trim() && !this.groupSelectedImage && !this.groupSelectedDocument)) return;
    this.chatService.sendGroupMessage(
      this.selectedGroup.groupId,
      this.newMessage,
      this.groupSelectedImage || undefined,
      undefined,
      this.groupSelectedDocument || undefined,
      this.groupSelectedFileName || undefined
    );
    this.newMessage = '';
    this.groupSelectedImage = null;
    this.groupImagePreview = null;
    this.groupSelectedDocument = null;
    this.groupSelectedFileName = null;
    this.cdr.detectChanges();
  }

  openCreateGroup() {
    this.showCreateGroup = true;
    this.newGroupName = '';
    this.newGroupDescription = '';
    this.newGroupMemberIds = [];
    this.loadFriends();
  }

  closeCreateGroup() {
    this.showCreateGroup = false;
  }

  toggleGroupMember(userId: string) {
    const idx = this.newGroupMemberIds.indexOf(userId);
    if (idx === -1) {
      this.newGroupMemberIds.push(userId);
    } else {
      this.newGroupMemberIds.splice(idx, 1);
    }
  }

  isMemberSelected(userId: string): boolean {
    return this.newGroupMemberIds.includes(userId);
  }

  createGroup() {
    if (!this.newGroupName.trim()) return;
    this.auth.createGroup({
      name: this.newGroupName,
      description: this.newGroupDescription,
      memberIds: this.newGroupMemberIds
    }).subscribe({
      next: () => {
        this.showCreateGroup = false;
        this.loadGroups();
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Failed to create group', err)
    });
  }

  leaveGroup() {
    if (!this.selectedGroup) return;
    if (!confirm(`Leave group "${this.selectedGroup.name}"?`)) return;
    this.auth.leaveGroup(this.selectedGroup.groupId).subscribe({
      next: () => {
        this.selectedGroup = null;
        this.isGroupMode = false;
        this.groupMessages = [];
        this.loadGroups();
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Failed to leave group', err)
    });
  }

  isGroupSender(msg: any): boolean {
    return (msg.senderId || msg.SenderId) === this.currentUserId;
  }

  // Returns up to 3 members for the group avatar cluster in the list
  getGroupPreviewMembers(group: any): any[] {
    const members = group.members || [];
    return members.slice(0, 3);
  }

  getGroupMemberAvatar(member: any): string | null {
    const img = member.profileImage || member.ProfileImage;
    if (!img || img.trim() === '' || img === 'null' || img === 'undefined') return null;
    if (img.startsWith('data:image') || img.startsWith('http')) return img;
    return `data:image/jpeg;base64,${img}`;
  }

  onGroupFileSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    if (file.type.startsWith('image/')) {
      const reader = new FileReader();
      reader.onload = () => {
        const b64 = (reader.result as string).split(',')[1];
        this.groupSelectedImage = b64;
        this.groupImagePreview = reader.result as string;
        this.cdr.detectChanges();
      };
      reader.readAsDataURL(file);
    } else {
      const reader = new FileReader();
      reader.onload = () => {
        const b64 = (reader.result as string).split(',')[1];
        this.groupSelectedDocument = b64;
        this.groupSelectedFileName = file.name;
        this.cdr.detectChanges();
      };
      reader.readAsDataURL(file);
    }
  }

  clearGroupAttachment() {
    this.groupSelectedImage = null;
    this.groupImagePreview = null;
    this.groupSelectedDocument = null;
    this.groupSelectedFileName = null;
  }
}
