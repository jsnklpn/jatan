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
var _boardTileContainer = null; // child to the board container
var _boardRoadContainer = null; // child to the board container
var _boardBuildingContainer = null; // child to the board container
var _invalidateCanvas = true; // set to true to redraw canvas on next animation frame

var _activeMouseButton = null;
var _boardDragMouseOffsetX = null;
var _boardDragMouseOffsetY = null;

var _hexToBeachMap = {}; // Populated when beach tiles are drawn
var _hexToResourceTileMap = {}; // Populated when resource tiles are drawn
var _portsPopulated = false;

// These hitboxes are the "game" size of image assets.
// These values are used for setting the proper center-point of the image and for alignment.
// We need to use these values because the actual image sizes vary slightly between items
// of the same time. For example, the Ore tile is taller than the others because of its mountain.
// Therefore, it will not be aligned the same as the other tiles if we use it's real image size.
var beachTileHitbox = { width: 308, height: 250, centerX: 169, centerY: 134 };
var beachShadowHitbox = { width: 417, height: 350, centerX: 208.5, centerY: 170 };
var desertTileHitbox = { width: 291, height: 221, centerX: 145.5, centerY: 112 };
var woodTileHitbox = { width: 291, height: 233, centerX: 145.5, centerY: 126 };
var brickTileHitbox = { width: 291, height: 231, centerX: 145.5, centerY: 122 };
var sheepTileHitbox = { width: 291, height: 220, centerX: 145.5, centerY: 112 };
var oreTileHitbox = { width: 291, height: 226, centerX: 145.5, centerY: 118 };
var wheatTileHitbox = { width: 291, height: 220, centerX: 145.5, centerY: 112 };
var cityHitbox = { width: 192, height: 126, centerX: 96, centerY: 63 };
var house1Hitbox = { width: 99, height: 86, centerX: 49.5, centerY: 43 };
var house2Hitbox = { width: 100, height: 76, centerX: 50, centerY: 38 };
var house3Hitbox = { width: 96, height: 81, centerX: 48, centerY: 40.5 };
var road1Hitbox = { width: 163, height: 83, centerX: 81.5, centerY: 41.5 };
var road2Hitbox = { width: 42, height: 95, centerX: 21, centerY: 47.5 };
var road3Hitbox = { width: 161, height: 83, centerX: 80.5, centerY: 41.5 };
var dock1Hitbox = { width: 123, height: 76, centerX: 70, centerY: 30 };
var dock2Hitbox = { width: 118, height: 74, centerX: 65, centerY: 30 };
var boatHitbox = { width: 65, height: 71, centerX: 32, centerY: 44 };

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

// Resource types enum
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

// Game states
var GameState = {
    NotStarted: 0,
    InitialPlacement: 1,
    GameInProgress: 2,
    EndOfGame: 3
};

// Player turn states. The states are needed for actions that require further action from the player.
var PlayerTurnState = {
    None: 0,
    NeedToRoll: 1,
    AnyPlayerSelectingCardsToLose: 2,
    PlacingRobber: 3,
    SelectingPlayerToStealFrom: 4,
    TakeAction: 5,
    PlacingRoad: 6,
    PlacingBuilding: 7,
    RequestingPlayerTrade: 8,
    MonopolySelectingResource: 9,
    RoadBuildingSelectingRoads: 10,
    YearOfPlentySelectingResources: 11
};

// Types of player buildings
var BuildingTypes = {
    Settlement: 0,
    City: 1
};

// Player colors enum
var PlayerColor = {
    Blue: 0,
    Red: 1,
    Green: 2,
    Yellow: 3,
    Pink: 4
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