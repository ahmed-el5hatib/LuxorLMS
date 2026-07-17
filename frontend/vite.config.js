import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api/v1/auth': {
        target: 'http://localhost:5001',
        changeOrigin: true,
      },
      '/api/v1/academic': {
        target: 'http://localhost:5002',
        changeOrigin: true,
      },
      '/api/v1/registration': {
        target: 'http://localhost:5003',
        changeOrigin: true,
      },
      '/api/v1/forums': {
        target: 'http://localhost:5004',
        changeOrigin: true,
      },
      '/api/v1/notifications': {
        target: 'http://localhost:5005',
        changeOrigin: true,
      },
      '/api/v1/storage': {
        target: 'http://localhost:5006',
        changeOrigin: true,
      },
      '/api/v1/attendance': {
        target: 'http://localhost:5007',
        changeOrigin: true,
      },
      '/api/v1/grading': {
        target: 'http://localhost:5008',
        changeOrigin: true,
      },
      '/api/v1/quizzes': {
        target: 'http://localhost:5009',
        changeOrigin: true,
      },
      '/api/v1/analytics': {
        target: 'http://localhost:5010',
        changeOrigin: true,
      },
      '/api/v1/reporting': {
        target: 'http://localhost:5011',
        changeOrigin: true,
      },
      '/api/v1/admin': {
        target: 'http://localhost:5012',
        changeOrigin: true,
      },
    },
  },
})
