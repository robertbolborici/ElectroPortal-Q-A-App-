import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.css'
})
export class NavBarComponent implements OnInit {
  isLoggedIn = false;
  searchQuery: string = '';
  userId: string | null = null;
  
  constructor(private router: Router, private userService: UserService, private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.isLoggedIn().subscribe((loggedIn) => {
      this.isLoggedIn = loggedIn;
      if (loggedIn) {
        this.userId = this.authService.getCurrentUserId();
      }
    });
  }

  goToUserProfile() {
    if (this.userId) {
      this.router.navigate(['/user-profile', this.userId]);
    }
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/home']);
  }
  
  goToRegister() {
    this.router.navigate(['/register']);
  }

  goToAbout(){
    this.router.navigate(['/about']);
  }

  goToDidYouKnow(){
    this.router.navigate(['/documentation']);
  }

  search() {
    if (this.searchQuery.trim()) {
      this.router.navigate(['/questions'], { queryParams: { query: this.searchQuery.trim() } });
    }
  }
}

