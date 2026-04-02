import axios from 'axios';

// -----------------------------
// Backend data shapes (frontend expects these from the API)
// -----------------------------
export type CustomerOption = {
  customerId: number;
  fullName: string;
};

export type OrderSummary = {
  orderId: number;
  orderTotal: number;
  orderDate: string;
};

export type CustomerDashboardData = {
  customerId: number;
  totalOrders: number;
  totalSpent: number;
  recentOrders: OrderSummary[];
};

export type OrderHistoryItem = {
  orderId: number;
  orderTotal: number;
  orderDate: string;
  orderStatus: string;
};

export type PriorityQueueItem = {
  orderId: number;
  lateDeliveryProbability: number;
};

export type NewOrderRequest = {
  customerId: number;
  orderTotal: number;
  orderNotes: string;
};

// -----------------------------
// Axios client (backend team: update baseURL to your deployed API)
// -----------------------------
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5020/api',
});

// -----------------------------
// Customer selection and dashboard
// -----------------------------
export const getCustomers = () => api.get<CustomerOption[]>('/store/customers');
export const getCustomerDashboard = (id: number) =>
  api.get<CustomerDashboardData>(`/store/customers/${id}/dashboard`);

// -----------------------------
// Orders: create and history
// -----------------------------
export const placeOrder = (order: NewOrderRequest) => api.post('/store/orders', order);
export const getOrderHistory = (customerId: number) =>
  api.get<OrderHistoryItem[]>(`/store/customers/${customerId}/orders`);

// -----------------------------
// Warehouse: late-delivery priority queue + scoring trigger
// -----------------------------
export const getPriorityQueue = () => api.get<PriorityQueueItem[]>('/store/warehouse/priority-queue');
export const runScoring = () => api.post('/store/warehouse/run-scoring');
