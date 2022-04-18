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

$(document).ready(function () {
    console.info("ready");
    getAllCreatureTypes();
});