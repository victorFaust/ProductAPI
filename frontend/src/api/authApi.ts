import client from './client';
import type { LoginRequest, LoginResponse, ApiResponse } from '../types';

export async function login(req: LoginRequest): Promise<LoginResponse> {
  const { data } = await client.post<ApiResponse<LoginResponse>>('/api/auth/login', req);
  if (!data.isSuccess || !data.data) throw new Error(data.responseMsg || 'Login failed');
  return data.data;
}

export async function refresh(refreshToken: string): Promise<LoginResponse> {
  const { data } = await client.post<ApiResponse<LoginResponse>>('/api/auth/refresh', { refreshToken });
  if (!data.isSuccess || !data.data) throw new Error(data.responseMsg || 'Refresh failed');
  return data.data;
}

export async function revoke(refreshToken: string): Promise<void> {
  await client.post('/api/auth/revoke', { refreshToken });
}

