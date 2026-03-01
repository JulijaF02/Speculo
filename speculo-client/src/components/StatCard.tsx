interface StatItem {
  label: string;
  value: string | number;
}

interface StatCardProps {
  icon: string;
  title: string;
  stats: StatItem[];
}

export default function StatCard({ icon, title, stats }: StatCardProps) {
  return (
    <div className="rounded-xl border border-zinc-800 bg-zinc-900 p-5">
      <div className="flex items-center gap-2 mb-4">
        <span className="text-xl">{icon}</span>
        <h3 className="text-sm font-medium text-zinc-300">{title}</h3>
      </div>
      <div className="space-y-2">
        {stats.map((stat) => (
          <div key={stat.label} className="flex justify-between items-center">
            <span className="text-sm text-zinc-500">{stat.label}</span>
            <span className="text-sm font-medium text-white">{stat.value}</span>
          </div>
        ))}
      </div>
    </div>
  );
}
