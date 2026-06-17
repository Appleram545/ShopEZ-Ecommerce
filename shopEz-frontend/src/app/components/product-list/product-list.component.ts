import { Component, OnInit } from "@angular/core";
import { ProductService } from "../../services/product.service";
import { CartService } from "../../services/cart.service";
import { AuthService } from "../../services/auth.service";
import { Product } from "../../models/product";
import { Router } from "@angular/router";

@Component({
  selector: "app-product-list",
  templateUrl: "./product-list.component.html",
  styleUrls: ["./product-list.component.css"],
})
export class ProductListComponent implements OnInit {
  Math = Math;
  allProducts: Product[] = [];
  filteredProducts: Product[] = [];
  pagedProducts: Product[] = [];
  categories: string[] = [];
  selectedCategory = "All";
  searchTerm = "";
  addedProductId: number | null = null;

  // Pagination
  currentPage = 1;
  pageSize = 12;
  totalPages = 1;
  pages: number[] = [];

  constructor(
    private productService: ProductService,
    public cart: CartService,
    public auth: AuthService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.productService.getProducts().subscribe((products) => {
      this.allProducts = products;
      this.categories = ["All", ...new Set(products.map((p) => p.category))];
      this.applyFilters();
    });
  }

  applyFilters(): void {
    this.filteredProducts = this.allProducts.filter((p) => {
      const matchCat =
        this.selectedCategory === "All" || p.category === this.selectedCategory;
      const matchSearch =
        p.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        p.desc.toLowerCase().includes(this.searchTerm.toLowerCase());
      return matchCat && matchSearch;
    });
    this.currentPage = 1;
    this.updatePagination();
  }

  updatePagination(): void {
    this.totalPages = Math.ceil(this.filteredProducts.length / this.pageSize);
    this.pages = Array.from({ length: this.totalPages }, (_, i) => i + 1);
    this.setPage(this.currentPage);
  }

  setPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    const start = (page - 1) * this.pageSize;
    this.pagedProducts = this.filteredProducts.slice(
      start,
      start + this.pageSize,
    );
    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  selectCategory(cat: string): void {
    this.selectedCategory = cat;
    this.applyFilters();
  }

  onSearch(): void {
    this.applyFilters();
  }

  addToCart(product: Product): void {
    if (!this.auth.isLoggedIn) {
      this.router.navigate(["/login"]);
      return;
    }
    this.cart.addToCart(product);
    this.addedProductId = product.id;
    setTimeout(() => (this.addedProductId = null), 1500);
  }

  viewDetails(id: number): void {
    this.router.navigate(["/products", id]);
  }

  getVisiblePages(): number[] {
    const delta = 2;
    const range: number[] = [];
    for (
      let i = Math.max(2, this.currentPage - delta);
      i <= Math.min(this.totalPages - 1, this.currentPage + delta);
      i++
    ) {
      range.push(i);
    }
    if (this.currentPage - delta > 2) range.unshift(-1);
    if (this.currentPage + delta < this.totalPages - 1) range.push(-1);
    if (this.totalPages > 1) range.unshift(1);
    if (this.totalPages > 1) range.push(this.totalPages);
    return [...new Set(range)];
  }
}
