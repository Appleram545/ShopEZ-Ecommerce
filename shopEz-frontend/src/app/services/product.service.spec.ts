import { TestBed } from "@angular/core/testing";

import {
  HttpClientTestingModule,
  HttpTestingController,
} from "@angular/common/http/testing";

import { ProductService } from "./product.service";

import { Product } from "../models/product";

import { environment } from "../../environments/environment";

describe("ProductService", () => {
  let service: ProductService;

  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/product`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
    });

    service = TestBed.inject(ProductService);

    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  // GET ALL PRODUCTS

  it("should fetch all products", () => {
    const mockProducts: Product[] = [
      {
        id: 1,
        name: "iPhone",
        description: "Apple mobile",
        price: 100000,
        imageUrl: "iphone.jpg",
        category: "Smartphones",
        stock: 10,
      },
    ];

    service.getProducts().subscribe((products) => {
      expect(products.length).toBe(1);

      expect(products[0].name).toBe("iPhone");
    });

    const req = httpMock.expectOne(apiUrl);

    expect(req.request.method).toBe("GET");

    req.flush(mockProducts);
  });

  // GET SINGLE PRODUCT

  it("should fetch single product", () => {
    const mockProduct: Product = {
      id: 1,
      name: "iPhone",
      description: "Apple mobile",
      price: 100000,
      imageUrl: "iphone.jpg",
      category: "Smartphones",
      stock: 10,
    };

    service.getProduct(1).subscribe((product) => {
      expect(product.id).toBe(1);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);

    expect(req.request.method).toBe("GET");

    req.flush(mockProduct);
  });

  // ADD PRODUCT

  it("should add product", () => {
    const newProduct = {
      name: "Samsung",
      description: "Samsung mobile",
      price: 50000,
      imageUrl: "samsung.jpg",
      category: "Smartphones",
      stock: 5,
    };

    service.addProduct(newProduct).subscribe();

    const req = httpMock.expectOne(apiUrl);

    expect(req.request.method).toBe("POST");

    req.flush(newProduct);
  });

  // UPDATE PRODUCT

  it("should update product", () => {
    const updatedProduct: Product = {
      id: 1,
      name: "Updated iPhone",
      description: "Updated mobile",
      price: 120000,
      imageUrl: "iphone.jpg",
      category: "Smartphones",
      stock: 20,
    };

    service.updateProduct(1, updatedProduct).subscribe();

    const req = httpMock.expectOne(`${apiUrl}/1`);

    expect(req.request.method).toBe("PUT");

    req.flush(updatedProduct);
  });

  // DELETE PRODUCT

  it("should delete product", () => {
    service.deleteProduct(1).subscribe();

    const req = httpMock.expectOne(`${apiUrl}/1`);

    expect(req.request.method).toBe("DELETE");

    req.flush(null);
  });
});
