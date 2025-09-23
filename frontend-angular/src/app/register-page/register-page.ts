import {Component, computed, effect, inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {map, Observable} from 'rxjs';
import {User} from '../model/User';
import {AsyncPipe, DatePipe, NgIf} from '@angular/common';
import {AuthService} from '../services/auth.service';
import {Router} from '@angular/router';
import {HttpErrorResponse} from '@angular/common/http';
import {ConfigService} from '../services/config.service';

@Component({
  selector: 'register-page',
  imports: [
    ReactiveFormsModule,
    NgIf,

  ],
  templateUrl: './register-page.html',
  styleUrl: './register-page.scss'
})
export class RegisterPage implements OnInit {

  // authService = inject(FaceSnapService) ;

  router! : Router

  registerForm! : FormGroup;


  loginSuccess = computed( () => this.authService.getToken() != null ) ;

  loginMessage = computed( () => this.authService.getLoginMessage() ) ;


  myEffect = effect(
    () => {
      if (this.loginSuccess()) {
        this.registerForm.reset();
        console.log('execute effect - login success, navigate to home page');
      }

    }
  )

  urlRegex = /(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_+.~#?&/=]*)/;

  constructor(private formBuilder: FormBuilder,
              private authService : AuthService,
              private rt : Router,
              private configService : ConfigService,) {
    this.initRegisterForm(formBuilder);
    this.router = rt;

  }

  ngOnInit() {
    this.authService.setUrl(this.configService.apiUrl);
  }


  initRegisterForm(formBuilder: FormBuilder) : void {

    this.registerForm = this.formBuilder.group({
        email: ['admin@gmail.com', [Validators.required, Validators.email]],
        password: ['admin', [Validators.required, Validators.minLength(6)]]
        //url: ['', [Validators.required, Validators.pattern(this.urlRegex)]],

      },{
        updateOn: 'blur'  // emit value and validation only on blur event
      }

    );

  }

  onRegister() {
    const email = this.registerForm.get('email')?.value;
    const password = this.registerForm.get('password')?.value;

    // Here you can handle the login logic, e.g., call an authentication service
    console.log('email:', email);
    console.log('Password:', password);

    console.log('send login to server for user ' + email + ' with password ' + password) ;

    this.authService.registerUser(email, password).subscribe({
      next: (response => {

        alert("Registration reussi ! Veuillez vous connecter.");

      }),

      error: (error => {
        const errorResponse = error as HttpErrorResponse;

        alert("Echec de l'inscription : " + (errorResponse.error.errorMessage));

        console.log('Registration echoue :' + (errorResponse.error.errorMessage));

      })

    });
  }

  onLogout() : void {
      this.authService.logout();
  }

}
