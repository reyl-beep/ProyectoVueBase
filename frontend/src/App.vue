<script setup lang="ts">
import { computed } from 'vue';
import { RouterLink, RouterView, useRoute, useRouter } from 'vue-router';
import { useAuth } from './composables/useAuth';

const auth = useAuth();
const route = useRoute();
const router = useRouter();

const isAuthenticated = computed(() => auth.isAuthenticated.value);
const isAdmin = computed(() => auth.isAdmin.value);
const displayName = computed(() => {
  if (!auth.state.user) return '';
  return auth.state.user.apellidos
    ? `${auth.state.user.nombre} ${auth.state.user.apellidos}`
    : auth.state.user.nombre;
});

const activeRoute = computed(() => route.name?.toString() ?? '');
const showLanding = computed(() => !isAuthenticated.value && activeRoute.value === 'home');

const logout = async () => {
  auth.logout();
  await router.push({ name: 'login' });
};
</script>

<template>
  <div class="min-h-screen bg-slate-100 text-slate-900 dark:bg-slate-950 dark:text-slate-100">
    <RouterView v-if="showLanding" />
    <template v-else>
      <header class="border-b border-slate-200 bg-white/90 shadow-sm backdrop-blur dark:border-slate-800 dark:bg-slate-900/90">
        <div class="mx-auto flex max-w-6xl items-center justify-between px-4 py-4 sm:px-6 lg:px-8">
          <RouterLink to="/" class="flex items-center gap-3">
            <span class="flex h-11 w-11 items-center justify-center rounded-full bg-gradient-to-br from-indigo-500 to-purple-600 text-lg font-semibold text-white shadow-lg">RR</span>
            <div class="flex flex-col">
              <span class="text-sm font-semibold tracking-[0.35em] text-slate-500 dark:text-slate-300">REY RECORDS</span>
              <span class="text-xl font-bold text-slate-900 dark:text-white">Panel creativo</span>
            </div>
          </RouterLink>
          <nav class="flex items-center gap-4 text-sm font-medium text-slate-600 dark:text-slate-200">
            <RouterLink
              v-if="isAuthenticated"
              to="/dashboard"
              class="rounded-full px-4 py-2 transition hover:bg-indigo-100 hover:text-indigo-600 dark:hover:bg-indigo-500/20"
              :class="{ 'bg-indigo-600 text-white shadow': activeRoute === 'dashboard' }"
            >
              Mi tablero
            </RouterLink>
            <RouterLink
              v-if="isAuthenticated && isAdmin"
              to="/admin"
              class="rounded-full px-4 py-2 transition hover:bg-purple-100 hover:text-purple-600 dark:hover:bg-purple-500/20"
              :class="{ 'bg-purple-600 text-white shadow': activeRoute === 'admin-dashboard' }"
            >
              Administración
            </RouterLink>
            <RouterLink
              v-if="!isAuthenticated"
              to="/login"
              class="rounded-full px-4 py-2 transition hover:bg-indigo-100 hover:text-indigo-600 dark:hover:bg-indigo-500/20"
              :class="{ 'bg-indigo-600 text-white shadow': activeRoute === 'login' }"
            >
              Iniciar sesión
            </RouterLink>
            <RouterLink
              v-if="!isAuthenticated"
              to="/register"
              class="rounded-full px-4 py-2 transition hover:bg-indigo-100 hover:text-indigo-600 dark:hover:bg-indigo-500/20"
              :class="{ 'bg-indigo-600 text-white shadow': activeRoute === 'register' }"
            >
              Registrarme
            </RouterLink>
            <button
              v-if="isAuthenticated"
              type="button"
              @click="logout"
              class="rounded-full px-4 py-2 font-semibold text-slate-600 transition hover:text-red-500 dark:text-slate-200 dark:hover:text-red-400"
            >
              Cerrar sesión
            </button>
          </nav>
        </div>
      </header>

      <main class="mx-auto flex max-w-6xl flex-col gap-8 px-4 py-10 sm:px-6 lg:px-8">
        <section v-if="isAuthenticated" class="rounded-2xl border border-slate-200 bg-white/90 p-6 shadow-sm dark:border-slate-800 dark:bg-slate-900/80">
          <h2 class="text-lg font-semibold text-slate-700 dark:text-slate-200">Bienvenido</h2>
          <p class="text-sm text-slate-600 dark:text-slate-300">
            {{ displayName }}
            <span v-if="auth.state.user" class="ml-1 text-xs uppercase tracking-wide text-indigo-500">{{ auth.state.user.rol }}</span>
          </p>
        </section>

        <RouterView />
      </main>
    </template>
  </div>
</template>
