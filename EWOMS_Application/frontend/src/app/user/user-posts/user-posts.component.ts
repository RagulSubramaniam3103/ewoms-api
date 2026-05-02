import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, ChangeDetectorRef, ViewChild, ElementRef, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

import { FormsModule } from '@angular/forms';

export interface EnterpriseStory {
  id: number;
  userName: string;
  userImage: string;
  hasUnseen: boolean;
  isLive?: boolean;
  segments: any[]; // Grouped posts
  currentSegmentIndex: number;
}

@Component({
  selector: 'app-user-posts',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-posts.component.html',
  styleUrls: ['./user-posts.component.css'],
  styles: [`:host { display: flex; flex: 1; flex-direction: column; height: 100%; overflow: hidden; }`]
})
export class UserPostsComponent implements OnInit, OnChanges {
  @Input() showOnlyMyPosts: boolean = false;
  @Input() showSavedOnly: boolean = false;
  @Input() userId: string | null = null;
  @Input() viewMode: 'list' | 'grid' | 'table' = 'list';
  @Input() role: string = 'user';
  @Input() userAvatar: string | null = null;
  @Output() viewProfile = new EventEmitter<string>();
  @Output() openImage = new EventEmitter<string>();
  @Output() postCount = new EventEmitter<number>();
  @Output() createNewPost = new EventEmitter<void>();

  posts: any[] = [];
  selectedPost: any = null;
  isLoading: boolean = false;
  isLoadingStories: boolean = false;
  currentUserId: string = '';
  
  // Comments System
  showComments: boolean = false;
  activeCommentPostId: number | null = null;
  currentPostComments: any[] = [];
  newCommentText: string = '';
  isSubmittingComment: boolean = false;
  isLoadingComments: boolean = false;
  
  // Story Creation
  showStoryCreator: boolean = false;
  storyCaption: string = '';
  selectedStoryFile: File | null = null;
  storyImagePreview: string | null = null;
  storyVideoPreview: string | null = null;
  isStoryVideo: boolean = false;
  isSharingStory: boolean = false;
  activeMenuPostId: number | null = null; // Track which post menu is open

  // STORIES DATA
  activeStory: EnterpriseStory | null = null;
  stories: EnterpriseStory[] = [];
  myStory: EnterpriseStory | null = null;
  isMuted: boolean = true; // Default to muted for compliance
  
  // Amazon-Style Zoom System
  @ViewChild('zoomLens') zoomLens!: ElementRef;
  @ViewChild('zoomResult') zoomResult!: ElementRef;
  @ViewChild('zoomImage') zoomImage!: ElementRef;

  constructor(
    private authService: AuthService, 
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.currentUserId = this.getUserIdFromToken();
    this.loadPosts();
    this.loadStories();
  }

