FROM nginx:alpine AS base
EXPOSE 80
EXPOSE 443

FROM node:alpine AS build
WORKDIR /src
COPY package.json .
RUN npm install
COPY . .
RUN npm run build

FROM base AS final
COPY --from=build /src/dist /usr/share/nginx/html
