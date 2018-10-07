$(document).ready(function () {

    if ($('#tip-message-modal').length !== 0) {
        $('#tip-message-modal').modal('show');
    }

    $('#re-evaluation').click(function () {
        $('#re-evaluation-form').submit();
    });

    $('#submit').click(function () {
        $('#submit-evaluation-form').submit();
    });
});