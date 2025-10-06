# Build do front
FROM node:20 AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

COPY .env.docker .env

RUN npm run build

# Servindo com Nginx
FROM nginx:alpine
WORKDIR /usr/share/nginx/html
RUN rm -rf ./*

COPY --from=build /app/dist .

# Ajusta porta para 3000
RUN sed -i 's/listen       80;/listen       3000;/' /etc/nginx/conf.d/default.conf

EXPOSE 3000
CMD ["nginx", "-g", "daemon off;"]
