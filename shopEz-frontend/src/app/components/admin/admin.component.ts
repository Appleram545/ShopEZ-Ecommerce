import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ProductService } from "../../services/product.service";
import { OrderService } from "../../services/order.service";
import { Product } from "../../models/product";
import { Order } from "../../models/order";

@Component({
  selector: "app-admin",
  templateUrl: "./admin.component.html",
  styleUrls: ["./admin.component.css"],
})
export class AdminComponent implements OnInit {
  activeTab: "products" | "orders" = "products";
  products: Product[] = [];
  orders: any[] = [];
  productForm!: FormGroup;
  editingProduct: Product | null = null;
  showForm = false;
  successMsg = "";
  expandedOrderId: number | null = null;

  constructor(
    private productService: ProductService,
    private orderService: OrderService,
    private fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadOrders();
    this.initForm();
  }

  initForm(product?: Product): void {
    this.productForm = this.fb.group({
      name: [product?.name || "", Validators.required],

      // FIXED HERE
      description: [product?.desc || "", Validators.required],

      price: [product?.price || "", [Validators.required, Validators.min(1)]],

      imageUrl: [product?.imageUrl || "", Validators.required],

      category: [product?.category || "", Validators.required],

      stock: [product?.stock || "", [Validators.required, Validators.min(0)]],
    });
  }

  loadProducts(): void {
    this.productService.getProducts().subscribe((p) => {
      this.products = p;
    });
  }

  loadOrders(): void {
    this.orderService.getOrders().subscribe({
      next: (orders: any[]) => {
        this.orders = orders;
        console.log("Admin orders:", orders);
      },
      error: (err) => console.error("Load orders error:", err),
    });
  }

  toggleOrder(orderId: number): void {
    this.expandedOrderId =
      this.expandedOrderId === orderId ? null : orderId;
  }

  openAddForm(): void {
    this.editingProduct = null;
    this.initForm();
    this.showForm = true;
  }

  openEditForm(product: Product): void {
    this.editingProduct = product;
    this.initForm(product);
    this.showForm = true;
  }

  cancelForm(): void {
    this.showForm = false;
    this.editingProduct = null;
  }

  submitForm(): void {
    console.log("SUBMIT CLICKED");

    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    // FIXED HERE
    const form = this.productForm.value;

    const data = {
      name: form.name,
      desc: form.description,
      price: form.price,
      imageUrl: form.imageUrl,
      category: form.category,
      stock: form.stock,
    };

    if (this.editingProduct) {
      this.productService
        .updateProduct(this.editingProduct.id, {
          ...data,
          id: this.editingProduct.id,
        })
        .subscribe({
          next: () => {
            this.loadProducts();
            this.showMsg("Product updated!");
            this.cancelForm();
          },
          error: (err) => {
            console.error("Update failed:", err);
          },
        });
    } else {
      this.productService.addProduct(data).subscribe({
        next: () => {
          this.loadProducts();
          this.showMsg("Product added!");
          this.cancelForm();
        },
        error: (err) => {
          console.error("Add failed:", err);
        },
      });
    }
  }

  deleteProduct(id: number): void {
    if (confirm("Delete this product?")) {
      this.productService.deleteProduct(id).subscribe({
        next: () => {
          this.products = this.products.filter((p) => p.id !== id);

          this.showMsg("Product deleted!");
        },
        error: (err) => {
          console.error("Delete failed:", err);
        },
      });
    }
  }

  showMsg(msg: string): void {
    this.successMsg = msg;

    setTimeout(() => {
      this.successMsg = "";
    }, 3000);
  }
}