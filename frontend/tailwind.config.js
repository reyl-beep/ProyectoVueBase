/** @type {import('tailwindcss').Config} */
export default {
  darkMode: 'class',
  content: ['./index.html', './src/**/*.{vue,ts,tsx}'],
  theme: {
    extend: {
      colors: {
        brand: {
          dark: '#050816',
          midnight: '#0b1026',
          electric: '#3b82f6',
          royal: '#6366f1',
          magenta: '#a855f7',
        },
      },
      backgroundImage: {
        'grid-light':
          'radial-gradient(circle at 1px 1px, rgba(99, 102, 241, 0.08) 1px, transparent 0)',
        'grid-dark':
          'radial-gradient(circle at 1px 1px, rgba(59, 130, 246, 0.12) 1px, transparent 0)',
      },
    },
  },
  plugins: [],
};
