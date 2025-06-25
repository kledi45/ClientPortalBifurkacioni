function Redirect(url) {
    window.location.replace(url);
}

function Post(data, URL) {
    $("#overlay").css('display', 'block');
    return jQuery.ajax({
        url: URL,
        type: "POST",
        data: data,
        cache: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
            $("#overlay").css('display', 'none');
        },
        error: function () {
            $("#overlay").css('display', 'none');
        }
    });
}

function PostWithoutLoader(data, URL) {
    return jQuery.ajax({
        url: URL,
        type: "POST",
        data: data,
        cache: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
        },
        error: function () {
        }
    });
}

function PostArray(data, URL) {
    $("#overlay").css('display', 'block');
    return jQuery.ajax({
        url: URL,
        type: "POST",
        data: data,
        cache: false,
        responseType: "arraybuffer",
        dataType: 'blob',
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
            $("#overlay").css('display', 'none');
        },
        error: function () {
            $("#overlay").css('display', 'none');
        }
    });
}

function PostSync(data, URL) {
    $("#overlay").css('display', 'block');
    return jQuery.ajax({
        url: URL,
        type: "POST",
        data: data,
        cache: false,
        processData: false,
        contentType: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
            $("#overlay").css('display', 'none');
        },
        error: function () {
            $("#overlay").css('display', 'none');
        }
    });
}
function PostJson(data, URL) {
    return jQuery.ajax({
        url: URL,
        type: "POST",
        data: data,
        contentType: "application/json",
        cache: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
            //$("#preLoader").fadeOut('fast');
            window.location.reload();
        },
        error: function () {
            //$("#preLoader").fadeOut('fast');
        }
    });
}
function PostFormData(data, URL) {
    $("#overlay").css('display', 'block');
    return jQuery.ajax({
        url: URL,
        type: "POST",
        data: data,
        cache: false,
        contentType: false,
        processData: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
            $("#overlay").css('display', 'none');

        },
        error: function () {
            Redirect(urlLogin);
            $("#overlay").css('display', 'none');

        }
    });
}
function Get(data, URL) {
    $("#overlay").css('display', 'block');
    return jQuery.ajax({
        url: URL,
        type: "GET",
        data: data,
        cache: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function (e) {
            $("#overlay").css('display', 'none');
        },
        error: function (e) {
            $("#overlay").css('display', 'none');
        }
    });
}

function GetWithJSON(data, URL) {
    
    $("#overlay").css('display', 'block');
    return jQuery.ajax({
        url: URL,
        type: "GET",
        dataType: "jsonp",
        data: data,
        cache: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
            $("#overlay").css('display', 'none');
        },
        error: function () {
            $("#overlay").css('display', 'none');
        }
    });
}

function GetWithoutLoader(data, URL) {
    return jQuery.ajax({
        url: URL,
        type: "GET",
        data: data,
        cache: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
           // $("#preloader").fadeOut('fast');
        },
        error: function () {
          //  $("#preloader").fadeOut('fast');
        }
    });
}
function GetSync(data, URL) {
    $("#overlay").css('display', 'block');
    return jQuery.ajax({
        url: URL,
        type: "GET",
        data: data,
        cache: false,
        async: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
            $("#overlay").css('display', 'none');
        },
        error: function () {
            $("#overlay").css('display', 'none');
        }
    });
}
function GetPartial(URL) {
    $("#overlay").css('display', 'block');
    return jQuery.ajax({
        url: URL,
        type: "GET",
        cache: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
            $("#overlay").css('display', 'none');
        },
        error: function () {
            $("#overlay").css('display', 'none');
        }
    });
}

function GetPartialWithoutStopLoader(URL) {

    return jQuery.ajax({
        url: URL,
        type: "GET",
        cache: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        error: function () {
            $("#overlay").css('display', 'none');
        }
    });
}

function GetEmpty(URL) {
    $("#overlay").css('display', 'block');
    return jQuery.ajax({
        url: URL,
        type: "GET",
        cache: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
            $("#overlay").css('display', 'none');
        },
        error: function () {
            $("#overlay").css('display', 'none');
        }
    });
}

function GetEmptySync(URL) {
    $("#overlay").css('display', 'block');
    return jQuery.ajax({
        url: URL,
        type: "GET",
        cache: false,
        async: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
            $("#overlay").css('display', 'none');
        },
        error: function () {
            $("#overlay").css('display', 'none');
        }
    });
}

function GetEmptyWithLoader(URL) {
    $("#overlay").css('display', 'block');
    return jQuery.ajax({
        url: URL,
        type: "GET",
        cache: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
            //$("#preloader").fadeOut('fast');
            $("#overlay").css('display', 'none');
        },
        error: function () {
            $("#overlay").css('display', 'none');
        }
    });
}

function DeleteEmpty(URL) {
    $("#overlay").css('display', 'block');
    return jQuery.ajax({
        url: URL,
        type: "DELETE",
        cache: false,
        statusCode: {
            401: function () {
                Redirect(urlLogin);
            }
        },
        complete: function () {
            $("#overlay").css('display', 'none');
        },
        error: function () {
            $("#overlay").css('display', 'none');
        }
    });
}

function showPartial(url, content) {
    $("#preloader").fadeIn("fast");
    var content = $("#" + content);
    content.fadeOut("fast", function () {
        content.html("");
        GetPartial(url).done(function (result) {
            content.html(result);
            content.fadeIn();
        });
    });
};

function loadFirstPartial(url, content) {
    var content = $("#" + content);
    content.fadeOut("fast", function () {
        GetEmpty(url).done(function (result) {
            content.html(result);
            content.fadeIn();
        });
    });
};


//function PostAsync(data, URL) {
//    return jQuery.ajax({
//        url: URL,
//        type: "POST",
//        data: data,
//        cache: false,
//        statusCode: {
//            401: function () {
//                Redirect(urlLogin);
//            }
//        },
//        complete: function () {
//            $("#preloader").fadeOut('fast');
//        },
//        error: function () {
//            $("#preloader").fadeOut('fast');
//        }
//    });
//}