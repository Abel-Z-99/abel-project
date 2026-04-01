import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const apiTarget = env.VITE_API_TARGET || 'http://localhost:5000'

  return {
    plugins: [vue()],
    build: {
      chunkSizeWarningLimit: 1200,
      rollupOptions: {
        output: {
          manualChunks(id) {
            if (id.includes('node_modules/element-plus')) return 'element-plus'
            if (id.includes('node_modules/vue')) return 'vue-core'
            if (id.includes('node_modules/axios')) return 'axios'
          },
        },
      },
    },
    server: {
      port: 5173,
      proxy: {
        '/api': {
          target: apiTarget,
          changeOrigin: true,
          configure(proxy) {
            proxy.on('error', (err) => {
              console.warn(
                `[vite proxy] 无法转发到 ${apiTarget}（${String((err as NodeJS.ErrnoException)?.code || '')}）。请先启动 WebApi，或检查 VITE_API_TARGET。`,
              )
            })
          },
        },
      },
    },
  }
})
