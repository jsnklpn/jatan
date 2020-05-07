var _gameHref = "";

$(function () {
    // Enable popovers
    $("[data-toggle='popover']").popover();

    // Clear password textbox and give it focus when modal opens
    $("#passwordModal").on("shown.bs.modal", function () {
        $("#tbPassword").val("");
        $("#tbPassword").focus();
    });

    // Save the url to the game we want to join
    $(".enter-password-button").click(function () {
        var gameHref = $(this).siblings(".join-href").val();
        _gameHref = gameHref;
        $("#passwordModal").modal("show");
    });

    $("#tbPassword").keypress(function (event) {
        if (event.which == 13) { // Enter pressed
            event.preventDefault();
            btnSubmitPassword_click();
        }
    });

    $("#btnSubmitPassword").click(btnSubmitPassword_click);
});

function btnSubmitPassword_click() {
    var enteredPassword = $("#tbPassword").val();
    var ref = _gameHref.replace("PASSWORD", encodeURIComponent(enteredPassword));

    // submit password
    window.location.href = ref;
}