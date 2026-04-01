<script setup lang="ts">
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const route = useRoute()
const auth = useAuthStore()

const go = (index: string) => router.push(index)
const logout = () => {
  auth.logout()
  router.push('/login')
}
</script>

<template>
  <el-container class="admin-layout">
    <el-aside width="232px" class="admin-aside">
      <div class="admin-brand">
        <span class="admin-brand-mark">W</span>
        <div class="admin-brand-text">
          <div class="admin-brand-title">WebShop</div>
          <div class="admin-brand-sub">管理后台</div>
        </div>
      </div>
      <el-menu
        :default-active="route.path"
        class="admin-menu"
        background-color="#0f172a"
        text-color="#94a3b8"
        active-text-color="#ffffff"
        @select="go"
      >
        <el-menu-item index="/dashboard">
          <span>仪表盘</span>
        </el-menu-item>
        <el-menu-item index="/products">
          <span>产品管理</span>
        </el-menu-item>
        <el-menu-item index="/orders">
          <span>订单管理</span>
        </el-menu-item>
        <el-menu-item index="/users">
          <span>用户管理</span>
        </el-menu-item>
      </el-menu>
    </el-aside>
    <el-container class="admin-right">
      <el-header class="admin-header" height="var(--admin-header-h)">
        <div class="admin-header-title">商城管理后台</div>
        <div class="admin-header-user">
          <span class="admin-username">{{ auth.username }}</span>
          <el-button type="primary" plain size="small" @click="logout">登出</el-button>
        </div>
      </el-header>
      <el-main class="admin-main" :class="{ 'admin-main--wide': route.meta.wideMain }">
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<style scoped>
.admin-layout {
  min-height: 100vh;
}

.admin-aside {
  background: var(--admin-sidebar);
  border-right: 1px solid rgba(255, 255, 255, 0.06);
  display: flex;
  flex-direction: column;
}

.admin-brand {
  display: flex;
  align-items: center;
  gap: 12px;
  height: var(--admin-header-h);
  padding: 0 20px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.admin-brand-mark {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  background: linear-gradient(135deg, #3b82f6, #2563eb);
  color: #fff;
  font-weight: 800;
  font-size: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.admin-brand-title {
  color: #f8fafc;
  font-weight: 700;
  font-size: 15px;
  line-height: 1.2;
}

.admin-brand-sub {
  color: #64748b;
  font-size: 12px;
  margin-top: 2px;
}

.admin-menu {
  border-right: none;
  flex: 1;
  padding-top: 8px;
}

.admin-menu :deep(.el-menu-item) {
  margin: 4px 10px;
  border-radius: 8px;
  height: 44px;
}

.admin-menu :deep(.el-menu-item:hover) {
  background-color: var(--admin-sidebar-hover) !important;
}

.admin-menu :deep(.el-menu-item.is-active) {
  background: linear-gradient(90deg, rgba(37, 99, 235, 0.35), rgba(37, 99, 235, 0.08)) !important;
  color: #fff !important;
}

.admin-right {
  background: var(--admin-bg);
  min-height: 100vh;
}

.admin-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 24px;
  background: #fff;
  border-bottom: 1px solid #e2e8f0;
  box-shadow: 0 1px 2px rgba(15, 23, 42, 0.04);
}

.admin-header-title {
  font-size: 16px;
  font-weight: 600;
  color: #0f172a;
}

.admin-header-user {
  display: flex;
  align-items: center;
  gap: 12px;
}

.admin-username {
  color: #64748b;
  font-size: 13px;
}

.admin-main {
  padding: 20px 24px 32px;
  max-width: 1400px;
  width: 100%;
  margin: 0 auto;
}

.admin-main--wide {
  max-width: none;
  padding-left: 28px;
  padding-right: 28px;
}
</style>
