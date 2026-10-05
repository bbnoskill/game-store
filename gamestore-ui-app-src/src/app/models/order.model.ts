export interface OrderItem {
  productId: string;
  price: number;
  quantity: number;
  discount: number;
  productName?: string;
}

export interface Order {
  id: string;
  customerId?: string;
  date?: string;
  status?: string;
  orderGames?: OrderItem[];
  details?: OrderItem[];
}

export interface PaymentMethod {
  title: string;
  description: string;
  imageUrl?: string;
}

export interface PaymentRequest {
  model?: {
    holder?: string;
    cardNumber?: string;
    monthExpire?: number;
    yearExpire?: number;
    cvv2?: number;
  };
  method: string;
}

export interface OrderHistoryItem {
  id: string;
  date: string;
  total?: number;
  items?: OrderItem[];
}
