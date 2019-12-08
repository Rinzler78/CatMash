var cats = [];

window.onload = function() {
    console.log("Loaded");
    
    initWebSocketLink();
};

function updateCats()
{
    $.ajax({
       url : baseuri + "/AllCats", // Le nom du script a changé, c'est send_mail.php maintenant !
       type : 'GET', // Le type de la requête HTTP, ici devenu POST
       dataType : 'json',
       success : function(data, statut){
           console.log("Success");

           cats = data;

           populateCats();
       },

       error : function(resultat, statut, erreur){
           console.log("Error");
       },

       complete : function(resultat, statut){
           console.log("Completed");
       }
    });
}

function populateCats()
{
    var ul = document.getElementById("CatsList");

    cats = cats.sort(function(cat1, cat2){
        return cat2.rate - cat1.rate;
    });
    
    while (ul.firstChild) {
        ul.removeChild(ul.firstChild);
    }

    for (var i = 0; i < cats.length; i++) {
        var cat = cats[i];
        
        ul.appendChild(createCatListItem(cat));
    }
}

function createCatListItem(cat)
{
    var li = document.createElement("li");
    li.setAttribute('id', "li+" + cat.id);
    
    li.textContent = "Rate (" + cat.rate + ") Number of Mash (" + cat.nbMash + ")";

    var img = document.createElement("img");
    img.src = cat.url;
    img.height = 100;
    img.width = img.height;

    li.appendChild(img);

    return li;
}

//Web Sockets
var catMashWebSocket = null;
function initWebSocketLink()
{
    if(catMashWebSocket != null && catMashWebSocket.readyState == 0)
        return;

    catMashWebSocket = new WebSocket(webSocketUri);

    catMashWebSocket.onopen = function (event) {
        console.log("Ws opened");
        PlayConnectedSound();
        updateCats();
    };

    catMashWebSocket.onclose = function (event) {
        console.log("Ws closed");
        PlayDisconnectedSound();
        initWebSocketLink();
    };

    catMashWebSocket.onerror = function (event) {
        console.log("Ws error");
        initWebSocketLink();
    };

    catMashWebSocket.onmessage = function (event) {
        console.log("Ws onmessage " + event.data);

        var array = event.data.split(":");
        var winnerId = array[0];
        var opponentId = array[1];

        var winnerCat = cats.find(function(a){
            return a.id == winnerId;
        });

        var opponentCat = cats.find(function(a){
            return a.id == opponentId;
        });

        ++winnerCat.rate;

        ++winnerCat.nbMash;
        ++opponentCat.nbMash;

        populateCats();

        PlayRankUpdateSound();
    };
}