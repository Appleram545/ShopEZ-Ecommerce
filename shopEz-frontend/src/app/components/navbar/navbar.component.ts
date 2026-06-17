import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { AuthService } from "../../services/auth.service";
import { CartService } from "../../services/cart.service";

@Component({
  selector: "app-navbar",
  templateUrl: "./navbar.component.html",
  styleUrls: ["./navbar.component.css"],
})
export class NavbarComponent implements OnInit {
  cartCount = 0;
  isMenuOpen = false;

  constructor(
    public auth: AuthService,
    public cart: CartService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.cart.cart$.subscribe(() => {
      this.cartCount = this.cart.itemCount;
    });
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(["/login"]);
  }

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }
}
