
$(document).ready(function () {

    Get(null, '/CRM/GetRequestComplaint').done(function (result) {
        $("#dropdowntree-for-request").kendoDropDownTree({
            placeholder: "Zgjedh tipin e kërkesës/ankesës ...",
            height: "auto",
            dataSource: result,
            filter: "startswith",
            clearButton: false,

        });
    });


    $.ajax({
        url: $('#read-users-for-request').val(),
        dataType: "json", // "jsonp" is required for cross-domain requests; use "json" for same-domain requests
        success: function (result) {
            $("#internalUsers-for-request").kendoDropDownList({
                dataSource: result,
                optionLabel: "Zgjedh zyrtarin...",
                dataTextField: "text",
                dataValueField: "value",
                filter: "contains"

            }).data("kendoDropDownList");
        },
        error: function (result) {
        }
    });

    $("#territory-id").kendoDropDownList({
        dataTextField: "text",
        optionLabel: "Perzgjedhni ...",
        dataValueField: "value",
        dataSource: {
            transport: {
                read: {
                    dataType: "json",
                    url: $("#territories-url").val(),
                }
            }
        }
    });


    $("#street-id").kendoDropDownList({
        autoBind: false,
        cascadeFrom: "Territory",
        dataTextField: "text",
        dataValueField: "value",
        optionLabel:"Perzgjedhni ...",
        dataSource: {
            type: "json",
            serverFiltering: true,
            transport: {
                read: {
                    url: $("#streets-url").val() + "?territory=" + $('#territory-id').val(),
                    type: "post",
                    dataType: "json",
                    data: function () {
                        return { territory: $('#territory-id').val() }
                    }
                }
            }
        }
    }).data("kendoDropDownList");






});



function validateFields2() {
    ;
    var internalUser = $("#internalUsers-for-request").data("kendoDropDownList").value();
    var caseType = $("#dropdowntree-for-request").data("kendoDropDownTree").value().id;
    var property = $("#property").data("kendoDropDownList") != null ? $("#property").data("kendoDropDownList").value() : null;

    if (caseType != null && caseType != "") {
        $("#dropdowntree-for-request-error").hide();

  
            if (internalUser != null && internalUser != "") {
                $("#internalUsers-for-request-error").hide();
            } else {
                $("#internalUsers-for-request-error").show();
                return false;
            }
        
    }
    else {
        $("#dropdowntree-for-request-error").show();
        return false;
    }
    var checkPersonalNumber;
    if ($("#personalNumber").val() != '') {
        ;
        var value1 = $("#personalNumber").val();
        var checkDigit = value1.slice(-1);
        var newValue = value1.slice(0, -1);
        ;
        var sum = 0;
        var l = newValue.length;
        var j = 2;
        for (i = l - 1; i >= 0; i--) {
            var temp = newValue[i];
            sum += temp * j;
            j++;
            if (j > 7) {
                j = 2;
            }
        }
        var reminder = sum % 11;
        var newDigit = 11 - reminder;

        checkPersonalNumber = parseInt((newDigit.toString()).slice(-1)) == parseInt(checkDigit);
        if (checkPersonalNumber)
            $("#personalNumbervalid-error").hide();
        else {
            $("#personalNumbervalid-error").show();
            return false;
        }

        
        ;

    }
    if (!checkPersonalNumber) {
        
        

    }

    var fields = ["firstName", "lastName", "personalNumber", "phoneNumber", "emailAddress","territory-id", "street-id"];
        for (var i = 0; i < fields.length; i++) {
            if ($("#" + fields[i]).val() != null && $("#" + fields[i]).val() != "") {
                $("#" + fields[i] + "-error").hide();
            } else {
                $("#" + fields[i] + "-error").show();
                return false;
            }
        }




    

    return true;
}

$("#saveForm2").click(function () {
    ;
    if (validateFields2()) {
        var object = {
            IdClientCaseType: $("#dropdowntree-for-request").data("kendoDropDownTree").value().id,
            IdEmployee: $("#internalUsers-for-request").data("kendoDropDownList").value(),
            Comment: $("#editor").val(),
            Name: $("#firstName").val(),
            SurName: $("#lastName").val(),
            PersonalNumber: $("#personalNumber").val(),
            PhoneNumber: $("#phoneNumber").val(),
            EmailAddress: $("#emailAddress").val(),
            IDStreet: $("#street-id").val(),
            IDTerritory: $("#territory-id").val(),
            DoorNumber: $("#doorNumber").val(),
            FloorNumber: $("#floorNumber").val(),
            EntryNumber: $("#entryNumber").val(),
            IDCaseSource: 3
        };

        Post({ data: object }, '/CRM/AddRequest').done(function (result) {
            window.location.reload();
        });
    } else {
        return;
    }

});



