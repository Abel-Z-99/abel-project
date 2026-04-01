import { API_BASE_URL } from './config'
import { clearAuth, getToken } from './auth'
import type { ApiResponse } from '../types/models'

function buildUrl(path: string, query?: Record<string, any>) {
  const base = path.startsWith('http') ? path : `${API_BASE_URL}${path.startsWith('/') ? '' : '/'}${path}`
  if (!query || Object.keys(query).length === 0) return base
  const qs = Object.entries(query)
    .filter(([, v]) => v !== undefined && v !== null && v !== '')
    .map(([k, v]) => `${encodeURIComponent(k)}=${encodeURIComponent(String(v))}`)
    .join('&')
  return qs ? `${base}${base.includes('?') ? '&' : '?'}${qs}` : base
}

export function request<T>(options: {
  method: WechatMiniprogram.RequestOption['method']
  path: string
  data?: any
  query?: Record<string, any>
  timeoutMs?: number
  auth?: boolean
}): Promise<T> {
  const url = buildUrl(options.path, options.query)
  const token = getToken()

  return new Promise<T>((resolve, reject) => {
    wx.request({
      url,
      method: options.method,
      data: options.data,
      timeout: options.timeoutMs ?? 10000,
      header: {
        'Content-Type': 'application/json',
        ...(options.auth === false ? {} : token ? { Authorization: `Bearer ${token}` } : {}),
      },
      success(res) {
        if (res.statusCode === 401) {
          clearAuth()
          wx.showToast({ title: '登录已过期，请重新登录', icon: 'none' })
          wx.reLaunch({ url: '/pages/login/index' })
          reject(new Error('Unauthorized'))
          return
        }

        const body = res.data as ApiResponse<T> | any
        if (body && typeof body.success === 'boolean') {
          if (!body.success) {
            reject(new Error(body.message || 'Request failed'))
            return
          }
          resolve(body.data as T)
          return
        }

        resolve(res.data as T)
      },
      fail(err) {
        reject(new Error((err as any)?.errMsg || 'Network error'))
      },
    })
  })
}

