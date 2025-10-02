<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';

const THEME_KEY = 'reyrecords-theme';
const isDarkMode = ref(true);

const experiences = [
  {
    title: 'Producción Integral',
    description:
      'Desde la composición hasta la masterización, ofrecemos un flujo creativo completo con ingenieros certificados y productores experimentados.',
    icon: '🎛️',
  },
  {
    title: 'Identidad Sonora',
    description:
      'Creamos paisajes sonoros únicos para artistas, marcas y experiencias inmersivas, impulsados por sintetizadores analógicos y software de vanguardia.',
    icon: '🎶',
  },
  {
    title: 'Lanzamiento Estratégico',
    description:
      'Planificamos campañas de lanzamiento en plataformas digitales, con métricas claras y alianzas con curadores y playlists influyentes.',
    icon: '🚀',
  },
];

const featuredArtists = [
  {
    name: 'Luna García',
    genre: 'Synthwave / Pop Futurista',
    description: 'Top 5 en playlists editoriales LATAM y 2 giras internacionales acompañadas por ReyRecords.',
  },
  {
    name: 'Kai Moreno',
    genre: 'R&B Experimental',
    description: 'Ganador del premio “Voz Emergente” 2023 con más de 25 millones de streams globales.',
  },
  {
    name: 'Noir Ensemble',
    genre: 'Soundtracks Cinemáticos',
    description: 'Compositores para cine independiente y videojuegos, con proyectos premiados en festivales europeos.',
  },
];

const themeLabel = computed(() => (isDarkMode.value ? 'Modo oscuro' : 'Modo claro'));

const toggleTheme = () => {
  isDarkMode.value = !isDarkMode.value;
};

const applyTheme = (dark: boolean) => {
  const root = document.documentElement;
  root.classList.toggle('dark', dark);
  localStorage.setItem(THEME_KEY, dark ? 'dark' : 'light');
};

onMounted(() => {
  const stored = localStorage.getItem(THEME_KEY);
  if (stored === 'light') {
    isDarkMode.value = false;
  } else {
    isDarkMode.value = true;
  }
  applyTheme(isDarkMode.value);
});

watch(isDarkMode, (value) => applyTheme(value));
</script>

