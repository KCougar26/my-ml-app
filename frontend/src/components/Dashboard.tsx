import { useEffect, useState } from 'react';
// 1. ADD checkOrderRisk TO YOUR IMPORTS
import { getCustomerDashboard, placeOrder, checkOrderRisk } from '../services/api';
import type { CustomerDashboardData, NewOrderRequest, FraudResponse } from '../services/api';

export const Dashboard = ({ customerId }: { customerId: number }) => {
  const [data, setData] = useState<CustomerDashboardData | null>(null);
  const [orderTotalInput, setOrderTotalInput] = useState<string>('49.99');
  const [orderNotesInput, setOrderNotesInput] = useState<string>('Standard delivery');
  const [creatingOrder, setCreatingOrder] = useState(false);
  
  // 2. ADD A STATE TO TRACK THE ML RESULT
  const [fraudRisk, setFraudRisk] = useState<FraudResponse | null>(null);

  const loadData = () => getCustomerDashboard(customerId).then(res => setData(res.data));

  useEffect(() => { loadData(); }, [customerId]);

  // 3. UPDATED HANDLER
  const handleNewOrder = async () => {
    const orderTotal = Number(orderTotalInput);
    if (Number.isNaN(orderTotal) || orderTotal <= 0) return;

    setCreatingOrder(true);
    
    try {
      // --- STEP A: ML CHECK ---
      // We send the data to the Python bridge first
      const riskCheck = await checkOrderRisk({
        customerId: customerId,
        orderTotal: orderTotal,
        orderNotes: orderNotesInput
      });
      
      setFraudRisk(riskCheck);

      // --- STEP B: BUSINESS LOGIC ---
      // If risk is CRITICAL, maybe we block the order?
      if (riskCheck.risk_level === 'HIGH') {
        const confirm = window.confirm("⚠️ HIGH FRAUD RISK DETECTED. Proceed anyway?");
        if (!confirm) {
          setCreatingOrder(false);
          return;
        }
      }

      // --- STEP C: SAVE TO DATABASE ---
      const newOrderPayload: NewOrderRequest = {
        customerId,
        orderTotal,
        orderNotes: orderNotesInput,
      };

      await placeOrder(newOrderPayload);
      setOrderTotalInput('49.99'); 
      setFraudRisk(null); // Clear risk after success
      loadData();
    } catch (error) {
      console.error("Order failed", error);
      alert("Error: Check if your Python API (Port 8000) is running!");
    } finally {
      setCreatingOrder(false);
    }
  };

  if (!data) return <p>Loading...</p>;

  return (
    <div>
      {/* ... (Keep your existing summary grid and recent orders) ... */}

      <div className="card">
        <h3>Place New Order</h3>
        
        {/* 4. SHOW A RISK BADGE IF DATA EXISTS */}
        {fraudRisk && (
          <div style={{ 
            padding: '10px', 
            borderRadius: '5px', 
            marginBottom: '15px',
            backgroundColor: fraudRisk.risk_level === 'HIGH' ? '#ffebee' : '#e8f5e9',
            border: `1px solid ${fraudRisk.risk_level === 'HIGH' ? '#f44336' : '#4caf50'}`
          }}>
            <strong>ML Assessment:</strong> {fraudRisk.risk_level} RISK 
            ({(fraudRisk.probability * 100).toFixed(1)}% match)
          </div>
        )}

        <div className="form-grid">
           {/* ... (Keep your inputs here) ... */}
        </div>

        <button 
          className="btn btn-success" 
          onClick={handleNewOrder} 
          disabled={creatingOrder}
        >
          {creatingOrder ? 'Analyzing & Placing...' : 'Place Order'}
        </button>
      </div>
    </div>
  );
};