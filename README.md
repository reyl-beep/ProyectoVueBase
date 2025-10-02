# Proyecto Base

Este repositorio contiene una base inicial para un proyecto compuesto por:

- **frontend**: aplicación Vue 3 configurada con TypeScript, Vite y Tailwind CSS.
- **backend**: proyecto de ASP.NET Core 8 con una Minimal API.
- **scripts**: carpeta para almacenar scripts de base de datos de Microsoft SQL Server.

## Estructura

```
.
├── backend
│   ├── BackendApi.csproj
│   └── Program.cs
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

Coloca tus scripts de base de datos en la carpeta `scripts`.
