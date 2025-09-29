import {Component, computed, effect, inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {map, Observable} from 'rxjs';
import {User} from '../model/User';
import {AsyncPipe, DatePipe, NgIf} from '@angular/common';
import {AuthService} from '../services/auth.service';
import {Router} from '@angular/router';
import {ConfigService} from '../services/config.service';
import {HttpErrorResponse} from '@angular/common/http';

@Component({
  selector: 'login-page',
  imports: [
    ReactiveFormsModule,
    NgIf,

  ],
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss'
})
export class LoginPage implements OnInit {

  // authService = inject(FaceSnapService) ;

  router! : Router

  loginForm! : FormGroup;


  loginSuccess = computed( () => this.authService.getToken() != null ) ;

  loginMessage = computed( () => this.authService.getLoginMessage() ) ;


  myEffect = effect(
    () => {
      if (this.loginSuccess()) {
        this.loginForm.reset();
        console.log('execute effect - login success, navigate to home page');
      }

    }
  )



  urlRegex = /(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_+.~#?&/=]*)/;

  constructor(private formBuilder: FormBuilder,
              private authService : AuthService,
              private rt : Router,
              private configService : ConfigService,) {
    this.initLoginForm(formBuilder);
    this.router = rt;

  }

  ngOnInit() {
    this.authService.setUrl(this.configService.apiUrl);
  }


  initLoginForm(formBuilder: FormBuilder) : void {

    this.loginForm = this.formBuilder.group({
        email: ['admin@gmail.com', [Validators.required, Validators.email]],
        password: ['admin', [Validators.required, Validators.minLength(6)]]
        //url: ['', [Validators.required, Validators.pattern(this.urlRegex)]],

      },{
        updateOn: 'blur'  // emit value and validation only on blur event
      }

    );

  }

  onLogin() {
    const email = this.loginForm.get('email')?.value;
    const password = this.loginForm.get('password')?.value;

    console.log('email:', email);
    console.log('Password:', password);

    console.log('send login to server for user ' + email + ' with password ' + password) ;

    this.authService.loginUser(email, password).subscribe({
      next: (response => {

        alert("Login reussi");

        // how to navigate to home page
        this.rt.navigate(['']);

      }),

      error: (error => {
        const errorResponse = error as HttpErrorResponse;

        console.log('login failed: ' + (errorResponse.error || errorResponse.statusText));
        alert("Login echoue: " + errorResponse.message);
        // Handle error appropriately

      })

    });
  }

  onLogout() : void {
      this.authService.logout();
  }

}
