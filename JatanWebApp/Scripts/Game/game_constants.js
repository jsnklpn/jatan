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
var cityHitbox = { width: 192, height: 126, centerX: 107, centerY: 100 };
var house1Hitbox = { width: 99, height: 86, centerX: 63, centerY: 62 };
var house2Hitbox = { width: 100, height: 76, centerX: 68, centerY: 54 };
var house3Hitbox = { width: 96, height: 81, centerX: 67, centerY: 56 };
var road1Hitbox = { width: 163, height: 83, centerX: 81.5, centerY: 41.5 };
var road2Hitbox = { width: 42, height: 95, centerX: 21, centerY: 47.5 };
var road3Hitbox = { width: 161, height: 83, centerX: 80.5, centerY: 41.5 };
var dock1Hitbox = { width: 123, height: 76, centerX: 70, centerY: 30 };
var dock2Hitbox = { width: 118, height: 74, centerX: 65, centerY: 30 };
var boatHitbox = { width: 65, height: 71, centerX: 32, centerY: 44 };
var theifHitbox = { width: 86, height: 123, centerX: 53, centerY: 97 };
var diceHitbox = { width: 128, height: 128, centerX: 64, centerY: 64 };
var resourceCardHitbox = { width: 132, height: 186, centerX: 66, centerY: 93 };
var woodIconHitbox = { width: 36, height: 23, centerX: 18, centerY: 11.5 };
var brickIconHitbox = { width: 35, height: 19, centerX: 17.5, centerY: 9.5 };
var wheatIconHitbox = { width: 27, height: 33, centerX: 13.5, centerY: 16.5 };
var sheepIconHitbox = { width: 39, height: 26, centerX: 19.5, centerY: 13 };
var oreIconHitbox = { width: 37, height: 25, centerX: 18.5, centerY: 12.5 };
var questionIconHitbox = { width: 32, height: 32, centerX: 16, centerY: 16 };

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
    "imgThief": { src: "/Content/Images/board/thief.png", data: null, hitbox: theifHitbox },
    "imgBoat": { src: "/Content/Images/board/boat.png", data: null, hitbox: boatHitbox },
    "imgDock1": { src: "/Content/Images/board/dock1.png", data: null, hitbox: dock1Hitbox },
    "imgDock2": { src: "/Content/Images/board/dock2.png", data: null, hitbox: dock2Hitbox },
    // Dice
    "imgDice1": { src: "/Content/Images/board/dice1.png", data: null, hitbox: diceHitbox },
    "imgDice2": { src: "/Content/Images/board/dice2.png", data: null, hitbox: diceHitbox },
    "imgDice3": { src: "/Content/Images/board/dice3.png", data: null, hitbox: diceHitbox },
    "imgDice4": { src: "/Content/Images/board/dice4.png", data: null, hitbox: diceHitbox },
    "imgDice5": { src: "/Content/Images/board/dice5.png", data: null, hitbox: diceHitbox },
    "imgDice6": { src: "/Content/Images/board/dice6.png", data: null, hitbox: diceHitbox },
    // Cards
    "imgCardWood": { src: "/Content/Images/board/cardWood.png", data: null, hitbox: resourceCardHitbox },
    "imgCardWheat": { src: "/Content/Images/board/cardWheat.png", data: null, hitbox: resourceCardHitbox },
    "imgCardSheep": { src: "/Content/Images/board/cardSheep.png", data: null, hitbox: resourceCardHitbox },
    "imgCardOre": { src: "/Content/Images/board/cardOre.png", data: null, hitbox: resourceCardHitbox },
    "imgCardBrick": { src: "/Content/Images/board/cardBrick.png", data: null, hitbox: resourceCardHitbox },
    // Small resource icons
    "imgIconWood": { src: "/Content/Images/site/icon_wood.png", data: null, hitbox: woodIconHitbox },
    "imgIconBrick": { src: "/Content/Images/site/icon_brick.png", data: null, hitbox: brickIconHitbox },
    "imgIconWheat": { src: "/Content/Images/site/icon_wheat.png", data: null, hitbox: wheatIconHitbox },
    "imgIconSheep": { src: "/Content/Images/site/icon_sheep.png", data: null, hitbox: sheepIconHitbox },
    "imgIconOre": { src: "/Content/Images/site/icon_ore.png", data: null, hitbox: oreIconHitbox },
    "imgIconQuestion": { src: "/Content/Images/site/icon_question.png", data: null, hitbox: questionIconHitbox }
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

var ResourceNameToTypeMap = {
    "None": 0,
    "Brick": 1,
    "Wood": 2,
    "Wheat": 3,
    "Sheep": 4,
    "Ore": 5
}

var ResourceTypeToNameMap = {
    0: "None",
    1: "Brick",
    2: "Wood",
    3: "Wheat",
    4: "Sheep",
    5: "Ore"
}

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
    PlacingSettlement: 7,
    PlacingCity: 8,
    RequestingPlayerTrade: 9,
    MonopolySelectingResource: 10,
    RoadBuildingSelectingRoads: 11,
    YearOfPlentySelectingResources: 12
};

var PlayerTurnStateText = {
    0: "",  // None
    1: "Waiting for player to roll the dice...", // NeedToRoll
    2: "Players are selecing cards to lose.", // AnyPlayerSelectingCardsToLose
    3: "Player is moving the robber.", // PlacingRobber
    4: "Player is choosing someone to rob.", // SelectingPlayerToStealFrom
    5: "Player is thinking...", // TakeAction
    6: "Player is placing a road.", // PlacingRoad
    7: "Player is building a settlement.", // PlacingSettlement
    8: "Player is building a city.", // PlacingCity
    9: "Player is requesting a trade.", // RequestingPlayerTrade
    10: "Player played the Monopoly card.", // MonopolySelectingResource
    11: "Player played the Road Building card.", // RoadBuildingSelectingRoads
    12: "Player played the Year of Plenty card." // YearOfPlentySelectingResources
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
    Yellow: 3
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

var ChatTextType = {
    User: "chat-text-user",
    Info: "chat-text-info",
    Warning: "chat-text-warning",
    Danger: "chat-text-danger"
};