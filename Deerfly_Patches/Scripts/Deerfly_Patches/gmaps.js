function initialMap() {
    var mapElement = $('#retailer-map-view')[0];
    retailerMap = new GMap(mapElement);
}

class GMap {
    constructor(mapElement, location = null, zoom = 10) {
        this._mapElement = mapElement;
        this._location = location;
        if (location === null) {
            this._location = new LatLng(43, -86);
        }
        this._zoom = zoom;

        this._map = new google.maps.Map(this._mapElement, {
            zoom: this._zoom,
            center: this._location
        });

        if (location === null) {
            this.setMapToCurrentLocation();
        }
    }

    setMapToCurrentLocation(map = this._map) {
        $.getJSON('http://freegeoip.net/json/', function (data) {
            map.setCenter(new google.maps.LatLng(data.latitude, data.longitude));
        });
    }
}

class LatLng {
    constructor(lat, lng) {
        this.lat = lat;
        this.lng = lng;
    }
}