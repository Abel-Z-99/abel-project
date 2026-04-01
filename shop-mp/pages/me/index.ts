import { clearAuth, ensureAuthed, getUsername } from '../../utils/auth'

Page({
  data: {
    username: '',
  },
  onShow() {
    ensureAuthed()
    this.setData({ username: getUsername() || '' })
  },
  logout() {
    clearAuth()
    wx.reLaunch({ url: '/pages/login/index' })
  },
})

