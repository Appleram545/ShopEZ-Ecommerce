import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { CartService } from "../../services/cart.service";
import { CartItem } from "../../models/cart-item";

@Component({
  selector: "app-cart",
  templateUrl: "./cart.component.html",
  styleUrls: ["./cart.component.css"],
})
export class CartComponent {
  constructor(
    public cart: CartService,
    private router: Router,
  ) {}

  get items(): CartItem[] {
    return this.cart.cartItems;
  }

  updateQty(productId: number, qty: number): void {
    this.cart.updateQuantity(productId, qty);
  }

  remove(productId: number): void {
    this.cart.removeFromCart(productId);
  }

  checkout(): void {
    this.router.navigate(["/checkout"]);
  }

  continueShopping(): void {
    this.router.navigate(["/products"]);
  }
}
