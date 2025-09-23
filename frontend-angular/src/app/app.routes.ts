import { Routes } from '@angular/router';
import {LoginPage} from './login-page/login-page';
import {ProductListPage} from './product-list-page/product-list-page.component';
import {CartPage} from './cart-page/cart-page';
import {RegisterPage} from './register-page/register-page';

export const routes: Routes = [

  { path: 'login', component: LoginPage },
  { path: 'register', component: RegisterPage },
  { path: 'cart', component: CartPage },
  { path: '', component: ProductListPage },

]
