$.connection.hub.logging = true;
var fileProxy = $.connection.fileHub;
var editor = ace.edit("editor");
var workingRow = 0;


// Recieves signal from the server and updates the client file
fileProxy.client.updateFile = function (value, row, column) {
    if (value) {
        var position = {
            row: row,
            column: column
        };

        var endPosition = editor.session.insert(position, value);


    }
}

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

var newLine = function (oldRow, newRow) {
    return (oldRow != newRow);
}

//Start the connection to the server
$.connection.hub.start().done(function () {
    // Temp group id = 1, will be file ID later on.
    var group = "1";
    fileProxy.server.addToGroup(group);

    var sendData = function (session) {
        var value = session.lines[0];
        var row = session.start.row;
        var column = session.start.column;
        fileProxy.server.broadcastFileToGroup(group, value, row, column);

    }
    
    editor.getSession().on('change', function (session) {
        var currentRow = session.start.row;
        console.log("Starting Row:" + currentRow);
        console.log(editor.getSession());
        //console.log(session);

        //keypress
        if ($("#editor").one("keyup", function (e) { // To make sure we only take changes when key is pressed
            //console.log(String.fromCharCode(e.which));
            if (session.action == "insert") {
                if (newLine) {

                }
                sendData(session);
                console.log("enter pressed");
            }
            else if (session.action == "remove") // TODO
            {
                console.log("remove...");
            }
            else {
                console.log("Not insert nor remove");
            }
        }));

        console.log("RESET");
        //EditSession.reset();
        //EditSession.resetCaches()

        console.log("ending row: " + session.start.row);
        
    });
    

});// hub starts ends

