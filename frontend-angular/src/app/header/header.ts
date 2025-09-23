import {Component, computed} from '@angular/core';
import {RouterLink, RouterLinkActive} from '@angular/router';
import {OrderService} from '../services/orderService';
import {AuthService} from '../services/auth.service';
import {NgIf} from '@angular/common';

@Component({
  selector: 'header',
  imports: [
    RouterLink,
    RouterLinkActive,
    NgIf
  ],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {

  orderCount  = computed( () => this.orderService.getOrderCount()) ;

  isAuthenticated = computed( () => this.authService.getUsers() != null);


  constructor(
      private orderService : OrderService,
      public authService: AuthService

      ) {
  }

  onLogoutClick() {
    this.authService.logout();
    this.orderService.clearOrderList();
  }

}
