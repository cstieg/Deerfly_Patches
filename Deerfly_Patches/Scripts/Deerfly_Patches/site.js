
/* ******************************* Testimonials ************************************************* */
function displayTestimonial(id, url, srcset) {
    $('#testimonial-display').html(`
        <a href="${url}">
            <img src="${url}" srcset="${srcset}" alt="image of customer testimonial" />
        </a>`);
    $('.testimonial-lable').removeClass('selected-testimonial');
    $('#' + id).addClass('selected-testimonial');
}
