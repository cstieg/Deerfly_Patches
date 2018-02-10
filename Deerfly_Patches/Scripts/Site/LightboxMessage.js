/* LightboxMessage.js */
// Dims the windows and displays a message in a light transparent box
// Usage: 
/*
    var lightbox = new LightboxMessage('Please wait...');
    lightbox.display();
    ...
    lightbox.destroy();

*/

// REQUIRES: RandomID.js, LightboxMessage.css
var LightboxMessage = function (message) {
    "use strict";
    this.message = message || '';
    this.containerId = 'lightbox' + randomId(4);
    
    this.$lightbox = $('<div id="' + this.containerId + '" class="lightbox">' +
                            '<span class="lightbox-message">' + this.message + '</span>' +
                       '</div>');

    // Displays the lightbox in the body element
    this.display = function () {
        $('body').append(this.$lightbox);  
    };

    // Changes the message in the lightbox
    this.setMessage = function (message) {
        this.message = message;
        this.$lightbox.find('.lightbox-message').text(this.message);
    };

    // Removes the lightbox element, cancelling the lightbox display
    this.destroy = function () {
        $("#" + this.containerId).remove();
    };
     
};