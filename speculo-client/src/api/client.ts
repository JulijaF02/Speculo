import axios from 'axios';

const identityApi = axios.create({
  baseURL: import.meta.env.VITE_IDENTITY_API_URL || '/api/identity',
});

const trackingApi = axios.create({
  baseURL: import.meta.env.VITE_TRACKING_API_URL || '/api/tracking',
});

const analyticsApi = axios.create({
  baseURL: import.meta.env.VITE_ANALYTICS_API_URL || '/api/analytics',
});

const addAuthInterceptor = (instance: ReturnType<typeof axios.create>) => {
  instance.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  });

  instance.interceptors.response.use(
    (response) => response,
    (error) => {
      if (error.response?.status === 401) {
        localStorage.removeItem('token');
        window.location.href = '/login';
      }
      return Promise.reject(error);
    }
  );
};

addAuthInterceptor(trackingApi);
addAuthInterceptor(analyticsApi);

export { identityApi, trackingApi, analyticsApi };
