import { Component } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd, Event as RouterEvent } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  showUIElements: boolean = true;
  showLeaderboard: boolean = true;
  showSidebar: boolean = true;
  currentRouteClass: string = '';

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {
    this.router.events.pipe(
      filter((event: RouterEvent): event is NavigationEnd => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      this.updateUIVisibility(event.url);
      this.currentRouteClass = event.urlAfterRedirects.split('/')[1] || 'home';
    });
  }

  private updateUIVisibility(url: string): void {
    const path = url.split('?')[0];
    this.showLeaderboard = !['/login', '/register', '/home','/about'].includes(path);
    this.showUIElements = !['/login', '/register'].includes(path);
    this.showSidebar = !['/about', '/login', '/register', '/post-question', '/home'].includes(path);
  }
}
