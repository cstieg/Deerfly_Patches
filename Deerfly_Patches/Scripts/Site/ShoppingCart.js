/* **************************** SHOPPING CART ************************************************ 
Requires: JQuery 3.1+
*/

var shoppingCart = (function () {
    "use strict";

    // anti-forgery token to use in post calls
    var antiForgeryToken = $('#anti-forgery-token input').val();

    // Handler for errors in post calls
    function postError(xhr) {
        var message = 'Error: ';
        if (xhr && xhr.responseJSON && xhr.responseJSON.Message) {
            message += '\n' + xhr.responseJSON.Message;
        }
        else {
            message += xhr.status;
        }
        alert(message);
    }

    // Adds a product with a given id to the shopping cart, and redirects to the shopping cart
    function buyNow(id) {
        var postData = {
            __RequestVerificationToken: antiForgeryToken,
            ID: id
        };
        $.ajax({
            type: 'POST',
            url: '/ShoppingCart/AddItem/',
            data: postData,
            success: function (result) {
                window.location.href = "/ShoppingCart";
            },
            error: function (result) { debugger; postError(result); }
        });
    }

    // Increments the quantity of an item in the shopping cart with a given id
    function incrementItem(id) {
        var postData = {
            __RequestVerificationToken: antiForgeryToken,
            ID: id
        };
        $.ajax({
            type: 'POST',
            url: '/ShoppingCart/IncrementItem/',
            data: postData,
            dataType: 'json',
            success: function (result) {
                refreshDetail(result.data.orderDetail);
                recalculate();
            },
            error: function (xhr) { postError(xhr); }
        });
    }

    // Decrements the quantity of an item in the shopping cart with a given id
    function decrementItem(id) {
        var postData = {
            __RequestVerificationToken: antiForgeryToken,
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
                if (result.data.needsRefresh) {
                    window.location.href = "/ShoppingCart";
                }

                refreshDetail(result.data.orderDetail);
                recalculate();
            },
            error: function (xhr) { postError(xhr); }
        });
    }

    // Removes an item with a given id from the shopping cart
    function removeItem(id) {
        var postData = {
            __RequestVerificationToken: antiForgeryToken,
            ID: id
        };
        $.ajax({
            type: 'POST',
            url: '/ShoppingCart/RemoveItem/',
            data: postData,
            dataType: 'json',
            success: function (result) {
                if (result.data.needsRefresh) {
                    window.location.href = "/ShoppingCart";
                }

                $('#item-' + id).remove();

                decrementCartBadge();

                // Reload page if no items remain in shopping cart
                if ($('.item-detail-line').length === 0) {
                    location.reload();
                }
                recalculate();
            },
            error: function (xhr) { postError(xhr); }
        });
    }

    // Removes a given promo code from the shopping cart
    function removePromoCode(promoCode) {
        var postData = {
            __RequestVerificationToken: antiForgeryToken,
            promoCode: promoCode
        };
        $.post('/ShoppingCart/RemovePromoCode', postData, function () {
            window.location.href = '/ShoppingCart';
        }, 'json')
            .fail(function (result) {
                postError(result);
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

    // ********************************** Shopping Cart Badge *******************************
    /* Example code for shopping cart badge HTML
            <a href="/shoppingcart">
                <span class="glyphicon glyphicon-shopping-cart"></span>Cart
                <span class="badge" id="shoppingCartCount"></span>
            </a>
    */

    var $shoppingCartCount = $('#shoppingCartCount');

    // Initializes the shopping cart badge with the number of items from the server
    function setCartBadge() {
        $.getJSON('/shoppingCart/ShoppingCartCount', function (data) {
            $shoppingCartCount.text(data.shoppingCartCount);
        });
    }

    // Increments the shopping cart badge
    function incrementCartBadge() {
        var currentCount = parseInt($shoppingCartCount.text());
        $shoppingCartCount.text(currentCount + 1);
    }

    // Decrements the shopping cart badge
    function decrementCartBadge() {
        var currentCount = parseInt($shoppingCartCount.text());
        $shoppingCartCount.text(currentCount - 1);
    }

    // return publicly accessible members
    return {
        antiForgeryToken: antiForgeryToken,

        buyNow: buyNow,
        incrementItem: incrementItem,
        decrementItem: decrementItem,
        removeItem: removeItem,
        removePromoCode: removePromoCode,

        refreshDetail: refreshDetail,
        recalculate: recalculate,

        setCartBadge: setCartBadge,
        incrementCartBadge: incrementCartBadge,
        decrementCartBadge: decrementCartBadge
    };

    // *********************** NOT USED *********************************************

    // Adds a product with a given id to the shopping cart
    /*
    addToShoppingCart: function (id) {
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
    */
})();

// every page should set the shopping cart badge upon loading
shoppingCart.setCartBadge();
