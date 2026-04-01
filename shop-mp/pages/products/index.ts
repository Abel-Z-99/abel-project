import { getProducts } from '../../api/products'
import { ensureAuthed } from '../../utils/auth'
import type { PagedResult, ProductDto } from '../../types/models'

const DEFAULT_PRODUCT_IMG = 'https://dummyimage.com/240x240/f3f4f6/9ca3af.png&text=WebShop'

Page({
  data: {
    keyword: '',
    page: 1,
    pageSize: 10,
    items: [] as ProductDto[],
    total: 0,
    loading: false,
    noMore: false,
    banners: [
      '/assets/images/products/xiaomi.png',
      '/assets/images/products/iPhone.png',
      '/assets/images/products/nike.png',
    ],
    defaultImage: DEFAULT_PRODUCT_IMG,
  },
  onShow() {
    ensureAuthed()
  },
  onLoad() {
    this.refresh()
  },
  onReachBottom() {
    this.loadMore()
  },
  onPullDownRefresh() {
    this.refresh().finally(() => wx.stopPullDownRefresh())
  },
  onInputKeyword(e: WechatMiniprogram.Input) {
    this.setData({ keyword: e.detail.value })
  },
  onSearch() {
    this.refresh()
  },
  async refresh() {
    this.setData({ page: 1, items: [], noMore: false })
    await this.fetchPage()
  },
  async loadMore() {
    if ((this.data as any).loading || (this.data as any).noMore) return
    this.setData({ page: (this.data as any).page + 1 })
    await this.fetchPage(true)
  },
  async fetchPage(append = false) {
    this.setData({ loading: true })
    try {
      const { page, pageSize, keyword } = this.data as any
      const res: PagedResult<ProductDto> = await getProducts({ page, pageSize, keyword })
      const nextItems = append ? [...(this.data as any).items, ...res.items] : res.items
      const noMore = nextItems.length >= res.total
      this.setData({ items: nextItems, total: res.total, noMore })
    } catch (e: any) {
      wx.showToast({ title: e?.message || '加载失败', icon: 'none' })
    } finally {
      this.setData({ loading: false })
    }
  },
  goDetail(e: WechatMiniprogram.BaseEvent) {
    const id = (e.currentTarget.dataset as any).id
    wx.navigateTo({ url: `/pages/product-detail/index?id=${id}` })
  },
})

