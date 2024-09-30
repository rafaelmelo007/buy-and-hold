import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { HeaderComponent } from './common/components/header/header.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  showHeader = true;

  constructor(private router: Router) {
    this.router.events.subscribe(() => {
      const currentUrl = this.router.url;
      this.showHeader = this.shouldShowHeader(currentUrl);
    });
  }

  shouldShowHeader(url: string): boolean {
    // Add paths where you don't want to show the header
    const noHeaderRoutes = [
      '/pages/account/login',
      '/pages/account/register',
      '/pages/account/forgot-password',
    ];
    return !noHeaderRoutes.some((route) => url.startsWith(route));
  }
}
