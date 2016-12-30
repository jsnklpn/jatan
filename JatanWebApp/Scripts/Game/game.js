var _currentGameManager = null;

var _loadQueue = null;
var _canvas = null;
var _stage = null;
var _boardContainer = null;
var _invalidateCanvas = true; // set to true to redraw canvas

// Map to hold all game assets to be preloaded
var _assetMap = {
    // Tiles
    "imgTileBeach": { src: "/Content/Images/board/tile_beach.png", data: null },
    "imgTileDesert": { src: "/Content/Images/board/tile_desert.png", data: null },
    "imgTileWood": { src: "/Content/Images/board/tile_wood.png", data: null },
    "imgTileBrick": { src: "/Content/Images/board/tile_brick.png", data: null },
    "imgTileSheep": { src: "/Content/Images/board/tile_sheep.png", data: null },
    "imgTileOre": { src: "/Content/Images/board/tile_ore.png", data: null },
    "imgTileWheat": { src: "/Content/Images/board/tile_wheat.png", data: null },
    // Background
    "imgWater": { src: "/Content/Images/board/water.jpg", data: null },
    // Buildings - cities
    "imgCity": { src: "/Content/Images/board/city.png", data: null },
    "imgCityBlue": { src: "/Content/Images/board/city_blue.png", data: null },
    "imgCityGreen": { src: "/Content/Images/board/city_green.png", data: null },
    "imgCityPink": { src: "/Content/Images/board/city_pink.png", data: null },
    "imgCityRed": { src: "/Content/Images/board/city_red.png", data: null },
    "imgCityYellow": { src: "/Content/Images/board/city_yellow.png", data: null },
    // Buildings - houses
    "imgHouse1": { src: "/Content/Images/board/house1.png", data: null },
    "imgHouse2": { src: "/Content/Images/board/house2.png", data: null },
    "imgHouse3": { src: "/Content/Images/board/house3.png", data: null },
    "imgHouse1Blue": { src: "/Content/Images/board/house_blue1.png", data: null },
    "imgHouse2Blue": { src: "/Content/Images/board/house_blue2.png", data: null },
    "imgHouse3Blue": { src: "/Content/Images/board/house_blue3.png", data: null },
    "imgHouse1Green": { src: "/Content/Images/board/house_green1.png", data: null },
    "imgHouse2Green": { src: "/Content/Images/board/house_green2.png", data: null },
    "imgHouse3Green": { src: "/Content/Images/board/house_green3.png", data: null },
    "imgHouse1Pink": { src: "/Content/Images/board/house_pink1.png", data: null },
    "imgHouse2Pink": { src: "/Content/Images/board/house_pink2.png", data: null },
    "imgHouse3Pink": { src: "/Content/Images/board/house_pink3.png", data: null },
    "imgHouse1Red": { src: "/Content/Images/board/house_red1.png", data: null },
    "imgHouse2Red": { src: "/Content/Images/board/house_red2.png", data: null },
    "imgHouse3Red": { src: "/Content/Images/board/house_red3.png", data: null },
    "imgHouse1Yellow": { src: "/Content/Images/board/house_yellow1.png", data: null },
    "imgHouse2Yellow": { src: "/Content/Images/board/house_yellow2.png", data: null },
    "imgHouse3Yellow": { src: "/Content/Images/board/house_yellow3.png", data: null },
    // Roads
    "imgRoad1": { src: "/Content/Images/board/road1.png", data: null },
    "imgRoad2": { src: "/Content/Images/board/road2.png", data: null },
    "imgRoad3": { src: "/Content/Images/board/road3.png", data: null },
    "imgRoad1Blue": { src: "/Content/Images/board/road_blue1.png", data: null },
    "imgRoad2Blue": { src: "/Content/Images/board/road_blue2.png", data: null },
    "imgRoad3Blue": { src: "/Content/Images/board/road_blue3.png", data: null },
    "imgRoad1Green": { src: "/Content/Images/board/road_green1.png", data: null },
    "imgRoad2Green": { src: "/Content/Images/board/road_green2.png", data: null },
    "imgRoad3Green": { src: "/Content/Images/board/road_green3.png", data: null },
    "imgRoad1Pink": { src: "/Content/Images/board/road_pink1.png", data: null },
    "imgRoad2Pink": { src: "/Content/Images/board/road_pink2.png", data: null },
    "imgRoad3Pink": { src: "/Content/Images/board/road_pink3.png", data: null },
    "imgRoad1Red": { src: "/Content/Images/board/road_red1.png", data: null },
    "imgRoad2Red": { src: "/Content/Images/board/road_red2.png", data: null },
    "imgRoad3Red": { src: "/Content/Images/board/road_red3.png", data: null },
    "imgRoad1Yellow": { src: "/Content/Images/board/road_yellow1.png", data: null },
    "imgRoad2Yellow": { src: "/Content/Images/board/road_yellow2.png", data: null },
    "imgRoad3Yellow": { src: "/Content/Images/board/road_yellow3.png", data: null },
    // Other
    "imgThief": { src: "/Content/Images/board/thief.png", data: null }
};

// Enums
var ResourceTypes = {
    None: 0,
    Brick: 1,
    Wood: 2,
    Wheat: 3,
    Sheep: 4,
    Ore: 5
};

// The indices of this array must match the ResourceTypes enum.
var _resourceToAssetKeys = [
    "imgTileDesert",
    "imgTileBrick",
    "imgTileWood",
    "imgTileWheat",
    "imgTileSheep",
    "imgTileOre"
];

$(function () {
    _canvas = $("#gameCanvas")[0];
    initSignalR();
    loadGameResources();
});

function initSignalR() {
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
}

