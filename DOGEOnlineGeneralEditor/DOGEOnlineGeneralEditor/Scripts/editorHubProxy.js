
var broadcastChanges = function (editor, signal)
{
    /*
    var currPos = editor.getCursorPosition();
    editor.session.insert(currPos, signal);

    console.log("Row: " + row);
    console.log("Column: " + column);
    console.log("Text: " + text);
    */
    //{ row, column }
    //editor.insert(editor.getCursorPosition(), "blabla");
}

$(function () {
    //Enable logging
    //$.connection.hub.logging = true;

    // Create a proxy to the file hub
    //var fileProxy = $.connection.fileHub; here

    // Create the editor
    var editor = ace.edit("editor");
    editor.setTheme("ace/theme/monokai");
    editor.getSession().setMode("ace/mode/javascript");

    //row, text, groupID
    /*fileProxy.client.updateFile = function (editor, groupID) { here

    }*/

    // Start the connection.
    // $.connection.hub.
    /*$.connection.hub.start().done(function () { here
        console.log("started...");*/
        /*
        $('#sendmessage').click(function () {
            // Call the Send method on the hub.
            chat.server.send($('#displayname').val(), $('#message').val());
            // Clear text box and reset focus for next comment.
            $('#message').val('').focus();
        });
        */


    //}); here

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
        var activeLine = e.start.row;

        if ($("#editor").keyup(function (){ // To make sure we only take changes when key is pressed space/enter
            if (e.action == "insert") {
                //broadcastChanges(editor, e);
                var lineNumber = e.start.row;
                var currPos = editor.getCursorPosition();
                var customPos = { row: lineNumber, column: 0 };
                var currentLine = e.getSelectionRange;
                /*
                while (activeLine < (e.end.row + 1)) {
                    editor.session.removeGutterDecoration(activeLine, )
                }
                */
                editor.session.insert(customPos, e.lines[0]);


            }
            else if (e.action == "remove") // TODO
            {

            }
        }));

        console.log(e);
    });
        

});