import client from './client';
import type { LoginRequest, LoginResponse, ApiResponse } from '../types';

export async function login(req: LoginRequest): Promise<LoginResponse> {
  const { data } = await client.post<ApiResponse<LoginResponse>>('/api/auth/login', req);
  if (!data.isSuccess || !data.data) throw new Error(data.responseMsg || 'Login failed');
  return data.data;
}
