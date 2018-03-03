
/* ******************************* Index ******************************************************** */
$(document).ready(function () {
    $('[data-toggle="popover"]').popover({ html: true, container: 'body' });
});


/* ******************************* Testimonials ************************************************* */
function displayTestimonial(id, url, srcset) {
    $('#testimonial-display').html(`
        <a href="${url}">
            <img src="${url}" srcset="${srcset}" alt="image of customer testimonial" />
        </a>`);
    $('.testimonial-label').removeClass('selected-testimonial');
    $('#' + id).addClass('selected-testimonial');
}


/* ****************************** Sortable Product Images **************************************** */
var productImages = document.getElementById('product-images');
if (productImages != null) {
    var sortable = Sortable.create(productImages, {
        onEnd: function (/**Event*/evt) {
            var $productImages = $(productImages);
            var productId = $('#ProductId').val();
            var imageOrder = [];
            $($productImages.children()).each(function (index, element) {
                imageOrder[index] = element.id.replace('image-', '');
            });
            var data = {
                imageOrder: JSON.stringify(imageOrder)
            };
            $.post('/products/orderWebImages/' + productId, data);
        }
    });
}

