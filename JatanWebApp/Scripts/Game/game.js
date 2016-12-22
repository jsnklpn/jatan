var _currentGameManager = null;
var _canvasHeight = 400;
var _canvasWidth = 1000;

// Classes
function Hex(x, y) {
    
}

// Enums
var ResourceTypes = {
    None: 0,
    Brick: 1,
    Wood: 2,
    Wheat: 3,
    Sheep: 4,
    Ore: 5
};

$(function () {

    // Declare a proxy to reference the hub.
    var gameHub = $.connection.gameHub;

    // Create a function that the hub can call to broadcast messages.
    gameHub.client.broadcastMessage = function (name, message) {
        var encodedName = $("<div />").text(name).html();
        var encodedMsg = $("<div />").text(message).html();

        $("#chatBoxList").append("<li><strong>" + encodedName + "</strong>:&nbsp;&nbsp;" + encodedMsg + "</li>");
        $("#chatBoxList").animate({ scrollTop: $("#chatBoxList")[0].scrollHeight }, 10);
    };

    gameHub.client.updateGameManager = function (gameManager) {
        _currentGameManager = gameManager;
        drawGame(gameManager);
    }

    // Start the connection.
    $.connection.hub.start().done(function () {

        $("#chatBoxInputText").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            // Enter key pressed
            if (keycode == "13") {
                var msgToSend = $("#chatBoxInputText").val();
                if (msgToSend.length > 0) {

                    // temp
                    if (msgToSend.toLowerCase() === "update")
                        gameHub.server.getGameManagerUpdate();
                    else {
                        gameHub.server.sendChatMessage("Jason", msgToSend);
                        $("#chatBoxInputText").val("").focus();
                    }
                }
            }
        });
    });

    var canvas = $("#gameCanvas");
    canvas[0].width = _canvasWidth;
    canvas[0].height = _canvasHeight;
    canvas.click(canvasClick);
    canvas.mousemove(canvasMouseMove);

});

function writeTextToChat(text) {
    $("#chatBoxList").append("<li>" + text + "</li>");
    $("#chatBoxList").animate({ scrollTop: $("#chatBoxList")[0].scrollHeight }, 10);
}

function canvasClick(e) {
    var parentOffset = $(this).offset();
    var relX = e.pageX - parentOffset.left;
    var relY = e.pageY - parentOffset.top;
    var ctx = this.getContext("2d");


}

function canvasMouseMove(e) {
    var parentOffset = $(this).offset();
    var relX = e.pageX - parentOffset.left;
    var relY = e.pageY - parentOffset.top;
    var ctx = this.getContext("2d");
    

}

function drawGame(gameManager) {
    var canvas = $("#gameCanvas")[0];
    var ctx = canvas.getContext("2d");

    ctx.fillStyle = "#e1f9ff";
    ctx.fillRect(0, 0, _canvasWidth, _canvasHeight);

    var board = gameManager.GameBoard;
    var boardKeys = Object.keys(board.ResourceTiles);
    for (var i = 0; i < boardKeys.length; i++) {
        var key = boardKeys[i];
        var value = board.ResourceTiles[key];
        drawHex(ctx, key, value);
    }
}

function drawHex(ctx, key, value) {
    var point = stringToPoint(key);
    var resource = value.Resource;
    var retrieveNumber = value.RetrieveNumber;

    var hexPoints = hexagonToAbsolutePoints(point);
    //var centerPoint = hexagonToAbsoluteCenter(point);

    fillPolygon(ctx, hexPoints, resourceToColor(resource));
    var boundingBox = getBoundingBox(hexPoints);
    var centerPoint = boundingBox.center();

    ctx.fillStyle = "#fff";
    ctx.fillText(key, centerPoint.x, centerPoint.y);

    writeTextToChat(point.toString() + ", " + resource + ", " + retrieveNumber);
}

function fillPolygon(ctx, points, color) {
    if (points.length < 3) {
        return; // not a polygon
    }
    ctx.fillStyle = color;
    ctx.beginPath();
    ctx.moveTo(points[0].x, points[0].y);
    for (var i = 1; i < points.length; i++) {
        ctx.lineTo(points[i].x, points[i].y);
    }
    ctx.closePath();
    ctx.fill();
}

function resourceToColor(resource) {
    if (resource === ResourceTypes.Brick)
        return "red";
    if (resource === ResourceTypes.None)
        return "black";
    if (resource === ResourceTypes.Ore)
        return "grey";
    if (resource === ResourceTypes.Sheep)
        return "green";
    if (resource === ResourceTypes.Wheat)
        return "yellow";
    if (resource === ResourceTypes.Wood)
        return "#050";
    return "white";
}

//===========================
// Coordinate helper methods
//===========================

var _hexPointMap = {};
var _relativeTileWidth = 100 / 6;
var _identityHexagonPoints = [
        new Point(0, 0.5),
        new Point(0.5, 0.25),
        new Point(0.5, -0.25),
        new Point(0, -0.5),
        new Point(-0.5, -0.25),
        new Point(-0.5, 0.25)];

function hexagonToAbsoluteCenter(hex) {
    var identityOffset = getIdentityHexOffset(hex);
    var offsetPoint = new Point(identityOffset.x * _relativeTileWidth, identityOffset.y * _relativeTileWidth);
    var p = [offsetPoint];
    var hexPointAbsolute = convertRelativePointsToAbsolute(p)[0];
    return hexPointAbsolute;
}

function hexagonToAbsolutePoints(hex) {
    var strHex = hex.toString();
    if (_hexPointMap.hasOwnProperty(strHex))
        return _hexPointMap[strHex];
    var hexPointsRelative = getRelativeHexegonPoints(hex);
    var hexPointsAbsolute = convertRelativePointsToAbsolute(hexPointsRelative);
    _hexPointMap[strHex] = hexPointsAbsolute;
    return hexPointsAbsolute;
}

function convertRelativePointsToAbsolute(points)
{
    var newPoints = [];
    for (var i = 0; i < points.length; i++)
    {
        // Offset by 50% so that 0,0 is actually in the center of the control.
        newPoints[i] = relativeToAbsolute(50 + points[i].x, 50 + points[i].y);
    }
    return newPoints;
}

function relativeToAbsolute(x, y)
{
    var w = _canvasHeight; // The height is the shortest
    var h = _canvasHeight;
    return new Point(w * (x / 100), h * (y / 100));
}

function getIdentityHexOffset(hex) {
    var newX = hex.x - (hex.y * 0.5);
    var newY = -0.75 * hex.y;
    return new Point(newX, newY);
}

function getRelativeHexegonPoints(hex)
{
    var points = getIdentityHexagonPoints();
    var identityOffset = getIdentityHexOffset(hex);
    var newPoints = [];
    for (var i = 0; i < points.length; i++) {
        var newX = (points[i].x + identityOffset.x) * _relativeTileWidth;
        var newY = (points[i].y + identityOffset.y) * _relativeTileWidth;
        newPoints[i] = new Point(newX, newY);
    }
    return newPoints;
}

function getIdentityHexagonPoints() {
    return _identityHexagonPoints;
}

