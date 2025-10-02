import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '../views/LoginView.vue';
import RegisterView from '../views/RegisterView.vue';
import DashboardView from '../views/DashboardView.vue';
import AdminDashboardView from '../views/AdminDashboardView.vue';
import { useAuth } from '../composables/useAuth';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: () => {
        const { isAuthenticated } = useAuth();
        return isAuthenticated.value ? '/dashboard' : '/login';
      },
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { requiresGuest: true },
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView,
      meta: { requiresGuest: true },
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: DashboardView,
      meta: { requiresAuth: true },
    },
    {
      path: '/admin',
      name: 'admin-dashboard',
      component: AdminDashboardView,
      meta: { requiresAdmin: true },
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/',
    },
  ],
});

router.beforeEach(async (to, from, next) => {
  const auth = useAuth();

  if (!auth.initialized.value) {
    await auth.initializeFromStorage();
  }

  if (to.meta.requiresAuth && !auth.isAuthenticated.value) {
    return next({ name: 'login', query: { redirect: to.fullPath } });
  }

  if (to.meta.requiresGuest && auth.isAuthenticated.value) {
    return next({ name: 'dashboard' });
  }

  if (to.meta.requiresAdmin && !auth.isAdmin.value) {
    return next({ name: 'dashboard' });
  }

  return next();
});

export default router;
