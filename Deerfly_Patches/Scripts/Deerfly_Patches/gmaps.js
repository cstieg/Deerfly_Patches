var gmaps = {
    init: function () {
        this.retailerMap;
        this.defaultMapLocation = new LatLng(43, -86);
        this.displayMap();
        this.setMapToCurrentLocation();
    },

    displayMap: function (location = this.defaultMapLocation) {
        var mapElement = $('#retailer-map-view')[0];
        this.retailerMap = new google.maps.Map(mapElement, {
            zoom: 15,
            center: location
        });
    },

    setMapToCurrentLocation: function (map = this.retailerMap) {
        $.getJSON('http://freegeoip.net/json/', function (data) {
            debugger;
            map.setCenter(new google.maps.LatLng(data.latitude, data.longitude));
        });
    },

    getCurrentLocation: function() {
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
};

function initialMap() {
    gmaps.init();
}

class LatLng {
    constructor(lat, lng) {
        this.lat = lat;
        this.lng = lng;
    }
}