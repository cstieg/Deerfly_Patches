var clientInfo = {
    clientId: document.getElementById('paypalClientId').innerText,
    mode: document.getElementById('paypalMode').innerText,
    paypalLoginReturnId: document.getElementById('paypalLoginReturnId').innerText
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
        debugger;


        // verify country

        // Make a call to the REST api to execute the payment
        return actions.payment.execute().then(function () {

            // verify order information, cancel if incorrect


            window.alert('Payment Complete!');
        });
    }

}, '#paypal-button-container');
