$(document).ready(function () {

    showLog(true, 'ready', null, null);

    removeTip();
    binding();
});

function removeTip() {

    showLog(true, 'removeTip', null, null);

    $('[id*=footer]').each(function () {
        $(this).find('span').html('');
        $(this).hide();
    });
}

function binding() {

    showLog(true, 'binding', null, null);

    $('#done').on('click', function () {
        validate();
    });
}

function validate() {

    showLog(true, 'validate', null, null);

    removeTip();

    let questionList = getQuestionList();
    validateRule(questionList);
}

function getQuestionList() {

    let questionList = $('[id*=question]').map(function () {

        let question = this;
        return {

            element: question,
            datas: $(question).data(),
            answerList: $(question).find('[id*=answer-]').map(function () {

                let answer = this;
                return {
                    element: answer,
                    datas: $(answer).data(),
                    answerCode: $(answer).find('[id*=answerCode]'),
                    otherAnswer: $(answer).find('[id*=otherAnswer]'),
                };
            }),
            footer: $(question).find('[id*=footer]'),
        };
    });

    showLog(true, 'getQuestionList', null, { questionList: questionList });

    return questionList;
}

function validateRule(questionList) {

    let validateFailCount = 0;
    questionList.each(function () {

        let question = this;
        let message = '';

        showLog(true, question.datas.questionId, {
            questionId: question.datas.questionId,
            question: question
        }, null);

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

        if (message !== '') {
            validateFailCount++;
            showTip(question.footer, message);
        }
    });

    if (validateFailCount === 0) {

        //let totalScore = getScore(questionList);
        //if (totalScore == 0) {
        //    let msg = '您的問卷己填答完畢，謝謝您的參與';
        //}

        $('#evaQuestForm').submit();
    }
}

function validateNeedAnswer(question, questionList) {

    let validate = false;
    if (question.datas.needAnswer === "Y") {

        let allowNaCondition = false;
        if (question.datas.allowNaCondition !== '') {

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

                showLog(true, 'allowNaCondition', {
                    condition: condition,
                    questionId: question.datas.questionId,
                }, null);

                let result = compareSameArray(condition.object.AnswerCode, condition.answerCodeList);
                conditionValidateResultList.push(result);

            });

            if ($.inArray(true, conditionValidateResultList) > -1) {

                allowNaCondition = true;
            }
        }

        if ((allowNaCondition === true) ||
            (getAnswerCodeList(question).length > 0)) {
            validate = true;
        }
    }
    else {
        validate = true;
    }

    showLog(true, 'validateNeedAnswer', null, { validate: validate });

    return validate;
}

function validateMinMultipleAnswers(question) {

    let validate = false;
    if (question.datas.answerType === 'M' &&
        question.datas.minMultipleAnswers !== '') {

        let answerCodeList = getAnswerCodeList(question);
        if (answerCodeList.length >= question.datas.minMultipleAnswers) {
            validate = true;
        }
    } else {
        validate = true;
    }

    showLog(true, 'validateMinMultipleAnswers', null, { validate: validate });

    return validate;
}

function validateMaxMultipleAnswers(question) {

    let validate = false;
    if (question.datas.answerType === 'M' &&
        question.datas.maxMultipleAnswers !== '') {

        let answerCodeList = getAnswerCodeList(question);
        if (answerCodeList.length <= question.datas.maxMultipleAnswers) {
            validate = true;
        }
    } else {
        validate = true;
    }

    showLog(true, 'validateMaxMultipleAnswers', null, { validate: validate });

    return validate;
}

