import { STORAGE_KEYS } from './config'

export function getToken(): string {
  return wx.getStorageSync(STORAGE_KEYS.token) || ''
}

export function getUsername(): string {
  return wx.getStorageSync(STORAGE_KEYS.username) || ''
}

export function setAuth(payload: { token: string; username: string }) {
  wx.setStorageSync(STORAGE_KEYS.token, payload.token)
  wx.setStorageSync(STORAGE_KEYS.username, payload.username)
}

export function clearAuth() {
  wx.removeStorageSync(STORAGE_KEYS.token)
  wx.removeStorageSync(STORAGE_KEYS.username)
}

export function ensureAuthed() {
  const token = getToken()
  if (!token) {
    wx.reLaunch({ url: '/pages/login/index' })
  }
}

