FROM node:18-alpine AS builder

WORKDIR /app

COPY package*.json ./

RUN npm install

RUN npm install -g @angular/cli

COPY . .

RUN ng build



FROM nginx:alpine AS final

#copy content of dist folder into nginx html folder
COPY --from=builder app/dist/gng_learn_snapFace/browser /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY docker_entrypoint.sh /docker_entrypoint.sh

EXPOSE 80

RUN chmod +x /docker_entrypoint.sh

CMD ["/docker_entrypoint.sh"]
