<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { getDashboardSummary } from '../api/dashboard'
import type { DashboardSummary } from '../types/models'

const summary = ref<DashboardSummary | null>(null)
const loading = ref(false)

type StatTone = 'blue' | 'cyan' | 'amber' | 'violet' | 'orange' | 'indigo' | 'slate' | 'emerald' | 'rose'

type StatItem = {
  label: string
  display: string
  tone: StatTone
}

const formatInt = (n: number) => n.toLocaleString('zh-CN')

const formatMoney = (n: number) =>
  n.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })

const stats = computed<StatItem[]>(() => {
  const s = summary.value
  if (!s) return []
  return [
    { label: '产品总数', display: formatInt(s.totalProducts), tone: 'blue' },
    { label: '在售产品', display: formatInt(s.productsOnSale), tone: 'cyan' },
    { label: '低库存产品', display: formatInt(s.lowStockProducts), tone: 'amber' },
    { label: '订单总数', display: formatInt(s.totalOrders), tone: 'violet' },
    { label: '未完成订单', display: formatInt(s.pendingOrders), tone: 'orange' },
    { label: '当日订单', display: formatInt(s.todayOrders), tone: 'indigo' },
    { label: '用户总数', display: formatInt(s.totalUsers), tone: 'slate' },
    { label: '活跃用户', display: formatInt(s.activeUsers), tone: 'emerald' },
    { label: '当日销售额', display: `¥ ${formatMoney(s.todaySales)}`, tone: 'rose' },
  ]
})

const loadData = async () => {
  loading.value = true
  summary.value = await getDashboardSummary()
  loading.value = false
}

onMounted(loadData)
</script>

<template>
  <el-card v-loading="loading" class="admin-page-card dashboard-card" shadow="hover">
    <template #header>
      <div class="dashboard-head">
        <div>
          <div class="dashboard-head-title">仪表盘摘要</div>
          <div class="dashboard-head-sub">核心业务指标一览</div>
        </div>
      </div>
    </template>

    <div v-if="stats.length" class="dash-grid">
      <div
        v-for="item in stats"
        :key="item.label"
        class="dash-tile"
        :class="`dash-tile--${item.tone}`"
      >
        <div class="dash-tile-label">{{ item.label }}</div>
        <div class="dash-tile-value">{{ item.display }}</div>
      </div>
    </div>
  </el-card>
</template>

<style scoped>
.dashboard-card :deep(.el-card__header) {
  padding: 22px 26px;
}

.dashboard-card :deep(.el-card__body) {
  padding: 26px 26px 30px;
}

.dashboard-head-title {
  font-size: 18px;
  font-weight: 700;
  color: #0f172a;
  letter-spacing: -0.02em;
}

.dashboard-head-sub {
  margin-top: 6px;
  font-size: 14px;
  color: #64748b;
}

.dash-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 20px 22px;
}

@media (min-width: 1400px) {
  .dash-grid {
    grid-template-columns: repeat(3, minmax(0, 1fr));
    gap: 24px 28px;
  }
}

@media (max-width: 900px) {
  .dash-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 520px) {
  .dash-grid {
    grid-template-columns: 1fr;
  }
}

.dash-tile {
  min-height: 132px;
  padding: 22px 22px 20px;
  border-radius: 14px;
  border: 1px solid #e2e8f0;
  background: #fff;
  box-shadow: 0 1px 2px rgba(15, 23, 42, 0.04);
  transition:
    box-shadow 0.2s ease,
    transform 0.2s ease;
}

.dash-tile:hover {
  box-shadow: 0 8px 24px rgba(15, 23, 42, 0.08);
  transform: translateY(-2px);
}

.dash-tile-label {
  font-size: 14px;
  font-weight: 500;
  color: #64748b;
  line-height: 1.35;
}

.dash-tile-value {
  margin-top: 14px;
  font-size: clamp(1.75rem, 2.4vw, 2.25rem);
  font-weight: 700;
  color: #0f172a;
  line-height: 1.15;
  font-variant-numeric: tabular-nums;
  letter-spacing: -0.03em;
}

.dash-tile--blue {
  background: linear-gradient(145deg, #eff6ff 0%, #ffffff 55%);
  border-color: #bfdbfe;
}

.dash-tile--cyan {
  background: linear-gradient(145deg, #ecfeff 0%, #ffffff 55%);
  border-color: #a5f3fc;
}

.dash-tile--amber {
  background: linear-gradient(145deg, #fffbeb 0%, #ffffff 55%);
  border-color: #fcd34d;
}

.dash-tile--violet {
  background: linear-gradient(145deg, #f5f3ff 0%, #ffffff 55%);
  border-color: #c4b5fd;
}

.dash-tile--orange {
  background: linear-gradient(145deg, #fff7ed 0%, #ffffff 55%);
  border-color: #fdba74;
}

.dash-tile--indigo {
  background: linear-gradient(145deg, #eef2ff 0%, #ffffff 55%);
  border-color: #a5b4fc;
}

.dash-tile--slate {
  background: linear-gradient(145deg, #f8fafc 0%, #ffffff 55%);
  border-color: #cbd5e1;
}

.dash-tile--emerald {
  background: linear-gradient(145deg, #ecfdf5 0%, #ffffff 55%);
  border-color: #6ee7b7;
}

.dash-tile--rose {
  background: linear-gradient(145deg, #fff1f2 0%, #ffffff 55%);
  border-color: #fda4af;
}
</style>
