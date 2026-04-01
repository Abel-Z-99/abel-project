import http from './http'
import type { PagedResult, ProductDto, ProductUpsertRequest } from '../types/models'

export function getProducts(params: { page: number; pageSize: number; keyword?: string }) {
  return http.get<unknown, PagedResult<ProductDto>>('/products', { params })
}

export function createProduct(payload: ProductUpsertRequest) {
  return http.post('/products', payload)
}

export function updateProduct(id: number, payload: ProductUpsertRequest) {
  return http.put(`/products/${id}`, payload)
}

export function deleteProduct(id: number) {
  return http.delete(`/products/${id}`)
}

export function uploadProductImage(file: File) {
  const formData = new FormData()
  formData.append('file', file)
  return http.post<unknown, { url: string }>('/uploads/image', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
}
