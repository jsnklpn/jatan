// Taken from https://github.com/daneden/animate.css
// Usage:
//   $("#yourElement").animateCss("bounce");
//
$.fn.extend({
    animateCss: function (animationName) {
        var animationEnd = "webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend";
        this.addClass("animated " + animationName).one(animationEnd, function () {
            $(this).removeClass("animated " + animationName);
        });
    }
});