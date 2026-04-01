import { getProductById } from '../../api/products'
import { ensureAuthed } from '../../utils/auth'
import type { ProductDto } from '../../types/models'

const DEFAULT_PRODUCT_IMG = 'https://dummyimage.com/720x480/f3f4f6/9ca3af.png&text=WebShop'

Page({
  data: {
    id: 0,
    product: null as ProductDto | null,
    quantity: 1,
    defaultImage: DEFAULT_PRODUCT_IMG,
  },
  onShow() {
    ensureAuthed()
  },
  async onLoad(options: Record<string, string | undefined>) {
    const id = Number(options.id || 0)
    this.setData({ id })
    if (!id) {
      wx.showToast({ title: '缺少商品ID', icon: 'none' })
      return
    }
    try {
      const p = await getProductById(id)
      this.setData({ product: p })
    } catch (e: any) {
      wx.showToast({ title: e?.message || '加载失败', icon: 'none' })
    }
  },
  dec() {
    const q = (this.data as any).quantity
    this.setData({ quantity: Math.max(1, q - 1) })
  },
  inc() {
    const q = (this.data as any).quantity
    const stock = ((this.data as any).product?.stock as number) || 999999
    this.setData({ quantity: Math.min(stock, q + 1) })
  },
  goCheckout() {
    const { id, quantity } = this.data as any
    wx.navigateTo({ url: `/pages/checkout/index?productId=${id}&quantity=${quantity}` })
  },
})

