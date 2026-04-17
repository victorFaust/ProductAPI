import client from './client';
import type { ProductDto, CreateProductRequest, ApiResponse } from '../types';

export async function getProducts(colour?: string): Promise<ProductDto[]> {
  const params = colour ? { colour } : {};
  const { data } = await client.get<ApiResponse<ProductDto[]>>('/api/products', { params });
  return data.data ?? [];
}

export async function createProduct(req: CreateProductRequest): Promise<ProductDto> {
  const { data } = await client.post<ApiResponse<ProductDto>>('/api/products', req);
  if (!data.isSuccess || !data.data) throw new Error(data.responseMsg || 'Failed to create product');
  return data.data;
}

export async function updateProduct(id: string, req: CreateProductRequest): Promise<ProductDto> {
  const { data } = await client.put<ApiResponse<ProductDto>>(`/api/products/${id}`, req);
  if (!data.isSuccess || !data.data) throw new Error(data.responseMsg || 'Failed to update product');
  return data.data;
}

export async function deleteProduct(id: string): Promise<void> {
  await client.delete(`/api/products/${id}`);
}

