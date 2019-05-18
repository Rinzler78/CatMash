﻿var nbCats = 0;
var cats = [];

window.onload = function() {
    console.log("Loaded");
    
    $.ajax({
       url : 'http://localhost:5000/CatMash/AllCats', // Le nom du script a changé, c'est send_mail.php maintenant !
       type : 'GET', // Le type de la requête HTTP, ici devenu POST
       dataType : 'json',
       success : function(data, statut){
           console.log("Success");

           cats = data;
           nbCats = data.length;

            for (var i = 0; i < cats.length; i++) {
                var cat = cats[i];

                var ul = document.getElementById("CatsList");
                ul.appendChild(createCatListItem(cat));
            }
       },

       error : function(resultat, statut, erreur){
           console.log("Error");
       },

       complete : function(resultat, statut){
           console.log("Completed");
       }
       });
};

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