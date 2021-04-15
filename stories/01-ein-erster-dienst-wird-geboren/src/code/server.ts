import http from 'http';
import express from 'express';
import random from './code';
import config from './config';

const router = express();

router.get('/', (_, res) => {
    res.status(200).send(random());
});

router.get('/health', (_, res) => {
    res.status(200).send('OK');
});

const httpServer = http.createServer(router);
httpServer.listen(config.server.port);
