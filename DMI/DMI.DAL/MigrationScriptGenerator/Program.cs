using MigrationScriptGenerator;

/*
//var fileWithPositions = @"..\..\..\data\position_tmp.SQL"; // (Windows)
var fileWithPositions = @"data/position_tmp.SQL"; // (Linux)
var positionRowsFromPayload = Application.LoadPositionsFromPayload(fileWithPositions);

var positionRowsFromTargetDatabase = Application.LoadPositionsFromDatabase();

Application.GenerateSQLScriptForPositionTable(
    positionRowsFromPayload,
    positionRowsFromTargetDatabase,
    null);
*/

//var fileWithLeeIndexes = @"..\..\..\data\leeindex_tmp.SQL"; // (Windows)
var fileWithLeeIndexes = @"data/leeindex_tmp.SQL"; // (Linux)
var leeIndexRowsFromPayload = Application.LoadLeeIndexesFromPayload(fileWithLeeIndexes);

var leeIndexRowsFromTargetDatabase = Application.LoadLeeIndexesFromDatabase();

Application.GenerateSQLScriptForLeeIndexTable(
    leeIndexRowsFromPayload,
    leeIndexRowsFromTargetDatabase,
    null);


