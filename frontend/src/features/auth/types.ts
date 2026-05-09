export type LoginResponse = {
  accessToken: string;
  expiresAt: string;
};

export type DecodedJwt = {
  sub: string;
  email: string;
  name: string;
  exp: number;
  iss?: string;
  aud?: string;
  jti?: string;
};

export type AuthUser = {
  id: string;
  email: string;
  name: string;
};

export type RegisterRequest = {
  email: string;
  password: string;
  name: string;
};

export type LoginRequest = {
  email: string;
  password: string;
};
