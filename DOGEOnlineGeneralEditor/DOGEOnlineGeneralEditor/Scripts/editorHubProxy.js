$.connection.hub.logging = true;
var fileProxy = $.connection.fileHub;
var editor = ace.edit("editor");
var silent = false;
var userNamesMarkerID = [];
var markerColors = [];



/*
    Client functions
*/
// Recieves signal from the server and updates the client file
fileProxy.client.updateClientFile = function (userWhoChanged, changedData) {
    silent = true;
    editor.getSession().getDocument().applyDelta(changedData);
    markChanges(userWhoChanged, changedData);
    silent = false;
};

/*
    Client functions End
*/
var markChanges = function (user, object) {
    var Range = ace.require('ace/range').Range;
    var range = new Range(object.start.row, object.start.column + 1, object.start.row, object.start.column + 50);
    var userMarkerColor = "";

    //Set the cursor content as the username
    document.styleSheets[0].addRule('.editor-marker:before', 'content: "' + user + '";');

    setTimeout(function () {

        var firstTimeUser = true;
        // Remove marker if it is there already
        for (var i = 0; i < userNamesMarkerID.length; i++) {
            if (userNamesMarkerID[i].user == user) {
                userMarkerColor = userNamesMarkerID[i].markerColor;
                editor.session.removeMarker(userNamesMarkerID[i].markerID);
                userNamesMarkerID.remove(i);
                firstTimeUser = false;
            }
        }

        //Initilizes colors for marker and chooses random number for that user session, from 0 to array length
        if (firstTimeUser) {
            initMarkerColors(0, markerColors.length - 1);
            userMarkerColor = markerColors[randomIntFromInterval(0, markerColors.length - 1)];
            // Core of the application
            if (user == "Frida") {
                userMarkerColor = "#FF1493";
            }
        }

        //Change the background color of the user marker
        document.styleSheets[0].addRule('.editor-marker', 'background: ' + userMarkerColor + ' ');
        document.styleSheets[0].addRule('.editor-marker:before', 'background: ' + userMarkerColor + '');

        var marker = editor.session.addMarker(range, "editor-marker", true);

        userNamesMarkerID.push({ user: user, markerID: marker, markerColor: userMarkerColor });
    }, 100);
};

var saveFile = function () {
    console.log("saving file...");
    $('#saveBtn').click();
};

var initMarkerColors = function () {

    markerColors[0] = "yellow";
    markerColors[1] = "red";
    markerColors[2] = "blue";
    markerColors[3] = "pink";
    markerColors[4] = "gray";
    markerColors[5] = "green";
    markerColors[6] = "#FF1493";
};

$("form").on("submit", function () {
    var form = $(this);
    $("#hiddenEditor").val(editor.getSession().getValue());
    $.ajax({
        method: "POST",
        url: form.attr("action"),
        data: form.serialize(),
        dataType: 'json',
        success: function (response) {
            console.log(response);
            if (response.success === false) {
                alert(response.responseText);
            }
        }
    });
    return false;
});

// Array Remove - By John Resig (MIT Licensed)
Array.prototype.remove = function (from, to) {
    var rest = this.slice((to || from) + 1 || this.length);
    this.length = from < 0 ? this.length + from : from;
    return this.push.apply(this, rest);
};

function randomIntFromInterval(min, max) {
    return Math.floor(Math.random() * (max - min + 1) + min);
}
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

        //setTimeout(function () {
            saveFile();
        //}, 3000);
        


        // Send the groupID, username of user making the changets and the object it self
        fileProxy.server.broadcastFileToGroup(group, userName, object);      
    });

    //Change currentposition if mouse curser is moved
    editor.getSession().selection.on('changeCursor', function () {
        $('#saveTextArea').text(editor.getSession().getValue());
    });
});// hub starts ends