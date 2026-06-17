import { CartItem } from "./cart-item";

export interface Order {
  id?: number;
  orderId?: number;
  userId?: number;
  customerId?: number;
  status?: string;
  date?: Date; 
  createdAt?: Date;
  items: CartItem[];
  total?: number;
  customerName?: string;
  customerEmail?: string;
  address?: string;
}

export interface OrderRequest {
  items: {
    productId: number;
    qty: number;
  }[];
}
