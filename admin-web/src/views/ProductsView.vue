<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'
import { createProduct, deleteProduct, getProducts, updateProduct, uploadProductImage } from '../api/products'
import type { ProductDto, ProductUpsertRequest } from '../types/models'

const loading = ref(false)
const list = ref<ProductDto[]>([])
const total = ref(0)
const query = reactive({
  page: 1,
  pageSize: 10,
  keyword: '',
  sortBy: undefined as string | undefined,
  sortDesc: false,
})
const dialogVisible = ref(false)
const editingId = ref<number | null>(null)
const imageUploading = ref(false)
const formRef = ref<FormInstance>()
const form = reactive<ProductUpsertRequest>({
  name: '',
  description: '',
  price: 0,
  stock: 0,
  imageUrl: '',
  isOnSale: true,
})
const rules: FormRules<ProductUpsertRequest> = {
  name: [{ required: true, message: 'Product name is required', trigger: 'blur' }],
  description: [{ required: true, message: 'Description is required', trigger: 'blur' }],
  price: [{ required: true, message: 'Price is required', trigger: 'change' }],
  stock: [{ required: true, message: 'Stock is required', trigger: 'change' }],
}

const readAsDataUrl = (file: File) =>
  new Promise<string>((resolve, reject) => {
    const reader = new FileReader()
    reader.onload = () => resolve(String(reader.result || ''))
    reader.onerror = () => reject(new Error('Failed to read image file'))
    reader.readAsDataURL(file)
  })

const loadImage = (src: string) =>
  new Promise<HTMLImageElement>((resolve, reject) => {
    const img = new Image()
    img.onload = () => resolve(img)
    img.onerror = () => reject(new Error('Failed to load image'))
    img.src = src
  })

const compressImageIfNeeded = async (file: File) => {
  // Skip compression for small files.
  if (file.size <= 700 * 1024) return file

  const dataUrl = await readAsDataUrl(file)
  const image = await loadImage(dataUrl)
  const canvas = document.createElement('canvas')
  const ctx = canvas.getContext('2d')
  if (!ctx) return file

  const maxEdge = 1600
  const ratio = Math.min(1, maxEdge / Math.max(image.width, image.height))
  canvas.width = Math.max(1, Math.round(image.width * ratio))
  canvas.height = Math.max(1, Math.round(image.height * ratio))
  ctx.drawImage(image, 0, 0, canvas.width, canvas.height)

  const outputType = file.type === 'image/png' ? 'image/png' : 'image/jpeg'
  const outputQuality = outputType === 'image/png' ? 0.92 : 0.82

  const blob = await new Promise<Blob | null>((resolve) =>
    canvas.toBlob(resolve, outputType, outputQuality),
  )
  if (!blob || blob.size >= file.size) return file

  const ext = outputType === 'image/png' ? '.png' : '.jpg'
  const baseName = file.name.replace(/\.[^/.]+$/, '')
  return new File([blob], `${baseName}-compressed${ext}`, { type: outputType })
}

const resetForm = () => {
  editingId.value = null
  form.name = ''
  form.description = ''
  form.price = 0
  form.stock = 0
  form.imageUrl = ''
  form.isOnSale = true
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
    const res = await getProducts(query)
    list.value = res.items
    total.value = res.total
  } finally {
    loading.value = false
  }
}

const openCreate = () => {
  resetForm()
  dialogVisible.value = true
}

const openEdit = (row: ProductDto) => {
  editingId.value = row.id
  form.name = row.name
  form.description = row.description
  form.price = row.price
  form.stock = row.stock
  form.imageUrl = row.imageUrl
  form.isOnSale = row.isOnSale
  dialogVisible.value = true
}

const submit = async () => {
  try {
    await formRef.value?.validate()
    if (editingId.value) await updateProduct(editingId.value, form)
    else await createProduct(form)
    ElMessage.success('Saved')
    dialogVisible.value = false
    await loadData()
  } catch (e: any) {
    const message = e?.message || ''
    if (message) ElMessage.error(message)
  }
}

const onSelectImage = async (e: Event) => {
  const input = e.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return

  try {
    imageUploading.value = true
    const compressed = await compressImageIfNeeded(file)
    const result = await uploadProductImage(compressed)
    form.imageUrl = result.url
    const savedKb = Math.max(0, Math.round((file.size - compressed.size) / 1024))
    ElMessage.success(savedKb > 0 ? `Image uploaded (saved ${savedKb}KB)` : 'Image uploaded')
  } catch (err: any) {
    ElMessage.error(err?.message || 'Image upload failed')
  } finally {
    imageUploading.value = false
    input.value = ''
  }
}

const onDelete = async (id: number) => {
  await deleteProduct(id)
  ElMessage.success('Deleted')
  await loadData()
}

onMounted(loadData)
</script>

<template>
  <el-card class="admin-page-card" shadow="hover">
    <template #header>
      <div class="admin-toolbar">
        <el-input v-model="query.keyword" placeholder="搜索产品" clearable style="max-width: 260px" @keyup.enter="loadData" />
        <div class="admin-toolbar-actions">
          <el-button @click="loadData">搜索</el-button>
          <el-button type="primary" @click="openCreate">新增产品</el-button>
        </div>
      </div>
    </template>
    <el-table :data="list" v-loading="loading" @sort-change="onSortChange">
      <el-table-column prop="id" label="ID" width="80" sortable="custom" />
      <el-table-column prop="name" label="产品名称" sortable="custom" min-width="160" />
      <el-table-column prop="price" label="单价" width="120" sortable="custom" />
      <el-table-column prop="stock" label="库存" width="100" sortable="custom" />
      <el-table-column prop="isOnSale" column-key="isOnSale" label="是否在售" width="120" sortable="custom">
        <template #default="{ row }">{{ row.isOnSale ? '是' : '否' }}</template>
      </el-table-column>
      <el-table-column label="操作" width="180">
        <template #default="{ row }">
          <el-button size="small" @click="openEdit(row)">修改</el-button>
          <el-button size="small" type="danger" @click="onDelete(row.id)">删除</el-button>
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

  <el-dialog v-model="dialogVisible" :title="editingId ? '修改产品' : '新增产品'" width="520px" destroy-on-close>
    <el-form ref="formRef" :model="form" :rules="rules" label-width="100px">
      <el-form-item label="名称" prop="name"><el-input v-model="form.name" /></el-form-item>
      <el-form-item label="描述" prop="description"><el-input v-model="form.description" /></el-form-item>
      <el-form-item label="单价" prop="price"><el-input-number v-model="form.price" :min="0" /></el-form-item>
      <el-form-item label="库存" prop="stock"><el-input-number v-model="form.stock" :min="0" /></el-form-item>
      <el-form-item label="图片">
        <div style="display:flex;gap:12px;align-items:center;flex-wrap:wrap;">
          <input type="file" accept="image/*" @change="onSelectImage" />
          <span>{{ imageUploading ? 'Uploading...' : '选择本地图片上传' }}</span>
          <el-input v-model="form.imageUrl" placeholder="上传的 URL 将填在这里" style="width:100%;" />
          <el-image
            v-if="form.imageUrl"
            :src="form.imageUrl"
            style="width:96px;height:96px;border-radius:8px;"
            fit="cover"
            :preview-src-list="[form.imageUrl]"
          />
        </div>
      </el-form-item>
      <el-form-item label="是否在售"><el-switch v-model="form.isOnSale" /></el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="dialogVisible=false">取消</el-button>
      <el-button type="primary" @click="submit">保存</el-button>
    </template>
  </el-dialog>
</template>
