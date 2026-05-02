import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-create-post',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-post.component.html',
  styleUrls: ['./create-post.component.css']
})
export class CreatePostComponent implements OnInit {
  userId: string = '';
  newPostCaption: string = '';
  selectedPostImage: File | null = null;
  imagePreview: string | null = null;
  
  isPosting = false;
  errorMsg = '';
  successMsg = '';

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.userId = this.getUserIdFromToken() || localStorage.getItem('userId') || '';
    console.log('Resolved UserId for post:', this.userId);
  }

  private getUserIdFromToken(): string {
    return this.authService.getUserIdFromToken();
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedPostImage = file;
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
        this.cdr.detectChanges(); // Instantly update UI to show image preview
      };
      reader.readAsDataURL(file);
    }
  }

  clearPostForm() {
    this.newPostCaption = '';
    this.selectedPostImage = null;
    this.imagePreview = null;
  }

  createPost() {
    // Re-verify UserId from token immediately before posting
    this.userId = this.getUserIdFromToken() || localStorage.getItem('userId') || '';

    if (!this.userId) {
      this.errorMsg = 'User ID is missing!';
      return;
    }
    if (!this.selectedPostImage) {
      this.errorMsg = 'Please select an image for your post.';
      return;
    }
    if (!this.newPostCaption.trim()) {
      this.errorMsg = 'Please write a caption for your post.';
      return;
    }

    this.isPosting = true;
    this.errorMsg = '';
    this.successMsg = '';

    const formData = new FormData();
    formData.append('UserId', this.userId);
    formData.append('Caption', this.newPostCaption);
    formData.append('postImage', this.selectedPostImage);

    this.authService.createPost(formData).subscribe({
      next: (res: any) => {
        this.isPosting = false;
        
        // Capture the message from API (e.g. "Post uploaded successfully")
        const apiMessage = typeof res === 'string' ? res : (res?.Message || res?.message || 'Post created successfully!');
        
        alert(apiMessage); // Show the API return value as an alert
        
        this.successMsg = apiMessage;
        this.clearPostForm();
        this.cdr.detectChanges(); 

        setTimeout(() => {
          this.successMsg = '';
          this.cdr.detectChanges();
        }, 3000);
      },
      error: (err) => {
        console.error('Failed to create post', err);
        this.isPosting = false;
        this.errorMsg = err?.error?.message || err?.error?.Message || 'Failed to create post.';
        this.cdr.detectChanges(); // Instant UI update
      }
    });
  }
}
