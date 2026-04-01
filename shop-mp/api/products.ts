import { request } from '../utils/http'
import type { PagedResult, ProductDto } from '../types/models'

export function getProducts(params: { page: number; pageSize: number; keyword?: string }) {
  return request<PagedResult<ProductDto>>({
    method: 'GET',
    path: '/products',
    query: {
      page: params.page,
      pageSize: params.pageSize,
      keyword: params.keyword,
      isOnSale: true,
    },
    auth: false,
  })
}

export function getProductById(id: number) {
  return request<ProductDto>({
    method: 'GET',
    path: `/products/${id}`,
    auth: false,
  })
}

