import { useState } from 'react';
import { trackingApi } from '../api/client';

type Tab = 'mood' | 'sleep' | 'workout' | 'money';

export default function LogEventPage() {
  const [activeTab, setActiveTab] = useState<Tab>('mood');
  const [success, setSuccess] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const tabs: { key: Tab; label: string }[] = [
    { key: 'mood', label: 'Mood' },
    { key: 'sleep', label: 'Sleep' },
    { key: 'workout', label: 'Workout' },
    { key: 'money', label: 'Money' },
  ];

  const handleSubmit = async (endpoint: string, data: Record<string, unknown>) => {
    setError('');
    setSuccess('');
    setLoading(true);
    try {
      await trackingApi.post(endpoint, data);
      setSuccess('Event logged successfully!');
    } catch (err: any) {
      const resp = err.response?.data;
      if (resp?.errors) {
        setError(Object.values(resp.errors).flat().join('. '));
      } else {
        setError(resp?.detail || resp?.title || 'Failed to log event');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-2xl mx-auto px-6 py-10">
      <h1 className="text-2xl font-bold text-white mb-8">Log Event</h1>

      <div className="flex gap-1 mb-6">
        {tabs.map((tab) => (
          <button
            key={tab.key}
            onClick={() => { setActiveTab(tab.key); setSuccess(''); setError(''); }}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              activeTab === tab.key
                ? 'bg-white text-black'
                : 'text-zinc-400 hover:text-white hover:bg-zinc-800'
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {success && (
        <div className="rounded-lg bg-green-500/10 border border-green-500/20 px-4 py-3 text-sm text-green-400 mb-4">
          {success}
        </div>
      )}
      {error && (
        <div className="rounded-lg bg-red-500/10 border border-red-500/20 px-4 py-3 text-sm text-red-400 mb-4">
          {error}
        </div>
      )}

      <div className="rounded-xl border border-zinc-800 bg-zinc-900 p-6">
        {activeTab === 'mood' && <MoodForm onSubmit={handleSubmit} loading={loading} />}
        {activeTab === 'sleep' && <SleepForm onSubmit={handleSubmit} loading={loading} />}
        {activeTab === 'workout' && <WorkoutForm onSubmit={handleSubmit} loading={loading} />}
        {activeTab === 'money' && <MoneyForm onSubmit={handleSubmit} loading={loading} />}
      </div>
    </div>
  );
}

interface FormProps {
  onSubmit: (endpoint: string, data: Record<string, unknown>) => Promise<void>;
  loading: boolean;
}

function MoodForm({ onSubmit, loading }: FormProps) {
  const [score, setScore] = useState(5);
  const [notes, setNotes] = useState('');

  return (
    <form onSubmit={(e) => { e.preventDefault(); onSubmit('api/event/mood', { score, notes: notes || undefined }); }} className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">
          Mood Score: <span className="text-white font-bold">{score}/10</span>
        </label>
        <input type="range" min={1} max={10} value={score} onChange={(e) => setScore(Number(e.target.value))} className="w-full accent-white" />
        <div className="flex justify-between text-xs text-zinc-500 mt-1">
          <span>Terrible</span><span>Great</span>
        </div>
      </div>
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">Notes (optional)</label>
        <textarea value={notes} onChange={(e) => setNotes(e.target.value)} maxLength={500} rows={3}
          className="w-full rounded-lg border border-zinc-800 bg-zinc-950 px-3 py-2 text-sm text-white placeholder-zinc-500 focus:border-zinc-600 focus:outline-none resize-none"
          placeholder="How are you feeling?" />
      </div>
      <button type="submit" disabled={loading} className="w-full rounded-lg bg-white px-4 py-2 text-sm font-medium text-black hover:bg-zinc-200 disabled:opacity-50 transition-colors">
        {loading ? 'Logging...' : 'Log Mood'}
      </button>
    </form>
  );
}

function SleepForm({ onSubmit, loading }: FormProps) {
  const [hours, setHours] = useState(7);
  const [quality, setQuality] = useState(5);
  const [notes, setNotes] = useState('');

  return (
    <form onSubmit={(e) => { e.preventDefault(); onSubmit('api/event/sleep', { hours, quality, notes: notes || undefined }); }} className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">Hours Slept</label>
        <input type="number" min={0} max={24} step={0.5} value={hours} onChange={(e) => setHours(Number(e.target.value))}
          className="w-full rounded-lg border border-zinc-800 bg-zinc-950 px-3 py-2 text-sm text-white focus:border-zinc-600 focus:outline-none" />
      </div>
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">
          Sleep Quality: <span className="text-white font-bold">{quality}/10</span>
        </label>
        <input type="range" min={1} max={10} value={quality} onChange={(e) => setQuality(Number(e.target.value))} className="w-full accent-white" />
      </div>
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">Notes (optional)</label>
        <textarea value={notes} onChange={(e) => setNotes(e.target.value)} maxLength={500} rows={3}
          className="w-full rounded-lg border border-zinc-800 bg-zinc-950 px-3 py-2 text-sm text-white placeholder-zinc-500 focus:border-zinc-600 focus:outline-none resize-none"
          placeholder="How did you sleep?" />
      </div>
      <button type="submit" disabled={loading} className="w-full rounded-lg bg-white px-4 py-2 text-sm font-medium text-black hover:bg-zinc-200 disabled:opacity-50 transition-colors">
        {loading ? 'Logging...' : 'Log Sleep'}
      </button>
    </form>
  );
}

function WorkoutForm({ onSubmit, loading }: FormProps) {
  const [type, setType] = useState('');
  const [minutes, setMinutes] = useState(30);
  const [score, setScore] = useState(5);
  const [notes, setNotes] = useState('');

  return (
    <form onSubmit={(e) => { e.preventDefault(); onSubmit('api/event/workout', { type, minutes, score, notes: notes || undefined }); }} className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">Workout Type</label>
        <input type="text" value={type} onChange={(e) => setType(e.target.value)} required maxLength={50}
          className="w-full rounded-lg border border-zinc-800 bg-zinc-950 px-3 py-2 text-sm text-white placeholder-zinc-500 focus:border-zinc-600 focus:outline-none"
          placeholder="Running, Weights, Yoga..." />
      </div>
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">Duration (minutes)</label>
        <input type="number" min={1} max={600} value={minutes} onChange={(e) => setMinutes(Number(e.target.value))}
          className="w-full rounded-lg border border-zinc-800 bg-zinc-950 px-3 py-2 text-sm text-white focus:border-zinc-600 focus:outline-none" />
      </div>
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">
          Intensity: <span className="text-white font-bold">{score}/10</span>
        </label>
        <input type="range" min={1} max={10} value={score} onChange={(e) => setScore(Number(e.target.value))} className="w-full accent-white" />
      </div>
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">Notes (optional)</label>
        <textarea value={notes} onChange={(e) => setNotes(e.target.value)} maxLength={500} rows={3}
          className="w-full rounded-lg border border-zinc-800 bg-zinc-950 px-3 py-2 text-sm text-white placeholder-zinc-500 focus:border-zinc-600 focus:outline-none resize-none"
          placeholder="How was the workout?" />
      </div>
      <button type="submit" disabled={loading} className="w-full rounded-lg bg-white px-4 py-2 text-sm font-medium text-black hover:bg-zinc-200 disabled:opacity-50 transition-colors">
        {loading ? 'Logging...' : 'Log Workout'}
      </button>
    </form>
  );
}

function MoneyForm({ onSubmit, loading }: FormProps) {
  const [amount, setAmount] = useState<number | ''>('');
  const [txType, setTxType] = useState<'Expense' | 'Income'>('Expense');
  const [category, setCategory] = useState('');
  const [merchant, setMerchant] = useState('');
  const [notes, setNotes] = useState('');

  return (
    <form onSubmit={(e) => { e.preventDefault(); onSubmit('api/event/money', { amount: Number(amount), type: txType, category, merchant: merchant || undefined, notes: notes || undefined }); }} className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">Amount</label>
        <input type="number" min={0.01} step={0.01} value={amount} onChange={(e) => setAmount(e.target.value ? Number(e.target.value) : '')} required
          className="w-full rounded-lg border border-zinc-800 bg-zinc-950 px-3 py-2 text-sm text-white placeholder-zinc-500 focus:border-zinc-600 focus:outline-none"
          placeholder="0.00" />
      </div>
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">Type</label>
        <div className="flex gap-2">
          <button type="button" onClick={() => setTxType('Expense')}
            className={`flex-1 rounded-lg px-4 py-2 text-sm font-medium transition-colors ${
              txType === 'Expense' ? 'bg-red-500/20 text-red-400 border border-red-500/30' : 'border border-zinc-800 text-zinc-400 hover:bg-zinc-800'
            }`}>Expense</button>
          <button type="button" onClick={() => setTxType('Income')}
            className={`flex-1 rounded-lg px-4 py-2 text-sm font-medium transition-colors ${
              txType === 'Income' ? 'bg-green-500/20 text-green-400 border border-green-500/30' : 'border border-zinc-800 text-zinc-400 hover:bg-zinc-800'
            }`}>Income</button>
        </div>
      </div>
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">Category</label>
        <input type="text" value={category} onChange={(e) => setCategory(e.target.value)} required maxLength={50}
          className="w-full rounded-lg border border-zinc-800 bg-zinc-950 px-3 py-2 text-sm text-white placeholder-zinc-500 focus:border-zinc-600 focus:outline-none"
          placeholder="Food, Rent, Salary..." />
      </div>
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">Merchant (optional)</label>
        <input type="text" value={merchant} onChange={(e) => setMerchant(e.target.value)} maxLength={100}
          className="w-full rounded-lg border border-zinc-800 bg-zinc-950 px-3 py-2 text-sm text-white placeholder-zinc-500 focus:border-zinc-600 focus:outline-none"
          placeholder="Store name..." />
      </div>
      <div>
        <label className="block text-sm font-medium text-zinc-300 mb-1.5">Notes (optional)</label>
        <textarea value={notes} onChange={(e) => setNotes(e.target.value)} maxLength={500} rows={3}
          className="w-full rounded-lg border border-zinc-800 bg-zinc-950 px-3 py-2 text-sm text-white placeholder-zinc-500 focus:border-zinc-600 focus:outline-none resize-none"
          placeholder="Transaction details..." />
      </div>
      <button type="submit" disabled={loading} className="w-full rounded-lg bg-white px-4 py-2 text-sm font-medium text-black hover:bg-zinc-200 disabled:opacity-50 transition-colors">
        {loading ? 'Logging...' : 'Log Transaction'}
      </button>
    </form>
  );
}
