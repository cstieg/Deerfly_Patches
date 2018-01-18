// Paypal.js

// Must have (hidden) divs with:
//   paypalClientId
//   paypalMode
// containing as innerText the respective data passed from the server in the ViewBag

// REQUIRES: JQUERY 3.1 !!

var clientInfo = {
    clientId: document.getElementById('paypalClientId').innerText,
    mode: document.getElementById('paypalMode').innerText
};

if (!paypal.isEligible()) {
    // Do not show PayPal experience
    $('#paypal-button-container').text('We are having problems displaying the PayPal button.  This may occur with Internet Explorer users. ' +
        'Please ensure that JavaScript is enabled, and that the security mode in the Internet zone is set to medium high. ' +
        'Also please consider using a modern browser such as Chrome, FireFox, Edge, Opera, Safari, etc.');
}

paypal.Button.render({

    env: clientInfo.mode,

    client: {
        sandbox: clientInfo.clientId,
        production: clientInfo.clientId
    },

    // Show the buyer a 'Pay Now' button in the checkout flow
    commit: true,

    // payment() is called when the button is clicked
    payment: function (data, actions) {
        // Get JSON order information from server
        return $.get('/paypal/GetOrderJson?country=' + getCountry())
            .then(function (data) {
                var payment = JSON.parse(data);

                // Make a call to the REST api to create the payment
                return actions.payment.create({ payment: payment });
            },
            // on error
            function (data) {
                alert('Error processing order: \n' + data.responseJSON.message);
                window.location.href = "/ShoppingCart";
            })
            .fail(function (data) {
                alert('Error processing PayPal order: \n' + data.responseJSON.message);
                window.location.href = "/ShoppingCart";
            });
    },

    // onAuthorize() is called when the buyer approves the payment
    onAuthorize: function (data, actions) {
        return actions.payment.get()
            .then(function (paymentDetails) {
                var lightbox = new LightboxMessage('Please wait...');
                lightbox.display();

                var verifyData = {
                    paymentDetails: JSON.stringify(paymentDetails)
                };

                $.ajax({
                    type: 'POST',
                    url: '/PayPal/VerifyAndSave',
                    data: verifyData,
                    success: function () {
                        // Execute the payment
                        return actions.payment.execute()
                            .then(function (data) {
                                // Email an order confirmation page to the buyer
                                $.post('/Mail/ConfirmOrder?cart=' + data.cart);

                                // Show a success page to the buyer
                                window.location.href = '/ShoppingCart/OrderSuccess?cart=' + data.cart;
                            },
                            function (data) {
                                alert('Error in PayPal processing order: \n' + data.responseJSON.message);
                                lightbox.destroy();
                            });
                    },
                    error: function (data) {
                        if (data.responseJSON) {
                            alert('Error processing order: ' + data.responseJSON.message);
                        }
                        else {
                            alert('Error processing order.');
                        }
                        lightbox.destroy();
                    }
                });
            });
    }

}, '#paypal-button-container');
