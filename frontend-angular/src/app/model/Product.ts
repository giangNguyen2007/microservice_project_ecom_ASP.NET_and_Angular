export class Product {
  id! : string | null;
  title! : string;
  description! : string;
  photoUrl! : string;
  stock! : number;
  price! : number;
  createdDate! : Date;
  category!: string;

  constructor(  title: string, description: string, photoUrl: string, stock: number, price: number, createdDate: Date, category: string) {
    this.title = title;
    this.description = description;
    this.photoUrl = photoUrl;
    this.stock = stock;
    this.price = price;
    this.createdDate = createdDate;
    this.category = category;
  }



}
