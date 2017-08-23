

function initialMap() {
    displayMap();
}

function displayMap() {
    var location = getCurrentLocation();
    if (!location) { return; }
    var mapElement = $('#retailer-map-view')[0];
    var map = new google.maps.Map(mapElement, {
        zoom: 15,
        center: location
    });
}

function getCurrentLocation() {
    
    var data = $.ajax({
        url: 'http://freegeoip.net/json/',
        dataType: 'json',
        async: false,
        error: function (err) {
            debugger;
            alert("Couldn't determine current location!");
        }
    });

    var response = data.responseJSON;
    return new LatLng(response.latitude, response.longitude);
}


class LatLng {
    constructor(lat, lng) {
        this.lat = lat;
        this.lng = lng;
    }
}