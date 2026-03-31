import React, { useEffect, useState } from 'react';
import { getCustomers } from '../services/api';

export const CustomerSelect = ({ onSelect }: { onSelect: (id: number) => void }) => {
  const [customers, setCustomers] = useState<any[]>([]);

  useEffect(() => {
    getCustomers().then(res => setCustomers(res.data));
  }, []);

  return (
    <div className="card" style={{maxWidth: '400px', margin: '100px auto'}}>
      <h2>Select Customer</h2>
      <select style={{width: '100%', padding: '10px'}} onChange={(e) => onSelect(Number(e.target.value))}>
        <option value="">Choose...</option>
        {customers.map(c => <option key={c.customerId} value={c.customerId}>{c.fullName}</option>)}
      </select>
    </div>
  );
};