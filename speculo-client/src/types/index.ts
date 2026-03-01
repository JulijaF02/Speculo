export interface AuthResponse {
  token: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  fullName: string;
}

export interface UserProfile {
  id: string;
  email: string;
  name: string;
}

export interface MoodStats {
  totalEntries: number;
  averageScore: number;
  latestScore: number;
}

export interface SleepStats {
  totalEntries: number;
  averageHours: number;
  averageQuality: number;
}

export interface MoneyStats {
  totalIncome: number;
  totalExpenses: number;
  totalTransactions: number;
}

export interface WorkoutStats {
  totalWorkouts: number;
  totalMinutes: number;
  averageScore: number;
}

export interface DashboardProjection {
  mood: MoodStats;
  sleep: SleepStats;
  money: MoneyStats;
  workouts: WorkoutStats;
}
