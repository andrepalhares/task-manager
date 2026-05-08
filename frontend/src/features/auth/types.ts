export type LoginResponse = {
  accessToken: string;
  expiresAt: string; // ISO datetime
};

export type DecodedJwt = {
  // The backend issues these claims explicitly. Names follow the
  // standard JWT registered claim names where applicable.
  sub: string; // user id (Guid as string)
  email: string;
  name: string;
  exp: number; // unix seconds
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
