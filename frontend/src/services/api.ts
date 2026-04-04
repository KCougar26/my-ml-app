/// <reference types="vite/client" />
import axios from 'axios';

// -----------------------------
// Backend data shapes (Matches .NET Models)
// -----------------------------

export interface CustomerOption {
  customerId: number;
  fullName: string;
}

export interface OrderSummary {
  orderId: number;
  orderTotal: number;
  orderDatetime: string; 
}

export interface CustomerDashboardData {
  totalOrders: number;
  totalSpent: number;
  recentOrders: OrderSummary[];
}

export interface OrderHistoryItem {
  orderId: number;
  orderTotal: number;
  orderDatetime: string;
}

export interface PriorityQueueItem {
  orderId: number;
  lateDeliveryProbability: number;
}

export interface NewOrderRequest {
  customerId: number;
  orderTotal: number;
  orderNotes?: string; 
}

export interface FraudResponse {
  probability: number;
  risk_level: 'HIGH' | 'MEDIUM' | 'LOW';
  is_fraud: boolean;
}

// -----------------------------
// Axios client configuration
// -----------------------------

// This instance handles your standard .NET Backend (Port 5020)
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5020/api',
});

// -----------------------------
// API Call Methods
// -----------------------------

// 1. Customer selection
export const getCustomers = () => 
  api.get<CustomerOption[]>('/store/customers');

// 2. Dashboard summary
export const getCustomerDashboard = (id: number) =>
  api.get<CustomerDashboardData>(`/store/customers/${id}/dashboard`);

// 3. Place a new order
export const placeOrder = (order: NewOrderRequest) => 
  api.post('/store/orders', order);

// 4. Full order history
export const getOrderHistory = (customerId: number) =>
  api.get<OrderHistoryItem[]>(`/store/customers/${customerId}/orders`);

// 5. Warehouse / ML Priority Queue
export const getPriorityQueue = () => 
  api.get<PriorityQueueItem[]>('/store/warehouse/priority-queue');

// 6. Trigger the ML Scoring Job
export const runScoring = () => 
  api.post('/store/warehouse/run-scoring');

// 7. Fraud Detection Check (Python FastAPI Bridge - Port 8000)
export const checkOrderRisk = async (orderData: any): Promise<FraudResponse> => {
  // We use a direct axios.post here because the port (8000) 
  // is different from the .NET base URL (5020).
  const response = await axios.post<FraudResponse>(
    'http://localhost:8000/predict', 
    orderData
  );
  
  return response.data;
};

export default api;