$.connection.hub.logging = true;
var fileProxy = $.connection.fileHub;
var editor = ace.edit("editor");
var currentPosition = { row: 0, column: 0 };
var clickTimerValid = true; // Used to delay the keyup on the editor (Prevent double clicks)

/*
    Client functions
*/
// Recieves signal from the server and updates the client file
fileProxy.client.updateClientFile = function (value, row, column) {
    if (value) {
        var position = {
            row: row,
            column: column
        };
        var endPosition = editor.session.insert(position, value);
    }
}

fileProxy.client.sendClientNewLine = function (currentRow, currentColumn) {
    var currentPos = { row: currentRow, column: currentColumn };
    editor.session.insert(currentPos, "\n");
}

/*
    Client functions End
*/
var highLightMarker = function () {
    //Highlits inserted text for X seconds
    var Range = require("ace/range").Range
    markerId = editor.session.addMarker(
        Range.fromPoints(position, endPosition), "ace_highlight-marker"
    )
    setTimeout(function () {
        session.removeMarker(markerId)
    }, 1000)
}

var enterPressed = function (e) {
    return e.which == 13;
}
var arrowKeyPressed = function (e) {
    return e.which == 37;
    //return (e.which == 37 || e.which == 38 || e.which == 39 || e.which == 40);
}

/*
    Server Functions
*/
//Start the connection to the server
$.connection.hub.start().done(function () {
    // Group for the hub is the file ID
    var group = $("#fileID").val();
    fileProxy.server.addToGroup(group);

    var sendData = function (session) {
        var value = session.lines[0];
        var row = session.start.row;
        var column = session.start.column;
        fileProxy.server.broadcastFileToGroup(group, value, row, column);

    }
    var sendNewLine = function (currentPos) {
        fileProxy.server.broadcastNewLineToGroup(group, currentPos.row, currentPos.column);
    }

    editor.getSession().on('change', function (session) {
        var rowInNow = session.start.row;

        //keypress
        if ($("#editor").one("keyup", function (e) { // To make sure we only take changes when key is pressed
            
            if (session.action == "insert") {
                if (enterPressed(e) && clickTimerValid) {
                    clickTimerValid = false;
                    var currentPos = currentPosition; // Save it to a new variable to prevent it to be changed before we use it
                    sendNewLine(currentPos);
                }
                else {
                    sendData(session);
                }
            }
            else if (session.action == "remove") // TODO
            {
                console.log("remove...");
            }
            else {
                console.log("Not insert nor remove");
            }
            currentPosition = editor.getCursorPosition();
        }));

        //console.log("RESET");
        //EditSession.reset();
        //EditSession.resetCaches()

        //console.log("ending row: " + session.start.row);
        clickTimerValid = true;
        
    });

    //Change currentposition if mouse curser is moved
    editor.getSession().selection.on('changeCursor', function () {
        if ($("#editor").mouseup(function () {
            currentPosition = editor.getCursorPosition();
        }));
        if ($("#editor").on("keyup", function (e) {
            if (arrowKeyPressed(e)) {
                currentPosition = editor.getCursorPosition();
            }
        }));
        
        $('#saveTextArea').text(editor.getSession().getValue());
    });
});// hub starts ends

