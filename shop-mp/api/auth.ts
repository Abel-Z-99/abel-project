import { request } from '../utils/http'
import type { AuthResponse } from '../types/models'

export function login(payload: { userNameOrEmail: string; password: string }) {
  return request<AuthResponse>({
    method: 'POST',
    path: '/auth/login',
    data: payload,
    auth: false,
  })
}

