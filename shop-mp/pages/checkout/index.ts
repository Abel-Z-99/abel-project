import { createOrder } from '../../api/orders'
import { getProductById } from '../../api/products'
import { ensureAuthed } from '../../utils/auth'
import type { ProductDto } from '../../types/models'

Page({
  data: {
    productId: 0,
    quantity: 1,
    product: null as ProductDto | null,
    subtotal: 0,
    shippingAddress: '',
    contactName: '',
    contactPhone: '',
    loading: false,
  },
  onShow() {
    ensureAuthed()
  },
  async onLoad(options: Record<string, string | undefined>) {
    const productId = Number(options.productId || 0)
    const quantity = Math.max(1, Number(options.quantity || 1))
    this.setData({ productId, quantity })
    if (!productId) {
      wx.showToast({ title: '缺少商品ID', icon: 'none' })
      return
    }
    const p = await getProductById(productId)
    this.setData({ product: p, subtotal: p.price * quantity })
  },
  onInputAddress(e: WechatMiniprogram.Input) {
    this.setData({ shippingAddress: e.detail.value })
  },
  onInputName(e: WechatMiniprogram.Input) {
    this.setData({ contactName: e.detail.value })
  },
  onInputPhone(e: WechatMiniprogram.Input) {
    this.setData({ contactPhone: e.detail.value })
  },
  async submit() {
    const { productId, quantity, shippingAddress, contactName, contactPhone } = this.data as any
    if (!shippingAddress || !contactName || !contactPhone) {
      wx.showToast({ title: '请完整填写收货信息', icon: 'none' })
      return
    }
    this.setData({ loading: true })
    try {
      await createOrder({
        shippingAddress,
        contactName,
        contactPhone,
        items: [{ productId, quantity }],
      })
      wx.showToast({ title: '下单成功', icon: 'success' })
      setTimeout(() => wx.switchTab({ url: '/pages/orders/index' }), 600)
    } catch (e: any) {
      wx.showToast({ title: e?.message || '下单失败', icon: 'none' })
    } finally {
      this.setData({ loading: false })
    }
  },
})

