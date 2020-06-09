var leftCatIndex = 0;
var rightCatIndex = 0;
var nbCats = 0;
var cats = [];

window.onload = function() {
    console.log("Loaded");
    
    $.ajax({
       url : baseuri + "/AllCats", 
       type : 'GET', 
       dataType : 'json',
       success : function(data, statut){
           console.log("Success");
           leftIndex = new Number();

           cats = data;
           nbCats = data.length;

           updateCatsImages();
       },

       error : function(resultat, statut, erreur){
           console.log("Error");
       },

       complete : function(resultat, statut){
           console.log("Completed");
       }
       });

    setImagesEventListener("leftCatImg");
    setImagesEventListener("rightCatImg");
};

function updateCatsImages()
{
    if(cats.length > 0)
    {
        var index = 0;

        if(cats.length > 1)
        {
            leftCatIndex = generateIndex(leftCatIndex);
            rightCatIndex = generateIndex(leftCatIndex);
        }

        var leftCat = cats[leftCatIndex];
        var rightCat = cats[rightCatIndex];

        //Left Cat
        setCatImage("leftCatImg", leftCat.url, leftCat.id);

        //Right Cat
        setCatImage("rightCatImg", rightCat.url, rightCat.id);
    }
}

function setCatImage(imageId, src, alt) {
    var img = document.getElementById(imageId);
    img.src = src;
    img.alt = alt;
}

function setImagesEventListener(imageId){
    document.getElementById(imageId).addEventListener('click', function (e) {
        onImageClicked(e.target.id);
    });
}

function onImageClicked(id)
{
    var winnerCat = null;
    var opponentCat = null;

    if(id == "leftCatImg")
    {
        winnerCat = cats[leftCatIndex];
        opponentCat = cats[rightCatIndex];
    }
    else if(id == "rightCatImg")
    {
        winnerCat = cats[rightCatIndex];
        opponentCat = cats[leftCatIndex];
    }

    PlayCatSelectedSound();

    ++winnerCat.rate;
    ++winnerCat.nbMash;
    ++opponentCat.nbMash;
   
    $.ajax({
        url : baseuri + "/Rate?winnerId="+winnerCat.id+"&opponentId="+opponentCat.id,
       type : 'POST',
       dataType : 'json',
       success : function(data, statut){
           console.log("Success");
       },

       error : function(resultat, statut, erreur){
           console.log("Error");
       },

       complete : function(resultat, statut){
           console.log("Completed");
       }
    });

    updateCatsImages();
}

function generateIndex(currentIndex) {
    var index = 0;

    do 
    {
        index = Math.floor(Math.random() * cats.length);
    }
    while((index == currentIndex));

    return index;
}