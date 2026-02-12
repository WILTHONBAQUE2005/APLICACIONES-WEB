import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <div style="padding:16px;color:white;font-weight:800;font-size:18px;">
      APP OK
    </div>
    <router-outlet></router-outlet>
  `
})
export class AppComponent {}
