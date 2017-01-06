//========================================================================
// game.js --- This is where all of the client-side game code is located.
//========================================================================

// Global variables

var _serverGameHub = null; // signal-R hub
var _currentGameManager = null;
// Resource tiles and ports are not sent with every manager update, so we save a separate reference to them
var _currentResourceTiles = null; 
var _currentPorts = null;

var _loadQueue = null;
var _canvas = null;
var _stage = null;
var _water = null;
var _boardContainer = null;
var _invalidateCanvas = true; // set to true to redraw canvas

var _boardDragMouseOffsetX = null;
var _boardDragMouseOffsetY = null;

// These hitboxes are the "game" size of image assets.
// These values are used for setting the proper center-point of the image and for alignment.
// We need to use these values because the actual image sizes vary slightly between items
// of the same time. For example, the Ore tile is taller than the others because of its mountain.
// Therefore, it will not be aligned the same as the other tiles if we use it's real image size.
var beachTileHitbox =   { width: 308, height: 250, centerX: 169,   centerY: 134 };
var beachShadowHitbox = { width: 417, height: 350, centerX: 208.5, centerY: 170 };
var desertTileHitbox =  { width: 291, height: 221, centerX: 145.5, centerY: 112 };
var woodTileHitbox =    { width: 291, height: 233, centerX: 145.5, centerY: 126 };
var brickTileHitbox =   { width: 291, height: 231, centerX: 145.5, centerY: 122 };
var sheepTileHitbox =   { width: 291, height: 220, centerX: 145.5, centerY: 112 };
var oreTileHitbox =     { width: 291, height: 226, centerX: 145.5, centerY: 118 };
var wheatTileHitbox =   { width: 291, height: 220, centerX: 145.5, centerY: 112 };
var cityHitbox =        { width: 192, height: 126, centerX: 96,    centerY: 63 };
var house1Hitbox =      { width: 99,  height: 86,  centerX: 49.5,  centerY: 43 };
var house2Hitbox =      { width: 100, height: 76,  centerX: 50,    centerY: 38 };
var house3Hitbox =      { width: 96,  height: 81,  centerX: 48,    centerY: 40.5 };
var road1Hitbox =       { width: 163, height: 83,  centerX: 81.5,  centerY: 41.5 };
var road2Hitbox =       { width: 42,  height: 95,  centerX: 21,    centerY: 47.5 };
var road3Hitbox =       { width: 161, height: 83,  centerX: 80.5,  centerY: 41.5 };
var dock1Hitbox =       { width: 123, height: 76,  centerX: 70,    centerY: 30 };
var dock2Hitbox =       { width: 118, height: 74,  centerX: 65,    centerY: 30 };
var boatHitbox =        { width: 65,  height: 71,  centerX: 32,    centerY: 44 };

