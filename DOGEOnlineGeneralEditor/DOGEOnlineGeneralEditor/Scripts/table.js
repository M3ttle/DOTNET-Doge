
//----------------Search-My-projects----------------//
var $mrows = $('#my-projects-table tbody tr');
$('#my-search').keyup(function () {
    var val = $.trim($(this).val()).replace(/ +/g, ' ').toLowerCase();

    $mrows.show().filter(function () {
        var text = $(this).children(":first").text().replace(/\s+/g, ' ').toLowerCase(); //Only name
        return ~text.indexOf(val) === 0;
    }).hide();
});

//----------------Search-Collaboration-projects----------------//
var $crows = $('#collaboration-projects-table tbody tr');
$("#collaboration-search").keyup(function () {
    $crows = $('#collaboration-projects-table tbody tr');
    var val = $.trim($(this).val()).replace(/ +/g, ' ').toLowerCase();

    $crows.show().filter(function () {
        var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
        return ~text.indexOf(val) === 0;
    }).hide();
});

//----------------Search-Public-projects----------------//
var $prows = $('#public-projects-table tbody tr');
$("#public-search").keyup(function () {
    var val = $.trim($(this).val()).replace(/ +/g, ' ').toLowerCase();

    $prows.show().filter(function () {
        var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
        return ~text.indexOf(val) === 0;
    }).hide();
});

//----------------Search-Files----------------//
var $frows = $('#files-table tbody tr');
$("#files-search").keyup(function () {
    var val = $.trim($(this).val()).replace(/ +/g, ' ').toLowerCase();

    $frows.show().filter(function () {
        var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
        return ~text.indexOf(val) === 0;
    }).hide();
});

//----------------Search-Users----------------//
var $urows = $('#users-table tbody tr');
$("#users-search").keyup(function () {
    var val = $.trim($(this).val()).replace(/ +/g, ' ').toLowerCase();

    $urows.show().filter(function () {
        var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
        return ~text.indexOf(val) === 0;
    }).hide();
});

//----------------Search-Text-Box----------------//

//My-Projects
$("#glyphicon-my-search").on('click', function (e) {
    $('#my-search').attr('data-default', $(this).width());
    $('#my-search').addClass("table-search-bar-open");
    $('#my-search').animate({ width: 500 }, 'slow');
    $('#my-search').focus();
});

//Collaboration-Projects
$("#glyphicon-collaboration-search").on('click', function (e) {
    $('#collaboration-search').attr('data-default', $(this).width());
    $('#collaboration-search').addClass("table-search-bar-open");
    $('#collaboration-search').animate({ width: 500 }, 'slow');
    $('#collaboration-search').focus();
});

//Public-Projects
$("#glyphicon-public-search").on('click', function (e) {
    $('#public-search').attr('data-default', $(this).width());
    $('#public-search').addClass("table-search-bar-open");
    $('#public-search').animate({ width: 500 }, 'slow');
    $('#public-search').focus();
});

//Files
$("#glyphicon-files-search").on('click', function (e) {
    $('#files-search').attr('data-default', $(this).width());
    $('#files-search').addClass("table-search-bar-open");
    $('#files-search').animate({ width: 500 }, 'slow');
    $('#files-search').focus();
});

//Users
$("#glyphicon-users-search").on('click', function (e) {
    $('#users-search').attr('data-default', $(this).width());
    $('#users-search').addClass("table-search-bar-open");
    $('#users-search').animate({ width: 500 }, 'slow');
    $('#users-search').focus();
});

//----------------Delete-Files----------------//
$(".trash-project").on('click', function (e) {
    var ID = $(this).prev().val();
    window.location.href = '/Project/Delete/' + ID;
});