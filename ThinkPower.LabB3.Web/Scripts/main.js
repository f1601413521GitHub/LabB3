

function hideTip() {

    $('[id*=footer]').each(function () {
        $(this).hide();
        if ($(this).find('span').html().trim()) {
            $(this).show();
        };
    });
}

function removeTip() {

    $('[id*=footer]').each(function () {
        $(this).find('span').html('');
        $(this).hide();
    });
}

function binding() {

    $('#done').on('click', function () {

        if (validate()) {
            $('#evaQuestForm').submit();
        }
    });
}

function validate() {

    removeTip();

    let questionList = getAnswerDetailList();

    return validateRule(questionList);
}

function getAnswerDetailList() {

    let questionList = $('[id*=question]').map(function () {

        let question = this;
        let questId = $(question).data('questionId');
        return {

            element: question,
            datas: $(question).data(),
            answerList: $(question).find('[id*=answer-entity]').map(function () {

                let answer = this;
                return {
                    element: answer,
                    datas: $(answer).data(),
                    answerCode: $(answer).find('[id*=answer-' + questId + '-code]'),
                    otherAnswer: $(answer).find('[id*=answer-' + questId + '-other]'),
                };
            }),
            footer: $(question).find('[id*=footer]'),
        };
    });

    return questionList;
}

function validateRule(questionList) {

    let validateFailCount = 0;
    questionList.each(function () {

        let question = this;
        let message = '';

        if (!validateNeedAnswer(question, questionList)) {
            message = "此題必須填答!";
        }
        else if (!validateMinMultipleAnswers(question)) {
            message = "此題至少須勾選" + question.datas.minMultipleAnswers + "個項目!";
        }
        else if (!validateMaxMultipleAnswers(question)) {
            message = "此題至多僅能勾選" + question.datas.maxMultipleAnswers + "個項目!";
        }
        else if (!validateSingleAnswerCondition(question, questionList)) {
            message = "此題僅能勾選1個項目!";
        }
        else if (!validateOtherAnswer(question)) {
            message = "請輸入其他說明文字!";
        }

        if (message) {
            validateFailCount++;
            showTip(question.footer, message);
        }
    });

    return (validateFailCount === 0);
}

function validateNeedAnswer(question, questionList) {

    let validate = false;

    if (getAnswerCodeList(question).length > 0) {

        validate = true;
    } else if (question.datas.needAnswer === "Y") {

        let allowNaCondition = false;
        if (question.datas.allowNaCondition) {

            let conditionValidateResultList = [];
            $(question.datas.allowNaCondition.Conditions).each(function () {

                let condition = {
                    object: this,
                    answerCodeList: [],
                };

                condition.quetion = questionList.filter(function () {
                    return (this.datas.questionId == condition.object.QuestionId);
                }).get()[0];

                condition.answerCodeList = getAnswerCodeList(condition.quetion);

                let result = compareSameArray(condition.object.AnswerCode, condition.answerCodeList);
                conditionValidateResultList.push(result);

            });

            if ($.inArray(true, conditionValidateResultList) > -1) {

                allowNaCondition = true;
            }
        }

        if (allowNaCondition) {
            validate = true;
        }
    }
    else {
        validate = true;
    }

    return validate;
}

function validateMinMultipleAnswers(question) {

    let validate = false;
    if (question.datas.answerType === 'M' &&
        question.datas.minMultipleAnswers) {

        let answerCodeList = getAnswerCodeList(question);
        if (answerCodeList.length >= question.datas.minMultipleAnswers) {
            validate = true;
        }
    } else {
        validate = true;
    }

    return validate;
}

function validateMaxMultipleAnswers(question) {

    let validate = false;
    if (question.datas.answerType === 'M' &&
        question.datas.maxMultipleAnswers) {

        let answerCodeList = getAnswerCodeList(question);
        if (answerCodeList.length <= question.datas.maxMultipleAnswers) {
            validate = true;
        }
    } else {
        validate = true;
    }

    return validate;
}

