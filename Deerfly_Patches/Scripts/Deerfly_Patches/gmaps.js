var retailerMap = {
    init: function () {
        var mapElement = $('#retailer-map-view')[0];
        this.map = new GMap(mapElement);
        this.retailers = [];
        this.getRetailers();
    },

    getRetailers: function () {
        var self = this;
        var url = $('#retailerJsonUrl').text();
        $.getJSON(url, function (data, result) {
            for (var i = 0; i < data.length; i++) {
                var latlng = LatLng.latlng(data[i].LatLng);
                var gMarker = self.map.addMarker(latlng, "");


                // Todo: add data to gmarker

                self.retailers.push(gMarker);
            }
        });
    }
};

function initialMap() {
    retailerMap.init();
}

function loadGMapsError() {
    alert("Wasn't able to load map :(");
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

        this._markers = [];

        if (location === null) {
            this.setMapToCurrentLocation();
        }
    }

    setMapToCurrentLocation(map = this._map) {
        $.getJSON('http://freegeoip.net/json/', function (data) {
            map.setCenter(new google.maps.LatLng(data.latitude, data.longitude));
        });
    }

    addMarker(location, content) {
        var newMarker = new google.maps.Marker({
            position: location,
            address: '',
            placeName: ''
        });
        newMarker.setMap(this._map);
        return newMarker;
    }
}


class LatLng {
    constructor(lat, lng) {
        this.lat = lat;
        this.lng = lng;
    }

    static latlng(obj) {
        return new LatLng(obj.Lat, obj.Lng);
    }
}

