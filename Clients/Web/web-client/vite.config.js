import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import basicSsl from '@vitejs/plugin-basic-ssl'
// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), basicSsl()],
  server: {
    port: 3000
  }

/*   server: {
    https: true, // ВАЖНО: именно true, а не объект
    port: 3000,
    strictPort: true,
    host: true,
    open: true // автоматически открыть браузер
  } */
})
