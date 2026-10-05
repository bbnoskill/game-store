import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <footer class="app-footer">
      <!-- 3-Column Info Section -->
      <section class="footer-info-section">
        <div class="footer-container">
          <div class="info-grid">
            <!-- Get In Touch -->
            <div class="info-card">
              <h3 class="info-title">Get In Touch</h3>
              <form class="contact-form" (ngSubmit)="onSubmitContact()">
                <div class="form-group">
                  <label for="contactName">Name</label>
                  <input type="text" id="contactName" name="name" [(ngModel)]="contactForm.name" />
                </div>
                <div class="form-group">
                  <label for="contactEmail">Email</label>
                  <input type="email" id="contactEmail" name="email" [(ngModel)]="contactForm.email" />
                </div>
                <div class="form-group">
                  <label for="contactMessage">Message</label>
                  <textarea id="contactMessage" name="message" rows="4" [(ngModel)]="contactForm.message"></textarea>
                </div>
                <button type="submit" class="btn-orange">Submit</button>
              </form>
            </div>

            <!-- Twitter Updates -->
            <div class="info-card">
              <h3 class="info-title">Twitter Updates</h3>
              <div class="twitter-updates">
                <div class="tweet-item">
                  <p class="tweet-game">Devil May Cry 5</p>
                  <a href="javascript:void(0)" class="tweet-link">http://goo.gl/ejVm</a>
                  <span class="tweet-time">about 1 hour ago reply</span>
                </div>
                <div class="tweet-item">
                  <p class="tweet-game">Dead or Alive 6 Special Edition</p>
                  <a href="javascript:void(0)" class="tweet-link">http://goo.gl/ejVm</a>
                  <span class="tweet-time">about 1 hour ago reply</span>
                </div>
                <div class="tweet-item">
                  <p class="tweet-game">Resident Evil 2 Deluxe Edition</p>
                  <a href="javascript:void(0)" class="tweet-link">http://goo.gl/ejVm</a>
                  <span class="tweet-time">about 1 hour ago reply</span>
                </div>
              </div>
              <button type="button" class="btn-orange">Follow Us</button>
            </div>

            <!-- From the Blog -->
            <div class="info-card">
              <h3 class="info-title">From the Blog</h3>
              <div class="blog-posts">
                <div class="blog-item">
                  <p>Proin commodo sodales arcu, a volutpat. Nulla convallis. <a href="javascript:void(0)" class="blog-link">Learn more</a></p>
                </div>
                <div class="blog-item">
                  <p>Proin commodo sodales arcu, volutpat. Nulla convallis a non. <a href="javascript:void(0)" class="blog-link">Learn more</a></p>
                </div>
                <div class="blog-item">
                  <p>Curabitur a aliquet turpis, sagittis eleifend sem. <a href="javascript:void(0)" class="blog-link">Learn more</a></p>
                </div>
                <div class="blog-item">
                  <p>Nam pellentesque finibus felis eget feugiat. Nulla a nisl non veh. <a href="javascript:void(0)" class="blog-link">Learn more</a></p>
                </div>
              </div>
              <button type="button" class="btn-orange">Visit Blog</button>
            </div>
          </div>
        </div>
      </section>

      <!-- Dark Navigation Footer Bar -->
      <section class="footer-nav-section">
        <div class="footer-container">
          <div class="nav-grid">
            <!-- Site Links (Active Navigation Links) -->
            <div class="nav-column">
              <h4 class="nav-heading">Site Links</h4>
              <ul class="nav-list">
                <li><a routerLink="/games" class="footer-nav-link">Games</a></li>
                <li><a routerLink="/genres" class="footer-nav-link">Genres</a></li>
                <li><a routerLink="/publishers" class="footer-nav-link">Publishers</a></li>
                <li><a routerLink="/orders" class="footer-nav-link">Orders</a></li>
                <li><a routerLink="/basket" class="footer-nav-link">My Bucket</a></li>
              </ul>
            </div>

            <!-- Company Information -->
            <div class="nav-column">
              <h4 class="nav-heading">Company Information</h4>
              <ul class="nav-list">
                <li><span class="footer-text-item">About</span></li>
                <li><span class="footer-text-item">Awards</span></li>
                <li><span class="footer-text-item">Reviews</span></li>
                <li><span class="footer-text-item">Testimonials</span></li>
                <li><span class="footer-text-item">Contact</span></li>
              </ul>
            </div>

            <!-- Social Media -->
            <div class="nav-column">
              <h4 class="nav-heading">Social Media</h4>
              <div class="social-icons">
                <a href="javascript:void(0)" class="social-icon" title="Twitter">
                  <svg viewBox="0 0 24 24" fill="currentColor">
                    <path d="M22.46 6c-.77.35-1.6.58-2.46.69.88-.53 1.56-1.37 1.88-2.38-.83.5-1.75.85-2.72 1.05C18.37 4.5 17.26 4 16 4c-2.35 0-4.27 1.92-4.27 4.29 0 .34.04.67.11.98C8.28 9.09 5.11 7.38 3 4.79c-.37.63-.58 1.37-.58 2.15 0 1.49.75 2.81 1.91 3.56-.71 0-1.37-.2-1.95-.5v.05c0 2.08 1.48 3.82 3.44 4.21a4.22 4.22 0 0 1-1.93.07 4.28 4.28 0 0 0 4 2.98 8.521 8.521 0 0 1-5.33 1.84c-.34 0-.68-.02-1.02-.06C3.44 20.29 5.7 21 8.12 21 16 21 20.33 14.46 20.33 8.79c0-.19 0-.37-.01-.56.84-.6 1.56-1.36 2.14-2.23z"/>
                  </svg>
                </a>
                <a href="javascript:void(0)" class="social-icon" title="Facebook">
                  <svg viewBox="0 0 24 24" fill="currentColor">
                    <path d="M22 12c0-5.52-4.48-10-10-10S2 6.48 2 12c0 4.84 3.44 8.87 8 9.8V15H8v-3h2V9.5C10 7.57 11.57 6 13.5 6H16v3h-2c-.55 0-1 .45-1 1v2h3v3h-3v6.95C18.05 21.45 22 17.19 22 12z"/>
                  </svg>
                </a>
                <a href="javascript:void(0)" class="social-icon" title="Email">
                  <svg viewBox="0 0 24 24" fill="currentColor">
                    <path d="M20 4H4c-1.1 0-1.99.9-1.99 2L2 18c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 4l-8 5-8-5V6l8 5 8-5v2z"/>
                  </svg>
                </a>
                <a href="javascript:void(0)" class="social-icon" title="Instagram">
                  <svg viewBox="0 0 24 24" fill="currentColor">
                    <path d="M12 2.163c3.204 0 3.584.012 4.85.07 3.252.148 4.771 1.691 4.919 4.919.058 1.265.069 1.645.069 4.849 0 3.205-.012 3.584-.069 4.849-.149 3.225-1.664 4.771-4.919 4.919-1.266.058-1.644.07-4.85.07-3.204 0-3.584-.012-4.849-.07-3.26-.149-4.771-1.699-4.919-4.92-.058-1.265-.07-1.644-.07-4.849 0-3.204.013-3.583.07-4.849.149-3.227 1.664-4.771 4.919-4.919 1.266-.057 1.645-.069 4.849-.069zm0-2.163c-3.259 0-3.667.014-4.947.072-4.358.2-6.78 2.618-6.98 6.98-.059 1.281-.073 1.689-.073 4.948 0 3.259.014 3.668.072 4.948.2 4.358 2.618 6.78 6.98 6.98 1.281.058 1.689.072 4.948.072 3.259 0 3.668-.014 4.948-.072 4.354-.2 6.782-2.618 6.979-6.98.059-1.28.073-1.689.073-4.948 0-3.259-.014-3.667-.072-4.947-.196-4.354-2.617-6.78-6.979-6.98-1.281-.059-1.69-.073-4.949-.073zm0 5.838c-3.403 0-6.162 2.759-6.162 6.162s2.759 6.163 6.162 6.163 6.162-2.759 6.162-6.163c0-3.403-2.759-6.162-6.162-6.162zm0 10.162c-2.209 0-4-1.79-4-4 0-2.209 1.791-4 4-4s4 1.791 4 4c0 2.21-1.791 4-4 4zm6.406-11.845c-.796 0-1.441.645-1.441 1.44s.645 1.44 1.441 1.44c.795 0 1.439-.645 1.439-1.44s-.644-1.44-1.439-1.44z"/>
                  </svg>
                </a>
              </div>
            </div>
          </div>

          <div class="footer-copyright">
            <p>&#64;2007 - 2019 All Rights Reserved</p>
          </div>
        </div>
      </section>
    </footer>
  `,
  styles: [`
    .app-footer {
      width: 100%;
      margin-top: auto;
    }
    .footer-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 24px;
    }

    /* 3-Column Info Section */
    .footer-info-section {
      background-color: #f5f5f7;
      padding: 48px 0;
      border-top: 1px solid #e0e0e0;
    }
    .info-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 32px;
    }
    .info-card {
      background: #ffffff;
      padding: 24px;
      border-radius: 4px;
      box-shadow: 0 1px 3px rgba(0,0,0,0.05);
      display: flex;
      flex-direction: column;
    }
    .info-title {
      font-size: 20px;
      font-weight: 700;
      color: #222;
      margin: 0 0 20px 0;
    }

    /* Contact Form */
    .contact-form {
      display: flex;
      flex-direction: column;
      gap: 12px;
      flex: 1;
    }
    .form-group {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    .form-group label {
      font-size: 13px;
      color: #555;
      font-weight: 500;
    }
    .form-group input,
    .form-group textarea {
      width: 100%;
      padding: 8px 10px;
      border: 1px solid #ccc;
      border-radius: 2px;
      font-size: 14px;
      box-sizing: border-box;
      font-family: inherit;
    }
    .form-group input:focus,
    .form-group textarea:focus {
      outline: none;
      border-color: #f57c00;
    }

    /* Twitter Updates */
    .twitter-updates {
      display: flex;
      flex-direction: column;
      gap: 16px;
      margin-bottom: 24px;
      flex: 1;
    }
    .tweet-item {
      border-bottom: 1px solid #eee;
      padding-bottom: 12px;
    }
    .tweet-item:last-child {
      border-bottom: none;
      padding-bottom: 0;
    }
    .tweet-game {
      font-weight: 600;
      font-size: 14px;
      color: #222;
      margin: 0 0 2px 0;
    }
    .tweet-link {
      font-size: 13px;
      color: #f57c00;
      text-decoration: none;
      display: block;
      margin-bottom: 2px;
    }
    .tweet-link:hover {
      text-decoration: underline;
    }
    .tweet-time {
      font-size: 12px;
      color: #888;
    }

    /* Blog */
    .blog-posts {
      display: flex;
      flex-direction: column;
      gap: 12px;
      margin-bottom: 24px;
      flex: 1;
    }
    .blog-item p {
      font-size: 13px;
      color: #444;
      line-height: 1.4;
      margin: 0;
      padding-bottom: 10px;
      border-bottom: 1px solid #eee;
    }
    .blog-item:last-child p {
      border-bottom: none;
    }
    .blog-link {
      color: #f57c00;
      text-decoration: none;
      font-weight: 500;
    }
    .blog-link:hover {
      text-decoration: underline;
    }

    /* Shared Orange Button */
    .btn-orange {
      background-color: #f57c00;
      color: #ffffff;
      border: none;
      padding: 10px 24px;
      font-size: 14px;
      font-weight: 600;
      border-radius: 2px;
      cursor: pointer;
      align-self: flex-start;
      margin-top: auto;
      transition: background-color 0.2s ease;
    }
    .btn-orange:hover {
      background-color: #e65100;
    }

    /* Dark Navigation Footer Bar */
    .footer-nav-section {
      background-color: #141517;
      color: #ffffff;
      padding: 48px 0 24px 0;
    }
    .nav-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 32px;
      margin-bottom: 36px;
    }
    .nav-heading {
      font-size: 15px;
      font-weight: 700;
      color: #ffffff;
      margin: 0 0 16px 0;
    }
    .nav-list {
      list-style: none;
      padding: 0;
      margin: 0;
      display: flex;
      flex-direction: column;
      gap: 8px;
    }
    .footer-nav-link {
      color: #9e9e9e;
      text-decoration: none;
      font-size: 13px;
      transition: color 0.2s ease;
      cursor: pointer;
    }
    .footer-nav-link:hover {
      color: #f57c00;
    }
    .footer-text-item {
      color: #9e9e9e;
      font-size: 13px;
    }

    /* Social Icons */
    .social-icons {
      display: flex;
      align-items: center;
      gap: 16px;
    }
    .social-icon {
      color: #ffffff;
      width: 20px;
      height: 20px;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      transition: color 0.2s ease;
    }
    .social-icon:hover {
      color: #f57c00;
    }
    .social-icon svg {
      width: 100%;
      height: 100%;
    }

    /* Copyright */
    .footer-copyright {
      border-top: 1px solid rgba(255, 255, 255, 0.08);
      padding-top: 20px;
      text-align: left;
    }
    .footer-copyright p {
      font-size: 12px;
      color: #777;
      margin: 0;
    }

    @media (max-width: 900px) {
      .info-grid {
        grid-template-columns: 1fr;
      }
      .nav-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class FooterComponent {
  contactForm = {
    name: '',
    email: '',
    message: ''
  };

  onSubmitContact(): void {
    if (this.contactForm.name && this.contactForm.email) {
      alert(`Thank you, ${this.contactForm.name}! Your message has been received.`);
      this.contactForm = { name: '', email: '', message: '' };
    }
  }
}
