import { createRouter, createWebHistory } from 'vue-router'

declare module 'vue-router' {
  interface RouteMeta {
    /** 主内容区取消 max-width（登录页外的后台布局统一使用） */
    wideMain?: boolean
  }
}

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/login', name: 'login', component: () => import('../views/LoginView.vue') },
    {
      path: '/',
      component: () => import('../layouts/MainLayout.vue'),
      meta: { wideMain: true },
      redirect: '/dashboard',
      children: [
        {
          path: 'dashboard',
          name: 'dashboard',
          component: () => import('../views/DashboardView.vue'),
        },
        { path: 'products', name: 'products', component: () => import('../views/ProductsView.vue') },
        { path: 'orders', name: 'orders', component: () => import('../views/OrdersView.vue') },
        { path: 'users', name: 'users', component: () => import('../views/UsersView.vue') },
      ],
    },
  ],
})

router.beforeEach((to) => {
  const token = localStorage.getItem('token')
  if (to.path !== '/login' && !token) return '/login'
  if (to.path === '/login' && token) return '/dashboard'
  return true
})

export default router
