import React, { useEffect, useState } from 'react';
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
  const loadQueue = () => getPriorityQueue().then(res => setQueue(res.data));

  // React lifecycle: initial load
  useEffect(() => { loadQueue(); }, []);

  // UI handler: trigger ML scoring and refresh queue
  const handleScoring = async () => {
    setLoading(true);
    await runScoring();
    await loadQueue();
    setLoading(false);
  };

  return (
    <div className="card">
      <div className="flex-row">
        <h2>Late Delivery Priority Queue</h2>
        <button className="btn btn-primary" onClick={handleScoring} disabled={loading}>
          {loading ? 'Scoring...' : 'Run Scoring'}
        </button>
      </div>
      <p>Showing top {queue.length} orders by predicted late-delivery probability.</p>
      <table>
        <thead>
          <tr>
            <th>Order ID</th>
            <th>Risk</th>
            <th>Visual</th>
          </tr>
        </thead>
        <tbody>
          {queue.map(o => (
            <tr key={o.orderId}>
              <td>#{o.orderId}</td>
              <td>{(o.lateDeliveryProbability * 100).toFixed(1)}%</td>
              <td>
                <div className="progress-bar">
                  <div className="progress-fill" style={{ width: `${o.lateDeliveryProbability * 100}%` }} />
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
