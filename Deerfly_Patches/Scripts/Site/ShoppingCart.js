// ShoppingCart.js

// Gets the anti-forgery token to use in post calls
function antiForgeryToken() {
    return $('#anti-forgery-token input').val();
}

// Handler for errors in post calls
function shoppingCartPostError(xhr) {
    var message = 'Error: ';
    if (xhr && xhr.responseJSON && xhr.responseJSON.message) {
        message += '\n' + xhr.responseJSON.message;
    }
    else {
        message += xhr.statusCode;
    }
    alert(message);
}

// Adds a product with a given id to the shopping cart
function addToShoppingCart(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    $.ajax({
        type: 'POST',
        url: '/ShoppingCart/AddItem/',
        data: postData,
        dataType: 'json',
        success: function (result) {
            incrementShoppingCartBadge();
        },
        error: function (xhr) { shoppingCartPostError(xhr); }
    });
}

// Adds a product with a given id to the shopping cart, and redirects to the shopping cart
function buyNow(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    $.ajax({
        type: 'POST',
        url: '/ShoppingCart/AddItem/',
        data: postData,
        dataType: 'json',
        success: function (result) {
            window.location = "/shoppingCart";
        },
        error: function (result) {
            if (result.status === 403) {
                window.location = "/shoppingCart";
            }
            else {
                shoppingCartPostError(result);
            }
        }
    });
}

// Increments the quantity of an item in the shopping cart with a given id
function incrementItem(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    $.ajax({
        type: 'POST',
        url: '/ShoppingCart/IncrementItem/',
        data: postData,
        dataType: 'json',
        success: function (result) {
            refreshDetail(result.data);
            recalculate();
        },
        error: function (xhr) { shoppingCartPostError(xhr); }
    });
}

// Decrements the quantity of an item in the shopping cart with a given id
function decrementItem(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };

    var qty = parseInt($('#qty-' + id).text());
    if (qty < 1) {
        alert('No items to remove!');
        return;
    }
    if (qty === 1) {
        alert('Minimum quantity of 1');
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/ShoppingCart/DecrementItem/',
        data: postData,
        dataType: 'json',
        success: function (result) {
            refreshDetail(result.data);
            recalculate();
        },
        error: function (xhr) { shoppingCartPostError(xhr); }
    });
}

// Removes an item with a given id from the shopping cart
function removeItem(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    $.ajax({
        type: 'POST',
        url: '/ShoppingCart/RemoveItem/',
        data: postData,
        dataType: 'json',
        success: function (result) {
            $('#item-' + id).remove();

            decrementShoppingCartBadge();

            // Reload page if no items remain in shopping cart
            if ($('.item-detail-line').length === 0) {
                location.reload();
            }
            recalculate();
        },
        error: function (xhr) { shoppingCartPostError(xhr); }
    });
}

// Refreshes the order detail with data supplied from server
function refreshDetail(orderDetail) {
    var id = orderDetail.Product.Id;

    var $orderDetail = $('#item-' + id);
    var $name = $orderDetail.find('.item-name');
    var $unitPrice = $orderDetail.find('.item-unit-price');
    var $quantity = $orderDetail.find('.item-qty-ct');
    var $extendedPrice = $orderDetail.find('.item-extended-price');
    var $shipping = $orderDetail.find('.item-shipping');
    var $totalPrice = $orderDetail.find('.item-total-price');

    $name.text(orderDetail.Product.Name);
    $unitPrice.text('$' + orderDetail.UnitPrice.toFixed(2));
    $quantity.text(orderDetail.Quantity);
    $extendedPrice.text('$' + orderDetail.ExtendedPrice.toFixed(2));
    $shipping.text('$' + orderDetail.Shipping.toFixed(2));
    $totalPrice.text('$' + orderDetail.TotalPrice.toFixed(2));
}

// Recalculates the total line based on the order details
function recalculate() {
    var extendedPriceTotal = 0;
    var shippingTotal = 0;
    var $itemDetailLines = $('.item-detail-line');
    $itemDetailLines.each(function () {
        var itemExtendedPrice = parseFloat($(this).find('.item-extended-price').text().slice(1)) || 0;
        var itemShipping = parseFloat($(this).find('.item-shipping').text().slice(1)) || 0;
        var itemTotalPrice = parseFloat($(this).find('.item-total-price').text().slice(1)) || 0;

        extendedPriceTotal += itemExtendedPrice;
        shippingTotal += itemShipping;
    });

    $('.item-detail-total .item-extended-price').text('$' + extendedPriceTotal.toFixed(2));
    $('.item-detail-total .item-shipping').text('$' + shippingTotal.toFixed(2));
    $('.item-detail-total .item-total-price').text('$' + (extendedPriceTotal + shippingTotal).toFixed(2));
}


// ****************** Shopping cart badge *******************************************************
// Initializes the shopping cart badge with the number of items from the server
(function getShoppingCartCount() {
    $.getJSON('/shoppingCart/ShoppingCartCount', function (data) {
        $('#shoppingCartCount').text(data.shoppingCartCount);
    });
})();

// Increments the shopping cart badge
function incrementShoppingCartBadge() {
    var currentCount = parseInt($('#shoppingCartCount').text());
    $('#shoppingCartCount').text(currentCount + 1);
}

// Decrements the shopping cart badge
function decrementShoppingCartBadge() {
    var currentCount = parseInt($('#shoppingCartCount').text());
    $('#shoppingCartCount').text(currentCount - 1);
}


/* ***************************** Country ************************************** */
// Sets the country selection from freegeoip.net
function setCountry() {
    $.getJSON('https://freegeoip.net/json/', function (data) {
        var country = data.country_code;
        var $countrySelect = $('#country-select');
        var $countryOption = $countrySelect.find('option[value="' + country + '"]');

        // Select 'other' if country is not in the list
        if ($countryOption.length === 0) {
            $countryOption = $countrySelect.find('option[value="--"]');
        }
        $countryOption.attr('selected', 'selected');
        updateCountryInShoppingCart(country);
    });
}

// Event called when country is manually changed
function countryChange() {
    var country = $('#country-select option:selected').val();
    updateCountryInShoppingCart(country);
}

// Updates the country on the server
function updateCountryInShoppingCart(country) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        country: country
    };
    $.post('/shoppingcart/setcountry', postData, function (response) {
        var cart = response.data;
        for (var i = 0; i < cart.Order.OrderDetails.length; i++) {
            var orderDetail = cart.Order.OrderDetails[i];
            refreshDetail(orderDetail);
        }
        recalculate();
    });
}

function getCountry() {
    return $('#country-select option:selected').val();
}
