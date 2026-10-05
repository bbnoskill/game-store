export interface CartItem {
  id?: string;
  productId: string;
  gameKey?: string;
  gameName: string;
  price: number;
  quantity: number;
  discount: number;
  image?: string;
  platform?: string;
  fileSize?: string;
}

export interface CartSummary {
  items: CartItem[];
  subtotal: number;
  taxes: number;
  total: number;
  itemCount: number;
}
