$(function () {
    $('[data-toggle="tooltip"]').tooltip();

    $.validator.addMethod('optionaldatetimepicker', function (value, element) {
        return !element.validity.badInput;
    }, 'Provide a valid date and time or leave the field empty.');

    $.validator.unobtrusive.adapters.addBool('optionaldatetimepicker');
});