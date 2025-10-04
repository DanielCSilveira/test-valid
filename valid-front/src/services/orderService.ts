import { apiClient } from './api';
import { type OrderDto, type OrderUpdateStatusDto } from '../types/order';

export const orderService = {
  getAll: async (): Promise<OrderDto[]> => {
    return apiClient.get<OrderDto[]>('/api/orders');
  },

  getById: async (id: string): Promise<OrderDto> => {
    return apiClient.get<OrderDto>(`/api/orders/${id}`);
  },

  create: async (order: Partial<OrderDto>): Promise<OrderDto> => {
    return apiClient.post<OrderDto>('/api/orders', order);
  },

  cancelar: async (id: string): Promise<void> => {
    const order: OrderUpdateStatusDto = {
      newStatus: "CANCELED"
    };
    return apiClient.put(`/api/orders/${id}/status`, order);
  },
};
