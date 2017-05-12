$.connection.hub.logging = false;
var fileProxy = $.connection.fileHub;
var editor = ace.edit("editor");
// Silent mode for the ace editor, to avoid loop when text is inserted from another user
var silent = false;
// Array of objects to compere marker ID, user and color of each user connected to the same file
var userNamesMarkerID = [];
var fileName = $('#Name').val();
// Array of colors fo the cursor
var markerColors = [];
// jquery notification timer so it wont spam the user with saved messages
var jqueryNotificationOn = true;


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

fileProxy.client.updateClientFileName = function (newFileName) {
    fileName = newFileName;
    $('#Name').val(fileName);
};

/*
    Client functions End
*/
var markChanges = function (user, object) {
    var Range = ace.require('ace/range').Range;
    var range = new Range(object.start.row, object.start.column + 1, object.start.row, object.start.column + 50);
    var userMarkerColor = "";

    //Set the cursor content as the username
    addCSSRule(document.styleSheets[0], ".editor-marker:before", "content", '"' + user + '"');

    setTimeout(function () {

        var firstTimeUser = true;
        // Remove marker if it is there already
        for (var i = 0; i < userNamesMarkerID.length; i++) {
            if (userNamesMarkerID[i].user === user) {
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
            // "Core of the application" - Special request from our friend Frida, she wants to have a pink cursor the whole time
            if (user === "Frida") {
                userMarkerColor = "#FF1493";
            }
        }

        //Change the background color of the user marker
        addCSSRule(document.styleSheets[0], ".editor-marker", "background", userMarkerColor);
        addCSSRule(document.styleSheets[0], ".editor-marker:before", "background", userMarkerColor);

        var marker = editor.session.addMarker(range, "editor-marker", true);

        console.log("User is: " + user);
        console.log("Marker ID: " + marker);

        userNamesMarkerID.push({ user: user, markerID: marker, markerColor: userMarkerColor });
    }, 100);
};

var initMarkerColors = function () {

    markerColors[0] = "blueViolet ";
    markerColors[1] = "red";
    markerColors[2] = "blue";
    markerColors[3] = "brown";
    markerColors[4] = "gray";
    markerColors[5] = "cadetBlue ";
    markerColors[6] = "chocolate ";
    markerColors[7] = "crimson ";
    markerColors[8] = "darkBlue";
    markerColors[9] = "darkCyan";
    markerColors[10] = "darkGoldenRod";
    markerColors[11] = "darkGray";
    markerColors[12] = "darkGreen";
    markerColors[13] = "darkMagenta";
    markerColors[14] = "darkOliveGreen";
    markerColors[15] = "darkRed";
    markerColors[16] = "darkSeaGreen";
    markerColors[17] = "darkViolet";
    markerColors[18] = "deepPink";
    markerColors[19] = "forestGreen";
    markerColors[20] = "gold";
    markerColors[21] = "indigo";
    markerColors[22] = "lime";
    markerColors[23] = "midnightBlue";
    markerColors[24] = "olive";
    markerColors[25] = "orangeRed";
    markerColors[26] = "peru";
    markerColors[27] = "purple";
    markerColors[28] = "teal";
};

$("form").on("submit", function () {

    var form = $(this);
    $("#hiddenEditor").val(editor.getSession().getValue());
    $('#editor-notification').children(0).text("Saving...").fadeIn("slow");

    $.ajax({
        method: "POST",
        url: form.attr("action"),
        data: form.serialize(),
        dataType: 'json',
        success: function (response) {
            // If ajax returns success but our test (not valid file name for example) returns false
            if (response.success === true) {
                updateFileName($("#fileID").val());
                
                // Timer for the notification, so it wont spam the user with messages
                if (jqueryNotificationOn) {
                    jqueryNotificationOn = false;
                    setTimeout(function () {
                        $('#editor-notification').children(0).removeClass("editor-error-message");
                        $('#editor-notification').children(0).text(response.responseText).fadeOut("slow");
                        jqueryNotificationOn = true;
                        
                    }, 1500);
                }
            }   
            else { // Not success    
                $('#editor-notification').children(0).addClass("editor-error-message");
                $('#editor-notification').children(0).text(response.responseText).show();
            }
        },
        error: function (response) {
            $('#editor-notification').children(0).addClass("editor-error-message");
            $('#editor-notification').children(0).text("Input contains illegal characters").show();


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

var randomIntFromInterval = function (min, max) {
    return Math.floor(Math.random() * (max - min + 1) + min);
};

// Adds css rules to different browsers
// Firefox does not support other clients changing their css
var addCSSRule = function (sheet, selector, rules, value) {

    if ("addRule" in sheet) { // Non-Firefox Browsers
        sheet.addRule(selector, rules + ": " + value + ";");
    }
    else if ("insertRule" in sheet) { // Firefox - Not in use because firefox doesnt allow other users/domains to change the user css
        // If solution will be found in the future then it should go here
    }
    else {
        console.log("Not a valid sheet");
    }
};

// Returns if filename has been changed
var fileNameChanged = function () {
    return $('#Name').val() !== fileName;
};
/*
    Server Functions
*/
var broadcastChanges = function (group, userName, object) {
    // Send the groupID, username of user making the changets and the object it self
    fileProxy.server.broadcastFileToGroup(group, userName, object);
};
var updateFileName = function (group) {
    if (fileNameChanged()) {
        fileName = $('#Name').val();
        fileProxy.server.broadcastFileNameToGroup(group, fileName);
    }
};
var saveFile = function () {
    // Loading saveBtn click
    $('#saveBtn').click();
};
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
        saveFile();
        broadcastChanges(group, userName, object);
    });

    //Change currentposition if mouse curser is moved
    editor.getSession().selection.on('changeCursor', function () {
        $('#saveTextArea').text(editor.getSession().getValue());
    });
});// hub starts ends