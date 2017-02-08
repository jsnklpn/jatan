// Taken from https://github.com/daneden/animate.css
// Usage:
//   $("#yourElement").animateOnce("bounce");
//
$.fn.extend({
    animateOnce: function (animationName) {
        var animationEnd = "webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend";
        this.addClass("animated " + animationName).one(animationEnd, function () {
            $(this).removeClass("animated " + animationName);
        });
        return this;
    },
    animateInfinite: function (animationName) {
        return this.addClass("animated infinite " + animationName);
    },
    animateStop: function (animationName) {
        return this.removeClass("animated infinite " + animationName);
    },
    showWithAnimation: function (animationName) {
        if (!this.hasClass("hidden")) {
            // Already showing. Don't need to animate again.
            return this;
        }
        $(this).removeClass("hidden");
        var animationEnd = "webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend";
        this.addClass("animated " + animationName).one(animationEnd, function () {
            $(this).removeClass("animated " + animationName);
        });
        return this;
    },
    hideWithAnimation: function (animationName) {
        if (this.hasClass("hidden")) {
            // Already hidden. Don't need to animate again.
            return this;
        }
        var animationEnd = "webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend";
        this.addClass("animated " + animationName).one(animationEnd, function () {
            $(this).removeClass("animated " + animationName);
            $(this).addClass("hidden");
        });
        return this;
    }
});
