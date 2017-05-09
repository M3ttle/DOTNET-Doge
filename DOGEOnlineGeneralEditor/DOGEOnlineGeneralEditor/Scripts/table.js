

//----------------public-projects----------------//

$("#public-projects tbody tr").click(function () {
    $('#public-projects tr').removeClass('highlighted');
    $(this).addClass('highlighted');
});

$(".open-button").on('click', function (e) {
    var ID = $('.highlighted td:nth-child(5)').html();
    if (ID) {
        window.location.href = '/Project/Details/' + ID;
    }
});

var $rows = $('#public-projects tbody tr');
$('#public-search').keyup(function () {
    var val = $.trim($(this).val()).replace(/ +/g, ' ').toLowerCase();

    $rows.show().filter(function () {
        var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
        return !~text.indexOf(val);
    }).hide();
});


//----------------my-projects----------------//

$("#my-projects tbody tr").click(function () {
    $('tr').removeClass('highlighted');
$(this).addClass('highlighted');
});

$("#shared-projects tbody tr").click(function () {
    $('tr').removeClass('highlighted');
$(this).addClass('highlighted');
});

$(".open-button").on('click', function (e) {
    var ID = $('#my-projects .highlighted td:nth-child(5)').html();
    if (ID) {
    window.location.href = '/Project/Details/' + ID;
}
});

$(".delete-button").on('click', function (e) {
    var ID = $('#my-projects .highlighted td:nth-child(5)').html();
    if (ID) {
    window.location.href = '/Project/Delete/' + ID;
}
});

$(".leave-button").on('click', function (e) {
    var ID = $('#shared-projects .highlighted td:nth-child(5)').html();
    if (ID) {
    $('#projectID').val(ID);
document.getElementById('leaveProjectSubmit').click();
    }
});

var $srows = $('#shared-projects tbody tr');
var $mrows = $('#my-projects tbody tr');
$('#my-search').keyup(function () {
    var val = $.trim($(this).val()).replace(/ +/g, ' ').toLowerCase();

    $srows.show().filter(function () {
        var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
        return !~text.indexOf(val);
    }).hide();

    $mrows.show().filter(function () {
        var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
        return !~text.indexOf(val);
    }).hide();
});
