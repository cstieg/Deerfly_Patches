function allowTextareaPattern() {
    "use strict";
    $('textarea[pattern]').each(function () {
        var regexPattern = $(this).attr('pattern');
        var re = new RegExp(regexPattern);

        $(this).on('keypress', function (e) {
            checkInput(e, e.key, re);
        });

        $(this).on('paste', function (e) {
            var pastedData = e.originalEvent.clipboardData.getData('text');
            checkInput(e, pastedData, re);
        })
    });

    function checkInput(e, inputData, re) {
        if (!re.test(inputData)) {
            alert('Input is not valid.');
            e.preventDefault();
        }
    }
}
