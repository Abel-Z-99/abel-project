import { getProducts } from '../../api/products'
import { ensureAuthed } from '../../utils/auth'
import type { PagedResult, ProductDto } from '../../types/models'

const DEFAULT_PRODUCT_IMG = '/assets/images/products/nike.png'
const BANNER_COUNT = 3

function pickRandom<T>(items: T[], count: number): T[] {
  if (items.length <= count) return items
  const copied = [...items]
  for (let i = copied.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1))
    ;[copied[i], copied[j]] = [copied[j], copied[i]]
  }
  return copied.slice(0, count)
}

Page({
  data: {
    keyword: '',
    page: 1,
    pageSize: 10,
    items: [] as ProductDto[],
    total: 0,
    loading: false,
    noMore: false,
    banners: [] as Array<{ imageUrl: string; id: number }>,
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
      const bannerSource = nextItems.length > 0 ? nextItems : res.items
      const banners = pickRandom(bannerSource, BANNER_COUNT).map((p) => ({
        imageUrl: p.imageUrl || DEFAULT_PRODUCT_IMG,
        id: p.id,
      }))
      this.setData({
        items: nextItems,
        total: res.total,
        noMore,
        banners,
      })
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
  goBannerDetail(e: WechatMiniprogram.BaseEvent) {
    const id = Number((e.currentTarget.dataset as any).id || 0)
    if (!id) {
      wx.showToast({ title: '当前轮播图无商品详情', icon: 'none' })
      return
    }
    wx.navigateTo({ url: `/pages/product-detail/index?id=${id}` })
  },
})

