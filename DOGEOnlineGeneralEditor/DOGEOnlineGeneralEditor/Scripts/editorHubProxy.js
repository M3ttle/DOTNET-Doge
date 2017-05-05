
var broadcastChanges = function (editor, signal)
{
    //console.log($('#saveTextArea').text(editor.getSession().getValue()));
    // console.log("Pos:");
    /*console.log("column: " + e.start.column);
    console.log("row: " + e.start.row);
    console.log();
    console.log("Value: " + e.lines);
    console.log("action: " + e.action); //if action == insert, remove
    console.log(e);
    */
    var row = signal.start.row;
    var column = signal.start.column;
    var text = signal.lines;

    console.log("Row: " + row);
    console.log("Column: " + column);
    console.log("Text: " + text);
    //{ row, column }
    editor.insert(editor.getCursorPosition(), "blabla");
}

$(function () {
    // Create the editor
    var editor = ace.edit("editor");
    editor.setTheme("ace/theme/monokai");
    editor.getSession().setMode("ace/mode/javascript");

    // Get text from the editor when save is pressed
    $('#save').on('click', function (e) {
        $('#saveTextArea').text(editor.getSession().getValue());
        return false;
    });

    // Get text when cursor changes
    editor.getSession().selection.on('changeCursor', function (e) {
        $('#saveTextArea').text(editor.getSession().getValue());
    });


    $("#mode").change(function () {
        var mode = $("#mode").val();
        editor.getSession().setMode(mode);
    });

    $("#theme").change(function () {
        var theme = $("#theme").val();
        editor.setTheme(theme);
    });

    $('#savefile').on('click', function (e) {
        return false;
    });

    editor.getSession().on('change', function (e) {
        broadcastChanges(editor, e);
    });


});

    /*
    console.log("Pos:");
    console.log("column: " + e.start.column);
    console.log("row: " + e.start.row);
    console.log();
    console.log("Value: " + e.lines);
    console.log("action: " + e.action); //if action == insert, remove
    */


// 1 , 2 , 3, 
//Fá cursor pos, hverju var bætt við, group ID, 