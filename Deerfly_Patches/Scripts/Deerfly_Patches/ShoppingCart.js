
function antiForgeryToken() {
    return $('#anti-forgery-token input')[0].value;
}

/* ************************** Shopping Cart **************************************************** */

function shoppingCartPostError(xhr, httpStatusMessage) {
    var message = 'Error: ';
    if (xhr && xhr.responseJSON.message) {
        message += '\n' + xhr.responseJSON.message;
    }
    alert(message);
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
        error: shoppingCartPostError
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
        error: shoppingCartPostError
    });
}

function incrementItemInShoppingCart(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    $.ajax({
        type: 'POST',
        url: '/Order/IncrementItemInShoppingCart/',
        data: postData,
        dataType: 'json',
        success: function (returnval) {
            var $qty = $('#qty-' + id)[0];
            $qty.innerText = parseInt($qty.innerText) + 1;
            recalculate();
        },
        error: shoppingCartPostError
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
        error: shoppingCartPostError
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
            if (itemsInDetailCount() === 0) {
                location.reload();
            }
            recalculate();
        },
        error: shoppingCartPostError
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
        var itemShipping = parseFloat($itemShipping.innerText.slice(1)) || 0;
        var itemTotalPrice = 1.0 * itemExtendedPrice + itemShipping;

        $itemExtendedPrice.innerHTML = '$' + itemExtendedPrice.toFixed(2);
        $itemShipping.innerHTML = '$' + itemShipping.toFixed(2);
        if (itemShipping === 0) {
            $itemShipping.innerHTML = 'FREE';
        }
        $itemTotalPrice.innerHTML = '$' + itemTotalPrice.toFixed(2);

        extendedPriceTotal += itemExtendedPrice;
        shippingTotal += itemShipping;
    });

    $('.item-detail-total .item-extended-price')[0].innerText = '$' + extendedPriceTotal.toFixed(2);
    $('.item-detail-total .item-shipping')[0].innerText = '$' + shippingTotal.toFixed(2);
    $('.item-detail-total .item-total-price')[0].innerText = '$' + (extendedPriceTotal + shippingTotal).toFixed(2);
}


function itemsInDetailCount() {
    var $itemDetailLines = $('.item-detail-line');
    return $itemDetailLines.length;
}

function setCountry() {
    var country = 'US';
    $.getJSON('https://freegeoip.net/json/', function (data) {
        var $countrySelect = $('#country-select');
        if (data.country_code === 'US') {
            $countrySelect.find('input[value="US"]').attr('checked', 'checked');
        }
        else {
            $countrySelect.find('input[value="International"]').attr('checked', 'checked');
        }
        countryChange();
    });
    countryChange();
}

function countryChange() {
    if (getCountry() === "US") {
        $('.item-detail-line .item-shipping').text('FREE');
    }
    else {
        $('.item-detail-line .item-shipping').each(function (i, elem) {
            elem.innerHTML = '$' + elem.nextElementSibling.innerHTML;
        });
    }
    recalculate();
}

function getCountry() {
    var isUS = $('#country-select input[value="US"]')[0].checked;
    return isUS ? "US" : "International";
}
