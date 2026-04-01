import axios from 'axios'
import { ElMessage } from 'element-plus'
import type { ApiResponse } from '../types/models'

const http = axios.create({
  baseURL: '/api',
  timeout: 10000,
})

http.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

http.interceptors.response.use(
  (response) => {
    const body = response.data as ApiResponse<unknown>
    if (typeof body?.success === 'boolean') {
      if (!body.success) return Promise.reject(new Error(body.message || 'Request failed'))
      return body.data
    }
    return response.data
  },
  (error) => {
    if (error?.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('username')
      ElMessage.error('Login expired, please login again.')
      window.location.href = '/login'
      return Promise.reject(new Error('Unauthorized'))
    }
    if (!error.response) {
      const msg =
        '无法连接后端：请先启动 WebApi（默认 http://localhost:5000）。若端口不同，请修改 admin-web/.env.development 中的 VITE_API_TARGET 后重启 npm run dev。'
      return Promise.reject(new Error(msg))
    }
    return Promise.reject(error)
  },
)

export default http
