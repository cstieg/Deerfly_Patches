/* ****************************** Image Upload Preview ***************************************** */
function imageUploadPreview(targetId) {
    var e = window.event;
    for (var i = 0; i < e.srcElement.files.length; i++) {
        var file = e.srcElement.files[i];
        var $targetImg = $(targetId);
        var reader = new FileReader();
        reader.onloadend = function () {
            $targetImg.attr("src", reader.result);
        };
        reader.readAsDataURL(file);
    }
}

function imageUpload(productId, container) {
    // global variable to keep track of pending image uploads
    imageUploadPendingCount = 0;
    showLightboxMessage("Please wait while image upload finishes", "ImageUploadPendingLightboxMessage");

    var e = window.event;
    var $container = $(container);
    for (var i = 0; i < e.srcElement.files.length; i++) {
        var file = e.srcElement.files[i];
        imageUploadSingle(file, productId, $container);
        imageUploadPendingCount++;
    }
}

function imageUploadSingle(file, productId, $container) {
    var reader = new FileReader();
    reader.onloadend = function () {
        var myFormData = new FormData();
        myFormData.append('file', file);
        $.ajax({
            type: 'POST',
            url: '/Products/AddImage/' + productId,
            data: myFormData,
            processData: false, // important
            contentType: false, // important
            success: function (response) {
                imageUploadFinished();
                var $newPicture = $(`
                            <picture id="image-${response.imageId}" class="flex">
                                <img src="${reader.result}" class="thumbnail"/>
                                <button onclick="imageDelete(${productId}, ${response.imageId});return false;">
                                    Delete Image
                                </button>
                            </picture>
                    `);
                $container.append($newPicture);
            },
            error: function (response) {
                imageUploadFinished();
                alert("Failed to upload image");
            }
        });

    };
    reader.readAsDataURL(file);
}

function imageDelete(productId, imageId) {
    $.ajax({
        type: 'POST',
        url: '/Products/DeleteImage/' + productId,
        data: { imageId: imageId },
        success: function (response) {
            $('#image-' + imageId).remove();
        },
        error: function (response) {
            alert("Failed to delete image");
        }
    });
}

function imageUploadFinished() {
    imageUploadPendingCount--;
    if (imageUploadPendingCount === 0) {
        hideLightboxMessage("ImageUploadPendingLightboxMessage");
    }
}