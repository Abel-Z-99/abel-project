<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'
import { createUser, getUsers, updateUserStatus } from '../api/users'
import type { AdminUserDto } from '../types/models'

const loading = ref(false)
const list = ref<AdminUserDto[]>([])
const total = ref(0)
const query = reactive({
  page: 1,
  pageSize: 10,
  keyword: '',
  sortBy: undefined as string | undefined,
  sortDesc: false,
})
const dialogVisible = ref(false)
const formRef = ref<FormInstance>()
const createForm = reactive({
  username: '',
  email: '',
  password: '',
  phone: '',
  avatarUrl: '',
})
const rules: FormRules<typeof createForm> = {
  username: [{ required: true, message: 'Username is required', trigger: 'blur' }],
  email: [{ required: true, message: 'Email is required', trigger: 'blur' }],
  password: [{ required: true, message: 'Password is required', trigger: 'blur' }],
}

const onSortChange = (payload: {
  column: { columnKey?: string }
  prop?: string
  order: string | null
}) => {
  const key = payload.column?.columnKey || payload.prop
  if (!payload.order || !key) {
    query.sortBy = undefined
    query.sortDesc = false
  } else {
    query.sortBy = key
    query.sortDesc = payload.order === 'descending'
  }
  query.page = 1
  loadData()
}

const loadData = async () => {
  loading.value = true
  try {
    const res = await getUsers(query)
    list.value = res.items
    total.value = res.total
  } finally {
    loading.value = false
  }
}

const toggleStatus = async (row: AdminUserDto) => {
  await updateUserStatus(row.id, !row.status)
  ElMessage.success('User status updated')
  await loadData()
}

const openCreate = () => {
  createForm.username = ''
  createForm.email = ''
  createForm.password = ''
  createForm.phone = ''
  createForm.avatarUrl = ''
  dialogVisible.value = true
}

const submitCreate = async () => {
  try {
    await formRef.value?.validate()
    await createUser(createForm)
    ElMessage.success('User created')
    dialogVisible.value = false
    await loadData()
  } catch (e: any) {
    const message = e?.message || ''
    if (message) ElMessage.error(message)
  }
}

onMounted(loadData)
</script>

<template>
  <el-card class="admin-page-card" shadow="hover">
    <template #header>
      <div class="admin-toolbar">
        <el-input v-model="query.keyword" placeholder="搜索用户" clearable style="max-width: 260px" @keyup.enter="loadData" />
        <div class="admin-toolbar-actions">
          <el-button @click="loadData">搜索</el-button>
          <el-button type="primary" @click="openCreate">新增用户</el-button>
        </div>
      </div>
    </template>
    <el-table :data="list" v-loading="loading" @sort-change="onSortChange">
      <el-table-column prop="id" label="ID" width="80" sortable="custom" />
      <el-table-column prop="username" label="用户名" min-width="120" sortable="custom" />
      <el-table-column prop="email" label="邮箱" min-width="180" sortable="custom" />
      <el-table-column prop="role" label="角色" width="100" sortable="custom" />
      <el-table-column prop="status" label="状态" width="100" sortable="custom">
        <template #default="{ row }">{{ row.status ? '活跃' : '禁用' }}</template>
      </el-table-column>
      <el-table-column label="操作" width="140">
        <template #default="{ row }">
          <el-button size="small" @click="toggleStatus(row)">{{ row.status ? '禁用' : '启用' }}</el-button>
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

  <el-dialog v-model="dialogVisible" title="Create User" width="520px">
    <el-form ref="formRef" :model="createForm" :rules="rules" label-width="100px">
      <el-form-item label="Username" prop="username"><el-input v-model="createForm.username" /></el-form-item>
      <el-form-item label="Email" prop="email"><el-input v-model="createForm.email" /></el-form-item>
      <el-form-item label="Password" prop="password"><el-input v-model="createForm.password" type="password" show-password /></el-form-item>
      <el-form-item label="Phone"><el-input v-model="createForm.phone" /></el-form-item>
      <el-form-item label="AvatarUrl"><el-input v-model="createForm.avatarUrl" /></el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="dialogVisible=false">Cancel</el-button>
      <el-button type="primary" @click="submitCreate">Create</el-button>
    </template>
  </el-dialog>
</template>
