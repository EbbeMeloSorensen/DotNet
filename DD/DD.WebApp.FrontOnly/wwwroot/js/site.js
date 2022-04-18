var creatureTypes = {};

function getAllCreatureTypes() {
    // Mocking for a start
    creatureTypes.items = new Array();

    creatureTypes.items = [
        { id: 0, name: "Goblin", armorClass: 7, hitPoints: 5, thaco: 17 },
        { id: 1, name: "Dragon", armorClass: -1, hitPoints: 72, thaco: 5 }
    ];

    drawCreatureTypeTable(creatureTypes);
}

function drawCreatureTypeTable(creatureTypeList) {
    $tbody = $("#table-body");
    $tbody.empty();
    for (var i = 0; i < creatureTypeList.items.length; i++) {
        $tr = $("<tr>");
        $("<td>").html(creatureTypeList.items[i].name).appendTo($tr);
        $("<td>").html(creatureTypeList.items[i].armorClass).appendTo($tr);
        $("<td>").html(creatureTypeList.items[i].hitPoints).appendTo($tr);
        $("<td>").html(creatureTypeList.items[i].thaco).appendTo($tr);
        $tbody.append($tr);
    }
}

//function dummyFunction() {
//    document.getElementById("output").innerHTML = (3.1415927).toFixed(2);

//    // Dollartegnet indikerer, at vi bruger en funktion fra jquery
//    $.ajax({
//        //url: "http://api.exchangeratesapi.io/v1/latest?access_key=3d5861a43985fbc3778eda7aaa2fefa9&symbols=USD,INR",
//        url: "https://localhost:44358/api/artists",
//        type: "GET",
//        dataType: "json",
//        success: function (result) {
//            //heroes = result. // "We set our gloal heroes list
//            //drawHeroTable(result);
//            //document.getElementById("output").innerHTML = (3.1415927).toFixed(2);
//        }
//    });
//}

// we will use jquery and ajax to make an asynchronous call to the web service. Jquery is a powerful javascript library 
// which makes many javascript functionality easier, especially when it comes to browser compatibility, You can also
// access DOM elements with jQuery faster than with plain javascript.
// Ajax is just a set of web development techniques that allows us to do the upcoming calls to our web service. Ajax
// stands for "Asyncrhonous Javascript and XML"


$(document).ready(function () {
    console.info("ready");
    getAllCreatureTypes();
});