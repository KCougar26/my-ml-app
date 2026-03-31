import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5020/api', 
});

export const getCustomers = () => api.get('/store/customers');
export const getCustomerDashboard = (id: number) => api.get(`/store/customers/${id}/dashboard`);
export const getPriorityQueue = () => api.get('/store/warehouse/priority-queue');
export const runScoring = () => api.post('/store/warehouse/run-scoring');
export const placeOrder = (order: any) => api.post('/store/orders', order);