// create a service FaceSnapService with the following properties and methods:
import {computed, Injectable, signal} from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpHeaders} from '@angular/common/http';
import {catchError, Observable, tap} from 'rxjs';

import {Product} from '../model/Product';
import {AuthService} from './auth.service';
import {environment} from '../../environments/environment';
import {ConfigService} from './config.service';

interface ProductRequestResponse {
  token: string;
  name: string;
  userId: string;
  role: string;
}

export interface NewProdctDto {
  title: string;
  description: string;
  photoUrl: string;
  price: number;
  stock: number;
  category: string;
}


@Injectable({
  // this means that the service is available application-wide as a singleton
  providedIn: 'root'
})
export class ProductService {

  private apiUrl : string;


  private prductList = signal<Product[] | null>(null) ;


  constructor(private http: HttpClient, private authService : AuthService, private configService: ConfigService) {

    this.apiUrl = this.configService.apiUrl + '/product';
  }


  createProduct(newProduct : NewProdctDto): Observable<Product>  {


    if (!this.authService.getToken() || !this.authService.getUsers()) {
      return new Observable(observer => {
        observer.error(new Error('Authentication required: Token or user not found'));
      });
    }

    // jwt token is added in header by the Interceptor
    return this.http.post<Product>(this.apiUrl,  newProduct);

  }

  deleteProduct(productId : string): Observable<any>  {

    if (!this.authService.getToken() || !this.authService.getUsers()) {
      return new Observable(observer => {
        observer.error(new Error('Authentication required: Token or user not found'));
      });
    }

    // jwt token is added in header by the Interceptor
    return this.http.delete<any>(this.apiUrl + "/" + productId);

  }

  fetchProducts(): void  {

    // if (!this.authService.getToken() || !this.authService.getUsers() ) {

    //   return ;
    // }

    // does not fetch if productList has already been fetched
    if (this.prductList()){
      return;
    }

    this.http.get<Product[]>(this.apiUrl).subscribe(
      {
        next: (products => {
          console.log('fetch products successful, number of accounts: ' + products.length);

          this.prductList.set(products);

        }),

        error: (error => {
          const errorResponse = error as HttpErrorResponse;

          console.log('Status: ' + (errorResponse.status ));

          console.log('fetch products failed: ' + (errorResponse.error));

        })

      }
    )

    return;

  }


  updateProductList(updatedList: Product[]){
    this.prductList.set(updatedList);
  }

  getProductList() : Product[] | null {
    return this.prductList();
  }

  getProductListLength() : number | undefined {
    return this.prductList()?.length;
  }


  logoutProductList() :  void {
    this.prductList.set(null);

  }

  getSingleProduct(productId : string) : Product | null {
    return this.prductList()?.find(p => p.id === productId) || null;
  }
}
