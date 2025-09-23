// config.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {AuthService} from './auth.service';
import {firstValueFrom} from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ConfigService {

  private config: any;

  constructor(private http: HttpClient) { }

  // loadConfig() {
  //   return this.http.get('/assets/config.json')
  //     .toPromise()
  //     .then(config => {
  //       this.config = config;
  //     });
  // }

  loadConfig(): Promise<void> {
    return firstValueFrom(this.http.get('/assets/config.json'))
      .then(cfg => { this.config = cfg; });
  }

  get apiUrl(): string {
    return this.config?.apiUrl;
  }

}

