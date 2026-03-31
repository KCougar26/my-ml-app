import React, { useEffect, useState } from 'react';
import { getCustomerDashboard, placeOrder } from '../services/api';

export const Dashboard = ({ customerId }: { customerId: number }) => {
  const [data, setData] = useState<any>(null);

  const loadData = () => getCustomerDashboard(customerId).then(res => setData(res.data));
  
  useEffect(() => { loadData(); }, [customerId]);

  const handleNewOrder = async () => {
    await placeOrder({ customerId, orderTotal: Math.random() * 100 });
    alert("Order Placed!");
    loadData();
  };

  if (!data) return <p>Loading...</p>;

  return (
    <div>
      <div className="grid">
        <div className="card"><h3>Total Orders</h3><p>{data.totalOrders}</p></div>
        <div className="card"><h3>Total Spent</h3><p>${data.totalSpent?.toFixed(2)}</p></div>
      </div>
      <div className="card">
        <h3>Recent Orders</h3>
        <ul>{data.recentOrders.map((o: any) => <li key={o.orderId}>Order #{o.orderId}: ${o.orderTotal.toFixed(2)}</li>)}</ul>
        <button className="btn btn-success" onClick={handleNewOrder}>Place New Test Order</button>
      </div>
    </div>
  );
};