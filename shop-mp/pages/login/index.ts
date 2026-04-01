import { login } from '../../api/auth'
import { setAuth } from '../../utils/auth'

Page({
  data: {
    userNameOrEmail: 'demo',
    password: 'Demo123!',
    loading: false,
  },
  onInputUser(e: WechatMiniprogram.Input) {
    this.setData({ userNameOrEmail: e.detail.value })
  },
  onInputPwd(e: WechatMiniprogram.Input) {
    this.setData({ password: e.detail.value })
  },
  async onSubmit() {
    const { userNameOrEmail, password } = this.data as any
    if (!userNameOrEmail || !password) {
      wx.showToast({ title: '请输入账号和密码', icon: 'none' })
      return
    }
    this.setData({ loading: true })
    try {
      const res = await login({ userNameOrEmail, password })
      setAuth({ token: res.token, username: res.username })
      wx.reLaunch({ url: '/pages/products/index' })
    } catch (e: any) {
      wx.showToast({ title: e?.message || '登录失败', icon: 'none' })
    } finally {
      this.setData({ loading: false })
    }
  },
})

