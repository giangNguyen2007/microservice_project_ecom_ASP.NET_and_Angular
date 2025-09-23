import {Component, OnInit, signal} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {Header} from './header/header';

@Component({
  selector: 'app-root',
  imports : [
    Header,
    RouterOutlet,
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App{
  protected readonly title = signal('gng_learn_snapFace');


}
