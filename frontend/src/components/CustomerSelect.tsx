import { useEffect, useState } from 'react'; // FIXED: Removed 'React'
import { getCustomers } from '../services/api';
import type { CustomerOption } from '../services/api';

// -----------------------------
// Select Customer screen (no auth)
// -----------------------------
export const CustomerSelect = ({ onSelect }: { onSelect: (id: number) => void }) => {
  // Frontend state: available customers to choose from
  const [customers, setCustomers] = useState<CustomerOption[]>([]);
  // Frontend state: current selected customer id (as a string for the <select>)
  const [selectedCustomerId, setSelectedCustomerId] = useState<string>('');

  // Backend integration: fetch selectable customers
  useEffect(() => {
    getCustomers().then(res => setCustomers(res.data));
  }, []);

  // UI handler: confirm selection and move into the app
  const handleContinue = () => {
    const idNum = Number(selectedCustomerId);
    if (!selectedCustomerId || isNaN(idNum)) return;
    onSelect(idNum);
  };

  return (
    <div className="card" style={{ maxWidth: '420px', margin: '100px auto' }}>
      <h2 style={{ marginBottom: '20px', textAlign: 'center' }}>Select Customer</h2>
      <select
        style={{ width: '100%', padding: '12px', borderRadius: '6px', border: '1px solid #ccc' }}
        value={selectedCustomerId}
        onChange={(e) => setSelectedCustomerId(e.target.value)}
      >
        <option value="">Choose a customer...</option>
        {customers.map(c => (
          <option key={c.customerId} value={c.customerId}>
            {c.fullName}
          </option>
        ))}
      </select>
      <button
        className="btn btn-primary"
        style={{ width: '100%', marginTop: '16px', padding: '12px' }}
        onClick={handleContinue}
        disabled={!selectedCustomerId}
      >
        Continue
      </button>
    </div>
  );
};