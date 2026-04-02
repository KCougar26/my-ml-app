import { useEffect, useState } from 'react';
import { getCustomerDashboard, placeOrder } from '../services/api';
import type { CustomerDashboardData, NewOrderRequest } from '../services/api';

// -----------------------------
// Customer Dashboard (summaries + recent orders + new order)
// -----------------------------
export const Dashboard = ({ customerId }: { customerId: number }) => {
  // Frontend state: dashboard data from backend
  const [data, setData] = useState<CustomerDashboardData | null>(null);
  // Frontend state: new order form inputs
  const [orderTotalInput, setOrderTotalInput] = useState<string>('49.99');
  const [orderNotesInput, setOrderNotesInput] = useState<string>('Standard delivery');
  // Frontend state: submission status
  const [creatingOrder, setCreatingOrder] = useState(false);

  // Backend integration: load dashboard summary data
  const loadData = () => getCustomerDashboard(customerId).then(res => setData(res.data));

  // React lifecycle: refresh data when customer changes
  useEffect(() => { loadData(); }, [customerId]);

  // UI handler: submit a new order to the backend
  const handleNewOrder = async () => {
    const orderTotal = Number(orderTotalInput);
    if (Number.isNaN(orderTotal) || orderTotal <= 0) return;

    const newOrderPayload: NewOrderRequest = {
      customerId,
      orderTotal,
      orderNotes: orderNotesInput,
    };

    setCreatingOrder(true);
    try {
      await placeOrder(newOrderPayload);
      setOrderTotalInput('49.99'); // Reset form
      loadData();
    } catch (error) {
      console.error("Order failed", error);
    } finally {
      setCreatingOrder(false);
    }
  };

  if (!data) return <p>Loading...</p>;

  return (
    <div>
      <div className="grid">
        <div className="card">
          <h3>Total Orders</h3>
          <p>{data.totalOrders}</p>
        </div>
        <div className="card">
          <h3>Total Spent</h3>
          <p>${data.totalSpent?.toFixed(2)}</p>
        </div>
      </div>

      <div className="card">
        <h3>Recent Orders</h3>
        <ul>
          {data.recentOrders.map(o => (
            <li key={o.orderId}>
              {/* FIXED: Changed o.orderDate to o.orderDatetime */}
              Order #{o.orderId}: ${o.orderTotal.toFixed(2)} on {new Date(o.orderDatetime).toLocaleDateString()}
            </li>
          ))}
        </ul>
      </div>

      <div className="card">
        <h3>Place New Order</h3>
        <div className="form-grid">
          <label style={{ display: 'block', marginBottom: '10px' }}>
            Order Total (USD)
            <input
              type="number"
              min="0"
              step="0.01"
              style={{ width: '100%', padding: '8px', marginTop: '5px' }}
              value={orderTotalInput}
              onChange={(e) => setOrderTotalInput(e.target.value)}
            />
          </label>
          <label style={{ display: 'block', marginBottom: '10px' }}>
            Order Notes
            <input
              type="text"
              style={{ width: '100%', padding: '8px', marginTop: '5px' }}
              value={orderNotesInput}
              onChange={(e) => setOrderNotesInput(e.target.value)}
            />
          </label>
        </div>
        <button 
          className="btn btn-success" 
          onClick={handleNewOrder} 
          disabled={creatingOrder}
        >
          {creatingOrder ? 'Placing Order...' : 'Place Order'}
        </button>
      </div>
    </div>
  );
};