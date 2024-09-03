import { defineConfig } from 'vite'
import path from "path"
import vue from '@vitejs/plugin-vue'
import { viteCommonjs, esbuildCommonjs } from '@originjs/vite-plugin-commonjs'
import vuetify, { transformAssetUrls } from 'vite-plugin-vuetify'


// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue({
    template: { transformAssetUrls }
  }),// https://github.com/vuetifyjs/vuetify-loader/tree/next/packages/vite-plugin
  vuetify({
    autoImport: true,
    
  }), viteCommonjs()],
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
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          // Split vendor libraries into a separate chunk
          vendor: ['vue', 'vue-router', 'vuex'],
          // You can also split other large dependencies or modules
        },
      },
    },
  },
  logLevel: 'warn'
});
