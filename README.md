# mon-projet-dico

Mono-repo simple contenant :
- backend .NET 9 (API minimal) — dossier `backend`
- frontend React + Vite — dossier `frontend`

Prérequis
- .NET 9 SDK
- Node.js (18+) et npm

Lancer le backend
1. Ouvrir un terminal
2. cd C:\Users\A114SR\mon-projet-dico\backend
3. dotnet run

Endpoint principal
- GET /weatherforecast (défini dans `backend/Program.cs`)

Lancer le frontend
1. Ouvrir un terminal
2. cd C:\Users\A114SR\mon-projet-dico\frontend
3. npm install
4. npm run dev

Build
- Backend : dotnet build backend
- Frontend : cd frontend && npm run build

Notes
- Le frontend contient aussi un README dans `frontend/README.md`.