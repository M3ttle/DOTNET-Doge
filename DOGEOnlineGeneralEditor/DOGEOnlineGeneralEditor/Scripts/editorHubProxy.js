$.connection.hub.logging = true;
var fileProxy = $.connection.fileHub;
var editor = ace.edit("editor");


// Recieves signal from the server and updates the client file
fileProxy.client.updateFile = function (value, row, column) {
    if (value) {
        var position = {
            row: row,
            column: column
        };

        editor.session.insert(position, value);
    }
}

//Start the connection to the server
$.connection.hub.start().done(function () {
    // Temp group id = 1, will be file ID later on.
    var group = "1";
    fileProxy.server.addToGroup(group);

    
    editor.getSession().on('change', function (session) {
        if ($("#editor").one("keydown", function () { // To make sure we only take changes when key is pressed
            if (session.action == "insert") {
                var value = session.lines[0];
                var row = session.start.row;
                var column = session.start.column;
                fileProxy.server.broadcastFileToGroup(group, value, row, column);
            }
            else if (session.action == "remove") // TODO
            {

            }
        }));
    });
    

});// hub starts ends

