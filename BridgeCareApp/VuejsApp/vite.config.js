import { defineConfig } from 'vite'
import path from "path"
import vue from '@vitejs/plugin-vue'
import { viteCommonjs, esbuildCommonjs } from '@originjs/vite-plugin-commonjs'
 
// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue(), viteCommonjs()],
  server: { port: 8080 },
  optimizeDeps: {
    esbuildOptions: {
      plugins: [
        // Solves:
        // https://github.com/vitejs/vite/issues/5308
        esbuildCommonjs(['kendo-ui'])
      ],
    },
  },
  resolve: {
    extensions: ['.mjs', '.js', '.ts', '.jsx', '.tsx', '.json', '.vue'],
    alias: [
        {
        find: '@',
        replacement: path.resolve(__dirname, 'src')
        }
    ]
  },
  logLevel: 'warn'
});
