$(document).ready(function () {

    if ($('#showMsg').length != 0) {
        $('#showMsg').click();
    }

    $('#re-evaluation').on('click', function () {
        $('#re-evaluation-form').submit();
    });
});