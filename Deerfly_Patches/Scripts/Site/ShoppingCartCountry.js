// ShoppingCartCountry.js
// REQUIRES: ShoppingCart.js, JQuery 3.1
/* ***************************** Country ************************************** */
// Sets the country selection from freegeoip.net

var shoppingCartCountry = (function () {
    "use strict";

    // Gets the country selected in the shopping cart
    function getCountry() {
        return $('#country-select input[name=country]:checked').val();
    }

    // Sets the country in the shopping cart and posts the change to 
    function setCountry() {
        $.getJSON('https://freegeoip.net/json/', function (data) {
            var country = data.country_code;
            var $countrySelect = $('#country-select');
            var $countryOption = $countrySelect.find('input[value="' + country + '"]');

            // Select 'other' if country is not in the list
            if ($countryOption.length === 0) {
                $countryOption = $countrySelect.find('input[value="--"]');
            }
            $countryOption.attr('checked', true);
            postCountry(country);
        }).fail(function () {
            postCountry("US");
        });
    }

    // Event called when country is manually changed
    function changeCountry() {
        postCountry(getCountry());
    }

    // Updates the country on the server
    function postCountry(country) {
        var postData = {
            __RequestVerificationToken: shoppingCart.antiForgeryToken,
            country: country
        };
        $.post('/shoppingcart/setcountry', postData, function (response) {
            var cart = response.data;
            for (var i = 0; i < cart.Order.OrderDetails.length; i++) {
                var orderDetail = cart.Order.OrderDetails[i];
                shoppingCart.refreshDetail(orderDetail);
            }
            shoppingCart.recalculate();
        });
    }

    // Return publicly accessible members
    return {
        getCountry: getCountry,
        setCountry: setCountry,
        changeCountry: changeCountry
    };
})();

// Automatically set country selector in shopping cart
shoppingCartCountry.setCountry();

