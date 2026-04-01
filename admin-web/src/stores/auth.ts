import { defineStore } from 'pinia'
import { loginApi } from '../api/auth'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || '',
    username: localStorage.getItem('username') || '',
  }),
  actions: {
    async login(userNameOrEmail: string, password: string) {
      const res = await loginApi({ userNameOrEmail, password })
      this.token = res.token
      this.username = res.username
      localStorage.setItem('token', this.token)
      localStorage.setItem('username', this.username)
    },
    logout() {
      this.token = ''
      this.username = ''
      localStorage.removeItem('token')
      localStorage.removeItem('username')
    },
  },
})
