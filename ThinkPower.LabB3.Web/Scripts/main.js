
$(document).ready(function () {
    console.log('main.js');
});

function LoadPartialView(type, url, data, appendToElement) {

    $.ajax({
        type: type,
        url: url,
        data: data,
        cache: false,
        async: false,
        success: function (result) {
            appendToElement.empty().append(result);
        },
        error: function () {
            console.log("系統發生錯誤。");
        }
    });
}