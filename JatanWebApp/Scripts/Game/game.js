$(function () {

    // Declare a proxy to reference the hub.
    var gameHub = $.connection.gameHub;

    // Create a function that the hub can call to broadcast messages.
    gameHub.client.broadcastMessage = function (name, message) {
        var encodedName = $('<div />').text(name).html();
        var encodedMsg = $('<div />').text(message).html();

        $('#chatBoxList').append('<li><strong>' + encodedName + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
        $("#chatBoxList").animate({ scrollTop: $("#chatBoxList")[0].scrollHeight }, 1000);
    };

    gameHub.client.updateGameManager = function (gameManager) {
        writeTextToChat(gameManager.toString());
        drawGame(gameManager);
    }

    // Start the connection.
    $.connection.hub.start().done(function () {

        $('#chatBoxInputText').keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            // Enter key pressed
            if (keycode == '13') {
                var msgToSend = $('#chatBoxInputText').val();
                if (msgToSend.length > 0) {

                    // temp
                    if (msgToSend.toLowerCase() === 'update')
                        gameHub.server.getGameManagerUpdate();
                    else {
                        gameHub.server.sendChatMessage('Jason', msgToSend);
                        $('#chatBoxInputText').val('').focus();
                    }
                }
            }
        });
    });

    var canvas = $('#gameCanvas');
    canvas[0].width = 1000;
    canvas[0].height = 400;
    canvas.click(canvasClick);
    canvas.mousemove(canvasMouseMove);

});

function writeTextToChat(text) {
    $('#chatBoxList').append('<li>' + text + '</li>');
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
    var canvas = $('#gameCanvas')[0];
    var ctx = canvas.getContext("2d");

    var board = gameManager.GameBoard;
}


