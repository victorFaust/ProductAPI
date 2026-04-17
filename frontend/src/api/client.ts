import axios from 'axios';

let authToken: string | null = null;

export function setAuthToken(token: string | null) {
  authToken = token;
}

const client = axios.create({
  baseURL: '',
});

client.interceptors.request.use((config) => {
  if (authToken) {
    config.headers.Authorization = `Bearer ${authToken}`;
  }
  return config;
});

export default client;
