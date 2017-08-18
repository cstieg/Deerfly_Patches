/* On DOM Ready */
$(function () {
    // Add emphasis on mouseover product items on order view
    $('.product-item img').mouseenter(function () {
        $(this).animate({ height: '+=5', width: '+=5' })
            .animate({ height: '-=5', width: '-=5' });
        // TODO: Use JQueryUI to animate
        // $(this).effect("pulsate");
    });



});









function antiForgeryToken() {
    return $('#anti-forgery-token input')[0].value;
}


function addToShoppingCart(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    $.ajax({
        type: 'POST',
        url: '/Order/AddOrderDetailToShoppingCart/',
        data: postData,
        dataType: 'json',
        success: function (returnval) {
            alert('Success!');          
        },
        error: function (returnval) {
            alert('Error adding item to shopping cart :( ');
        }
    });
}

function buyNow(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    $.ajax({
        type: 'POST',
        url: '/Order/AddOrderDetailToShoppingCart/',
        data: postData,
        dataType: 'json',
        success: function (returnval) {
            window.location = "/shoppingCart";
        },
        error: function (returnval) {
            alert('Error adding item to shopping cart :( ');
        }
    });
}

function incrementItemInShoppingCart(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    $.ajax({
        type: 'POST',
        url: '/Order/AddOrderDetailToShoppingCart/',
        data: postData,
        dataType: 'json',
        success: function (returnval) {
            var $qty = $('#qty-' + id)[0];
            $qty.innerText = parseInt($qty.innerText) + 1;
            recalculate();
        },
        error: function (returnval) {
            alert('Error incrementing item in shopping cart :( ');
        }
    });
}


function decrementItemInShoppingCart(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    var $qty = $('#qty-' + id)[0];

    var qty = parseInt($qty.innerText);
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
        url: '/Order/DecrementItemInShoppingCart/',
        data: postData,
        dataType: 'json',
        success: function (returnval) {
            $qty.innerText = parseInt($qty.innerText) - 1;
            recalculate();
        },
        error: function (returnval) {
            alert('Error decrementing item in shopping cart :( ');
        }
    });
}

function removeItemInShoppingCart(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    $.ajax({
        type: 'POST',
        url: '/Order/RemoveItemInShoppingCart/',
        data: postData,
        dataType: 'json',
        success: function (returnval) {
            var $item = $('#item-' + id)[0];
            $item.remove();
            if (itemsInDetailCount() == 0) {
                location.reload();
            }
            recalculate();
        },
        error: function (returnval) {
            alert('Error removing item from shopping cart :( ');
        }
    });
}

function recalculate() {
    var $itemDetailLines = $('.item-detail-line');
    var extendedPriceTotal = 0;
    var shippingTotal = 0;
    $itemDetailLines.each(function () {
        var linePrice = parseFloat($(this).find('.item-unit-price')[0].innerText.slice(1));
        var lineQty = parseInt($(this).find('.item-qty-ct')[0].innerText);

        var $itemExtendedPrice = $(this).find('.item-extended-price')[0];
        var $itemShipping = $(this).find('.item-shipping')[0];
        var $itemTotalPrice = $(this).find('.item-total-price')[0];

        var itemExtendedPrice = 1.0 * linePrice * lineQty;
        var itemShipping = parseFloat($itemShipping.innerText.slice(1));
        var itemTotalPrice = 1.0 * itemExtendedPrice + itemShipping;

        $itemExtendedPrice.innerHTML = '$' + itemExtendedPrice.toFixed(2);
        $itemShipping.innerHTML = '$' + itemShipping.toFixed(2);
        $itemTotalPrice.innerHTML = '$' + itemTotalPrice.toFixed(2);

        extendedPriceTotal += itemExtendedPrice;
        shippingTotal += itemShipping;
    });

    $('.item-detail-total .item-extended-price')[0].innerText = '$' + extendedPriceTotal.toFixed(2);
    $('.item-detail-total .item-shipping')[0].innerText = '$' + shippingTotal.toFixed(2);
    $('.item-detail-total .item-total-price')[0].innerText = '$' + (extendedPriceTotal + shippingTotal).toFixed(2);
}


function updateShippingAddress() {
    var formdata = $('#shipping-address').serializeArray();
    $.ajax({
        type: 'POST',
        url: '/Checkout/UpdateShippingAddress',
        data: formdata,
        dataType: 'json',
        success: function(result, data) {
            
        },
        error: function(result) {
            debugger;
        }
    });
}

function itemsInDetailCount() {
    var $itemDetailLines = $('.item-detail-line');
    return $itemDetailLines.length;
}


function setCountry() {
    var country = 'US';
    $.getJSON('http://freegeoip.net/json/', function (data) {
        var $countrySelect = $('#country-select');
        if (country === 'US') {
            $countrySelect.find('input[value="US"]').attr('checked', 'checked');
        }
        else {
            $countrySelect.find('input[value="International"]').attr('checked', 'checked');
        }
    });
}

function toggleCountry() {
    debugger;
}