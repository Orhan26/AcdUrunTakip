


function ShowSpinner() {
    $(".page-body").after('<div class="spinner"></div>')
}

function SelectAll(text) {
    if (text.text == "Tümünü seç") {
        $("input[type=checkbox]").each(function () {
            $("input[type=checkbox]").prop("checked", true);
        });
        var html = $("#selectAllBtn").html();
        html = html.replace("seç", "kaldır");
        $("#selectAllBtn").html(html);
    }
    else {
        var html = $("#selectAllBtn").html();
        html = html.replace("kaldır","seç");
        $("#selectAllBtn").html(html);
        $("input[type=checkbox]").each(function () {
            $("input[type=checkbox]").prop("checked", false);
        });
    }
};

function OpenSearch() {
    $("input[type=text]").toggle(1000);
    $('thead tr:eq(1) th').toggle();
};

function borderTime(txt) {
           setTimeout(function () {
            $(txt).removeClass('RedBorder');
        }, 3000);   
}
function HideSpinner() {
    $(".spinner").remove()
}

function isEmpty(val) {
    return val == undefined
        || val == "undefined"
        || val == null
        || val == "null"
        || val == '00000000-0000-0000-0000-000000000000'
        || (val == '' && typeof val !== 'boolean')
        || val == "0001-01-01 00:00:00"
        || val == "1800-01-01 00:00:00"
        || (Array.isArray(val) && val.length == 0)
        || (val instanceof jQuery) && val.length === 0;
}

function isNotEmpty(val) {
    return !isEmpty(val);
}

function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
        }
    }
};

//toLocaleString("tr-TR") sonucu oluşan data için convert yapılacaktır
function LocalDateConvert(dateTime) {
    var dateArr = dateTime.split(" ");
    if (dateArr.length < 2)
        return '';
    var date = dateArr[0].split(".");
    var time = dateArr[1];
    //yyyy-MM-dd HH:mm:ss
    var convertTime = date[2] + '-' + date[1] + '-' + date[0] + ' ' + time;
    return convertTime;

}

function NewGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}


function isValidEmail(emailAddress) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(emailAddress);
};

//mask:(999) 999-9999
function isValidPhone(phone) {
    var phoneResult = /\(?([0-9]{3})\)?([-]?)([0-9]{3})\2([0-9]{4})/;
    var digits = phone.replace(/\D/g, "");
    return (digits.match(phoneResult) !== null);
}