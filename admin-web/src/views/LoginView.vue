<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const auth = useAuthStore()
const loading = ref(false)
const form = reactive({
  userNameOrEmail: 'admin',
  password: 'Admin123!',
})

const onSubmit = async () => {
  try {
    loading.value = true
    await auth.login(form.userNameOrEmail, form.password)
    ElMessage.success('Login success')
    router.push('/dashboard')
  } catch (e: any) {
    ElMessage.error(e?.message || 'Login failed')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="login-shell">
    <el-card class="login-card" shadow="never">
      <template #header>用户登录</template>
      <el-form @submit.prevent="onSubmit">
        <el-form-item label="用户名或邮箱地址">
          <el-input v-model="form.userNameOrEmail" />
        </el-form-item>
        <el-form-item label="密码">
          <el-input v-model="form.password" show-password type="password" />
        </el-form-item>
        <el-button type="primary" :loading="loading" @click="onSubmit" style="width:100%">
          登录
        </el-button>
      </el-form>
    </el-card>
  </div>
</template>
