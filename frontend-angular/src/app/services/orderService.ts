import {Injectable, signal} from '@angular/core';
import {RegisteredOrder, SingleOrderInCart, SingleRegisteredOrder} from '../model/Order';
import {Product} from '../model/Product';
import {CreateNewOrderDto} from '../cart-page/cart-page';
import {environment} from '../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {ConfigService} from './config.service';


@Injectable({
  providedIn: 'root'
})
export class OrderService {

  private apiUrl: string;


  public currentOrderList = signal<SingleOrderInCart[]>([]);
  public totalOrderCount = signal<number>(0);


  constructor( private http: HttpClient, private configService : ConfigService) {

    this.apiUrl = this.configService.apiUrl + '/order';
  }


  incrementQty(product : Product): void {

    const items = this.currentOrderList() ;

    const productIndex = items.findIndex(item => item.product.id === product.id);

    // if product already exists in the order, increase quantity by 1
    if (productIndex > -1) {

      console.log("Incrementing quantity for product: " + product.id);

      items[productIndex].quantity += 1;
      this.currentOrderList.set([...items]);

      this.totalOrderCount.set(this.totalOrderCount() + 1);
      return;

    }
    else
    {
      this.totalOrderCount.set(this.totalOrderCount() + 1);

      const newItem = new SingleOrderInCart(product, 1);
      this.currentOrderList.set([...this.currentOrderList(), newItem]);

    }
  }


  decrementQty(product : Product): void {
    const items = this.currentOrderList()

    if ((items.length === 0)) {
      return;
    }

    const itemIndex = items.findIndex(item => item.product.id === product.id);

    if (itemIndex === -1) {
      return;
    }

    if (items[itemIndex].quantity === 1) {
      items.splice(itemIndex, 1);
      this.currentOrderList.set([...items]);
      this.totalOrderCount.set(this.totalOrderCount() - 1);
    }

    else if (items[itemIndex].quantity > 1) {
      items[itemIndex].quantity -= 1;
      this.totalOrderCount.set(this.totalOrderCount() - 1);
      this.currentOrderList.set([...items]);
    }
  }

  getOrderCount(): number {
    return this.totalOrderCount();
  }

  getOrderList(): SingleOrderInCart[] {
    return this.currentOrderList();
  }

  submitOrder(postOrderDto : CreateNewOrderDto) : Observable<{message : string, newOrders: RegisteredOrder}> {


    console.log('Order details: ' + JSON.stringify(postOrderDto));

    return this.http.post<{message : string, newOrders: RegisteredOrder}>(this.apiUrl, postOrderDto)

  }

  fetchOrders(): Observable<RegisteredOrder[]> {

    console.log('Fetching orders from ' + this.apiUrl);

    return this.http.get<RegisteredOrder[]>(this.apiUrl)

  }

  clearOrderList() {
    this.currentOrderList.set([]);
    this.totalOrderCount.set(0);
  }

}
