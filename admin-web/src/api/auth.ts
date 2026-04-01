import http from './http'
import type { AuthResponse } from '../types/models'

export function loginApi(payload: { userNameOrEmail: string; password: string }) {
  return http.post<unknown, AuthResponse>('/auth/login', payload)
}