// Map to hold all game assets to be preloaded
var _assetMap = {
    // Tiles
    "imgTileBeach": { src: "/Content/Images/board/tile_beach.png", data: null, hitbox: beachTileHitbox },
    "imgTileBeachShadow": { src: "/Content/Images/board/tile_beach_shadow.png", data: null, hitbox: beachShadowHitbox },
    "imgTileDesert": { src: "/Content/Images/board/tile_desert.png", data: null, hitbox: desertTileHitbox },
    "imgTileWood": { src: "/Content/Images/board/tile_wood.png", data: null, hitbox: woodTileHitbox },
    "imgTileBrick": { src: "/Content/Images/board/tile_brick.png", data: null, hitbox: brickTileHitbox },
    "imgTileSheep": { src: "/Content/Images/board/tile_sheep.png", data: null, hitbox: sheepTileHitbox },
    "imgTileOre": { src: "/Content/Images/board/tile_ore.png", data: null, hitbox: oreTileHitbox },
    "imgTileWheat": { src: "/Content/Images/board/tile_wheat.png", data: null, hitbox: wheatTileHitbox },
    // Background
    "imgWater": { src: "/Content/Images/board/water.jpg", data: null, hitbox: null },
    // Buildings - cities
    "imgCity": { src: "/Content/Images/board/city.png", data: null, hitbox: cityHitbox },
    "imgCityBlue": { src: "/Content/Images/board/city_blue.png", data: null, hitbox: cityHitbox },
    "imgCityGreen": { src: "/Content/Images/board/city_green.png", data: null, hitbox: cityHitbox },
    "imgCityPink": { src: "/Content/Images/board/city_pink.png", data: null, hitbox: cityHitbox },
    "imgCityRed": { src: "/Content/Images/board/city_red.png", data: null, hitbox: cityHitbox },
    "imgCityYellow": { src: "/Content/Images/board/city_yellow.png", data: null, hitbox: cityHitbox },
    // Buildings - houses (3 different types just for variety. They have identical purposes.)
    "imgHouse1": { src: "/Content/Images/board/house1.png", data: null, hitbox: house1Hitbox },
    "imgHouse2": { src: "/Content/Images/board/house2.png", data: null, hitbox: house2Hitbox },
    "imgHouse3": { src: "/Content/Images/board/house3.png", data: null, hitbox: house3Hitbox },
    "imgHouse1Blue": { src: "/Content/Images/board/house_blue1.png", data: null, hitbox: house1Hitbox },
    "imgHouse2Blue": { src: "/Content/Images/board/house_blue2.png", data: null, hitbox: house2Hitbox },
    "imgHouse3Blue": { src: "/Content/Images/board/house_blue3.png", data: null, hitbox: house3Hitbox },
    "imgHouse1Green": { src: "/Content/Images/board/house_green1.png", data: null, hitbox: house1Hitbox },
    "imgHouse2Green": { src: "/Content/Images/board/house_green2.png", data: null, hitbox: house2Hitbox },
    "imgHouse3Green": { src: "/Content/Images/board/house_green3.png", data: null, hitbox: house3Hitbox },
    "imgHouse1Pink": { src: "/Content/Images/board/house_pink1.png", data: null, hitbox: house1Hitbox },
    "imgHouse2Pink": { src: "/Content/Images/board/house_pink2.png", data: null, hitbox: house2Hitbox },
    "imgHouse3Pink": { src: "/Content/Images/board/house_pink3.png", data: null, hitbox: house3Hitbox },
    "imgHouse1Red": { src: "/Content/Images/board/house_red1.png", data: null, hitbox: house1Hitbox },
    "imgHouse2Red": { src: "/Content/Images/board/house_red2.png", data: null, hitbox: house2Hitbox },
    "imgHouse3Red": { src: "/Content/Images/board/house_red3.png", data: null, hitbox: house3Hitbox },
    "imgHouse1Yellow": { src: "/Content/Images/board/house_yellow1.png", data: null, hitbox: house1Hitbox },
    "imgHouse2Yellow": { src: "/Content/Images/board/house_yellow2.png", data: null, hitbox: house2Hitbox },
    "imgHouse3Yellow": { src: "/Content/Images/board/house_yellow3.png", data: null, hitbox: house3Hitbox },
    // Roads (1 = '\', 2 = '|', 3 = '/')
    "imgRoad1": { src: "/Content/Images/board/road1.png", data: null, hitbox: road1Hitbox },
    "imgRoad2": { src: "/Content/Images/board/road2.png", data: null, hitbox: road2Hitbox },
    "imgRoad3": { src: "/Content/Images/board/road3.png", data: null, hitbox: road3Hitbox },
    "imgRoad1Blue": { src: "/Content/Images/board/road_blue1.png", data: null, hitbox: road1Hitbox },
    "imgRoad2Blue": { src: "/Content/Images/board/road_blue2.png", data: null, hitbox: road2Hitbox },
    "imgRoad3Blue": { src: "/Content/Images/board/road_blue3.png", data: null, hitbox: road3Hitbox },
    "imgRoad1Green": { src: "/Content/Images/board/road_green1.png", data: null, hitbox: road1Hitbox },
    "imgRoad2Green": { src: "/Content/Images/board/road_green2.png", data: null, hitbox: road2Hitbox },
    "imgRoad3Green": { src: "/Content/Images/board/road_green3.png", data: null, hitbox: road3Hitbox },
    "imgRoad1Pink": { src: "/Content/Images/board/road_pink1.png", data: null, hitbox: road1Hitbox },
    "imgRoad2Pink": { src: "/Content/Images/board/road_pink2.png", data: null, hitbox: road2Hitbox },
    "imgRoad3Pink": { src: "/Content/Images/board/road_pink3.png", data: null, hitbox: road3Hitbox },
    "imgRoad1Red": { src: "/Content/Images/board/road_red1.png", data: null, hitbox: road1Hitbox },
    "imgRoad2Red": { src: "/Content/Images/board/road_red2.png", data: null, hitbox: road2Hitbox },
    "imgRoad3Red": { src: "/Content/Images/board/road_red3.png", data: null, hitbox: road3Hitbox },
    "imgRoad1Yellow": { src: "/Content/Images/board/road_yellow1.png", data: null, hitbox: road1Hitbox },
    "imgRoad2Yellow": { src: "/Content/Images/board/road_yellow2.png", data: null, hitbox: road2Hitbox },
    "imgRoad3Yellow": { src: "/Content/Images/board/road_yellow3.png", data: null, hitbox: road3Hitbox },
    // Other
    "imgThief": { src: "/Content/Images/board/thief.png", data: null, hitbox: null },
    "imgBoat": { src: "/Content/Images/board/boat.png", data: null, hitbox: boatHitbox },
    "imgDock1": { src: "/Content/Images/board/dock1.png", data: null, hitbox: dock1Hitbox },
    "imgDock2": { src: "/Content/Images/board/dock2.png", data: null, hitbox: dock2Hitbox }
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

// The side of a hexagon that is pointed on the top and bottom.
var EdgeDir = {
    Right: 0,
    BottomRight: 1,
    BottomLeft: 2,
    Left: 3,
    TopLeft: 4,
    TopRight: 5
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

// These hex keys are ordered in the same order that the tile Bitmaps are added to the canvas stage.
var _hexKeys = [
    "(0,2)",
    "(1,2)",
    "(2,2)",

    "(-1,1)",
    "(0,1)",
    "(1,1)",
    "(2,1)",

    "(-2,0)",
    "(-1,0)",
    "(0,0)",
    "(1,0)",
    "(2,0)",

    "(-2,-1)",
    "(-1,-1)",
    "(0,-1)",
    "(1,-1)",

    "(-2,-2)",
    "(-1,-2)",
    "(0,-2)"
];

// Populated when beach tiles are drawn
var _hexToBeachMap = {};

// Populated when resource tiles are drawn
var _hexToResourceTileMap = {};

var _portsPopulated = false;

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
        updateGameModel(gameManager);
    }

    // Start the connection.
    $.connection.hub.start().done(function () {

        // save a reference to the server hub object.
        _serverGameHub = gameHub.server;

        initButtons();

        //$("#chatBoxInputText").keypress(function (event) {
        //    var keycode = (event.keyCode ? event.keyCode : event.which);
        //    // Enter key pressed
        //    if (keycode == "13") {
        //        var msgToSend = $("#chatBoxInputText").val();
        //        if (msgToSend.length > 0) {

        //            // temp
        //            if (msgToSend.toLowerCase() === "update")
        //                gameHub.server.getGameManagerUpdate();
        //            else {
        //                gameHub.server.sendChatMessage("Jason", msgToSend);
        //                $("#chatBoxInputText").val("").focus();
        //            }
        //        }
        //    }
        //});
    });
}

