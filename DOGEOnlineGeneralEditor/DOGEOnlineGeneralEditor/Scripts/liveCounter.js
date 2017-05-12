
document.addEventListener("DOMContentLoaded", function (event) {
    $.connection.hub.start();
    $.connection.hub.logging = false;
    var generalProxy = $.connection.generalHub;

    generalProxy.client.updateVisitorCounter = function (newCounter) {
        $('#visitor-counter').text(newCounter);
    };
}); // document ready ends