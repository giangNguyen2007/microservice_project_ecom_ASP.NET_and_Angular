// create a service FaceSnapService with the following properties and methods:
import {computed, Injectable, signal} from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpHeaders} from '@angular/common/http';
import {catchError, config, Observable, tap} from 'rxjs';
import {User} from '../model/User';
import {environment} from '../../environments/environment';
import {ConfigService} from './config.service';

interface LoginResponse {
  message : string;
  token: string;
  role: string;
}



@Injectable({
  // this means that the service is available application-wide as a singleton
  providedIn: 'root'
})
export class AuthService {

  // private apiUrl = environment.authAPiUrl;
  private apiUrl = "";

  users$ !: Observable<User[]>;

  private user = signal<User | null>(null) ;
  private token = signal<string | null>(null) ;
  private loginMessage = signal<string | null>(null) ;



  constructor(private http: HttpClient) {

    // this.apiUrl = this.configService.apiUrl + '/auth';

    const savedToken = localStorage.getItem('token');
    const savedUser = localStorage.getItem('user');

    if (savedToken && savedUser) {
      this.token.set(savedToken);
      this.user.set(JSON.parse(savedUser));
    }


  }

  setUrl(url: string) : void {
    this.apiUrl = url + '/auth';
    console.log('AuthService apiUrl set to: ' + this.apiUrl);
  }


  loginUser( email: string, password: string):  Observable<LoginResponse>{
    // create a json object with email and password
    const loginData = { email, password };

    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, loginData).pipe(

      // handle sucessful response => save data
      tap( response => {

        console.log('login successful, token: ' + response.token);
        console.log('login successful, role: ' + response.role);

        this.token.set(response.token);
        this.user.set({email: email, role: response.role as "normal" | "admin" | null});

        // save in local storage
        localStorage.setItem('token', response.token);
        localStorage.setItem('user', JSON.stringify(this.user()));

      })

    );

  }

  registerUser( email: string, password: string): Observable<any> {
    // create a json object with email and password
    const loginData = { email, password };

    return this.http.post<any>(`${this.apiUrl}/register`, loginData);

  }

  logout() :  void {
    this.user.set(null);
    this.token.set(null);
    this.loginMessage.set(null);

    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }


  getToken() : string | null {

    return this.token();
  }

  getUsers() : User | null{
    return this.user();
  }

  getLoginMessage() : string | null{
    return this.loginMessage();
  }

}
