export interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
}

export interface PagedResult<T> {
  page: number
  pageSize: number
  total: number
  items: T[]
}

export interface AuthResponse {
  token: string
  expiresAt: string
  userId: number
  username: string
  email: string
  roles: string[]
}

export interface ProductDto {
  id: number
  name: string
  description: string
  price: number
  stock: number
  imageUrl: string
  isOnSale: boolean
  createdAt: string
}

export interface CreateOrderItemRequest {
  productId: number
  quantity: number
}

export interface CreateOrderRequest {
  shippingAddress: string
  contactName: string
  contactPhone: string
  items: CreateOrderItemRequest[]
}

export interface CreateOrderResponse {
  id: number
  orderNo: string
  totalAmount: number
  status: string
}

export interface OrderItemDto {
  productId: number
  productName: string
  quantity: number
  unitPrice: number
  totalPrice: number
}

export interface OrderDto {
  id: number
  orderNo: string
  status: string
  totalAmount: number
  createdAtFormatted: string
  items: OrderItemDto[]
}

