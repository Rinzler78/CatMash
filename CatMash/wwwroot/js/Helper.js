var baseuri = window.location.protocol + "//" + window.location.hostname + ":" + window.location.port + "/CatMash";
var webSocketUri = (window.location.protocol == "http" ? "ws" : "wss") + "://" + window.location.hostname + ":" + window.location.port + "/ws";

function PlaySound(melody) {
    var path = "/sounds/";
    var snd = new Audio(path + melody + ".mp3");
    snd.play();
}

function PlayConnectedSound() {
    PlaySound('connected');
}

function PlayDisconnectedSound() {
    PlaySound('disconnected');
}

function PlayRankUpdateSound() {
    PlaySound('rateSound');
}

function PlayCatSelectedSound() {
    PlaySound('blop');
}