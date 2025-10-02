<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import { api } from '../services/api';
import { useAuth } from '../composables/useAuth';
import type { RegisterRequest, Resultado, TokenResponse } from '../types';

const router = useRouter();
const auth = useAuth();

const form = reactive<RegisterRequest>({
  nombre: '',
  apellidos: '',
  correo: '',
  password: '',
});

const confirmPassword = ref('');
const loading = ref(false);
const feedback = ref<Resultado<TokenResponse> | null>(null);

const onSubmit = async () => {
  if (form.password !== confirmPassword.value) {
    feedback.value = { value: false, message: 'Las contraseñas no coinciden.', data: null };
    return;
  }

  loading.value = true;
  feedback.value = null;
  const result = await api.register({ ...form, apellidos: form.apellidos || null });
  if (result.value && result.data) {
    auth.setSession(result.data);
    await auth.refreshUser();
    await router.push('/dashboard');
  } else {
    feedback.value = result as Resultado<TokenResponse>;
  }
  loading.value = false;
};
</script>

<template>
  <section class="mx-auto w-full max-w-md rounded-2xl border border-slate-200 bg-white/90 p-6 shadow-sm dark:border-slate-800 dark:bg-slate-900/90">
    <h1 class="text-2xl font-semibold text-slate-800 dark:text-white">Crear cuenta</h1>
    <p class="mt-1 text-sm text-slate-500 dark:text-slate-300">Regístrate para subir tus canciones y monitorear tu impacto.</p>

    <form class="mt-6 space-y-5" @submit.prevent="onSubmit">
      <div class="space-y-2">
        <label for="nombre" class="block text-sm font-medium text-slate-600 dark:text-slate-200">Nombre</label>
        <input
          id="nombre"
          v-model="form.nombre"
          type="text"
          required
          autocomplete="given-name"
          class="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500/40 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-100"
        />
      </div>

      <div class="space-y-2">
        <label for="apellidos" class="block text-sm font-medium text-slate-600 dark:text-slate-200">Apellidos</label>
        <input
          id="apellidos"
          v-model="form.apellidos"
          type="text"
          autocomplete="family-name"
          class="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500/40 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-100"
        />
      </div>

      <div class="space-y-2">
        <label for="correo" class="block text-sm font-medium text-slate-600 dark:text-slate-200">Correo electrónico</label>
        <input
          id="correo"
          v-model="form.correo"
          type="email"
          required
          autocomplete="email"
          class="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500/40 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-100"
        />
      </div>

      <div class="space-y-2">
        <label for="password" class="block text-sm font-medium text-slate-600 dark:text-slate-200">Contraseña</label>
        <input
          id="password"
          v-model="form.password"
          type="password"
          required
          autocomplete="new-password"
          class="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500/40 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-100"
        />
      </div>

      <div class="space-y-2">
        <label for="confirm-password" class="block text-sm font-medium text-slate-600 dark:text-slate-200">Confirmar contraseña</label>
        <input
          id="confirm-password"
          v-model="confirmPassword"
          type="password"
          required
          autocomplete="new-password"
          class="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500/40 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-100"
        />
      </div>

      <button
        type="submit"
        :disabled="loading"
        class="w-full rounded-lg bg-indigo-600 px-4 py-2 font-semibold text-white shadow transition hover:bg-indigo-700 disabled:cursor-not-allowed disabled:opacity-60"
      >
        {{ loading ? 'Registrando...' : 'Crear cuenta' }}
      </button>

      <p v-if="feedback && !feedback.value" class="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-600 dark:bg-red-500/15 dark:text-red-200">
        {{ feedback.message }}
      </p>
    </form>

    <p class="mt-6 text-center text-sm text-slate-500 dark:text-slate-300">
      ¿Ya tienes una cuenta?
      <RouterLink to="/login" class="font-semibold text-indigo-600 hover:underline">Inicia sesión</RouterLink>
    </p>
  </section>
</template>
