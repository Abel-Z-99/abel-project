import http from './http'
import type { AdminOrderDto, PagedResult } from '../types/models'

export function getOrders(params: {
  page: number
  pageSize: number
  keyword?: string
  sortBy?: string
  sortDesc?: boolean
}) {
  const { page, pageSize, keyword, sortBy, sortDesc } = params
  return http.get<unknown, PagedResult<AdminOrderDto>>('/orders', {
    params: {
      page,
      pageSize,
      ...(keyword ? { keyword } : {}),
      ...(sortBy ? { sortBy, sortDesc: sortDesc ?? false } : {}),
    },
  })
}

export function updateOrderStatus(id: number, status: string) {
  return http.put(`/orders/${id}/status`, { status })
}
