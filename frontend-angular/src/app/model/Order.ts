import {Product} from './Product';

// represents a single order item in the cart
export class SingleOrderInCart {

  product! : Product;
  quantity! : number;

  constructor( product: Product, quantity: number) {
    this.product = product;
    this.quantity = quantity;
  }
}

// represents an order fetched from backend
// an order groups multiple SingleRegisteredOrder items
// each SingleRegisteredOrder represents a single product in the order
export class RegisteredOrder {
  id! : string;
  userEmall!: string;

  orderStatus!: string;
  orderDate! : Date;
  orderItemList!: SingleRegisteredOrder[];

  coonstructor(id: string, userEmall: string, orderStatus: string, orderDate: Date, orderItemList: SingleRegisteredOrder[]) {
    this.id = id;
    this.userEmall = userEmall;
    this.orderStatus = orderStatus;
    this.orderDate = orderDate;
    this.orderItemList = orderItemList;
  }

}

// represents an order fetched from backend
export class SingleRegisteredOrder {
  id! : string;
  productId! : string;
  quantity! : number;

  // to be filled later when displaying order history
  product!: Product | null;

  constructor(id: string, productId: string, quantity: number) {
    this.id = id;
    this.productId = productId;
    this.quantity = quantity;

  }
}
