using MigrationScriptGenerator;

//var fileWithPositions = @"..\..\..\data\position_tmp.SQL"; // (Windows)
var fileWithPositions = @"data/position_tmp.SQL"; // (Linux)
var positionRowsFromPayload = Application.LoadPositionsFromPayload(fileWithPositions);

var positionRowsFromTargetDatabase = Application.LoadPositionsFromDatabase();

Application.GenerateSQLScriptForPositionTable(
    positionRowsFromPayload,
    positionRowsFromTargetDatabase,
    null);

