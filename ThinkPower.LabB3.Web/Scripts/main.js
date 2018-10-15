
var questionIndex = 0;
var questionList = null;
var answerDetailList = null;

const urlInfo = {
    evaQuestV2: '/RiskEvaluation/EvaQuestV2',
    acceptRiskRankV2: '/RiskEvaluation/AcceptRiskRankV2',
    evaluationRankV2: '/RiskEvaluation/EvaluationRankV2',
};



$(document).ready(function () {

    $('#options-fndre001').on('click', function () {

        bindingLayout($(this).data('questId'));
    });

    $('#options-fndre002').on('click', function () {

        bindingLayout($(this).data('questId'));
    });

});


function binding() {

    $('#done').on('click', function () {

        if (validate()) {
            $('#evaQuestForm').submit();
        }
    });
}

function bindingEvaluationRankEvent() {

    showModalMsg();

    $('#re-evaluation').click(function () {
        $('#re-evaluation-form').submit();
    });

    $('#submit').click(function () {
        $('#submit-evaluation-form').submit();
    });
}



function validate() {

    removeTip();

    let questionList = getAnswerDetailInfoList();

    return validateRule(questionList);
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



function getAnswerDetailInfoList() {

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



function setModalMsg(msg) {

    $(".modal-body").html(msg);
}

function showModalMsg() {

    if ($('#tip-message-modal').length !== 0) {
        $('#tip-message-modal').modal('show');
    }
}



function removeTipV2(footer) {

    footer.hide();
    footer.find('span').html('');
}

function validateRuleV2(question, questionList) {

    let message = null;

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

    return message;
}

function toggleTip(question, message) {

    if (message) {
        showTip(question.footer, message);
    }
    else {
        removeTipV2(question.footer);
    }
}

function toggleQuestion() {

    questionList.hide();
    questionList.eq(questionIndex).show();

    if (questionIndex === 0) {

        $('#button-prev').hide();
        $('#button-next').show();
        $('#button-done').hide();

    } else if (questionIndex === (questionList.length - 1)) {

        $('#button-prev').show();
        $('#button-next').hide();
        $('#button-done').show();

    } else {

        $('#button-prev').show();
        $('#button-next').show();
        $('#button-done').hide();
    }
}



function getQuestionAnswer() {

    return {
        QuestionnaireAnswerEntity: {
            QuestUid: $('#questEntity_Uid').val(),
            QuestId: $('#questEntity_QuestId').val(),
            AnswerDetailEntities: getAnswerDetailList()
        }
    };
}

function getAnswerDetailList() {

    let answerType = null;
    let questionId = null;

    let answerCode = null;
    let answerCodeList = null;
    let other = null;

    let answerDetail = null;
    let answerDetailList = [];

    $.each($('[id*=question]'), function (index, element) {

        answerType = $(element).data('answerType');
        questionId = $(element).data('questionId');

        if (answerType === 'F') {

            answerCode = $('[id*=answer-' + questionId + '-code]').val();
            answerDetail = convertAnswerDetail(questionId, answerCode, null);
            answerDetailList.push(answerDetail);

        } else if (answerType === 'S' || answerType === 'M') {

            answerCodeList = $('[id*=answer-' + questionId + '-code]:checked');

            $.each(answerCodeList, function (i, e) {

                answerCode = $(e).val();

                other = $('[id*=answer-' + questionId + '-other-' + answerCode + ']').val();

                if (other) {

                    answerDetail = convertAnswerDetail(questionId, answerCode, other);
                    answerDetailList.push(answerDetail);

                } else {
                    answerDetail = convertAnswerDetail(questionId, answerCode, null);
                    answerDetailList.push(answerDetail);
                }
            });
        }
    });

    return answerDetailList;
}

function convertAnswerDetail(questionId, answerCode, otherAnswer) {

    return {
        QuestionId: questionId,
        AnswerCode: answerCode,
        OtherAnswer: otherAnswer
    };
}



function bindingLayout(questId) {

    let resultInfo = loadPartialView('get', urlInfo.evaQuestV2, { QuestId: questId });

    if (!resultInfo.success) {

        $("#result-partial-view").empty().append(resultInfo.errorMsg);

    } else {

        $("#result-partial-view").empty().append(resultInfo.info);
        bindingEvaQuestV2();
    }
}

function bindingEvaQuestV2(errorQuestionIndex) {

    hideTip();

    questionIndex = (errorQuestionIndex) ? errorQuestionIndex : 0;
    questionList = $('[id*=question]');
    answerDetailList = getAnswerDetailInfoList();

    toggleQuestion();

    if (!errorQuestionIndex && errorQuestionIndex !== 0) {
        bindingButtonEvent();
    }
}

function bindingEvaluationRankV2() {

    let resultInfo = null;

    let modal = $('#tip-message-modal').find('.modal-body');

    if (modal.length > 0 && modal.html().trim()) {
        showModalMsg();
    }

    $('#re-evaluation').click(function () {

        resultInfo = loadPartialView('get', urlInfo.evaQuestV2,
            { QuestId: $('#QuestId').val() },
        );

        if (!resultInfo.success) {

            $("#result-partial-view").empty().append(resultInfo.errorMsg);

        } else {

            $("#result-partial-view").empty().append(resultInfo.info);
            bindingEvaQuestV2();
        }
    });

    $('#submit').click(function () {

        resultInfo = acceptRiskRank('get', urlInfo.acceptRiskRankV2,
            { QuestAnswerId: $('#QuestAnswerId').val() },
        );

        if (!resultInfo.success) {

            $("#result-partial-view").empty().append(resultInfo.errorMsg);

        } else {

            $(".eva-rank-container").empty();
            setModalMsg(resultInfo.info.replace(/"/g, ''));
            showModalMsg();
        }
    });
}



function loadPartialView(type, url, data) {

    return callAjax(type, url, data);
}

function acceptRiskRank(type, url, data) {

    return callAjax(type, url, data);
}

function callAjax(type, url, data) {

    let resultInfo = { success: false, errorMsg: null, info: null, isJson: false };

    $.ajax({
        type: type,
        url: url,
        data: data,
        cache: false,
        async: false,
        success: function (result) {

            resultInfo.success = true;
            if (result.isJson) {

                resultInfo.isJson = true;
                resultInfo.info = result.validateInfo;

            } else {

                resultInfo.info = result;
            }
        },
        error: function () {
            resultInfo.errorMsg = '<div class="validation-summary-errors text-danger"><ul><li>系統發生錯誤，請於上班時段來電客服中心0800-015-000，造成不便敬請見諒。</li></ul ></div >';
        }
    });

    return resultInfo;
}



function bindingButtonEvent() {

    $('#button-prev').on('click', function () {

        questionHandler('prev');
    });

    $('#button-next').on('click', function () {

        questionHandler('next');
    });

    $('#button-done').on('click', function () {

        questionHandler('done');
    });
}

function questionHandler(action) {

    let question = answerDetailList[questionIndex];
    let validateResult = validateRuleV2(question, answerDetailList);
    toggleTip(question, validateResult);

    if (!validateResult) {

        if (action === 'prev') {

            questionIndex = (questionIndex > 0) ? (questionIndex - 1) : 0;

            toggleQuestion();

        } else if (action === 'next') {

            questionIndex = (questionIndex < (questionList.length - 1)) ?
                (questionIndex + 1) : questionIndex;

            toggleQuestion();

        } else if (action === 'done') {

            if (validateRule(answerDetailList)) {

                let resultInfo = loadPartialView('post', urlInfo.evaluationRankV2,
                    getQuestionAnswer(),
                );

                if (!resultInfo.success) {

                    $("#result-partial-view").empty().append(resultInfo.errorMsg);

                } else if (resultInfo.isJson) {

                    let errorQuestionIndex = null;

                    $.each(resultInfo.info, function (id, msg) {

                        let question = answerDetailList.filter(function (i, e) {

                            let state = (e.datas.questionId === id);

                            if (state && errorQuestionIndex === null) {
                                errorQuestionIndex = i;
                            }

                            return state;
                        })[0];

                        toggleTip(question, msg);

                    });

                    bindingEvaQuestV2(errorQuestionIndex);

                } else {

                    $("#result-partial-view").empty().append(resultInfo.info);

                    bindingEvaluationRankV2();
                }
            }
        }
    }
}