<template>
  <div
    class="min-h-screen bg-gradient-to-br from-slate-50 via-white to-slate-100 text-slate-900 transition-colors duration-500 dark:from-black dark:via-brand-dark dark:to-brand-midnight dark:text-slate-100">
    <div class="relative mx-auto flex min-h-screen max-w-7xl flex-col px-4 pb-16 pt-6 sm:px-6 lg:px-12">
      <div
        class="absolute inset-0 -z-10 bg-grid-light bg-[length:24px_24px] opacity-40 transition dark:bg-grid-dark dark:opacity-70"
        aria-hidden="true"></div>
      <div
        class="absolute inset-0 -z-20 bg-gradient-to-br from-brand-electric/10 via-white/60 to-brand-magenta/10 transition dark:from-brand-midnight/40 dark:via-black/60 dark:to-brand-magenta/30">
      </div>

      <header class="flex flex-wrap items-center justify-between gap-4 py-6">
        <div class="flex items-center gap-3">
          <div class="flex h-12 w-12 items-center justify-center rounded-full bg-brand-electric/80 shadow-lg">
            <span class="text-2xl font-semibold text-white">RR</span>
          </div>
          <div>
            <p class="text-xs uppercase tracking-[0.4em] text-slate-500 dark:text-slate-300">ReyRecords</p>
            <h1 class="text-xl font-semibold text-slate-900 dark:text-white sm:text-2xl">
              Laboratorio Creativo Musical
            </h1>
          </div>
        </div>

        <nav class="flex flex-wrap items-center gap-4 text-sm font-medium text-slate-700 dark:text-slate-200">
          <a class="transition hover:text-brand-royal dark:hover:text-white" href="#servicios">Servicios</a>
          <a class="transition hover:text-brand-royal dark:hover:text-white" href="#talento">Talento</a>
          <a class="transition hover:text-brand-royal dark:hover:text-white" href="#contacto">Contacto</a>
        </nav>

        <div class="flex flex-wrap items-center gap-3 text-sm">
          <button type="button" @click="toggleTheme"
            class="inline-flex items-center gap-2 rounded-full border border-slate-900/10 bg-slate-900/5 px-4 py-2 font-medium text-slate-900 shadow-sm backdrop-blur transition hover:bg-slate-900/10 dark:border-white/20 dark:bg-white/10 dark:text-white dark:hover:bg-white/20">
            <span aria-hidden="true" class="text-lg">{{ isDarkMode ? '🌙' : '☀️' }}</span>
            <span class="hidden sm:inline">{{ themeLabel }}</span>
          </button>
          <a href="#login"
            class="rounded-full border border-slate-900/10 px-4 py-2 font-semibold text-slate-800 transition hover:border-brand-royal hover:text-brand-royal dark:border-white/20 dark:text-slate-100 dark:hover:border-white dark:hover:text-white">Iniciar
            sesión</a>
          <a href="#registro"
            class="rounded-full bg-gradient-to-r from-brand-electric to-brand-magenta px-4 py-2 font-semibold text-white shadow-lg transition hover:from-brand-royal hover:to-brand-magenta/80">Crear
            cuenta</a>
        </div>
      </header>

      <main class="flex flex-1 flex-col gap-16">
        <section class="grid gap-10 lg:grid-cols-2 lg:items-center">
          <div class="space-y-8">
            <div
              class="inline-flex items-center gap-2 rounded-full border border-slate-900/10 bg-white/80 px-4 py-1 text-xs font-semibold uppercase tracking-[0.3em] text-slate-700 backdrop-blur dark:border-white/20 dark:bg-white/10 dark:text-slate-200">
              <span class="text-brand-magenta">Nuevo</span>
              <span>Estudios inmersivos 2024</span>
            </div>
            <h2 class="text-4xl font-bold leading-tight text-slate-900 dark:text-white sm:text-5xl">
              Eleva tu sonido con experiencias sonoras que conquistan escenarios globales.
            </h2>
            <p class="max-w-xl text-lg text-slate-700 dark:text-slate-200">
              En ReyRecords diseñamos música que conecta emocionalmente con audiencias globales. Nuestro equipo de
              productores, compositores y estrategas digitales transforma ideas en catálogos memorables.
            </p>
            <div class="flex flex-wrap items-center gap-4">
              <a href="#descubrir"
                class="inline-flex items-center gap-3 rounded-full bg-gradient-to-r from-brand-electric via-brand-royal to-brand-magenta px-6 py-3 text-base font-semibold text-white shadow-xl transition hover:shadow-brand-electric/40">
                Explorar servicios
                <span aria-hidden="true">→</span>
              </a>
              <a href="#recuperar"
                class="inline-flex items-center gap-2 rounded-full border border-slate-900/10 px-6 py-3 text-base font-medium text-slate-700 transition hover:border-brand-royal hover:text-brand-royal dark:border-white/20 dark:text-slate-200 dark:hover:border-white dark:hover:text-white">
                Recuperar contraseña
              </a>
            </div>
            <dl class="grid max-w-xl gap-4 sm:grid-cols-3">
              <div
                class="rounded-2xl border border-slate-200 bg-white/80 p-4 text-center shadow-sm dark:border-white/10 dark:bg-black/30">
                <dt class="text-sm text-slate-500 dark:text-slate-300">Años creando</dt>
                <dd class="text-3xl font-bold text-slate-900 dark:text-white">15+</dd>
              </div>
              <div
                class="rounded-2xl border border-slate-200 bg-white/80 p-4 text-center shadow-sm dark:border-white/10 dark:bg-black/30">
                <dt class="text-sm text-slate-500 dark:text-slate-300">Producciones</dt>
                <dd class="text-3xl font-bold text-slate-900 dark:text-white">180</dd>
              </div>
              <div
                class="rounded-2xl border border-slate-200 bg-white/80 p-4 text-center shadow-sm dark:border-white/10 dark:bg-black/30">
                <dt class="text-sm text-slate-500 dark:text-slate-300">Streams</dt>
                <dd class="text-3xl font-bold text-slate-900 dark:text-white">500M</dd>
              </div>
            </dl>
          </div>
          <div class="relative">
            <div class="absolute -top-10 -left-8 h-24 w-24 rounded-full bg-brand-electric/40 blur-3xl"></div>
            <div class="absolute -bottom-10 -right-6 h-32 w-32 rounded-full bg-brand-magenta/40 blur-3xl"></div>
            <div
              class="rounded-3xl border border-slate-200 bg-white/70 p-6 shadow-2xl backdrop-blur dark:border-white/10 dark:bg-gradient-to-br dark:from-white/10 dark:via-white/5 dark:to-black/40">
              <div class="space-y-6">
                <div class="rounded-2xl bg-white/80 p-6 shadow-sm dark:bg-black/40">
                  <p class="text-sm uppercase tracking-[0.3em] text-brand-magenta">Estudio A</p>
                  <p class="mt-2 text-3xl font-semibold text-slate-900 dark:text-white">Sala Neón</p>
                  <p class="mt-4 text-sm text-slate-600 dark:text-slate-300">
                    Consola SSL Fusion, colección boutique de sintetizadores analógicos y cabinas inmersivas para
                    sesiones Dolby Atmos.
                  </p>
                </div>
                <div class="grid gap-4 md:grid-cols-2">
                  <div
                    class="rounded-2xl border border-slate-200 bg-white/80 p-4 shadow-sm dark:border-white/10 dark:bg-black/30">
                    <p class="text-xs uppercase text-slate-500 dark:text-slate-400">Equipo destacado</p>
                    <p class="mt-3 text-base font-semibold text-slate-900 dark:text-white">Moog One · Prophet-6 ·
                      Ableton Suite</p>
                  </div>
                  <div
                    class="rounded-2xl border border-slate-200 bg-white/80 p-4 shadow-sm dark:border-white/10 dark:bg-black/30">
                    <p class="text-xs uppercase text-slate-500 dark:text-slate-400">Asesoría creativa</p>
                    <p class="mt-3 text-base font-semibold text-slate-900 dark:text-white">Mentorías de composición y
                      branding sonoro</p>
                  </div>
                </div>
                <div
                  class="rounded-2xl border border-slate-200 bg-white/80 p-4 shadow-sm dark:border-white/10 dark:bg-black/30">
                  <p class="text-xs uppercase text-slate-500 dark:text-slate-400">Proyectos recientes</p>
                  <ul class="mt-3 space-y-2 text-sm text-slate-700 dark:text-slate-200">
                    <li>• Banda sonora interactiva para experiencia VR “Aurora Boreal”.</li>
                    <li>• Producción y lanzamiento de EP “Nébula” con Kai Moreno.</li>
                    <li>• Diseño sonoro para marca tecnológica SkyPulse.</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </section>

        <section id="servicios" class="space-y-10">
          <div class="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
            <div>
              <p class="text-sm uppercase tracking-[0.4em] text-brand-magenta">Servicios</p>
              <h3 class="mt-2 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl">
                Experiencias integrales para artistas visionarios
              </h3>
            </div>
            <p class="max-w-xl text-base text-slate-700 dark:text-slate-200">
              Personalizamos cada proyecto para potenciar la identidad artística, optimizar recursos y garantizar
              entregables listos para plataformas globales.
            </p>
          </div>
          <div class="grid gap-6 md:grid-cols-3">
            <article v-for="experience in experiences" :key="experience.title"
              class="group relative overflow-hidden rounded-3xl border border-slate-200 bg-white/80 p-6 shadow-lg transition hover:-translate-y-1 hover:shadow-brand-royal/40 dark:border-white/10 dark:bg-black/40">
              <div
                class="absolute inset-0 bg-gradient-to-br from-brand-electric/10 via-transparent to-brand-magenta/20 opacity-0 transition group-hover:opacity-100">
              </div>
              <div class="relative space-y-4">
                <span
                  class="inline-flex h-12 w-12 items-center justify-center rounded-full bg-slate-900/5 text-2xl text-slate-800 dark:bg-white/10 dark:text-white">{{
                  experience.icon }}</span>
                <h4 class="text-xl font-semibold text-slate-900 dark:text-white">{{ experience.title }}</h4>
                <p class="text-sm text-slate-700 dark:text-slate-200">{{ experience.description }}</p>
              </div>
            </article>
          </div>
        </section>

        <section id="talento" class="space-y-10">
          <div class="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
            <div>
              <p class="text-sm uppercase tracking-[0.4em] text-brand-magenta">Talento ReyRecords</p>
              <h3 class="mt-2 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl">
                Artistas que redefinen el sonido latino
              </h3>
            </div>
            <p class="max-w-xl text-base text-slate-700 dark:text-slate-200">
              Colaboramos con artistas emergentes y consolidados para construir propuestas genuinas con impacto
              internacional.
            </p>
          </div>
          <div class="grid gap-6 md:grid-cols-3">
            <article v-for="artist in featuredArtists" :key="artist.name"
              class="rounded-3xl border border-slate-200 bg-white/80 p-6 shadow-lg backdrop-blur dark:border-white/10 dark:bg-black/30">
              <p class="text-sm uppercase tracking-[0.3em] text-brand-magenta">{{ artist.genre }}</p>
              <h4 class="mt-3 text-2xl font-semibold text-slate-900 dark:text-white">{{ artist.name }}</h4>
              <p class="mt-3 text-sm text-slate-700 dark:text-slate-200">{{ artist.description }}</p>
            </article>
          </div>
        </section>

        <section id="descubrir"
          class="grid gap-8 rounded-3xl border border-slate-200 bg-white/85 p-8 shadow-2xl transition dark:border-white/10 dark:bg-black/30 lg:grid-cols-[2fr,1fr]">
          <div class="space-y-6">
            <h3 class="text-3xl font-bold text-slate-900 dark:text-white">Conecta con nuestra célula creativa</h3>
            <p class="text-base text-slate-700 dark:text-slate-200">
              Reservá una sesión estratégica con nuestro equipo para mapear la narrativa sonora de tu próximo
              lanzamiento, analizar métricas clave y construir una hoja de ruta personalizada.
            </p>
            <ul class="grid gap-4 text-sm text-slate-700 dark:text-slate-200 md:grid-cols-2">
              <li class="flex items-start gap-3">
                <span class="mt-1 text-brand-magenta">✔</span>
                Auditoría de catálogos y presencia digital.
              </li>
              <li class="flex items-start gap-3">
                <span class="mt-1 text-brand-magenta">✔</span>
                Sesiones de co-writing y vocal coaching.
              </li>
              <li class="flex items-start gap-3">
                <span class="mt-1 text-brand-magenta">✔</span>
                Diseño de experiencias inmersivas y live shows.
              </li>
              <li class="flex items-start gap-3">
                <span class="mt-1 text-brand-magenta">✔</span>
                Acompañamiento legal y publishing.
              </li>
            </ul>
            <a href="#contacto"
              class="inline-flex items-center gap-3 rounded-full border border-slate-900/10 px-6 py-3 text-base font-semibold text-slate-800 transition hover:border-brand-royal hover:text-brand-royal dark:border-white/20 dark:text-white dark:hover:border-white">
              Agenda una reunión
              <span aria-hidden="true">↗</span>
            </a>
          </div>
          <div
            class="flex flex-col justify-between rounded-2xl bg-gradient-to-br from-brand-electric/20 via-brand-royal/20 to-brand-magenta/20 p-6 text-slate-900 shadow-lg dark:text-slate-100">
            <div>
              <p class="text-sm uppercase tracking-[0.3em] text-slate-600 dark:text-slate-200">Accesos</p>
              <h4 class="mt-4 text-2xl font-semibold text-slate-900 dark:text-white">Portal ReyRecords</h4>
              <p class="mt-3 text-sm text-slate-700 dark:text-slate-200">
                Ingresa a tu panel para gestionar sesiones, descargar stems y seguir métricas de tus lanzamientos.
              </p>
            </div>
            <div class="mt-6 space-y-3 text-sm">
              <a id="login" href="#"
                class="block rounded-full border border-slate-900/20 px-4 py-3 text-center font-semibold text-slate-900 transition hover:border-brand-royal hover:text-brand-royal dark:border-white/30 dark:text-white dark:hover:border-white">Iniciar
                sesión</a>
              <a id="registro" href="#"
                class="block rounded-full bg-gradient-to-r from-brand-electric to-brand-magenta px-4 py-3 text-center font-semibold text-white shadow-lg transition hover:from-brand-royal hover:to-brand-magenta/80">Registrarse</a>
              <a id="recuperar" href="#"
                class="block rounded-full border border-transparent px-4 py-3 text-center font-medium text-slate-700 underline-offset-4 transition hover:text-brand-royal hover:underline dark:text-slate-200 dark:hover:text-white">Recuperar
                contraseña</a>
            </div>
          </div>
        </section>
      </main>

      <footer id="contacto"
        class="mt-20 grid gap-8 rounded-3xl border border-slate-200 bg-white/80 p-8 text-sm text-slate-600 shadow-lg transition dark:border-white/10 dark:bg-black/40 dark:text-slate-300 sm:grid-cols-2">
        <div class="space-y-3">
          <h4 class="text-2xl font-semibold text-slate-900 dark:text-white">¿Listo para producir algo inolvidable?</h4>
          <p>
            Escríbenos a <a class="font-medium text-brand-royal hover:underline dark:text-white"
              href="mailto:hola@reyrecords.studio">hola@reyrecords.studio</a>
            o agenda una llamada con nuestro equipo creativo.
          </p>
          <p class="text-xs uppercase tracking-[0.3em] text-slate-500 dark:text-slate-400">Ciudad de México · Bogotá ·
            Madrid</p>
        </div>
        <div class="space-y-4">
          <p class="font-semibold text-slate-900 dark:text-white">Suscríbete a nuestra newsletter</p>
          <form class="flex flex-col gap-3 sm:flex-row">
            <label class="sr-only" for="email">Correo electrónico</label>
            <input id="email" type="email" placeholder="correo@ejemplo.com"
              class="w-full rounded-full border border-slate-300 bg-white px-4 py-3 text-slate-900 placeholder:text-slate-400 focus:border-brand-royal focus:outline-none focus:ring-2 focus:ring-brand-magenta/60 dark:border-white/20 dark:bg-white/10 dark:text-white dark:placeholder:text-slate-300" />
            <button type="submit"
              class="rounded-full bg-gradient-to-r from-brand-electric to-brand-magenta px-6 py-3 font-semibold text-white shadow-lg transition hover:from-brand-royal hover:to-brand-magenta/80">
              Unirme
            </button>
          </form>
          <p class="text-xs text-slate-500 dark:text-slate-400">© {{ new Date().getFullYear() }} ReyRecords. Todos los
            derechos reservados.</p>
        </div>
      </footer>
    </div>
  </div>
</template>

<style scoped>
:global(html) {
  scroll-behavior: smooth;
}

:global(body) {
  @apply antialiased;
}
</style>
