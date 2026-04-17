export interface ProductDto {
  id: string;
  name: string;
  description: string;
  price: number;
  colour: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProductRequest {
  name: string;
  description: string;
  price: number;
  colour: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface ApiResponse<T> {
  isSuccess: boolean;
  responseCode: number;
  responseMsg: string;
  data: T | null;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: string;
}
