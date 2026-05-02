import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectorRef, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';

interface EnterpriseStory {
  id: any;
  userName: string;
  userImage: string;
  hasUnseen: boolean;
  segments: any[];
  currentSegmentIndex: number;
}

@Component({
  selector: 'app-community-feed',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './community-feed.component.html',
  styleUrls: ['./community-feed.component.css']
})
export class CommunityFeedComponent implements OnInit {
  @Input() role: string = 'user';
  @Input() userAvatar: string | null = null;
  @Output() viewProfile = new EventEmitter<string>();
  @Output() openImage = new EventEmitter<string>();

  posts: any[] = [];
  stories: EnterpriseStory[] = [];
  isLoading = false;
  viewMode: 'list' | 'table' | 'grid' = 'list';
  
  // Comments
  showComments = false;
  currentPostComments: any[] = [];
  isLoadingComments = false;
  newCommentText = '';
  isSubmittingComment = false;
  activeCommentPostId: number | null = null;

  // Stories
  activeStory: EnterpriseStory | null = null;
  showStoryCreator = false;
  storyCaption = '';
  selectedStoryFile: File | null = null;
  storyImagePreview: string | null = null;
  isSharingStory = false;

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadFeed();
    this.loadStories();
  }

  loadFeed() {
    this.isLoading = true;
    this.authService.getUserPosts().subscribe({
      next: (res: any) => {
        this.posts = res.map((p: any) => {
          // POST CONTENT IMAGE
          p.displayImage = p.displayImage || p.postImage || p.PostImage || p.image || p.Image || null;
          // AUTHOR PROFILE IMAGE
          p.authorImage = p.authorImage || p.userImage || p.UserImage || p.profileImage || p.ProfileImage || null;
          p.isRevealed = false;
          return p;
        });
        this.isLoading = false;
        if (this.stories.length === 0) {
          this.generateStoriesFromPosts(this.posts);
        }
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error loading feed', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadStories() {
    const userId = (this.authService as any).getUserIdFromToken();
    if (!userId) return;
    this.authService.getStories(userId).subscribe({
      next: (res: any) => {
        this.stories = (res || []).map((s: any) => ({
          id: s.userId || s.UserId,
          userName: s.userName || s.UserName,
          userImage: s.userImage || s.UserImage,
          hasUnseen: s.hasUnseen !== undefined ? s.hasUnseen : s.HasUnseen,
          segments: (s.segments || s.Segments || []).map((seg: any) => ({
            id: seg.id || seg.Id,
            displayImage: seg.displayImage || seg.DisplayImage,
            caption: seg.caption || seg.Caption,
            createdAt: seg.createdAt || seg.CreatedAt
          })),
          currentSegmentIndex: 0
        }));
        console.log('CommunityFeed Stories Loaded:', this.stories);
        this.cdr.detectChanges();
      }
    });
  }

  generateStoriesFromPosts(allPosts: any[]) {
    // Filter posts from the last 24 hours
    const twentyFourHoursAgo = new Date(Date.now() - 24 * 60 * 60 * 1000);
    const recentPosts = allPosts.filter(p => {
      const postDate = p.createdAt ? new Date(p.createdAt) : new Date();
      return postDate >= twentyFourHoursAgo;
    });

    // Group by user
    const userMap = new Map<string, any[]>();
    recentPosts.forEach(p => {
      const userId = (this.authService as any).getUserIdFromToken();
      if (String(p.userId) === String(userId)) return;
      if (!userMap.has(p.userId)) userMap.set(p.userId, []);
      userMap.get(p.userId)!.push(p);
    });

    // Convert to story models
    const generatedStories: EnterpriseStory[] = Array.from(userMap.entries()).map(([uid, uPosts]: any) => {
      const first = uPosts[0];
      return {
        id: uid,
        userName: first.userName || 'Team Member',
        userImage: first.authorImage || first.userImage,
        hasUnseen: true,
        segments: uPosts.map((p: any) => ({
          id: p.id,
          displayImage: p.displayImage || p.postImage,
          caption: p.caption,
          createdAt: p.createdAt
        })),
        currentSegmentIndex: 0
      } as EnterpriseStory;
    });

    this.stories = [...generatedStories];
    this.cdr.detectChanges();
  }

  // Engagement
  likePost(post: any) {
    const userId = (this.authService as any).getUserIdFromToken();
    if (!userId) return;
    this.authService.togglePostLike(post.id, userId).subscribe({
      next: (res: any) => {
        post.isLiked = res.isLiked;
        post.likeCount = res.likeCount;
        this.cdr.detectChanges();
      }
    });
  }

  toggleComments(post: any) {
    this.activeCommentPostId = post.id;
    this.showComments = true;
    this.loadComments(post.id);
  }

  loadComments(postId: number) {
    this.isLoadingComments = true;
    this.authService.getComments(postId).subscribe({
      next: (res: any) => {
        this.currentPostComments = res;
        this.isLoadingComments = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoadingComments = false;
        this.cdr.detectChanges();
      }
    });
  }

  submitComment() {
    if (!this.newCommentText.trim() || !this.activeCommentPostId) return;
    const userId = (this.authService as any).getUserIdFromToken();
    if (!userId) return;

    this.isSubmittingComment = true;
    this.authService.addComment(this.activeCommentPostId, userId, this.newCommentText).subscribe({
      next: () => {
        this.newCommentText = '';
        this.isSubmittingComment = false;
        this.loadComments(this.activeCommentPostId!);
        // Update local post count
        const post = this.posts.find(p => p.id === this.activeCommentPostId);
        if (post) post.commentCount++;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isSubmittingComment = false;
        this.cdr.detectChanges();
      }
    });
  }

  closeComments() {
    this.showComments = false;
    this.activeCommentPostId = null;
  }

  // Stories Logic
  openStory(story: EnterpriseStory) {
    this.activeStory = story;
    this.activeStory.currentSegmentIndex = 0;
    this.cdr.detectChanges();
  }

  prevSegment(event: Event) {
    event.stopPropagation();
    if (!this.activeStory) return;
    if (this.activeStory.currentSegmentIndex > 0) {
      this.activeStory.currentSegmentIndex--;
    } else {
      this.closeStory();
    }
    this.cdr.detectChanges();
  }

  nextSegment(event: Event) {
    event.stopPropagation();
    if (!this.activeStory) return;
    if (this.activeStory.currentSegmentIndex < this.activeStory.segments.length - 1) {
      this.activeStory.currentSegmentIndex++;
    } else {
      this.closeStory();
    }
    this.cdr.detectChanges();
  }

  onImgError(event: any) {
    console.error('Story Image Load Failed:', event);
  }

  closeStory() {
    this.activeStory = null;
    this.cdr.detectChanges();
  }

  openStoryCreator() { 
    this.showStoryCreator = true; 
    this.cdr.detectChanges();
  }
  closeStoryCreator() { 
    this.showStoryCreator = false;
    this.storyImagePreview = null;
    this.selectedStoryFile = null;
  }

  onStoryFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedStoryFile = file;
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.storyImagePreview = e.target.result;
        this.cdr.detectChanges();
      };
      reader.readAsDataURL(file);
    }
  }

  shareStory() {
    if (!this.selectedStoryFile) return;
    const userId = (this.authService as any).getUserIdFromToken();
    if (!userId) return;

    this.isSharingStory = true;
    const formData = new FormData();
    formData.append('userId', userId);
    formData.append('caption', this.storyCaption);
    formData.append('storyImage', this.selectedStoryFile);

    this.authService.shareStory(formData).subscribe({
      next: () => {
        this.isSharingStory = false;
        this.closeStoryCreator();
        this.loadStories();
      },
      error: () => {
        this.isSharingStory = false;
        this.cdr.detectChanges();
      }
    });
  }

  // Moderation
  toggleBlur(post: any) {
    if (!confirm('Update broadcast visibility?')) return;
    this.authService.blurPost(post.id).subscribe({
      next: () => {
        post.isBlurred = !post.isBlurred;
        this.cdr.detectChanges();
      }
    });
  }

  archivePost(post: any) {
    const reason = prompt('Reason for archiving:');
    if (reason === null) return;
    this.authService.deleteAndArchivePost(post.id, reason).subscribe({
      next: () => {
        this.posts = this.posts.filter(p => p.id !== post.id);
        this.cdr.detectChanges();
      }
    });
  }

  onViewProfile(userId: string) {
    this.viewProfile.emit(userId);
  }

  getAuthorAvatar(post: any) {
    const img = post.authorImage || post.userImage || post.profileImage;
    return img ? (img.startsWith('data:image') ? img : 'data:image/jpeg;base64,' + img) : 'https://ui-avatars.com/api/?name=' + post.userName;
  }

  getPostById(id: number | null): any {
    return this.posts.find(p => p.id === id);
  }

  getCurrentUserAvatar() {
    return this.userAvatar ? (this.userAvatar.startsWith('data:image') ? this.userAvatar : 'data:image/jpeg;base64,' + this.userAvatar) : 'https://ui-avatars.com/api/?name=You';
  }
}
