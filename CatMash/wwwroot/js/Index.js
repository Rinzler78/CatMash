window.onload = function() {
    console.log("Loaded");

    $.ajax({
       url : 'http://localhost:5000/Home/AllCats', // Le nom du script a changé, c'est send_mail.php maintenant !
       type : 'GET', // Le type de la requête HTTP, ici devenu POST
       dataType : 'json',
       success : function(data, statut){
           console.log("Success");
           console.log(data);

           var cats = data;

           var leftCat = cats[0];
           var rightCat = cats[1];

           //Left Cat
           setCatImage("leftCatImg", leftCat.url, leftCat.id);

           //Right Cat
           setCatImage("rightCatImg", rightCat.url, rightCat.id);
       },

       error : function(resultat, statut, erreur){
           console.log("Error");
       },

       complete : function(resultat, statut){
           console.log("Completed");
       }
    });
};

function setCatImage(imageId, src, alt){
    document.getElementById(imageId).src = src;
    document.getElementById(imageId).alt = alt;
}
