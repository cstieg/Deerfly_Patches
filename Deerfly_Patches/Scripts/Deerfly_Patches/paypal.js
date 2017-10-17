// Paypal.js

// Must have (hidden) divs with:
//   paypalClientId
//   paypalMode
// containing as innerText the respective data passed from the server in the ViewBag

var clientInfo = {
    clientId: document.getElementById('paypalClientId').innerText,
    mode: document.getElementById('paypalMode').innerText,
};

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
            })
            .catch(function (data) {
                alert("Error processing order :(");
            });
    },

    // onAuthorize() is called when the buyer approves the payment
    onAuthorize: function (data, actions) {
        return actions.payment.get()
            .then(function (paymentDetails) {
                var verifyData = {
                    paymentDetails: JSON.stringify(paymentDetails)
                };
                $.post("/ShoppingCart/VerifyAndSave", verifyData)
                    .then(function () {
                        // Execute the payment
                        return actions.payment.execute();
                    })
                    .then(function (data) {
                        // Show a success page to the buyer
                        window.location.href = "/ShoppingCart/OrderSuccess";
                    })
                    .catch(function (data) {
                        alert("Error processing order :(");
                    });
            });
    }

}, '#paypal-button-container');
