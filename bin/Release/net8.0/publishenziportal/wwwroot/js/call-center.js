var myInterval = null;
$(document).ready(function () {
    myInterval = setInterval(getCallFromClients, 5000);

    $('#share_files').on("shown.bs.modal", function () {
        myStopFunction();
    });
    $('#share_files').on("hide.bs.modal", function () {
        myInterval = setInterval(getCallFromClients, 5000);
    });
});
function getFormatedCallDate(date) {
    var dateFromCallCheck = new Date(date)

    return dateFromCallCheck.getDate() + "-" + dateFromCallCheck.getMonth() + "-" + dateFromCallCheck.getFullYear() + "-" + ('0' + dateFromCallCheck.getHours()).slice(-2) + ":" + ('0' + dateFromCallCheck.getMinutes()).slice(-2) + ' ' + (dateFromCallCheck.getHours() < 12 ? 'AM' : 'PM');;


}
function getCallFromClients() {
    $.ajax({
        url: $("#get-incoming-clients-url").val(),
        type: "GET",
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            if (data.id > 0) {
                //myStopFunction()
                $.get($("#get-recorded-calls").val(), function (dataFromCalls) {
                    
                    if (dataFromCalls) {
                        if ((getFormatedCallDate(dataFromCalls.date) == getFormatedCallDate(data.eventtime)) && data.cid_num == dataFromCalls.caller) {
                        }
                        else {
                            $("#phoneCallInline").val(data.cid_num);
                            $("#register-date").val(data.eventtime);
                            $("#register-reciver").val(data.cid_dnid);
                            $('#share_files').modal('show');
                            GetEmpty($("#call-center-home").val()).done(function (responseCallCenter) {
                                $("#call_center_modal").html(responseCallCenter);
                            });
                        }
                    }
                    else {
                        $("#phoneCallInline").val(data.cid_num);
                        $("#register-date").val(data.eventtime);
                        $("#register-reciver").val(data.cid_dnid);
                        $('#share_files').modal('show');
                        GetEmpty($("#call-center-home").val()).done(function (responseCallCenter) {
                            $("#call_center_modal").html(responseCallCenter);
                        });
                    }
                })


            }

        },
        complete: function (data) {
        },
        error: function (event) {
        }
    });
    //}
}

function myStopFunction() {
    clearInterval(myInterval);
}