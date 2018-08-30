$(document).ready(function () {

    removeTip();
    binding();
});

function binding() {

    console.log('binding');
    $('#done').on('click', function () {
        $('#evaQuestForm').submit();
    });

    $('#test').on('click', function () {
        validate();
    });
}

function removeTip() {

    console.log('removeTip');
    $('[id*=footer]').each(function () {
        $(this).find('span').html('');
        $(this).hide();
    });
}

function getQuestionList() {

    console.log('getQuestionList');
    return $('[id*=quest]').map(function () {
        return {
            element: this,
            datas: $(this).data(),
            answerList: $(this).find('[id*=answer-]').map(function () {
                return {
                    element: this,
                    datas: $(this).data(),
                    //answerCode: $(this).find('[id*=answerCode]'),
                };
            }),
        };
    });
}

function validateRule(questionList) {

    console.log('validateRule');
    questionList.each(function () {

        let question = this;
        if (question.datas.needAnswer == "Y") {

            if (question.datas.allowNaCondition) {

                let validateResult = [];
                $(question.datas.allowNaCondition.Conditions).each(function () {

                    let condition = {
                        object: this,
                        answerCodeList: [],
                    };

                    condition.quetion = questionList.filter(function () {
                        return (this.datas.questionId == condition.object.QuestionId);
                    }).get()[0];

                    $(condition.quetion.answerList).each(function () {

                        let answer = this;
                        let answerCode;

                        if ((condition.quetion.datas.answerType == 'S') ||
                            (condition.quetion.datas.answerType == 'M')) {

                            if ($(answer.element).find('[id*=answerCode]').is(':checked')) {
                                answerCode = answer.datas.answerCode;
                            }

                        } else if (condition.quetion.datas.answerType == 'F') {

                            answerCode = $(answer.element).find('[id*=answerCode]').val();
                        }

                        if (answerCode) {
                            condition.answerCodeList.push(answerCode.toString());
                        }
                    });
                    
                    let result = compareArrayHasCommonValue(condition.object.AnswerCode, condition.answerCodeList);
                    validateResult.push(result);

                    console.log({
                        conditionAnswerCode: condition.object.AnswerCode.join(),
                        conditionAnswerList: condition.answerCodeList.join(),
                        id: condition.object.QuestionId,
                        compareArrayHasCommonValue: result,
                    });
                });
                //validateResult = [];

                if ($.inArray(true, validateResult) == -1) {
                    let footer = $(question.element).find('[id*=footer]');
                    footer.show();
                    footer.find('span').html("此題必須填答!");
                }

            } else {

            }
        }
    });
}

function validate() {

    console.log('validate');
    removeTip();

    var $subject;
    var $answer;

    var questionList = getQuestionList();
    //console.log(questionList);

    validateRule(questionList);
}



