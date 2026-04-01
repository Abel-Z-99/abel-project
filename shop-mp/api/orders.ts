import { request } from '../utils/http'
import type { CreateOrderRequest, CreateOrderResponse, OrderDto } from '../types/models'

export function createOrder(payload: CreateOrderRequest) {
  return request<CreateOrderResponse>({
    method: 'POST',
    path: '/orders',
    data: payload,
    auth: true,
  })
}

export function getMyOrders() {
  return request<OrderDto[]>({
    method: 'GET',
    path: '/orders/mine',
    auth: true,
  })
}

