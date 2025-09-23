import {Component, computed, effect, signal} from '@angular/core';
import {OrderService} from '../services/orderService';
import {CurrencyPipe, DatePipe, NgIf, SlicePipe} from '@angular/common';
import {Router} from '@angular/router';
import {AuthService} from '../services/auth.service';
import {RegisteredOrder, SingleOrderInCart, SingleRegisteredOrder} from '../model/Order';
import {HttpErrorResponse} from '@angular/common/http';
import {ProductService} from '../services/product.service';
import {Product} from '../model/Product';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';

export interface CreateNewOrderDto {

  // field name must match the backend Dto
  items: { productId: string, quantity: number }[];

  montantTotal: number;

  // payementInfo?: {
  bankUserId: string;
  bankAccountId: string;
  //

}

@Component({
  selector: 'app-cart-page',
  imports: [
    CurrencyPipe,
    NgIf,
    DatePipe,
    FormsModule,
    ReactiveFormsModule,
  ],
  templateUrl: './cart-page.html',
  styleUrl: './cart-page.scss'
})
export class CartPage {

  // ============== DEFINE SIGNALS & COMPUTED ==============

  currentOrderList = computed(() => this.orderService.getOrderList());

  userOrderHistory = signal<RegisteredOrder[]>([]);

  userEmail = computed(() => this.authService.getUsers()?.email);


  // ============== DEFINE PROPERTIES ==============
  checkoutForm! : FormGroup;

  checkoutFormVisible = false;


  // ============== DEFINE CONSTRUCTOR ==============

  constructor(
    public orderService: OrderService,
    public authService: AuthService,
    private productService: ProductService,
    private formBuilder: FormBuilder,
  ) {

    this.initCheckoutForm(formBuilder);
  }


  myEffect_auth = effect(
    () => {
      // fetch user orders after user logs in
      if (this.userEmail()) {
        this.fetchUserOrderHistory();
      }
    }
  )

  // myEffect_order = effect(
  //   () => {
  //     // fetch order list when it changes
  //     if (this.currentOrderList().length > 0){
  //       this.fetchUserOrderHistory()
  //     }
  //   }
  // )

  initCheckoutForm(formBuilder: FormBuilder) : void {


    this.checkoutForm = this.formBuilder.group({
        bankUserId: ['2e859a57-fa9f-4ab4-b58b-186fd4665b86', [Validators.required]],
        bankAccountId: ['47161a5e-7200-49b8-bb12-a6aab326b6f5', [Validators.required]],

      },{
        updateOn: 'blur'  // emit value and validation only on blur event
      }

    );

  }

  // ============== DEFINE CALLBACK METHODS ==============

  showCheckoutForm() {

    this.checkoutFormVisible = true;

  }

  onCheckoutClose() {
    this.checkoutFormVisible = false;
  }

  // send new Order to backend
  onCheckoutFormSubmit() {

    // collected data from checkout form
    const bankUserId = this.checkoutForm.get('bankUserId')?.value;
    const bankAccountId = this.checkoutForm.get('bankAccountId')?.value;

    if(!bankUserId || !bankAccountId) {
      alert('Please fill in all required fields.');
      return;
    }


    // iterate over currentOrderList and create a postOrderDto object
    const orderItems = this.currentOrderList()
      .filter(orderItem => orderItem.product.id !== null)
      .map((item: SingleOrderInCart) => {
        return {
          // already checked for null above, so use type assertion
          productId: item.product.id as string,
          quantity: item.quantity
        };
      });

    if (this.authService.getUsers()?.email == null) {
      return;
    }

    /// create orderDto object

    const orderDto: CreateNewOrderDto = {
      items : orderItems,
      bankUserId: bankUserId,
      bankAccountId: bankAccountId,

      montantTotal : this.currentOrderList()
        .reduce((total, item) => total + (item.product.price * item.quantity), 0),
    }

    this.orderService.submitOrder(orderDto).subscribe({

      next: (response) => {
        alert('Commande reussie!');
        console.log('Order submitted successfully:', response);

        // clear current order list
        this.orderService.clearOrderList();

        // refetch user orders History to include the new order
        this.fetchUserOrderHistory();

      },

      error: (error) => {
        const errorResponse = error as HttpErrorResponse;
        alert('Echecs lors de la soumission de la commande:'+ errorResponse.message);
        console.log('Error submitting order:', errorResponse.message);
        //alert('There was an error submitting your order. Please try again.');
      }

    });

  }



  fetchUserOrderHistory(){
    // check if user is logged in
    if (this.authService.getToken() == null || this.authService.getUsers() == null) {
      return;
    }

    this.orderService.fetchOrders().subscribe({

      next: (response : RegisteredOrder[]) => {
        console.log('Orders fetched successfully:', response);


        // the backend sends single order item with only productId and quantity
        // to display order with product info, we need to fetch product details from productService
        // assuming that productService already has the product list fetched and cached
        const response_with_product_info : RegisteredOrder[] = response.map((order: RegisteredOrder) => {

          const orderItemList_with_product_info =  order.orderItemList.map(
            (order : SingleRegisteredOrder) => (
              {
                ...order,
                product : this.productService.getSingleProduct(order.productId) || null
              } as SingleRegisteredOrder
            )
          )

          order.orderItemList = orderItemList_with_product_info;

          return order;

        })

        this.userOrderHistory.set(response_with_product_info);
      },

      error: (error) => {
        const errorResponse = error as HttpErrorResponse;
        console.log('Error fetching orders:', errorResponse.message);
        console.log('Error fetching orders:', errorResponse.error);
      }

    });
  }

  onIncrementClick(product : Product) {
    this.orderService.incrementQty(product);
  }

  onDecrementClick(product : Product) {
    this.orderService.decrementQty(product);
  }

}
