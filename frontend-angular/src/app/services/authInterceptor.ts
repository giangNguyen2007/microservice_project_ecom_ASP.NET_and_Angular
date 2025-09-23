import {
  HttpHeaders,
  HttpInterceptorFn,

} from '@angular/common/http';
import {inject} from '@angular/core';
import {AuthService} from './auth.service';



export const authInterceptor : HttpInterceptorFn = (req, next) =>  {

    const authService = inject(AuthService);

    const token = authService.getToken();
    if (!token) {
      return next(req);
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    const modifiedReq = req.clone({ headers });

    return next(modifiedReq);

};

