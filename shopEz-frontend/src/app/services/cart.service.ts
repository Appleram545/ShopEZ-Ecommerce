import { Injectable } from "@angular/core";
import { BehaviorSubject, skip } from "rxjs";
import { CartItem } from "../models/cart-item";
import { Product } from "../models/product";
import { AuthService } from "./auth.service";

@Injectable({ providedIn: "root" })
export class CartService {
  private cartSubject = new BehaviorSubject<CartItem[]>([]);
  cart$ = this.cartSubject.asObservable();

  constructor(private auth: AuthService) {
    this.auth.logoutEvent.pipe(skip(1)).subscribe(() => {
      this.cartSubject.next([]);
    });
  }

  get cartItems(): CartItem[] {
    return this.cartSubject.value;
  }
  get itemCount(): number {
    return this.cartItems.reduce((sum, i) => sum + i.quantity, 0);
  }
  get total(): number {
    return this.cartItems.reduce(
      (sum, i) => sum + i.product.price * i.quantity,
      0,
    );
  }

  addToCart(product: Product): void {
    const items = [...this.cartItems];

    const existing = items.find((i) => i.product.id === product.id);

    if (existing) {
      // STOP adding above stock
      if (existing.quantity >= product.stock) {
        alert("Only " + product.stock + " items available in stock");
        return;
      }

      existing.quantity++;
    } else {
      items.push({
        product,
        quantity: 1,
      });
    }

    this.cartSubject.next(items);
  }

  removeFromCart(productId: number): void {
    this.cartSubject.next(
      this.cartItems.filter((i) => i.product.id !== productId),
    );
  }

  updateQuantity(productId: number, quantity: number): void {
    const items = [...this.cartItems];

    const item = items.find((i) => i.product.id === productId);

    if (!item) return;

    // remove if qty 0
    if (quantity <= 0) {
      this.removeFromCart(productId);
      return;
    }

    // STOP above stock
    if (quantity > item.product.stock) {
      alert("Maximum stock reached");
      return;
    }

    item.quantity = quantity;

    this.cartSubject.next(items);
  }

  clearCart(): void {
    this.cartSubject.next([]);
  }
}
