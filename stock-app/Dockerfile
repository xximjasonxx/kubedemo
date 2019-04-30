FROM node:10.15-jessie-slim as build
WORKDIR /code

COPY package.json package.json
RUN npm install

COPY . .
RUN npm run-script build

FROM node:10.15-jessie-slim as final
WORKDIR /app

COPY --from=build /code/build ./
RUN npm install -g serve

EXPOSE 80
ENTRYPOINT [ "serve", "-l", "80" ]