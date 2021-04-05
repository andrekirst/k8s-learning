FROM node:15.13 as builder
LABEL name=code
LABEL intermediate=true
WORKDIR /root/
COPY ["package.json", "package-lock.json", "./"]
COPY [".prettierrc", "code.ts", "config.ts", "server.ts", "tsconfig.json", "./"]
RUN npm install\
    && npm install typescript -g\
    && npm run build

FROM node:alpine
LABEL name=code
WORKDIR /root/
COPY --from=builder /root/ ./
RUN ls /root/
ENTRYPOINT ["node", "/root/build/server.js"]
EXPOSE 80