function loadGameResources() {
    _loadQueue = new createjs.LoadQueue();
    _loadQueue.on("complete", onLoadQueueCompleted);
    _loadQueue.on("progress", onLoadQueueProgressChanged);
    var resourceKeys = Object.keys(_assetMap);
    for (var i = 0; i < resourceKeys.length; i++) {
        var resKey = resourceKeys[i];
        _loadQueue.loadFile({ id: resKey, src: _assetMap[resKey].src });
    }
}

function onLoadQueueProgressChanged(event) {
    var progressString = (Math.floor(100 * event.progress)).toString() + "%";
    $("#percentLoadedText").text(progressString);
    $("#resourceRrogressBar").css("width", progressString);
}

function onLoadQueueCompleted(event) {
    $("#percentLoadedText").text("Done!");

    // put loaded resources into resource map
    var resourceKeys = Object.keys(_assetMap);
    for (var i = 0; i < resourceKeys.length; i++) {
        var resKey = resourceKeys[i];
        var result = _loadQueue.getResult(resKey);
        if (result) {
            _assetMap[resKey].data = result;
        }
    }

    // wait a second so the user can see that it completed
    setTimeout(completedLoading, 500);
}

function completedLoading() {
    $("#loadingResourcesDiv").hide();
    initCanvasStage();
}

function initCanvasStage() {

    resizeCanvas();

    _stage = new createjs.Stage("gameCanvas");
    _stage.enableMouseOver(30);

    // draw water
    var water = new createjs.Bitmap(_assetMap["imgWater"].data);
    water.x = 0;
    water.y = 0;

    _stage.addChild(water);

    _boardContainer = new createjs.Container();

    var TILE_BEACH_WIDTH = _assetMap["imgTileBeach"].data.width;
    var TILE_BEACH_HEIGHT = _assetMap["imgTileBeach"].data.height;
    var TILE_WIDTH = _assetMap["imgTileDesert"].data.width;
    var TILE_HEIGHT = _assetMap["imgTileDesert"].data.height;

    // draw hexagons
    var beachMargin = -25;
    var beachWidth = TILE_BEACH_WIDTH + beachMargin;
    var beachHeight = TILE_BEACH_HEIGHT + beachMargin;
    for (var row = 0; row < 5; row++) {
        var shiftX = 0;
        var numAcross = 5;
        if (row === 0 || row === 4) { shiftX = beachWidth; numAcross = 3; }
        if (row === 1 || row === 3) { shiftX = beachWidth / 2; numAcross = 4; }
        for (var col = 0; col < numAcross; col++) {
            var beach = new createjs.Bitmap(_assetMap["imgTileBeach"].data);
            beach.x = (beachWidth / 2) + col * beachWidth + shiftX;
            beach.y = (beachHeight / 2) + row * (beachHeight * 0.75);
            beach.regX = TILE_BEACH_WIDTH / 2;
            beach.regY = TILE_BEACH_HEIGHT / 2;

            var srcKey = _resourceToAssetKeys[Math.floor(Math.random() * _resourceToAssetKeys.length)];
            var tile = new createjs.Bitmap(_assetMap[srcKey].data);
            tile.regX = TILE_WIDTH / 2;
            tile.regY = TILE_HEIGHT / 2;
            tile.x = beach.x;
            tile.y = beach.y - 12;

            tile.addEventListener("click", handleClick);
            tile.addEventListener("mouseover", handleMouseOver);
            tile.addEventListener("mouseout", handleMouseOut);

            _boardContainer.addChild(beach);
            _boardContainer.addChild(tile);
        }
    }

    _boardContainer.regX = _boardContainer.getBounds().width / 2;
    _boardContainer.regY = _boardContainer.getBounds().height / 2;
    _boardContainer.scaleX = 0.6;
    _boardContainer.scaleY = 0.6;
    centerBoardInCanvas();

    _stage.addChild(_boardContainer);
    checkRender(); // start animation loop
}

function checkRender() {
    if (resizeCanvas()) {
        _invalidateCanvas = true;
        centerBoardInCanvas();
    }
    if (_invalidateCanvas) {
        _invalidateCanvas = false;
        _stage.update();
    }
    requestAnimationFrame(checkRender);
}

function resizeCanvas() {
    var cw = _canvas.clientWidth;
    var ch = _canvas.clientHeight;
    if (_canvas.width !== cw || _canvas.height !== ch) {
        _canvas.width = cw;
        _canvas.height = ch;
        return true;
    }
    return false;
}

function centerBoardInCanvas() {
    _boardContainer.x = _canvas.width / 2;
    _boardContainer.y = _canvas.height / 2;
}

function handleMouseOver(event) {
    var obj = event.target;
    //obj.stage.setChildIndex(obj, obj.stage.numChildren - 1);
    //obj.shadow = new createjs.Shadow("#fff", 0, 0, 30);
    obj.filters = [new createjs.ColorFilter(1.1, 1.1, 1.1, 1, 0, 0, 0, 0)];
    obj.cache(-500, -500, 1000, 1000);
    _invalidateCanvas = true;
}

function handleMouseOut(event) {
    var obj = event.target;
    obj.filters = [];
    //obj.shadow = null;
    obj.updateCache();
    _invalidateCanvas = true;
}

function handleClick(event) {
    var obj = event.target;
    //obj.skewX += 5;
    //obj.rotation += 5;
    _invalidateCanvas = true;
}

function writeTextToChat(text) {
    $("#chatBoxList").append("<li>" + text + "</li>");
    $("#chatBoxList").animate({ scrollTop: $("#chatBoxList")[0].scrollHeight }, 10);
}

function drawGame(gameManager) {
    
}


//===========================
// Drawing helper functions
//===========================

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

