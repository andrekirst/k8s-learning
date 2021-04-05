"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function random() {
    var chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var str = '';
    for (var i = 0; i < 10; i++) {
        str += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return str;
}
exports.default = random;
