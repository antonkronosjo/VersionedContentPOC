import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
    plugins: [react()],
    server: {
        proxy: {
            '/api': {
                target: 'https://localhost:7211', // your backend URL
                changeOrigin: true,
                secure: false, // needed if using self-signed HTTPS
                rewrite: (path) => path.replace(/^\/api/, '/api'), // optional
            },
        }
    }
})
