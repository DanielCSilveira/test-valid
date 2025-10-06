import { apiClient } from './api';
import { type CustomerDto } from '../types/customer';

export const customerService = {
  getAll: async (): Promise<CustomerDto[]> => {
    return apiClient.get<CustomerDto[]>('/api/Customers');
  },

  getById: async (id: string): Promise<CustomerDto> => {
    return apiClient.get<CustomerDto>(`/api/Customers/${id}`);
  },

  create: async (customer: Partial<CustomerDto>): Promise<CustomerDto> => {
    return apiClient.post<CustomerDto>('/api/Customers', customer);
  },

  update: async (id: string, customer: Partial<CustomerDto>): Promise<void> => {
    return apiClient.put(`/api/Customers/${id}`, customer);
  },

  delete: async (id: string): Promise<void> => {
    return apiClient.delete(`/api/Customers/${id}`);
  },
};
