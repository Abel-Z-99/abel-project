<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { getOrders, updateOrderStatus } from '../api/orders'
import type { AdminOrderDto } from '../types/models'

const loading = ref(false)
const list = ref<AdminOrderDto[]>([])
const total = ref(0)
const query = reactive({ page: 1, pageSize: 10 , keyword: '' })
const detailVisible = ref(false)
const currentOrder = ref<AdminOrderDto | null>(null)

const loadData = async () => {
  loading.value = true
  try {
    const res = await getOrders(query)
    list.value = res.items
    total.value = res.total
  } finally {
    loading.value = false
  }
}

const onChangeStatus = async (id: number, status: string) => {
  await updateOrderStatus(id, status)
  ElMessage.success('Status updated')
  await loadData()
}

const showDetail = (row: AdminOrderDto) => {
  currentOrder.value = row
  detailVisible.value = true
}

onMounted(loadData)
</script>

<template>
  <el-card class="admin-page-card" shadow="hover">
    <template #header>
      <div class="admin-toolbar">
        <el-input v-model="query.keyword" placeholder="搜索订单" clearable style="max-width: 260px" @keyup.enter="loadData" />
        <div class="admin-toolbar-actions">
          <el-button @click="loadData">搜索</el-button>
        </div>
      </div>
    </template>
    <el-table :data="list" v-loading="loading">
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="orderNo" label="订单编号" width="220">
        <template #default="{ row }">
          <el-button link type="primary" @click="showDetail(row)">
            {{ row.orderNo }}
          </el-button>
        </template>
      </el-table-column>
      <el-table-column prop="customer.username" label="用户" />
      <el-table-column prop="totalAmount" label="订单金额" width="120" />
      <el-table-column prop="status" label="状态" width="120" />
      <el-table-column label="操作" width="380">
        <template #default="{ row }">
          
          <el-button size="small" @click="onChangeStatus(row.id,'Paid')">已支付</el-button>
          <el-button size="small" @click="onChangeStatus(row.id,'Shipped')">已发货</el-button>
          <el-button size="small" @click="onChangeStatus(row.id,'Completed')">已完成</el-button>
          <el-button size="small" type="danger" @click="onChangeStatus(row.id,'Cancelled')">已取消</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
      background
      layout="total, prev, pager, next"
      :current-page="query.page"
      :page-size="query.pageSize"
      :total="total"
      @current-change="(p:number)=>{query.page=p;loadData()}"
    />
  </el-card>

  <el-dialog v-model="detailVisible" title="订单详情" width="700px">
    <template v-if="currentOrder">
      <p><b>订单编号:</b> {{ currentOrder.orderNo }}</p>
      <p><b>用户:</b> {{ currentOrder.customer.username }} ({{ currentOrder.customer.email }})</p>
      <p><b>状态:</b> {{ currentOrder.status }}</p>
      <p><b>总金额:</b> {{ currentOrder.totalAmount }}</p>
      <el-table :data="currentOrder.items" style="margin-top: 12px">
        <el-table-column prop="productId" label="Id" width="100" />
        <el-table-column prop="productName" label="产品名称" />
        <el-table-column prop="quantity" label="购买数量" width="80" />
        <el-table-column prop="unitPrice" label="单价" width="120" />
        <el-table-column prop="totalPrice" label="总价" width="120" />
      </el-table>
    </template>
  </el-dialog>
</template>
