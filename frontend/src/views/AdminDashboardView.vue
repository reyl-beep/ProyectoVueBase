<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { api } from '../services/api';
import type { DashboardGlobal, Resultado } from '../types';

const loading = ref(true);
const error = ref<Resultado | null>(null);
const global = ref<DashboardGlobal | null>(null);

const load = async () => {
  loading.value = true;
  error.value = null;
  const result = await api.getAdminDashboard();
  if (result.value && result.data) {
    global.value = result.data;
  } else {
    error.value = result as Resultado;
  }
  loading.value = false;
};

onMounted(() => {
  load();
});
</script>

<template>
  <section class="rounded-2xl border border-purple-200 bg-white/95 p-6 shadow-sm dark:border-purple-800/60 dark:bg-slate-900/90">
    <header class="flex flex-col gap-2 border-b border-purple-100 pb-4 dark:border-purple-900/70">
      <h2 class="text-xl font-semibold text-purple-700 dark:text-purple-300">Panel administrativo</h2>
      <p class="text-sm text-purple-500 dark:text-purple-200">
        Visualiza el rendimiento de los artistas registrados en la plataforma.
      </p>
    </header>

    <div v-if="loading" class="py-10 text-center text-sm text-purple-500 dark:text-purple-200">Cargando información global...</div>

    <div v-else>
      <p v-if="error" class="mt-4 rounded-lg bg-red-50 px-4 py-3 text-sm text-red-600 dark:bg-red-500/15 dark:text-red-200">
        {{ error.message }}
      </p>

      <div v-else-if="global">
        <div class="grid gap-4 py-4 sm:grid-cols-3">
          <div class="rounded-xl border border-purple-200 bg-white p-4 shadow-sm dark:border-purple-700 dark:bg-slate-950/60">
            <p class="text-xs font-semibold uppercase tracking-wide text-purple-500">Artistas activos</p>
            <p class="mt-2 text-3xl font-bold text-purple-700 dark:text-purple-300">{{ global.usuarios.length }}</p>
          </div>
          <div class="rounded-xl border border-purple-200 bg-white p-4 shadow-sm dark:border-purple-700 dark:bg-slate-950/60">
            <p class="text-xs font-semibold uppercase tracking-wide text-purple-500">Total de reproducciones</p>
            <p class="mt-2 text-3xl font-bold text-indigo-600 dark:text-indigo-300">
              {{ global.usuarios.reduce((acc, user) => acc + user.canciones.reduce((inner, song) => inner + song.totalReproducciones, 0), 0).toLocaleString() }}
            </p>
          </div>
          <div class="rounded-xl border border-purple-200 bg-white p-4 shadow-sm dark:border-purple-700 dark:bg-slate-950/60">
            <p class="text-xs font-semibold uppercase tracking-wide text-purple-500">Ingresos consolidados</p>
            <p class="mt-2 text-3xl font-bold text-emerald-600 dark:text-emerald-300">
              $
              {{ global.usuarios.reduce((acc, user) => acc + user.canciones.reduce((inner, song) => inner + Number(song.montoGanado), 0), 0).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }}
            </p>
          </div>
        </div>

        <div class="space-y-6">
          <article v-for="usuario in global.usuarios" :key="usuario.usuario.usuarioId" class="rounded-xl border border-purple-200 bg-white shadow-sm dark:border-purple-700 dark:bg-slate-950/60">
            <header class="flex flex-col gap-1 border-b border-purple-100 px-6 py-4 dark:border-purple-900/60">
              <div class="flex items-center justify-between">
                <div>
                  <p class="text-lg font-semibold text-slate-800 dark:text-slate-100">{{ usuario.usuario.nombre }} {{ usuario.usuario.apellidos ?? '' }}</p>
                  <p class="text-sm text-slate-500 dark:text-slate-300">{{ usuario.usuario.correo }}</p>
                </div>
                <span class="rounded-full bg-purple-600/10 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-purple-600 dark:text-purple-300">
                  {{ usuario.canciones.length }} canciones
                </span>
              </div>
            </header>
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-purple-100 text-sm dark:divide-purple-900/50">
                <thead class="bg-purple-50/70 dark:bg-purple-900/40">
                  <tr class="text-left text-xs font-semibold uppercase tracking-wide text-purple-500 dark:text-purple-200">
                    <th class="px-6 py-3">Canción</th>
                    <th class="px-6 py-3">Reproducciones</th>
                    <th class="px-6 py-3">Ingresos</th>
                    <th class="px-6 py-3">Publicación</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-purple-100 dark:divide-purple-900/40">
                  <tr v-for="song in usuario.canciones" :key="song.cancionId" class="hover:bg-purple-50/50 dark:hover:bg-purple-900/20">
                    <td class="px-6 py-3">
                      <p class="font-medium text-slate-800 dark:text-slate-100">{{ song.titulo }}</p>
                      <p v-if="song.descripcion" class="text-xs text-slate-500 dark:text-slate-300">{{ song.descripcion }}</p>
                    </td>
                    <td class="px-6 py-3 text-slate-600 dark:text-slate-300">{{ song.totalReproducciones.toLocaleString() }}</td>
                    <td class="px-6 py-3 text-emerald-600 dark:text-emerald-300">
                      $
                      {{ Number(song.montoGanado).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }}
                    </td>
                    <td class="px-6 py-3 text-slate-500 dark:text-slate-300">{{ new Date(song.fechaPublicacion).toLocaleDateString() }}</td>
                  </tr>
                  <tr v-if="usuario.canciones.length === 0">
                    <td colspan="4" class="px-6 py-4 text-center text-sm text-slate-500 dark:text-slate-300">Sin canciones registradas.</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </article>
        </div>
      </div>
    </div>
  </section>
</template>
