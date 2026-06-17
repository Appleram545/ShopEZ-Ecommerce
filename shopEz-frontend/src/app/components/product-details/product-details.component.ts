import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { ProductService } from "../../services/product.service";
import { CartService } from "../../services/cart.service";
import { AuthService } from "../../services/auth.service";
import { Product } from "../../models/product";

@Component({
  selector: "app-product-details",
  templateUrl: "./product-details.component.html",
  styleUrls: ["./product-details.component.css"],
})
export class ProductDetailsComponent implements OnInit {
  product: Product | undefined;
  quantity = 1;
  added = false;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    public cart: CartService,
    public auth: AuthService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get("id"));

    this.productService.getProduct(id).subscribe({
      next: (p: any) => {
        console.log("FULL PRODUCT:", p);

        this.product = {
          id: p.id,
          name: p.name,
          desc: p.desc,
          price: p.price,
          imageUrl: p.imageUrl,
          category: p.category,
          stock: p.stock,
        };

        console.log("DESC VALUE:", this.product.desc);
      },

      error: (err) => {
        console.error(err);
      },
    });
  }

  addToCart(): void {
    if (!this.auth.isLoggedIn) {
      this.router.navigate(["/login"]);
      return;
    }
    if (!this.product) return;
    for (let i = 0; i < this.quantity; i++) {
      this.cart.addToCart(this.product);
    }
    this.added = true;
    setTimeout(() => (this.added = false), 2000);
  }

  goBack(): void {
    this.router.navigate(["/products"]);
  }
}
