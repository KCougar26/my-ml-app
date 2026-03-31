import { useState } from 'react';
import './App.css';
import { CustomerSelect } from './components/CustomerSelect';
import { Dashboard } from './components/Dashboard';
import { Warehouse } from './components/Warehouse';

function App() {
  const [userId, setUserId] = useState<number | null>(null);
  const [view, setView] = useState<'dash' | 'wh'>('dash');

  if (!userId) return <CustomerSelect onSelect={setUserId} />;

  return (
    <div className="container">
      <nav className="nav flex-row">
        <div>
          <button className={view === 'dash' ? 'active' : ''} onClick={() => setView('dash')}>Dashboard</button>
          <button className={view === 'wh' ? 'active' : ''} onClick={() => setView('wh')}>Warehouse</button>
        </div>
        <button onClick={() => setUserId(null)}>Logout (User: {userId})</button>
      </nav>

      {view === 'dash' ? <Dashboard customerId={userId} /> : <Warehouse />}
    </div>
  );
}

export default App;