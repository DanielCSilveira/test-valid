export interface OrderDto {
  id: string;
  customerId: string;
  customerName:string;
  amount: number;
  status: string | null;
  createdAt: string;
}

export interface OrderUpdateStatusDto {
 newStatus:string;
}
