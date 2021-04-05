"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
var http_1 = __importDefault(require("http"));
var express_1 = __importDefault(require("express"));
var code_1 = __importDefault(require("./code"));
var config_1 = __importDefault(require("./config"));
var router = express_1.default();
router.get('/', function (_, res) {
    res.status(200).send(code_1.default());
});
router.get('/health', function (_, res) {
    res.status(200).send('OK');
});
var httpServer = http_1.default.createServer(router);
httpServer.listen(config_1.default.server.port);
