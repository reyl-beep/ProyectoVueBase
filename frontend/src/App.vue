<script setup lang="ts">
const message = 'Bienvenido a tu proyecto Vue 3 con TypeScript y Tailwind CSS';
</script>

<template>
  <main class="min-h-screen flex items-center justify-center">
    <section class="max-w-xl rounded-2xl bg-white p-10 shadow-lg">
      <h1 class="text-3xl font-bold text-sky-600">{{ message }}</h1>
      <p class="mt-4 text-slate-600">
        Este es un punto de partida ligero utilizando Vite. Empieza a desarrollar en
        <code class="rounded bg-slate-100 px-2 py-1 text-sm text-slate-700">src/App.vue</code>.
      </p>
    </section>
  </main>
</template>

<style scoped>
code {
  font-family: ui-monospace, SFMono-Regular, SFMono-Regular, Menlo, Monaco, Consolas, 'Liberation Mono',
    'Courier New', monospace;
}
</style>