  loadStories() {
    this.currentUserId = this.getUserIdFromToken();
    if (!this.currentUserId) return;

    this.isLoadingStories = true;
    this.authService.getStories(this.currentUserId).subscribe({
      next: (res: any) => {
        const allStories = (res || []).map((s: any) => ({
          id: s.userId || s.UserId,
          userName: s.userName || s.UserName,
          userImage: s.userImage || s.UserImage,
          hasUnseen: s.hasUnseen !== undefined ? s.hasUnseen : s.HasUnseen,
          isLive: Math.random() > 0.8,
          segments: (s.segments || s.Segments || []).map((seg: any) => {
            const rawData = seg.displayImage || seg.DisplayImage || '';
            // Heuristic: Check for common video signatures in base64 (MP4/WebM)
            // MP4 often starts with 'AAAA' (\x00\x00\x00) or 'ftyp'
            const isVideoHeuristic = rawData.startsWith('data:video') || 
                                     rawData.startsWith('AAAA') || 
                                     rawData.startsWith('GkXf'); // WebM/MKV
            
            return {
              id: seg.id || seg.Id,
              displayImage: rawData,
              caption: seg.caption || seg.Caption,
              isSeen: seg.isSeen !== undefined ? seg.isSeen : seg.IsSeen,
              isVideo: isVideoHeuristic
            };
          }),
          currentSegmentIndex: 0
        }));

        this.myStory = allStories.find((s: any) => String(s.id) === String(this.currentUserId)) || null;
        this.stories = allStories.filter((s: any) => String(s.id) !== String(this.currentUserId));

        console.log('UserPosts Stories Loaded:', this.stories.length, 'MyStory:', !!this.myStory);
        this.isLoadingStories = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading stories', err);
        this.isLoadingStories = false;
        this.cdr.detectChanges();
      }
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    // Reload posts whenever userId input changes (e.g., switching viewed profile)
    if (changes['userId'] && !changes['userId'].firstChange) {
      this.loadPosts();
      this.cdr.detectChanges();
    }
  }

  loadPosts() {
    const currentUserId = this.getUserIdFromToken();
    console.log('UserPosts: currentUserId from token:', currentUserId);
    let filter = '';
    if (this.userId) {
      filter = this.userId;
    } else if (this.showOnlyMyPosts) {
      filter = currentUserId;
    }
    
    console.log('UserPosts: Loading posts with filter:', filter, 'currentUserId:', currentUserId, 'showSavedOnly:', this.showSavedOnly);
    this.isLoading = true;
    
    try {
      const postObs = this.showSavedOnly 
        ? this.authService.getSavedPosts(currentUserId) 
        : this.authService.getUserPosts(filter, currentUserId);

      console.log('UserPosts: postObs created, subscribing...');

      postObs.subscribe({
        next: (res: any) => {
          console.log('UserPosts: Raw API Response Received:', res);
        // Fast mapping
        const processed = (res || []).map((p: any) => {
          // POST CONTENT IMAGE (The photo shared in the post)
          p.displayImage = p.postImage || p.PostImage || p.image || p.Image || null;
          
          // AUTHOR PROFILE IMAGE (The person who posted)
          p.authorImage = p.userImage || p.UserImage || p.profileImage || p.ProfileImage || p.profileimage || null;
          
          p.isRevealed = false;
          p.newCommentText = ''; 
          return p;
        });
        
        console.log('UserPosts: Processed posts:', processed.length);
        this.posts = processed;
        if (this.stories.length === 0) {
          this.generateStoriesFromPosts(processed);
        }
        this.postCount.emit(processed.length);
        this.isLoading = false;
        this.cdr.markForCheck(); 
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.isLoading = false;
        console.error('UserPosts: API Error:', err);
      }
    });
    } catch (e) {
      console.error('UserPosts: Execution Error in loadPosts:', e);
    }
  }
  generateStoriesFromPosts(allPosts: any[]) {
    // 1. Filter posts from the last 24 hours only
    const twentyFourHoursAgo = new Date(Date.now() - 24 * 60 * 60 * 1000);
    const recentPosts = allPosts.filter(p => {
      const postDate = p.createdAt ? new Date(p.createdAt) : new Date();
      return postDate >= twentyFourHoursAgo;
    });

    // 2. Group by user
    const userMap = new Map<string, any[]>();
    
    recentPosts.forEach(p => {
      // Use loose equality or explicit string conversion for safety
      if (String(p.userId) === String(this.currentUserId)) return;
      if (!userMap.has(p.userId)) {
        userMap.set(p.userId, []);
      }
      userMap.get(p.userId)!.push(p);
    });

    // 3. Convert to story models with segments
    const liveStories: EnterpriseStory[] = Array.from(userMap.entries()).map((entry: any) => {
      const [userId, posts] = entry;
      const latestPost = posts[0]; 
      return {
        id: latestPost.id,
        userName: latestPost.userName || 'Team Member',
        userImage: latestPost.authorImage,
        hasUnseen: true,
        isLive: Math.random() > 0.85,
        segments: posts,
        currentSegmentIndex: 0
      } as EnterpriseStory;
    });

    this.stories = [...liveStories]; // Removed "Your Story" from array to prevent redundancy
    this.cdr.detectChanges();
  }

  private getUserIdFromToken(): string {
    const token = localStorage.getItem('jwtToken');
    if (!token) return '';
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      
      const payload = JSON.parse(jsonPayload);
      console.log('JWT Payload:', payload);

      return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
          || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
          || payload.nameid
          || payload.sub
          || payload.UserId
          || payload.userId
          || payload.id
          || '';
    } catch (e) {
      console.error('Error decoding JWT for userId', e);
      return '';
    }
  }

  revealPost(event: Event, post: any) {
    if (post.isBlurred && !post.isRevealed) {
      event.stopPropagation();
      
      // ONLY ALLOW OWNER TO REVEAL
      if (post.userId === this.currentUserId) {
        post.isRevealed = true;
        this.cdr.detectChanges();
      } else {
        console.warn('Privacy: Only the owner can unveil this restricted content.');
      }
    }
  }

  likePost(post: any) {
    const currentUserId = this.getUserIdFromToken();
    if (!currentUserId) {
      alert('You must be logged in to like posts.');
      return;
    }

    // Trigger heart animation if not already liked
    if (!post.isLiked) {
      post.showHeartAnim = true;
      setTimeout(() => {
        post.showHeartAnim = false;
        this.cdr.detectChanges();
      }, 800);
    }

    this.authService.togglePostLike(post.id, currentUserId).subscribe({
      next: (res: any) => {
        post.isLiked = res.isLiked;
        post.likeCount = res.likeCount;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error toggling like', err)
    });
  }

  savePost(post: any) {
    const currentUserId = this.getUserIdFromToken();
    console.log('UserPosts: Attempting to save post:', post.id, 'for user:', currentUserId);
    if (!currentUserId) return;

    this.authService.toggleSavePost(post.id, currentUserId).subscribe({
      next: (res: any) => {
        console.log('UserPosts: Save result:', res);
        post.isSaved = res.isSaved;
        const msg = res.isSaved ? 'Intelligence successfully synchronized to your vault.' : 'Intelligence removed from your vault.';
        alert(msg);
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error toggling save', err);
        alert('Operational Failure: Could not synchronize intelligence to vault.');
      }
    });
  }

  openPopup(post: any) {
    if (this.viewMode === 'grid' || this.viewMode === 'table') {
      // PRIVACY CHECK: If blurred, only the owner or an Admin/Manager can open the high-fidelity zoom
      if (post.isBlurred && post.userId !== this.currentUserId && this.role === 'user') {
        console.warn('Privacy: Access to high-fidelity zoom restricted for this content.');
        return;
      }

      // Open the Amazon Zoom Modal for Grid (My Contributions) or Table (Management Hub)
      this.openImage.emit('data:image/jpeg;base64,' + post.displayImage);
    }
  }

  onViewProfile(userId: string) {
    this.viewProfile.emit(userId);
  }

  closePopup() {
    this.selectedPost = null;
    this.cdr.detectChanges();
  }

  // --- COMMENTS LOGIC ---
  toggleComments(post: any) {
    if (this.activeCommentPostId === post.id && this.showComments) {
      this.showComments = false;
      this.activeCommentPostId = null;
      return;
    }

    this.activeCommentPostId = post.id;
    this.showComments = true;
    this.loadComments(post.id);
  }

  loadComments(postId: number) {
    this.isLoadingComments = true;
    this.authService.getComments(postId).subscribe({
      next: (res) => {
        this.currentPostComments = res;
        this.isLoadingComments = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading comments', err);
        this.isLoadingComments = false;
        this.cdr.detectChanges();
      }
    });
  }

  // --- AMAZON ZOOM LOGIC (FEED VERSION) ---
  toggleZoom(show: boolean, lens: any, result: any, img: any, displayImage: string) {
    if (!lens || !result) return;
    lens.style.display = show ? 'block' : 'none';
    result.style.display = show ? 'block' : 'none';
    
    if (show && displayImage) {
      result.style.backgroundImage = `url('data:image/jpeg;base64,${displayImage}')`;
      result.style.backgroundSize = `${img.width * 2.5}px ${img.height * 2.5}px`;
    }
  }

  onZoomMove(event: MouseEvent, lens: any, result: any, img: any) {
    if (!lens || !result || !img) return;
    const rect = img.getBoundingClientRect();
    let x = event.clientX - rect.left - (lens.offsetWidth / 2);
    let y = event.clientY - rect.top - (lens.offsetHeight / 2);
    if (x > img.width - lens.offsetWidth) x = img.width - lens.offsetWidth;
    if (x < 0) x = 0;
    if (y > img.height - lens.offsetHeight) y = img.height - lens.offsetHeight;
    if (y < 0) y = 0;
    lens.style.left = x + 'px';
    lens.style.top = y + 'px';
    const cx = result.offsetWidth / lens.offsetWidth;
    const cy = result.offsetHeight / lens.offsetHeight;
    result.style.backgroundPosition = `-${x * cx}px -${y * cy}px`;
  }

  submitComment(post?: any) {
    const currentUserId = this.getUserIdFromToken();
    const targetPost = post || this.posts.find(p => p.id === this.activeCommentPostId);
    const commentText = targetPost ? targetPost.newCommentText : this.newCommentText;

    if (!currentUserId || !targetPost || !commentText?.trim()) return;

    this.isSubmittingComment = true;
    this.authService.addComment(targetPost.id, currentUserId, commentText).subscribe({
      next: (res) => {
        if (post) {
          post.newCommentText = '';
          post.commentCount++; // Optimistic update
        } else {
          this.newCommentText = '';
        }
        this.isSubmittingComment = false;
        this.loadComments(targetPost.id);
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isSubmittingComment = false;
        console.error('Error adding comment', err);
      }
    });
  }

  getAuthorAvatar(post: any) {
    if (!post) return null;
    const img = post.authorImage;
    if (!img) return `https://ui-avatars.com/api/?name=${post.userName || 'U'}&background=7c3aed&color=fff`;
    if (img.startsWith('data:image')) return img;
    return `data:image/jpeg;base64,${img}`;
  }

  closeComments() {
    this.showComments = false;
    this.activeCommentPostId = null;
  }

  // STORIES LOGIC
  openStory(story: EnterpriseStory) {
    this.activeStory = story;
    this.activeStory.currentSegmentIndex = 0;
    console.log('Opening Story:', this.activeStory);
    this.markSegmentAsSeen(story);
    this.startStoryTimer();
    this.cdr.detectChanges();
  }

  markSegmentAsSeen(story: EnterpriseStory) {
    if (!this.currentUserId || !story) return;
    const segment = story.segments[story.currentSegmentIndex];
    if (segment.isSeen) return;

    this.authService.markStoryAsSeen(segment.id, this.currentUserId).subscribe({
      next: () => {
        segment.isSeen = true;
        // Check if all segments are now seen
        story.hasUnseen = story.segments.some(s => !s.isSeen);
        this.cdr.detectChanges();
      }
    });
  }

  private storyTimer: any;
  startStoryTimer() {
    if (this.storyTimer) clearInterval(this.storyTimer);
    this.storyTimer = setInterval(() => {
      this.nextSegment();
    }, 5000); // 5 seconds per segment
  }

  nextSegment(event?: Event) {
    if (event) event.stopPropagation();
    if (!this.activeStory) return;

    if (this.activeStory.currentSegmentIndex < this.activeStory.segments.length - 1) {
      this.activeStory.currentSegmentIndex++;
      this.markSegmentAsSeen(this.activeStory);
      this.startStoryTimer(); // Reset timer
    } else {
      this.closeStory();
    }
    this.cdr.detectChanges();
  }

  prevSegment(event: Event) {
    event.stopPropagation();
    if (!this.activeStory) return;

    if (this.activeStory.currentSegmentIndex > 0) {
      this.activeStory.currentSegmentIndex--;
      this.markSegmentAsSeen(this.activeStory);
      this.startStoryTimer(); // Reset timer
    }
    this.cdr.detectChanges();
  }

  toggleMute(event: Event) {
    event.stopPropagation();
    this.isMuted = !this.isMuted;
    this.cdr.detectChanges();
  }

  closeStory() {
    if (this.storyTimer) clearInterval(this.storyTimer);
    this.activeStory = null;
    this.cdr.detectChanges();
  }

  triggerNewPost() {
    this.createNewPost.emit();
  }

  handleMyStoryClick() {
    if (this.myStory) {
      this.openStory(this.myStory);
    } else {
      this.openStoryCreator();
    }
  }

  // --- STORY CREATION LOGIC ---
  openStoryCreator() {
    this.showStoryCreator = true;
    this.storyCaption = '';
    this.selectedStoryFile = null;
    this.storyImagePreview = null;
    this.cdr.detectChanges();
  }

  closeStoryCreator() {
    this.showStoryCreator = false;
  }

  onStoryFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedStoryFile = file;
      this.isStoryVideo = file.type.startsWith('video/');
      
      if (this.isStoryVideo) {
        const video = document.createElement('video');
        video.preload = 'metadata';
        video.onloadedmetadata = () => {
          window.URL.revokeObjectURL(video.src);
          if (video.duration > 30.5) { // Allowing a tiny buffer
            alert('Operational Constraint: Video broadcasts are limited to 30 seconds or less.');
            this.selectedStoryFile = null;
            this.isStoryVideo = false;
            this.storyVideoPreview = null;
            this.cdr.detectChanges();
            return;
          }
          this.proceedWithFileRead(file);
        };
        video.src = URL.createObjectURL(file);
      } else {
        this.proceedWithFileRead(file);
      }
    }
  }