function history() {


    console.log(questionList);

    $('[id*=quest]').each(function () {

        $subject = {
            element: $(this),
            datas: $(this).data(),
            answerList: [],
            answerValidate: null,
            answeranswerValidateTip: null,
        };


        $subject.element.find('.question-answer').each(function () {

            $answer = {
                element: $(this),
                datas: $(this).data(),
                value: null,
            };

            if ($subject.datas.answertype == 'S') {

                $answer.value = $answer.element.find('.question-answer-code').find(':checked')
                    .map(function () { return $(this).data('answercode'); }).get();

            } else if ($subject.datas.answertype == 'M') {

                $answer.value = $answer.element.find('.question-answer-code').find(':checked')
                    .map(function () { return $(this).data('answercode'); }).get();

            } else if ($subject.datas.answertype == 'F') {

                $answer.value = $answer.element.find('.question-answer-code').find('input')
                    .map(function () { return $(this).val(); }).get();
            }

            $subject.answerList.push($answer);

            $answer = null;
        });

        debugger;
        //allSubjectArray.push($subject);
        console.log([$subject.datas.questionid, $subject.datas.needanswer]);
        if ($subject.datas.needanswer == 'Y') {
            if ($subject.datas.allownacondition) {

                //console.log({
                //    allSubjectArray: allSubjectArray,
                //    AllowNaCondition: $subject.datas.allownacondition,
                //    QuestionId: $subject.datas.questionid,
                //});
                debugger;
                var ruleValidate = [];

                $($subject.datas.allownacondition.Conditions).each(function () {

                    var $conditions = this;
                    var question = $(".question-subject[data-questionid='" +
                        this.QuestionId + "']");
                    var questionDatas = question.data();

                    var $ruleInfo = {
                        element: question,
                        datas: questionDatas,
                        values: null,
                    };

                    if ($ruleInfo.datas.answertype == 'S') {

                        $ruleInfo.values = $ruleInfo.element
                            .find('.question-answer-code').find(':checked')
                            .map(function () { return $(this).data('answercode').toString(); })
                            .get();

                    } else if ($ruleInfo.datas.answertype == 'M') {

                        $ruleInfo.values = $ruleInfo.element
                            .find('.question-answer-code').find(':checked')
                            .map(function () { return $(this).data('answercode').toString(); })
                            .get();

                    } else if ($ruleInfo.datas.answertype == 'F') {

                        $ruleInfo.values = $ruleInfo.element
                            .find('.question-answer-code').find('input')
                            .map(function () { return $(this).val().toString(); })
                            .get();
                    }

                    console.log({
                        nowQuestId: $subject.datas.questionid,
                        rule: $conditions,
                        ruleQuestionId: $conditions.QuestionId,
                        ruleAnswerCode: $conditions.AnswerCode,
                        getAnswerCode: $ruleInfo.values,
                    });

                    ruleValidate.push(compareArrayHasCommonValue($ruleInfo.values, $conditions.AnswerCode));
                });

                //{"Conditions":[{"QuestionId":"Q003","AnswerCode":["6"]},{"QuestionId":"Q005","AnswerCode":["1","3","5"]}]}
                //當題目編號Q003填答答案代碼【包含】6時，【或】題目編號Q005填答答案代碼包含1【及】3【及】5時符合。

                if ($.inArray(false, ruleValidate) >= 0) {
                    $subject.answerValidateTip = "此題必須填答!";
                }

            } else {

                $subject.answerValidate = $subject.answerList
                    .map(function (answer) { return answer.value; })
                    .map(function (value) {
                        return value.toString().trim().length > 0;
                    });

                if ($.inArray(true, $subject.answerValidate) == -1) {
                    $subject.answerValidateTip = "此題必須填答!";
                }
            }
        }

        //$subject.datas.minmultipleanswers = 2;
        //$subject.datas.maxmultipleanswers = 3;

        console.log({
            answerList: $subject.answerList,
            answerValidate: $subject.answerValidate,
            min: $subject.datas.minmultipleanswers,
            max: $subject.datas.maxmultipleanswers,
            valueExistCount: $subject.answerValidate ?
                $subject.answerValidate.filter(function (exist) { return exist == true; }).length :
                -99,
        });

        if (!$subject.answerValidateTip &&
            $subject.datas.answertype == 'M' &&
            $subject.datas.minmultipleanswers &&
            $subject.answerValidate) {

            var valueExistCount = $subject.answerValidate
                .filter(function (exist) { return exist == true; }).length;

            if (valueExistCount < $subject.datas.minmultipleanswers) {
                $subject.answerValidateTip = '此題至少須勾選' +
                    $subject.datas.minmultipleanswers + '個項目!';
            }

        }

        if (!$subject.answerValidateTip &&
            $subject.datas.answertype == 'M' &&
            $subject.datas.maxmultipleanswers &&
            $subject.answerValidate) {

            var valueExistCount = $subject.answerValidate
                .filter(function (exist) { return exist == true; }).length;

            if (valueExistCount > $subject.datas.maxmultipleanswers) {
                $subject.answerValidateTip = '此題至多僅能勾選' +
                    $subject.datas.maxmultipleanswers + '個項目!';
            }
        }

        if (!$subject.answerValidateTip &&
            $subject.datas.singleanswercondition) {
            //TODO
            if (true) {
                $subject.answerValidateTip = "此題僅能勾選1個項目!";
            }
        }

        if ($subject.answerValidateTip) {
            $subject.element.find('.question-subject-footer-tip span').html($subject.answerValidateTip);
        }
        ///

        $subject = null;
    });
}

function checkValue() {
    var values = ['', 0, NaN, undefined, null, 1, ' '];
    $(values).each(function (idx, val) {
        console.log(idx, val, val ? true : false);
    });
}

function compareArrayHasCommonValue(a, b) {
    let array1 = $(a).toArray();
    let array2 = $(b).toArray();

    let result = false;
    $.each(array1, function (index, value) {

        if ($.inArray(value, array2) > -1) {
            result = true;
        }
    });
    return result;
}