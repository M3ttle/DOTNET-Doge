$.connection.hub.logging = true;
var fileProxy = $.connection.fileHub;
var editor = ace.edit("editor");
var silent = false;
var userNamesMarkerID = [];

/*
    Client functions
*/
// Recieves signal from the server and updates the client file
fileProxy.client.updateClientFile = function (userWhoChanged, changedData) {
    silent = true;
    editor.getSession().getDocument().applyDelta(changedData);
    markChanges(userWhoChanged, changedData);
    silent = false;
}

/*
    Client functions End
*/
var markChanges = function (user, object) {
    var Range = ace.require('ace/range').Range;
    var range = new Range(object.start.row, object.start.column +1, object.start.row, object.start.column + 50);

    //Set the cursor content as the username
    document.styleSheets[0].addRule('.editorMarker:before', 'content: "' + user + '";');

    setTimeout(function () {

        // Remove marker if it is there already
        for (var i = 0; i < userNamesMarkerID.length; i++) {
            if (userNamesMarkerID[i].user == user) {
                editor.session.removeMarker(userNamesMarkerID[i].markerID);
                userNamesMarkerID.remove(i);
            }
        }

        var marker = editor.session.addMarker(range, "editorMarker", true);
        userNamesMarkerID.push({user: user, markerID: marker});
    }, 100);
}

// Use ajax here to save the file
var saveFile = function () {

}

// Array Remove - By John Resig (MIT Licensed)
Array.prototype.remove = function (from, to) {
    var rest = this.slice((to || from) + 1 || this.length);
    this.length = from < 0 ? this.length + from : from;
    return this.push.apply(this, rest);
};
/*
    Server Functions
*/
//Start the connection to the server
$.connection.hub.start().done(function () {
    // Group for the hub is the file ID
    var group = $("#fileID").val();
    var userName = $("#userNameLogged").val();
    fileProxy.server.addToGroup(group);

    editor.getSession().on('change', function (object) {
        if (silent) {
            return;
        }
        // Save the file
        saveFile();
        // Send the groupID, username of user making the changets and the object it self
        fileProxy.server.broadcastFileToGroup(group, userName, object);      
    });

    //Change currentposition if mouse curser is moved
    editor.getSession().selection.on('changeCursor', function () {
        $('#saveTextArea').text(editor.getSession().getValue());
    });
});// hub starts ends