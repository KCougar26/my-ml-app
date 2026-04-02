import { useState } from 'react';
import './App.css';
import { CustomerSelect } from './components/CustomerSelect';
import { Dashboard } from './components/Dashboard';
import { OrderHistory } from './components/OrderHistory';
import { Warehouse } from './components/Warehouse';

// -----------------------------
// Top-level app container and basic routing
// -----------------------------
function App() {
  // Frontend state: selected customer (null means not selected yet)
  const [customerId, setCustomerId] = useState<number | null>(null);
  // Frontend state: current view/tab
  const [view, setView] = useState<'dash' | 'history' | 'wh'>('dash');

  // UI: show customer selection before allowing navigation
  if (!customerId) return <CustomerSelect onSelect={setCustomerId} />;

  return (
    <div className="container">
      <nav className="nav flex-row">
        <div>
          <button className={view === 'dash' ? 'active' : ''} onClick={() => setView('dash')}>
            Dashboard
          </button>
          <button className={view === 'history' ? 'active' : ''} onClick={() => setView('history')}>
            Order History
          </button>
          <button className={view === 'wh' ? 'active' : ''} onClick={() => setView('wh')}>
            Warehouse
          </button>
        </div>
        <button onClick={() => setCustomerId(null)}>Logout (Customer: {customerId})</button>
      </nav>

      {view === 'dash' && <Dashboard customerId={customerId} />}
      {view === 'history' && <OrderHistory customerId={customerId} />}
      {view === 'wh' && <Warehouse />}
    </div>
  );
}

export default App;
