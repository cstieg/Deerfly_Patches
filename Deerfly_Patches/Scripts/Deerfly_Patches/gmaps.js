var retailerMap = {
    init: function () {
        var self = this;
        var mapElement = $('#retailer-map-view')[0];
        this.location = JSON.parse($('#location').text());
        this.zoom = parseInt($('#zoom').text());
        this.map = new GMap(mapElement, this.location, this.zoom);
        this.retailers = [];

        this.map.addPanListener(function () {
            self.updateRetailers(self.map.getBounds());
        });
        // TODO: zoom


        //this.getRetailers();
    },

    updateRetailers: function (mapBounds) {
        var self = this;
        var maxLat = mapBounds.f.f;
        var leftLng = mapBounds.b.b;
        var minLat = mapBounds.f.b;
        var rightLng = mapBounds.b.f;
        $.getJSON('/retailermap/updateretailerjson' +
            '?maxlat=' + maxLat + 
            '&leftlng=' + leftLng +
            '&minlat=' + minLat +
            '&rightlng=' + rightLng, function(result) {
                self.retailers = result;
                self.showRetailers();
        });
    },

    getRetailers: function () {
        var self = this;
        var $retailers = $('.retailer-item');
        if ($retailers.length == 0) {
            $('#retailer-list').html('<p>Sorry, no retailers in this area. You may <a href="/order">order here.</a></p>');
        }

        for (var i = 0; i < $retailers.length; i++) {
            $retailer = $($retailers[i]);
            var latlng = JSON.parse($retailer.find('.retailer-latlng').text());
            var info = $retailer.html();

            var gMarker = this.map.addMarker(latlng, info);

            // calculate distance from location
            gMarker.distance = distance(latlng, this.location);
            $retailer.find('.retailer-distance').text(gMarker.distance.toFixed(1) + ' miles from your location');

            $retailer.bind('mouseover', function (event) {
                self.map.showInfoWindow(gMarker);
            });
        }

        this.sortRetailersByDistance();
    },

    showRetailers: function() {
        var self = this;
        $retailerList = $('#retailer-list');
        $retailerList.html('');
        if (this.retailers.length == 0) {
            $('#retailer-list').html('<p>Sorry, no retailers in this area. You may <a href="/order">order here.</a></p>');
        }
        for (var i = 0; i < this.retailers.length; i++) {
            var retailer = this.retailers[i];
            retailer.LatLng.lat = retailer.LatLng.Lat;
            retailer.LatLng.lng = retailer.LatLng.Lng;
            retailer.distance = distance(retailer.LatLng, this.location);

            var itemHTML = this.getRetailerListItem(retailer);

            var gMarker = this.map.addMarker(retailer.LatLng, itemHTML.html());

            // calculate distance from location
            gMarker.distance = retailer.distance;
            itemHTML.bind('mouseenter', { gM: gMarker }, function(event) {
                self.map.showInfoWindow(event.data.gM);
            });
            gMarker = null;
            $retailerList.append(itemHTML);
        }
    },

    getRetailerListItem: function (retailer) {
        $retailerItemTemplate = $('#retailer-item-li-template .retailer-item-li');
        // clear out data from any previous iteration
        $('#retailer-item-li-template .retailer-item').children().text('');
        $retailerItemTemplate.find('.retailer-name').text(retailer.Name);
        $retailerItemTemplate.find('.retailer-distance').text(retailer.distance.toPrecision(3) + ' miles from your location');
        $retailerItemTemplate.find('.retailer-address').html(retailer.Address.Address1 + '<br />' +
            retailer.Address.City + ', ' + retailer.Address.State + '<br />');
        if (retailer.Address.Phone) {
            $retailerItemTemplate.find('.retailer-phone').html(retailer.Address.Phone + '<br />');
        }
        if (retailer.Website) {
            $retailerItemTemplate.find('.retailer-website')
                .text(retailer.Website)
                .prop('href', retailer.Website);
        }
        return $retailerItemTemplate.clone();
    },

    sortRetailersByDistance() {
        var $retailers = $('.retailer-item');
        var $retailerList = $retailers.find('.retailer-item-li');

        $retailerList.detach().sort(function (a, b) {
            var aDistance = parseInt($(a).find('.retailer-distance').text());
            var bDistance = parseInt($(b).find('.retailer-distance').text());
            return (aDistance > bDistance) ? (aDistance < bDistance) ? 1 : 0 : -1;
        });
        $retailers.append($retailerList);
    }

};

function initialMap() {
    retailerMap.init();

    /*
    if (navigator.geolocation) {
        var options = { timeout: 10000 };
        navigator.geolocation.getCurrentPosition(function (pos) {
            $.post("/retailermap/setuserlocation?lat=" + pos.coords.latitude + "&lng=" + pos.coords.longitude, function () {
                debugger;

            });
        },
        function (err) {
                debugger;
                retailerMap.init();
        },
        options);
    }
    else {
        alert("Browser does not support geolocation");
        retailerMap.init();
    } */

}


function loadGMapsError() {
    alert("Wasn't able to load map :(");
    window.location = "/order";
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
        this._mouseIsDown = false;
        this._isPanning = false;
        if (location === null) {
            this.setMapToCurrentLocation();
        }
    }


    // not used
    setMapToCurrentLocation(map = this._map) {
        $.getJSON('http://freegeoip.net/json/', function (data) {
            map.setCenter(new google.maps.LatLng(data.latitude, data.longitude));
        });
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

    deleteAllMarkers() {
        for (var i = 0; i < this._markers.length; i++) {
            var marker = this._markers[i];
            marker.setMap(null);
        }
        this._markers = [];
    }

    showInfoWindow(marker) {
        marker.setAnimation(google.maps.Animation.BOUNCE);
        marker.setAnimation(null);
        this._infoWindow.setContent(marker.content);
        this._infoWindow.open(this._map, marker);
    }

    getBounds() {
        return this._map.getBounds();
    }

    addPanListener(callback) {
        var self = this;
        this._map.addListener('mousedown', function () {
            self._mouseIsDown = true;
        });
        this._map.addListener('mouseup', function () {
            self._mouseIsDown = false;
            if (self._isPanning) {
                callback();
            }
            self._isPanning = false;
        });
        this._map.addListener('center_changed', function () {
            self._isPanning = true;
            if (!self._mouseIsDown) {
                callback();
            }
        });
    }
}


class LatLng {
    constructor(lat, lng) {
        this.lat = lat;
        this.lng = lng;
    }
}


// Based on Haversine Formula
// Based on code by Salvador Dali, https://stackoverflow.com/questions/27928/calculate-distance-between-two-latitude-longitude-points-haversine-formulda
/**
 * 
 * @param {LatLng} point1
 * @param {LatLng} point2
 */
function distance(point1, point2) {
    // pi / 180
    var p = 0.017453292519943295;

    var c = Math.cos;
    var a = 0.5 - c((point2.lat - point1.lat) * p) / 2 +
        c(point1.lat * p) * c(point2.lat * p) *
        (1 - c((point2.lng - point1.lng) * p)) / 2;
    // 2 * R; R = 6371 km
    var km = 12742 * Math.asin(Math.sqrt(a));
    var miles = km * 0.621371;
    return miles;
}

