import http from './http'
import type { AdminOrderDto, PagedResult } from '../types/models'

export function getOrders(params: { page: number; pageSize: number; keyword?: string; }) {
  return http.get<unknown, PagedResult<AdminOrderDto>>('/orders', { params })
}

export function updateOrderStatus(id: number, status: string) {
  return http.put(`/orders/${id}/status`, { status })
}
