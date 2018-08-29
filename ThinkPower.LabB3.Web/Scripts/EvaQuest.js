$(document).ready(function () {

    Registration();

});

function Registration() {

    $('.question-footer-submit').find('input').on('click', function () {
        $('.EvaQuestForm').submit();
    });

    $('.question-footer-test').find('input').on('click', function () {
        ValidateForm();
    });
}

function RemoveTip() {

    $('.question-subject-footer-tip span').each(function () {
        $(this).html('');
    });
}

function ValidateForm() {

    RemoveTip();

    const $question = {
        element: $('.question'),
        datas: $('.question').data(),
    };

    //var allSubjectArray = [];
    var $subject;
    var $answer;

    $('.question-subject').each(function () {

        $subject = {
            element: $(this),
            datas: $(this).data(),
            answerList: [],
            answerValidate: null,
            answeranswerValidateTip: null,
        };

        //QuestionDefine：
        //QuestionId                 題目編號
        //QuestionContent            題目內容描述
        //NeedAnswer                 是否必答
        //AllowNaCondition           可不做答條件
        //AnswerType                 答題型態
        //MinMultipleAnswers         複選最少答項數
        //MaxMultipleAnswers         複選最多答項數
        //SingleAnswerCondition      複選限制單一做答條件
        //CountScoreType             計分種類

        //QuestionAnswerDefine：
        //AnswerCode         答案代碼
        //AnswerContent      答案內容描敍
        //Memo               備註說明
        //HaveOtherAnswer    是否答題有輸入說明  Y:是 N:否
        //NeedOtherAnswer    答題說明是否為必填  Y:是 N:否
        //Score              計分分數 

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
        console.log([$subject.datas.questionid,$subject.datas.needanswer]);
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

                    ruleValidate.push(CompareArray($ruleInfo.values, $conditions.AnswerCode));
                });

                //{"Conditions":[{"QuestionId":"Q003","AnswerCode":["6"]},{"QuestionId":"Q005","AnswerCode":["1","3","5"]}]}
                //當題目編號Q003填答答案代碼【包含】6時，【或】題目編號Q005填答答案代碼包含1【及】3【及】5時符合。

                if ($.inArray(false, ruleValidate) >=0) {
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

function CheckValueExist(value) {
    return value;
}

function CheckValue() {
    var values = ['', 0, NaN, undefined, null, 1, ' '];
    $(values).each(function (idx, val) {
        console.log(idx, val, val ? true : false);
    });
}

function CompareArray(array1, array2) {
    return ($(array1).not(array2).length == 0) &&
        ($(array2).not(array1).length == 0);
}