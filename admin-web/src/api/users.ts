import http from './http'
import type { AdminUserDto, PagedResult } from '../types/models'

export function getUsers(params: { page: number; pageSize: number; keyword?: string }) {
  return http.get<unknown, PagedResult<AdminUserDto>>('/admin/users', { params })
}

export function updateUserStatus(id: number, status: boolean) {
  return http.put(`/admin/users/${id}/status`, { status })
}

export function createUser(payload: {
  username: string
  email: string
  password: string
  phone?: string
  avatarUrl?: string
}) {
  return http.post<unknown, AdminUserDto>('/admin/users', payload)
}