function validateSingleAnswerCondition(question, questionList) {

    let validate = false;
    if (question.datas.singleAnswerCondition !== '') {

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

            showLog(true, 'singleAnswerCondition', {
                condition: condition,
                questionId: question.datas.questionId,
            }, null);

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

    showLog(true, 'validateSingleAnswerCondition', null, { validate: validate });

    return validate;
}

function validateOtherAnswer(question) {

    let validate = false;

    let hasOtherAnswerCondition = 0;
    $(question.answerList).each(function (index, answer) {

        if (question.datas.answerType === "F") {

            hasOtherAnswerCondition++;
            if (answer.otherAnswer.val().trim() !== '') {
                validate = true;
            }
        }
        else if ((answer.datas.haveOtherAnswer === "Y") &&
            (answer.datas.needOtherAnswer === "Y") &&
            answer.answerCode.is(':checked') === true) {

            hasOtherAnswerCondition++;
            if (answer.otherAnswer.val().trim() !== '') {
                validate = true;
            }
        }
    });

    if (hasOtherAnswerCondition === 0) {
        validate = true;
    }

    showLog(true, 'validateOtherAnswerResult', { question: question }, { validate: validate });

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

            if (answer.answerCode.val().trim() !== '') {
                answerCode = answer.answerCode.val().trim();
            }
        }

        if (answerCode !== '') {
            answerCodeList.push(answerCode.toString());
        }
    });

    showLog(true, 'getAnswerCodeList', null, { answerCodeList: answerCodeList });

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

            if (answer.answerCode.val().trim() !== '') {
                answerScore = answer.datas.score;
            }
        }

        if (answerScore !== '') {
            answerScoreList.push(parseInt(answerScore));
        }
    });

    showLog(true, 'getAnswerScoreList', null, { answerScoreList: answerScoreList });

    return answerScoreList;
}

function showTip(footer, msg) {

    footer.show();
    footer.find('span').html(msg);
}

function showLog(show, method, information, result) {

    if (show === true) {
        let log = {
            show: show,
            method: method,
            information: information,
            result: result,
        };
        console.log(log);
    }
}

function getScore(questionList) {

    let totalScore = 0;
    let questEntity = {
        element: $('[id*=questEntity]'),
    };

    questEntity.datas = questEntity.element.data();

    let scoreList = [];
    if (questEntity.datas.needScore === "Y") {

        questionList.each(function () {

            let question = this;
            let answerScoreList = getAnswerScoreList(question);
            let answerScore;

            showLog(true, 'Math', null, {
                sum: sum(answerScoreList),
                max: Math.max.apply(Math, answerScoreList),
                min: Math.min.apply(Math, answerScoreList),
                avg: avg(sum(answerScoreList), answerScoreList.length),
            });

            if (question.datas.countScoreType === 1) {
                answerScore = sum(answerScoreList);
            }
            else if (question.datas.countScoreType === 2) {
                answerScore = Math.max.apply(Math, answerScoreList);
            }
            else if (question.datas.countScoreType === 3) {
                answerScore = Math.min.apply(Math, answerScoreList);
            }
            else if (question.datas.countScoreType === 4) {
                answerScore = avg(sum(answerScoreList), answerScoreList.length);
            }

            scoreList.push(answerScore);
        });
        showLog(true, 'scoreList', null, scoreList);

        if (questEntity.datas.scoreKind === 1) {
            totalScore = sum(scoreList);
        }
        showLog(true, 'questScore', null, questEntity.datas.questScore);

        if (totalScore > questEntity.datas.questScore) {
            totalScore = questEntity.datas.questScore;
        }
        showLog(true, 'totalScore', null, scoreList);
    }

    showLog(true, 'getScore', { questEntity, questEntity }, { totalScore: totalScore });

    return totalScore;
}

function sum(values) {

    let number = 0;
    $.each(values, function () {
        number += parseInt(this);
    });
    return number;
}

function avg(sum, length) {
    return (length > 0) ? (sum / length) : 0;
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

    showLog(true, compareSameArray, {
        conditionAnswerCodeList: conditionAnswerCodeList,
        answerCodeList: answerCodeList,
    }, { validate: validate });

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

    showLog(true, 'compareSingleAnswerCondition', {
        conditionAnswerCodeList: conditionAnswerCodeList,
        answerCodeList: answerCodeList,
    }, { validate: validate });

    return validate;
}





function compareArrayHasCommonValue(conditionAnswerCodeList, answerCodeList) {

    let validate = false;

    $.each(conditionAnswerCodeList, function (index, answerCode) {

        if ($.inArray(answerCode, answerCodeList) > -1) {
            validate = true;
        }
    });

    showLog({
        method: 'compareArrayHasCommonValue',
        information: {
            conditionAnswerCodeList: conditionAnswerCodeList,
            answerCodeList: answerCodeList,
            validate: validate
        }
    });
    return validate;
}

function testValueState() {
    var values = ['', 0, NaN, undefined, null, 1, ' '];
    $(values).each(function (idx, val) {
        console.log(idx, val, val ? true : false);
    });
}