import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { CartService } from "../../services/cart.service";
import { OrderService } from "../../services/order.service";
import { AuthService } from "../../services/auth.service";

@Component({
  selector: "app-checkout",
  templateUrl: "./checkout.component.html",
  styleUrls: ["./checkout.component.css"],
})
export class CheckoutComponent implements OnInit {
  checkoutForm!: FormGroup;
  orderPlaced = false;
  orderId: number | null = null;
  isLoading = false;
  errorMsg = "";

  constructor(
    private fb: FormBuilder,
    public cart: CartService,
    private orderService: OrderService,
    public auth: AuthService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    if (this.cart.cartItems.length === 0) {
      this.router.navigate(["/cart"]);
      return;
    }
    this.checkoutForm = this.fb.group({
      name: [this.auth.currentUser?.name || "", Validators.required],
      email: [
        this.auth.currentUser?.email || "",
        [Validators.required, Validators.email],
      ],
      address: ["", Validators.required],
      city: ["", Validators.required],
      pincode: ["", [Validators.required, Validators.pattern(/^\d{6}$/)]],
    });
  }

  placeOrder(): void {
    if (this.checkoutForm.invalid) {
      this.checkoutForm.markAllAsTouched();
      return;
    }
    this.isLoading = true;
    this.errorMsg = "";

    
    const orderReq = {
      items: this.cart.cartItems.map((i) => ({
        productId: i.product.id,
        qty: i.quantity,
      })),
    };

    this.orderService.placeOrder(orderReq).subscribe({
      next: (order: any) => {
        this.orderId = order.id || order.orderId || null;
        this.orderPlaced = true;
        this.cart.clearCart();
        this.isLoading = false;
      },
      error: (err) => {
        console.error("Order error:", err);
        this.errorMsg =
          err?.error?.message ||
          err?.error?.title ||
          "Failed to place order. Please try again.";
        this.isLoading = false;
      },
    });
  }

  goOrders(): void {
    this.router.navigate(["/my-orders"]);
  }
  goShopping(): void {
    this.router.navigate(["/products"]);
  }
}
