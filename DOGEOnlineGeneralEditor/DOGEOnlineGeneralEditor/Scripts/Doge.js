// Document ready without jquery
document.addEventListener("DOMContentLoaded", function (event) {

    //----------------_Layout----------------//
    $('.script').css('display', 'block');
    $('.noscript').css('display', 'none');

    //----------------ToolTips----------------//
    $('[data-toggle="tooltip"]').tooltip();
});