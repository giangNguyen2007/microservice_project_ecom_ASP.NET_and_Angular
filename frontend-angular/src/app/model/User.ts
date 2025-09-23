// create a user class with the following properties:
export class User {

  email!: string;
  role!: "normal" | "admin" | null;

  constructor( email: string, role: "admin" | "normal" | null) {
    this.email = email;
    this.role = role;

  }

}
