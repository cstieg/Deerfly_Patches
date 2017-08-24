var retailerMap = {
    init: function () {
        var mapElement = $('#retailer-map-view')[0];
        this.map = new GMap(mapElement);
        this.getRetailers();
    },

    getRetailers: function () {
        var self = this;
        var $retailers = $('.retailer-item');
        for (var i = 0; i < $retailers.length; i++) {
            $retailer = $($retailers[i]);
            var latlng = JSON.parse($retailer.find('.retailer-latlng').text());
            var info = $retailer.html();
            debugger;
            var gMarker = this.map.addMarker(latlng, info);
            $retailer.bind('mouseover', function (event) {
                self.map.showInfoWindow(gMarker);
            });
        }
    },

};

function initialMap() {
    retailerMap.init();
}

function loadGMapsError() {
    alert("Wasn't able to load map :(");
}


class GMap {
    constructor(mapElement, location = null, zoom = 5) {
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
        this._infoWindow = new google.maps.InfoWindow();

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

    setMapToZip(zip, map = this._map) {

    }

    addMarker(location, content) {
        var newMarker = new google.maps.Marker({
            position: new google.maps.LatLng(location),
            address: '',
            placeName: '',
            content: content,
            parentClass: this
        });

        google.maps.event.addListener(newMarker, 'mouseover', function (event) {
            // parentClass is included in the marker object as a hack to get a reference back to the class
            newMarker.parentClass.showInfoWindow(newMarker);
        });
        newMarker.setMap(this._map);
        this._markers.push(newMarker);

        return newMarker;
    }

    showInfoWindow(marker) {
        marker.setAnimation(google.maps.Animation.BOUNCE);
        marker.setAnimation(null);
        this._infoWindow.setContent(marker.content);
        this._infoWindow.open(this._map, marker);
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

