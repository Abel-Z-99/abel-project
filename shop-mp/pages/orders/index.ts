import { getMyOrders } from '../../api/orders'
import { ensureAuthed } from '../../utils/auth'
import type { OrderDto } from '../../types/models'

Page({
  data: {
    orders: [] as OrderDto[],
    loading: false,
  },
  onShow() {
    ensureAuthed()
    this.load()
  },
  onPullDownRefresh() {
    this.load().finally(() => wx.stopPullDownRefresh())
  },
  async load() {
    this.setData({ loading: true })
    try {
      const data = await getMyOrders()
      this.setData({ orders: data })
    } catch (e: any) {
      wx.showToast({ title: e?.message || '加载失败', icon: 'none' })
    } finally {
      this.setData({ loading: false })
    }
  },
})

