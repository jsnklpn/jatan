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

    // Start the connection.
    $.connection.hub.start().done(function () {

        $('#chatBoxInputText').keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                var msgToSend = $('#chatBoxInputText').val();
                if (msgToSend.length > 0) {
                    gameHub.server.sendChatMessage('Jason', msgToSend);
                    $('#chatBoxInputText').val('').focus();
                }
            }
        });

    });
});