$.connection.hub.logging = true;
var fileProxy = $.connection.fileHub;
var editor = ace.edit("editor");
var silent = false;

/*
    Client functions
*/
// Recieves signal from the server and updates the client file
fileProxy.client.updateClientFile = function (changedData) {
    silent = true;
    editor.getSession().getDocument().applyDelta(changedData);
    silent = false;
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
        fileProxy.server.broadcastFileToGroup(group, session);

    }
    var sendNewLine = function (currentPos) {
        fileProxy.server.broadcastNewLineToGroup(group, currentPos.row, currentPos.column);
    }

    editor.getSession().on('change', function (object) {
        if (silent) {
            return;
        }
        sendData(object);        
    });

    //Change currentposition if mouse curser is moved
    editor.getSession().selection.on('changeCursor', function () {
        $('#saveTextArea').text(editor.getSession().getValue());
    });
});// hub starts ends