function initButtons() {
    $("#btnUpdateGameManager").click(function () {
        _serverGameHub.getGameManagerUpdate();
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
    $("#resourceProgressBar").css("width", progressString);
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
    setTimeout(completedLoading, 50);
}

function completedLoading() {
    $("#loadingResourcesDiv").hide();
    initCanvasStage();
}

function initCanvasStage() {

    _stage = new createjs.Stage("gameCanvas");
    //_stage.enableMouseOver(10);
    _boardContainer = new createjs.Container();

    // draw water
    _water = new createjs.Bitmap(_assetMap["imgWater"].data);
    _water.mouseEnabled = false;
    _stage.addChild(_water);

    // draw hexagons
    var beachAsset = _assetMap["imgTileBeach"];
    var beachWidth = beachAsset.hitbox.width;
    var beachHeight = beachAsset.hitbox.height;
    // save the sprites in a list so we can add them to the container in the correct order.
    var beachBitmaps = [];
    var beachShadows = [];
    var roads = [];
    var hexIndex = 0;
    for (var row = 0; row < 5; row++) {
        var shiftX = 0;
        var numAcross = 5;
        if (row === 0 || row === 4) { shiftX = beachWidth; numAcross = 3; }
        if (row === 1 || row === 3) { shiftX = beachWidth / 2; numAcross = 4; }
        for (var col = 0; col < numAcross; col++) {
            var hexKey = _hexKeys[hexIndex++];
            var beach = new createjs.Bitmap(beachAsset.data);
            //beach.mouseEnabled = false;
            beach.regX = beachAsset.hitbox.centerX;
            beach.regY = beachAsset.hitbox.centerY;
            beach.x = (beachWidth / 2) + col * beachWidth + shiftX;
            beach.y = (beachHeight / 2) + row * (beachHeight * 0.75);
            _hexToBeachMap[hexKey] = beach;

            // create drop shadow
            var beachShadow = new createjs.Bitmap(_assetMap["imgTileBeachShadow"].data);
            beachShadow.mouseEnabled = false;
            beachShadow.regX = _assetMap["imgTileBeachShadow"].hitbox.centerX;
            beachShadow.regY = _assetMap["imgTileBeachShadow"].hitbox.centerY;
            beachShadow.x = beach.x;
            beachShadow.y = beach.y;

            // create roads
            //var roadAssets = [_assetMap["imgRoad1"], _assetMap["imgRoad2"], _assetMap["imgRoad3"]];
            //var roadBitmaps = [new createjs.Bitmap(roadAssets[0].data), new createjs.Bitmap(roadAssets[1].data), new createjs.Bitmap(roadAssets[2].data)];
            //for (var i = 0; i < roadBitmaps.length; i++) {
            //    roads.push(roadBitmaps[i]);
            //    roadBitmaps[i].regX = roadAssets[i].hitbox.centerX;
            //    roadBitmaps[i].regY = roadAssets[i].hitbox.centerY;

            //    roadBitmaps[i].addEventListener("click", handleClick);
            //    roadBitmaps[i].addEventListener("mouseover", handleMouseOver);
            //    roadBitmaps[i].addEventListener("mouseout", handleMouseOut);
            //}
            //roadBitmaps[0].x = beach.x + beachWidth * 0.25;
            //roadBitmaps[0].y = beach.y - beachHeight * 0.39;
            //roadBitmaps[1].x = beach.x - beachWidth * 0.5;
            //roadBitmaps[1].y = beach.y;
            //roadBitmaps[2].x = beach.x - beachWidth * 0.25;
            //roadBitmaps[2].y = beach.y - beachHeight * 0.39;


            beachShadows.push(beachShadow);
            beachBitmaps.push(beach);
        }
    }

    // Resource tiles need to always be drawn on top, so they get added after beaches.
    for (var i = 0; i < beachShadows.length; i++) {
        _boardContainer.addChild(beachShadows[i]);
    }
    for (var i = 0; i < beachBitmaps.length; i++) {
        _boardContainer.addChild(beachBitmaps[i]);
    }
    //for (var i = 0; i < roads.length; i++) {
    //    _boardContainer.addChild(roads[i]);
    //}
    
    _boardContainer.regX = _boardContainer.getBounds().width / 2;
    _boardContainer.regY = _boardContainer.getBounds().height / 2;
    _boardContainer.scaleX = 0.65;
    _boardContainer.scaleY = 0.65;
    
    _stage.addChild(_boardContainer);

    // allow user to move and scale the board with the mouse
    initMouseWheelScaling();

    checkRender(); // start animation loop
}

function populateResourceTiles() {

    // If we're here, then we should have a game manager instance from the server.
    if (_currentGameManager === null || _currentResourceTiles === null) {
        return;
    }

    var resTiles = [];
    var tileIndex = 0;
    for (var row = 0; row < 5; row++) {
        var numAcross = 5;
        if (row === 0 || row === 4) { numAcross = 3; }
        if (row === 1 || row === 3) { numAcross = 4; }
        for (var col = 0; col < numAcross; col++) {

            var hexKey = _hexKeys[tileIndex];
            var beach = _hexToBeachMap[hexKey];

            var resource = _currentResourceTiles[hexKey]["Resource"];
            var srcKey = _resourceToAssetKeys[resource];
            var tile = new createjs.Bitmap(_assetMap[srcKey].data);
            tile.regX = _assetMap[srcKey].hitbox.centerX;
            tile.regY = _assetMap[srcKey].hitbox.centerY;
            tile.x = beach.x; // center on beach tile
            tile.y = beach.y; // center on beach tile

            tile.addEventListener("click", handleClick);
            tile.addEventListener("mouseover", handleMouseOver);
            tile.addEventListener("mouseout", handleMouseOut);

            resTiles.push(tile);

            // save the bitmap to the hexmap so we can get which hex it refers to.
            _hexToResourceTileMap[hexKey] = tile;

            tileIndex++;
        }
    }
    for (var i = 0; i < resTiles.length; i++) {
        _boardContainer.addChild(resTiles[i]);
    }

    _invalidateCanvas = true;
}

function populatePorts() {
    // If we're here, then we should have a game manager instance from the server.
    if (_currentGameManager === null || _currentResourceTiles === null) {
        return;
    }

    var boatAsset = _assetMap["imgBoat"];
    var dockForwardAsset = _assetMap["imgDock1"];
    var dockBackwardAsset = _assetMap["imgDock2"];
    var dfw = dockForwardAsset.hitbox.width;
    var dfh = dockForwardAsset.hitbox.height;
    var dbw = dockBackwardAsset.hitbox.width;
    var dbh = dockBackwardAsset.hitbox.height;

    var beachAsset = _assetMap["imgTileBeach"];
    var bw = beachAsset.hitbox.width;
    var bh = beachAsset.hitbox.height;

    var hexEdges = Object.keys(_currentPorts);
    for (var i = 0; i < hexEdges.length; i++) {
        var hexEdge = hexEdges[i];
        var resource = _currentPorts[hexEdge];
        var tmp = gethexAndDirectionFromEdge(hexEdge);
        var hex = tmp[0];
        var dir = tmp[1];
        var hexValues = hexToValueArray(hex);

        var tile = _hexToResourceTileMap[hex];
        var beach = _hexToBeachMap[hex];
        var forwardDock = false;

        // Determine how the dock should be angled
        if (dir === EdgeDir.TopLeft || dir === EdgeDir.BottomRight) {
            forwardDock = false;
        }
        else if (dir === EdgeDir.TopRight || dir === EdgeDir.BottomLeft) {
            forwardDock = true;
        }
        else if (dir === EdgeDir.Left) {
            if (hexValues[1] <= 0) { // bottom half
                forwardDock = true;
            } else {// top half
                forwardDock = false;
            }
        }
        else if (dir === EdgeDir.Right) {
            if (hexValues[1] <= 0) { // bottom half
                forwardDock = false;
            } else { // top half
                forwardDock = true;
            }
        } else {
            forwardDock = true;
        }
        
        var dockAsset = forwardDock ? dockForwardAsset : dockBackwardAsset;
        var dw = forwardDock ? dfw : dbw;
        var dh = forwardDock ? dfh : dbh;

        var boat = new createjs.Bitmap(boatAsset.data);
        var dock1 = new createjs.Bitmap(dockAsset.data);
        var dock2 = new createjs.Bitmap(dockAsset.data);
        boat.regX = boatAsset.hitbox.centerX;
        boat.regY = boatAsset.hitbox.centerY;
        dock1.regX = dockAsset.hitbox.centerX;
        dock1.regY = dockAsset.hitbox.centerY;
        dock2.regX = dockAsset.hitbox.centerX;
        dock2.regY = dockAsset.hitbox.centerY;

        if (dir === EdgeDir.Right) {
            dock1.x = beach.x + (bw / 2 + dw / 4 + 10);
            dock1.y = beach.y + (bh / 6 - dh / 4);
            dock2.x = beach.x + (bw / 2 + dw / 4 + 10);
            dock2.y = beach.y - (bh / 6 + dh / 2);
            if (!forwardDock) {
                dock1.y += bh / 6;
                dock2.y += bh / 6;
            }
            boat.x = beach.x + bw;
            boat.y = beach.y;
            boat.y += (forwardDock ? -0.5 : 0.5) * boatAsset.hitbox.height;
        }
        else if (dir === EdgeDir.Left) {
            dock1.x = beach.x - (bw / 2 + dw / 4 + 10);
            dock1.y = beach.y + (bh / 6 - dh / 4);
            dock2.x = beach.x - (bw / 2 + dw / 4 + 10);
            dock2.y = beach.y - (bh / 6 + dh / 2);
            if (forwardDock) {
                dock1.y += bh / 6;
                dock2.y += bh / 6;
            }
            boat.x = beach.x - bw;
            boat.y = beach.y;
            boat.y += (forwardDock ? 0.5 : -0.5) * boatAsset.hitbox.height;
        }
        else if (dir === EdgeDir.BottomRight) {
            dock1.x = beach.x + dw / 2 - 10;
            dock1.y = beach.y + bh / 2 + dh / 6;
            dock2.x = beach.x + bw / 2 + dw / 4;
            dock2.y = beach.y + bh / 6 + dw / 4;

            boat.x = beach.x + (2 * bw) / 3;
            boat.y = beach.y + (2 * bh) / 3;
        }
        else if (dir === EdgeDir.TopLeft) {
            dock1.x = beach.x - (dw / 2 - 10);
            dock1.y = beach.y - (bh / 2 + dh / 6);
            dock2.x = beach.x - (bw / 2 + dw / 4);
            dock2.y = beach.y - (bh / 6 + dw / 4);

            boat.x = beach.x - (2 * bw) / 3;
            boat.y = beach.y - (2 * bh) / 3;
        }
        else if (dir === EdgeDir.TopRight) {
            dock1.x = beach.x + (dw / 2 - 10);
            dock1.y = beach.y - (bh / 2 + dh / 6);
            dock2.x = beach.x + (bw / 2 + dw / 4);
            dock2.y = beach.y - (bh / 6 + dw / 4);

            boat.x = beach.x + (2 * bw) / 3;
            boat.y = beach.y - (2 * bh) / 3;
        }
        else if (dir === EdgeDir.BottomLeft) {
            dock1.x = beach.x - (dw / 2 - 10);
            dock1.y = beach.y + bh / 2 + dh / 6;
            dock2.x = beach.x - (bw / 2 + dw / 4);
            dock2.y = beach.y + bh / 6 + dw / 4;

            boat.x = beach.x - (2 * bw) / 3;
            boat.y = beach.y + (2 * bh) / 3;
        }
        

        _boardContainer.addChild(dock1);
        _boardContainer.addChild(dock2);
        _boardContainer.addChild(boat);
    }

    _portsPopulated = true;
    _invalidateCanvas = true;
}

function gethexAndDirectionFromEdge(hexEdge) {
    var hexes = hexEdgeToHexagons(hexEdge);
    var primaryHex = null;
    var otherHex = null;
    if (jQuery.inArray(hexes[0], _hexKeys) !== -1) {
        primaryHex = hexes[0];
        otherHex = hexes[1];
    } else {
        primaryHex = hexes[1];
        otherHex = hexes[0];
    }
    var primary = hexToValueArray(primaryHex);
    var other = hexToValueArray(otherHex);
    var x1 = primary[0]; var y1 = primary[1];
    var x2 = other[0]; var y2 = other[1];
    var direction = null;
    if (x2 === (x1 + 0) && y2 === (y1 + 1)) { direction = EdgeDir.TopLeft; }
    else if (x2 === (x1 + 1) && y2 === (y1 + 1)) { direction = EdgeDir.TopRight; }
    else if (x2 === (x1 + 1) && y2 === (y1 + 0)) { direction = EdgeDir.Right; }
    else if (x2 === (x1 + 0) && y2 === (y1 - 1)) { direction = EdgeDir.BottomRight; }
    else if (x2 === (x1 - 1) && y2 === (y1 - 1)) { direction = EdgeDir.BottomLeft; }
    else if (x2 === (x1 - 1) && y2 === (y1 + 0)) { direction = EdgeDir.Left; }
    return [primaryHex, direction];
}

function checkRender() {
    if (resizeCanvas()) {
        _invalidateCanvas = true;
        centerBoardInCanvas();
        resizeWaterBackground();
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

function resizeWaterBackground() {
    var scaleX = 1;
    var scaleY = 1;
    if (_water.image.width < _canvas.width) {
        scaleX = _canvas.width / _water.image.width;
    }
    if (_water.image.height < _canvas.height) {
        scaleY = _canvas.height / _water.image.height;
    }
    if (scaleX > scaleY) {
        _water.scaleX = scaleX;
        _water.scaleY = scaleX;
    } else {
        _water.scaleX = scaleY;
        _water.scaleY = scaleY;
    }
}

function centerBoardInCanvas() {
    // TODO: This should use offsets
    _boardContainer.x = _canvas.width / 2;
    _boardContainer.y = _canvas.height / 2;
}

function initMouseWheelScaling() {
    var maxScale = 1.0;
    var minScale = 0.4;
    var scaleStep = 0.02;
    $("#gameCanvas").bind('mousewheel DOMMouseScroll', function (event) {
        if (event.originalEvent.wheelDelta > 0 || event.originalEvent.detail < 0) {
            // scroll up
            if (_boardContainer.scaleX < maxScale) {
                if (_boardContainer.scaleX >= (maxScale - scaleStep)) {
                    _boardContainer.scaleX = maxScale;
                    _boardContainer.scaleY = maxScale;
                } else {
                    _boardContainer.scaleX += scaleStep;
                    _boardContainer.scaleY += scaleStep;
                }
                _invalidateCanvas = true;
            }
        }
        else {
            // scroll down
            if (_boardContainer.scaleX > minScale) {
                if (_boardContainer.scaleX <= (minScale + scaleStep)) {
                    _boardContainer.scaleX = minScale;
                    _boardContainer.scaleY = minScale;
                } else {
                    _boardContainer.scaleX -= scaleStep;
                    _boardContainer.scaleY -= scaleStep;
                }
                _invalidateCanvas = true;
            }
        }
    });
    // make grame board draggable
    _boardContainer.on("pressmove", function (event) {
        // save offset so we can drag any part of the board.
        if (_boardDragMouseOffsetX === null) {
            _boardDragMouseOffsetX = _boardContainer.x - event.stageX;
            _boardDragMouseOffsetY = _boardContainer.y - event.stageY;
        }

        _boardContainer.x = event.stageX + _boardDragMouseOffsetX;
        _boardContainer.y = event.stageY + _boardDragMouseOffsetY;

        // Don't let the board leave the canvas.
        if (_boardContainer.x > _canvas.width) _boardContainer.x = _canvas.width;
        else if (_boardContainer.x < 0) _boardContainer.x = 0;
        if (_boardContainer.y > _canvas.height) _boardContainer.y = _canvas.height;
        else if (_boardContainer.y < 0) _boardContainer.y = 0;

        _invalidateCanvas = true;
    });
    _boardContainer.on("pressup", function (event) {
        _boardDragMouseOffsetX = null;
        _boardDragMouseOffsetY = null;
    });
}

function handleMouseOver(event) {
    var obj = event.target;
    //obj.stage.setChildIndex(obj, obj.stage.numChildren - 1);
    //obj.shadow = new createjs.Shadow("#fff", 0, 0, 30);
    obj.filters = [new createjs.ColorFilter(1.2, 1.2, 1.2, 1, 0, 0, 0, 0)];
    obj.cache(-500, -500, 1000, 1000);
    _invalidateCanvas = true;
}

function handleMouseOut(event) {
    var obj = event.target;
    //obj.shadow = null;
    obj.filters = [];
    obj.updateCache();
    _invalidateCanvas = true;
}

function handleClick(event) {
    var obj = event.target;

    var hex = getHexFromResourceTileBitmap(obj);
    if (hex !== null) {
        console.log(hex + " clicked!");
    }

    _invalidateCanvas = true;
}

function writeTextToChat(text) {
    $("#chatBoxList").append("<li>" + text + "</li>");
    $("#chatBoxList").animate({ scrollTop: $("#chatBoxList")[0].scrollHeight }, 10);
}

function updateGameModel(gameManager) {
    _currentGameManager = gameManager;
    var resourceTiles = gameManager["GameBoard"]["ResourceTiles"];
    var ports = gameManager["GameBoard"]["Ports"];
    if (resourceTiles) {
        _currentResourceTiles = resourceTiles;
        // if we haven't populated the resource tiles on the board yet, do it.
        if (getDictLength(_hexToResourceTileMap) === 0) {
            populateResourceTiles();
        }
    }
    if (ports) {
        _currentPorts = ports;
        if (!_portsPopulated) {
            populatePorts();
        }
    }

    // TODO ...
}


//===========================
// Drawing helper functions
//===========================

function getDictLength(dict) {
    return Object.keys(dict).length;
}

function getHexFromResourceTileBitmap(bitmap) {
    var hexKeys = Object.keys(_hexToResourceTileMap);
    for (var i = 0; i < hexKeys.length; i++) {
        var hexKey = hexKeys[i];
        var obj = _hexToResourceTileMap[hexKey];
        if (obj === bitmap) {
            return hexKey;
        }
    }
    return null;
}

// Returns a list of hexagons from a hexedge. (e.g. "[(-2,0),(-3,0)]" returns the list ["(-2,0)", "(-3,0)"])
function hexEdgeToHexagons(hexEdge) {
    var ob1 = hexEdge.indexOf("(", 0);
    var cb1 = hexEdge.indexOf(")", 0);
    var ob2 = hexEdge.indexOf("(", cb1 + 1);
    var cb2 = hexEdge.indexOf(")", cb1 + 1);
    var hex1 = hexEdge.slice(ob1, cb1 + 1);
    var hex2 = hexEdge.slice(ob2, cb2 + 1);
    var hexes = [hex1, hex2];
    return hexes;
}

function hexToValueArray(hex) {
    var strX = hex.slice(1, hex.indexOf(","));
    var strY = hex.slice(hex.indexOf(",") + 1, hex.indexOf(")"));
    var x = parseInt(strX);
    var y = parseInt(strY);
    return [x, y];
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