  private proceedWithFileRead(file: File) {
    const reader = new FileReader();
    reader.onload = (e: any) => {
      if (this.isStoryVideo) {
        this.storyVideoPreview = e.target.result;
        this.storyImagePreview = null;
      } else {
        this.storyImagePreview = e.target.result;
        this.storyVideoPreview = null;
      }
      this.cdr.detectChanges();
    };
    reader.readAsDataURL(file);
  }

  submitStory() {
    this.currentUserId = this.getUserIdFromToken();
    console.log('StoryCreator: currentUserId for share:', this.currentUserId);

    if (!this.selectedStoryFile || !this.currentUserId) {
      alert('Please select an image and ensure you are logged in.');
      return;
    }

    this.isSharingStory = true;
    const formData = new FormData();
    formData.append('UserId', this.currentUserId);
    formData.append('Caption', this.storyCaption);
    formData.append('storyImage', this.selectedStoryFile);

    this.authService.shareStory(formData).subscribe({
      next: () => {
        this.isSharingStory = false;
        this.closeStoryCreator();
        this.loadStories(); // Refresh tray
        alert('Broadcast shared successfully!');
      },
      error: (err) => {
        this.isSharingStory = false;
        console.error('Error sharing story', err);
        const errorMsg = err.error?.message || err.error?.Details || 'Failed to share broadcast.';
        alert(errorMsg);
      }
    });
  }

