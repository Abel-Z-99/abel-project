import http from './http'
import type { AdminUserDto, PagedResult } from '../types/models'

export function getUsers(params: {
  page: number
  pageSize: number
  keyword?: string
  sortBy?: string
  sortDesc?: boolean
}) {
  const { page, pageSize, keyword, sortBy, sortDesc } = params
  return http.get<unknown, PagedResult<AdminUserDto>>('/admin/users', {
    params: {
      page,
      pageSize,
      ...(keyword ? { keyword } : {}),
      ...(sortBy ? { sortBy, sortDesc: sortDesc ?? false } : {}),
    },
  })
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
