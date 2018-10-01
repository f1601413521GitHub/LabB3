$(document).ready(function () {

    if ($('#showMsg').length != 0) {
        $('#showMsg').click();
    }

    $('#re-evaluation').click(function () {
        $('#re-evaluation-form').submit();
    });

    $('#submit').click(function () {
        $('#submit-evaluation-form').submit();
    });
});