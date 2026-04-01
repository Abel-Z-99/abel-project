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

export interface ProductUpsertRequest {
  name: string
  description: string
  price: number
  stock: number
  imageUrl: string
  isOnSale: boolean
}

export interface OrderItemDto {
  productId: number
  productName: string
  quantity: number
  unitPrice: number
  totalPrice: number
}

export interface AdminOrderDto {
  id: number
  orderNo: string
  status: string
  totalAmount: number
  createdAtFormatted: string
  customer: { userId: number; username: string; email: string }
  items: OrderItemDto[]
}

export interface AdminUserDto {
  id: number
  username: string
  email: string
  phone: string
  role: string
  status: boolean
  createdAt: string
}

export interface DashboardSummary {
  totalProducts: number
  productsOnSale: number
  lowStockProducts: number
  totalUsers: number
  activeUsers: number
  totalOrders: number
  todayOrders: number
  todaySales: number
  pendingOrders: number
}
