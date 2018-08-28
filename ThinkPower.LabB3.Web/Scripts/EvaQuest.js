$(document).ready(function () {

    var collection = [];
    $('.question-subject-answer-content').each(function () {
        collection.push(JSON.stringify($(this).data()));
    });
    console.log(collection);

});