import { useEffect, useState } from 'react'; // FIXED: Removed 'React'
import { getPriorityQueue, runScoring } from '../services/api';
import type { PriorityQueueItem } from '../services/api';

// -----------------------------
// Warehouse screen (late-delivery priority queue)
// -----------------------------
export const Warehouse = () => {
  // Frontend state: priority queue items (top 50 from backend)
  const [queue, setQueue] = useState<PriorityQueueItem[]>([]);
  // Frontend state: scoring run in progress
  const [loading, setLoading] = useState(false);

  // Backend integration: fetch priority queue
  const loadQueue = () => {
    setLoading(true);
    getPriorityQueue()
      .then(res => setQueue(res.data))
      .finally(() => setLoading(false));
  };

  // React lifecycle: initial load
  useEffect(() => { 
    loadQueue(); 
  }, []);

  // UI handler: trigger ML scoring and refresh queue
  const handleScoring = async () => {
    setLoading(true);
    try {
      await runScoring();
      // Small delay to let the backend process the model results
      setTimeout(() => loadQueue(), 500); 
    } catch (err) {
      console.error("Scoring failed:", err);
      setLoading(false);
    }
  };

  return (
    <div className="card">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
        <h2 style={{ margin: 0 }}>Late Delivery Priority Queue</h2>
        <button 
          className="btn btn-primary" 
          onClick={handleScoring} 
          disabled={loading}
        >
          {loading ? 'Scoring...' : 'Run Scoring'}
        </button>
      </div>
      
      <p>Showing {queue.length} orders by predicted late-delivery probability.</p>
      
      <table>
        <thead>
          <tr>
            <th>Order ID</th>
            <th>Risk</th>
            <th style={{ width: '40%' }}>Visual Risk Level</th>
          </tr>
        </thead>
        <tbody>
          {queue.map(o => {
            const riskPercent = o.riskScore || 0;
            return (
              <tr key={o.orderId}>
                <td>#{o.orderId}</td>
                <td>{riskPercent.toFixed(1)}%</td>
                <td>
                  <div className="progress-bar" style={{ background: '#eee', borderRadius: '4px', height: '10px' }}>
                    <div 
                      className="progress-fill" 
                      style={{ 
                        width: `${riskPercent}%`, 
                        height: '100%', 
                        background: riskPercent > 70 ? '#ff4d4f' : riskPercent > 40 ? '#faad14' : '#52c41a',
                        borderRadius: '4px',
                        transition: 'width 0.5s ease'
                      }} 
                    />
                  </div>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
};
