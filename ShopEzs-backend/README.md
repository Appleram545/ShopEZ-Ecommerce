# ShopEz – Microservices E-Commerce Backend


## Default Admin Credentials
```
Email:    admin@mail.com
Password: admin123
```



## Dapper Integration (ProductService)

ProductService uses **Dapper** instead of EF Core for:
- Product listing with pagination
- Search & filtering by name/description
- High-performance read queries

## Running Unit Tests

```bash
# UserService tests
cd UserService && dotnet test

# ProductService tests
cd ProductService && dotnet test

# OrderService tests
cd OrderService && dotnet test

# CartService tests
cd CartService && dotnet test
```