function validateSingleAnswerCondition(question, questionList) {

    let validate = false;
    if (question.datas.singleAnswerCondition) {

        let conditionValidateResultList = [];
        $(question.datas.singleAnswerCondition.Conditions).each(function () {

            let condition = {
                object: this,
                answerCodeList: [],
            };

            condition.quetion = questionList.filter(function () {
                return (this.datas.questionId == condition.object.QuestionId);
            }).get()[0];

            condition.answerCodeList = getAnswerCodeList(condition.quetion);

            let result = compareSingleAnswerCondition(condition.object.AnswerCode, condition.answerCodeList);
            conditionValidateResultList.push(result);

        });

        if ($.inArray(false, conditionValidateResultList) === -1) {
            validate = true;
        }
    }
    else {
        validate = true;
    }

    return validate;
}

function validateOtherAnswer(question) {

    let validate = false;

    let hasOtherAnswerCondition = 0;
    $(question.answerList).each(function (index, answer) {

        if (question.datas.answerType === "F") {

            validate = true;
        }
        else if ((answer.datas.haveOtherAnswer === "Y") &&
            (answer.datas.needOtherAnswer === "Y") &&
            answer.answerCode.is(':checked') === true) {

            hasOtherAnswerCondition++;
            if (answer.otherAnswer.val().trim()) {
                validate = true;
            }
        }
    });

    if (hasOtherAnswerCondition === 0) {
        validate = true;
    }

    return validate;
}

function getAnswerCodeList(quetion) {

    let answerCodeList = [];

    $(quetion.answerList).each(function () {

        let answer = this;
        let answerCode = '';

        if ((quetion.datas.answerType === 'S') ||
            (quetion.datas.answerType === 'M')) {

            if (answer.answerCode.is(':checked') === true) {
                answerCode = answer.datas.answerCode;
            }

        } else if (quetion.datas.answerType === 'F') {

            if (answer.answerCode.val().trim()) {
                answerCode = answer.answerCode.val().trim();
            }
        }

        if (answerCode) {
            answerCodeList.push(answerCode.toString());
        }
    });

    return answerCodeList;
}


function getAnswerScoreList(quetion) {

    let answerScoreList = [];

    $(quetion.answerList).each(function () {

        let answer = this;
        let answerScore = '';

        if ((quetion.datas.answerType === 'S') ||
            (quetion.datas.answerType === 'M')) {

            if (answer.answerCode.is(':checked') === true) {
                answerScore = answer.datas.score;
            }

        } else if (quetion.datas.answerType === 'F') {

            if (answer.answerCode.val().trim()) {
                answerScore = answer.datas.score;
            }
        }

        if (answerScore) {
            answerScoreList.push(parseInt(answerScore));
        }
    });

    return answerScoreList;
}

function showTip(footer, msg) {

    footer.show();
    footer.find('span').html(msg);
}

function compareSameArray(conditionAnswerCodeList, answerCodeList) {

    let validate = false;
    let answerCodeInArray = [];
    $.each(conditionAnswerCodeList, function (index, answerCode) {

        if ($.inArray(answerCode, answerCodeList) > -1) {
            answerCodeInArray.push(true);
        }
    });

    if ((answerCodeInArray.length > 0) &&
        (answerCodeInArray.length === answerCodeList.length)) {
        validate = true;
    }

    return validate;
}

function compareSingleAnswerCondition(conditionAnswerCodeList, answerCodeList) {

    let validate = false;

    if (answerCodeList.length === 1) {
        validate = true;
    } else {

        let singleAnswerCode = conditionAnswerCodeList[0];
        let answerCodeInArray = false;

        if ($.inArray(singleAnswerCode, answerCodeList) > -1) {
            answerCodeInArray = true;
        }

        if (!answerCodeInArray) {
            validate = true;
        }
    }

    return validate;
}