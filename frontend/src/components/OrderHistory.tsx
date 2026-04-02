import React, { useEffect, useState } from 'react';
import { getOrderHistory } from '../services/api';
import type { OrderHistoryItem } from '../services/api';

// -----------------------------
// Order History page for the selected customer
// -----------------------------
export const OrderHistory = ({ customerId }: { customerId: number }) => {
  // Frontend state: list of historical orders from backend
  const [orders, setOrders] = useState<OrderHistoryItem[]>([]);
  // Frontend state: loading indicator
  const [loading, setLoading] = useState(true);

  // Backend integration: fetch order history for the selected customer
  const loadHistory = () => getOrderHistory(customerId).then(res => setOrders(res.data));

  // React lifecycle: load history when customer changes
  useEffect(() => {
    setLoading(true);
    loadHistory().finally(() => setLoading(false));
  }, [customerId]);

  if (loading) return <p>Loading order history...</p>;

  return (
    <div className="card">
      <h2>Order History</h2>
      <table>
        <thead>
          <tr>
            <th>Order ID</th>
            <th>Date</th>
            <th>Status</th>
            <th>Total</th>
          </tr>
        </thead>
        <tbody>
          {orders.map(order => (
            <tr key={order.orderId}>
              <td>#{order.orderId}</td>
              <td>{new Date(order.orderDate).toLocaleDateString()}</td>
              <td>{order.orderStatus}</td>
              <td>${order.orderTotal.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
