# --- build stage -------------------------------------------------------------
FROM node:20-alpine AS build
WORKDIR /app

# Copia somente o manifest para otimizar cache. O lock é regenerado dentro
# do container pois pode conter deps opcionais específicas da plataforma do
# desenvolvedor (ex.: chokidar/fsevents em macOS) que não existem no Linux.
COPY package.json package-lock.json* ./
RUN npm install --no-audit --no-fund --loglevel=error

COPY . .
RUN npm run build -- --configuration production

# --- runtime stage -----------------------------------------------------------
FROM nginx:1.27-alpine AS runtime

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

EXPOSE 80
HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
    CMD wget -qO- http://localhost/ >/dev/null || exit 1
