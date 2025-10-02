# Proyecto Base

Este repositorio contiene una base inicial para un proyecto compuesto por:

- **frontend**: aplicación Vue 3 configurada con TypeScript, Vite y Tailwind CSS.
- **backend**: proyecto de ASP.NET Core 8 con una Minimal API lista para usarse desde Visual Studio 2022.
- **scripts**: carpeta para almacenar scripts de base de datos de Microsoft SQL Server.

## Estructura

```
.
├── ProyectoVueBase.sln
├── backend
│   ├── BackendApi.csproj
│   ├── Program.cs
│   └── Properties
│       └── launchSettings.json
├── frontend
│   ├── index.html
│   ├── package.json
│   ├── src
│   │   ├── App.vue
│   │   ├── assets
│   │   │   └── main.css
│   │   └── main.ts
│   ├── tailwind.config.js
│   └── vite.config.ts
├── scripts
│   └── README.md
└── README.md
```

## Requisitos

- Node.js 18+
- .NET 8 SDK
- Visual Studio 2022 (opcional, para abrir la solución incluida)

## Comandos útiles

### Frontend

```bash
cd frontend
npm install
npm run dev
```

### Backend

```bash
cd backend
dotnet restore
dotnet run
```

Cuando la API esté en ejecución, la interfaz de Swagger UI estará disponible automáticamente en `http://localhost:5180/swagger` (o el puerto mostrado en consola). La ruta raíz de la aplicación redirige a esta página para facilitar el descubrimiento de endpoints.

Coloca tus scripts de base de datos en la carpeta `scripts`.
