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
  orderDatetime: string; // Matches C# OrderDatetime
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
  // Note: 'orderNotes' is not in the current shop.db schema, 
  // so we keep it optional to prevent backend errors.
  orderNotes?: string; 
}

// -----------------------------
// Axios client configuration
// -----------------------------
const api = axios.create({
  // Use VITE_API_BASE_URL in Vercel settings. 
  // Falls back to localhost for your MacBook development.
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

export default api;