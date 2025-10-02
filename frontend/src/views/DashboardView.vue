<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { api } from '../services/api';
import { useAuth } from '../composables/useAuth';
import type { DashboardUsuario, Resultado } from '../types';

const auth = useAuth();
const dashboard = ref<DashboardUsuario | null>(null);
const loading = ref(true);
const error = ref<Resultado | null>(null);

const load = async () => {
  loading.value = true;
  error.value = null;
  const result = await api.getDashboard();
  if (result.value && result.data) {
    dashboard.value = result.data;
    auth.state.user = result.data.usuario;
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
  <section class="rounded-2xl border border-slate-200 bg-white/90 p-6 shadow-sm dark:border-slate-800 dark:bg-slate-900/90">
    <header class="flex flex-col gap-2 border-b border-slate-200 pb-4 dark:border-slate-800">
      <h2 class="text-xl font-semibold text-slate-800 dark:text-white">Resumen de canciones</h2>
      <p class="text-sm text-slate-500 dark:text-slate-300">Monitorea tus lanzamientos, reproducciones y ganancias obtenidas.</p>
    </header>

    <div v-if="loading" class="py-10 text-center text-sm text-slate-500 dark:text-slate-300">
      Cargando tu tablero...
    </div>

    <div v-else>
      <p v-if="error" class="mt-4 rounded-lg bg-red-50 px-4 py-3 text-sm text-red-600 dark:bg-red-500/15 dark:text-red-200">
        {{ error.message }}
      </p>

      <div v-else-if="dashboard">
        <div class="grid gap-4 py-4 sm:grid-cols-3">
          <div class="rounded-xl border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-700 dark:bg-slate-950/50">
            <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Canciones publicadas</p>
            <p class="mt-2 text-3xl font-bold text-indigo-600 dark:text-indigo-400">{{ dashboard.canciones.length }}</p>
          </div>
          <div class="rounded-xl border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-700 dark:bg-slate-950/50">
            <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Reproducciones totales</p>
            <p class="mt-2 text-3xl font-bold text-purple-600 dark:text-purple-400">
              {{ dashboard.canciones.reduce((acc, song) => acc + song.totalReproducciones, 0).toLocaleString() }}
            </p>
          </div>
          <div class="rounded-xl border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-700 dark:bg-slate-950/50">
            <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Ingresos estimados</p>
            <p class="mt-2 text-3xl font-bold text-emerald-600 dark:text-emerald-400">
              $
              {{ dashboard.canciones.reduce((acc, song) => acc + Number(song.montoGanado), 0).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }}
            </p>
          </div>
        </div>

        <div class="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm dark:border-slate-700 dark:bg-slate-950/60">
          <table class="min-w-full divide-y divide-slate-200 dark:divide-slate-800">
            <thead class="bg-slate-50/80 dark:bg-slate-900/70">
              <tr class="text-left text-xs font-semibold uppercase tracking-wide text-slate-500 dark:text-slate-300">
                <th class="px-4 py-3">Canción</th>
                <th class="px-4 py-3">Reproducciones</th>
                <th class="px-4 py-3">Ganancias</th>
                <th class="px-4 py-3">Publicado</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-200 text-sm dark:divide-slate-800">
              <tr v-for="song in dashboard.canciones" :key="song.cancionId" class="transition hover:bg-slate-50 dark:hover:bg-slate-800/40">
                <td class="px-4 py-3">
                  <p class="font-medium text-slate-800 dark:text-slate-100">{{ song.titulo }}</p>
                  <p v-if="song.descripcion" class="text-xs text-slate-500 dark:text-slate-400">{{ song.descripcion }}</p>
                </td>
                <td class="px-4 py-3 text-slate-600 dark:text-slate-300">{{ song.totalReproducciones.toLocaleString() }}</td>
                <td class="px-4 py-3 text-emerald-600 dark:text-emerald-400">
                  $
                  {{ Number(song.montoGanado).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }}
                </td>
                <td class="px-4 py-3 text-slate-500 dark:text-slate-400">{{ new Date(song.fechaPublicacion).toLocaleDateString() }}</td>
              </tr>
              <tr v-if="dashboard.canciones.length === 0">
                <td colspan="4" class="px-4 py-6 text-center text-sm text-slate-500 dark:text-slate-300">
                  Aún no has registrado canciones. ¡Comparte tu primer sencillo!
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </section>
</template>