  // --- ADMIN ACTIONS ---
  toggleBlur(post: any) {
    const action = post.isBlurred ? 'unveil' : 'restrict';
    if (!confirm(`Are you sure you want to ${action} this post content?`)) return;
    
    this.authService.blurPost(post.id).subscribe({
      next: (res: any) => {
        post.isBlurred = !post.isBlurred;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error toggling blur', err)
    });
  }

  archivePost(post: any) {
    const reason = prompt('Reason for archiving:');
    if (reason === null) return;
    this.authService.deleteAndArchivePost(post.id, reason).subscribe({
      next: () => {
        this.posts = this.posts.filter(p => p.id !== post.id);
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error archiving post', err)
    });
  }

  // --- POST ACTION MENU METHODS ---
  togglePostMenu(postId: number, event: MouseEvent) {
    event.stopPropagation();
    this.activeMenuPostId = this.activeMenuPostId === postId ? null : postId;
    this.cdr.detectChanges();
  }

  downloadPostImage(post: any) {
    const link = document.createElement('a');
    link.href = 'data:image/jpeg;base64,' + post.displayImage;
    // Professional Naming: EWOMS_UserName_ID.jpg
    const safeName = post.userName ? post.userName.replace(/\s+/g, '_') : 'INTEL';
    link.download = `EWOMS_${safeName}_${post.id}.jpg`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    this.activeMenuPostId = null;
  }

  savePostToVault(post: any) {
    this.savePost(post);
    this.activeMenuPostId = null;
  }

  copyPostLink(post: any) {
    const dummyUrl = `${window.location.origin}/post/${post.id}`;
    navigator.clipboard.writeText(dummyUrl).then(() => {
      alert('Secure link copied to clipboard.');
    });
    this.activeMenuPostId = null;
  }

  reportPost(post: any) {
    alert('Intelligence report submitted to Admin for review.');
    this.activeMenuPostId = null;
  }

  @HostListener('document:click')
  closeMenus() {
    if (this.activeMenuPostId !== null) {
      this.activeMenuPostId = null;
      this.cdr.detectChanges();
    }
  }

  onImgError(event: any) {
    console.warn('Intelligence Broadcast: Visual component failed to synchronize.', event);
  }

  getCurrentUserAvatar() {
    if (this.userAvatar) {
      if (this.userAvatar.startsWith('data:image') || this.userAvatar.startsWith('http')) {
        return this.userAvatar;
      }
      return 'data:image/jpeg;base64,' + this.userAvatar;
    }
    return `https://ui-avatars.com/api/?name=Personnel&background=6366f1&color=fff`;
  }
}
