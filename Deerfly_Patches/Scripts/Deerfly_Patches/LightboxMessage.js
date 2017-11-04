/* LightboxMessage.js */

function showLightboxMessage(message, containerId = "lightbox") {
    var $lightbox = $(
        `<div id="${containerId}" 
            style="
                position: fixed; 
                top: 0; 
                left: 0; 
                width: 100%; 
                height: 100%; 
                text-align: center; 
                background-color: black; 
                filter: alpha(opacity=50); 
                opacity: 0.8;
                z-index: 100;">
            <span style="
                padding: 30px;
                border: gray outset 3px;
                border-radius: 14px;
                color: black;
                background-color: white;
                position: fixed;
                top: 30%;
                left: 30%;
                width: 40vw;
                min-height: 20vh;
                max-height: 50vh;
                overflow: auto;
                z-index: 1000;
                font-size: 18pt;">
                ${message}
            </span>
        </div>`);
    $('body').append($lightbox);
}

function hideLightboxMessage(containerId) {
    $("#" + containerId).remove();
}
