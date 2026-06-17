import { Component, OnInit } from "@angular/core";
import { OrderService } from "../../services/order.service";
import { AuthService } from "../../services/auth.service";
import { Order } from "../../models/order";
import { Router } from "@angular/router";

@Component({
  selector: "app-order-history",
  templateUrl: "./order-history.component.html",
  styleUrls: ["./order-history.component.css"],
})
export class OrderHistoryComponent implements OnInit {
  orders: Order[] = [];
  expandedOrderId: number | null = null;
  loading = true;
  error = "";

  constructor(
    private orderService: OrderService,
    public auth: AuthService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.orderService.getOrders().subscribe({
      next: (orders) => {
        this.orders = orders.sort(
          (a, b) =>
            new Date(b.createdAt || b.date!).getTime() -
            new Date(a.createdAt || a.date!).getTime(),
        );
        this.loading = false;
      },
      error: (err) => {
        console.error("Orders error:", err);
        this.error = "Could not load orders. Please try again.";
        this.loading = false;
        // Do NOT redirect to login here
      },
    });
  }

  toggleExpand(orderId: number): void {
    this.expandedOrderId = this.expandedOrderId === orderId ? null : orderId;
  }

  continueShopping(): void {
    this.router.navigate(["/products"]);
  }
}
