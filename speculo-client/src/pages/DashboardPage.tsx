import { useState, useEffect } from 'react';
import { analyticsApi } from '../api/client';
import StatCard from '../components/StatCard';
import type { DashboardProjection } from '../types';

export default function DashboardPage() {
  const [data, setData] = useState<DashboardProjection | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchDashboard = async () => {
    setLoading(true);
    setError('');
    try {
      const res = await analyticsApi.get<DashboardProjection>('/api/stats/dashboard');
      setData(res.data);
    } catch {
      setError('Failed to load dashboard data');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchDashboard(); }, []);

  if (loading) {
    return (
      <div className="max-w-4xl mx-auto px-6 py-10">
        <p className="text-zinc-500 text-sm">Loading dashboard...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-4xl mx-auto px-6 py-10">
        <div className="rounded-lg bg-red-500/10 border border-red-500/20 px-4 py-3 text-sm text-red-400">
          {error}
        </div>
      </div>
    );
  }

  const hasData = data && (
    data.mood.totalEntries > 0 ||
    data.sleep.totalEntries > 0 ||
    data.money.totalTransactions > 0 ||
    data.workouts.totalWorkouts > 0
  );

  return (
    <div className="max-w-4xl mx-auto px-6 py-10">
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-2xl font-bold text-white">Dashboard</h1>
        <button
          onClick={fetchDashboard}
          className="px-3 py-1.5 rounded-lg border border-zinc-800 text-sm text-zinc-400 hover:text-white hover:bg-zinc-800 transition-colors"
        >
          Refresh
        </button>
      </div>

      {!hasData ? (
        <div className="rounded-xl border border-zinc-800 bg-zinc-900 p-8 text-center">
          <p className="text-zinc-400 text-sm">No data yet. Start logging events to see your stats here.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <StatCard
            icon="😊"
            title="Mood"
            stats={[
              { label: 'Total Entries', value: data!.mood.totalEntries },
              { label: 'Average Score', value: data!.mood.averageScore.toFixed(1) },
              { label: 'Latest Score', value: data!.mood.latestScore },
            ]}
          />
          <StatCard
            icon="😴"
            title="Sleep"
            stats={[
              { label: 'Total Entries', value: data!.sleep.totalEntries },
              { label: 'Avg Hours', value: data!.sleep.averageHours.toFixed(1) },
              { label: 'Avg Quality', value: data!.sleep.averageQuality.toFixed(1) },
            ]}
          />
          <StatCard
            icon="💰"
            title="Finances"
            stats={[
              { label: 'Total Income', value: `$${data!.money.totalIncome.toFixed(2)}` },
              { label: 'Total Expenses', value: `$${data!.money.totalExpenses.toFixed(2)}` },
              { label: 'Transactions', value: data!.money.totalTransactions },
            ]}
          />
          <StatCard
            icon="💪"
            title="Workouts"
            stats={[
              { label: 'Total Workouts', value: data!.workouts.totalWorkouts },
              { label: 'Total Minutes', value: data!.workouts.totalMinutes },
              { label: 'Avg Intensity', value: data!.workouts.averageScore.toFixed(1) },
            ]}
          />
        </div>
      )}
    </div>
  );
}
