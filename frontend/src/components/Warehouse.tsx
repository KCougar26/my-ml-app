import React, { useEffect, useState } from 'react';
import { getPriorityQueue, runScoring } from '../services/api';

export const Warehouse = () => {
  const [queue, setQueue] = useState([]);
  const [loading, setLoading] = useState(false);

  const loadQueue = () => getPriorityQueue().then(res => setQueue(res.data));
  useEffect(() => { loadQueue(); }, []);

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
        <button className="btn btn-primary" onClick={handleScoring}>{loading ? "Scoring..." : "Run Scoring"}</button>
      </div>
      <table>
        <thead><tr><th>Order ID</th><th>Risk</th></tr></thead>
        <tbody>
          {queue.map((o: any) => (
            <tr key={o.orderId}>
              <td>#{o.orderId}</td>
              <td>{(o.lateDeliveryProbability * 100).toFixed(1)}%</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};