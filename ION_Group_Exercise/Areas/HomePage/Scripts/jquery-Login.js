$(function () {

    //Validate user inputs and restrict characters -> / \ , . ^
    //A bootstrap popover message is also seen when an invalid
    //character is inputted
    $('#Username,#Password').keyup(function (e) {
        if (!Login.isValid($(this).val())) {
            $(this).popover('show');
        }
        else {
            $(this).popover('hide');
        }
    });


    //Submit form / Login user
    $('#loginForm').on("submit", function (e) {
        e.preventDefault();

        $('#Username,#Password').popover('hide');

        //Validate form for any empty fields
        if (!Login.validateForm()) {
            return false;
        }

        var form = $('#loginForm').serialize();

        //AJAX call to serve-side controller to validate login
        $.ajax({
            url: '/HomePage/HomePage/CheckLogin',
            type: 'POST',
            data: form,
            beforeSend: function () {
                $('#btnLogin').prop("disabled", true);
                $('#loader').show();
            },
            success: function (data) {
                setTimeout(() => {
                    if (data.success == false) {
                        $('#loader').hide();
                        $('#btnLogin').prop("disabled", false);
                        $("#err").html(data.errorMessage).show();
                    }
                    else {

                        $("#pageDiv").html('');
                        $("#login").modal('hide');
                        $("#pageDiv").html(data);
                        $(".modal-backdrop").remove();
                        $('#loader').hide();
                    }
                }, 3000); //timeout/delay between user login/logouts requests
            },
            error: function (data) {
                alert(data.responseText);
            }
        });
    });

    //Logout user
    $('#btnLogout').on("click", function (e) {
        e.preventDefault();
        $.ajax({
            url: '/HomePage/HomePage/Logout',
            beforeSend: function () {
                $('#btnLogout').prop("disabled", true);
                $('#loaderLogout').show();
                $('#btnLogin').hide();
            },
            success: function (data) {
                setTimeout(() => {
                    $("#pageDiv").html('');
                    $('#loaderLogout').hide();
                    $(".modal-backdrop").remove();
                    $("#pageDiv").html(data);
                    $('#btnLogin').show();
                }, 3000); //timeout/delay between user login/logouts requests
            },
            error: function (data) {
                alert(data.responseText);
            }
        });
    });

    var Login = {

        isValid: (str) => {
            if (str.indexOf('/') > -1 ||
                str.indexOf('\\') > -1 ||
                str.indexOf(',') > -1 ||
                str.indexOf('^') > -1 ||
                str.indexOf('.') > -1) {
                return false;
            }
            else {
                return true;
            }
        },

        validateForm: () => {
            var check = true;

            if ($("#Username").val() != "" && $("#Username").val() != null) {
                if (!Login.isValid($("#Username").val())) {
                    $("#err").html('Username or Password must not contain these characters \ / , . ^').show();
                    $("#Username").css("border", "1.5px solid red");
                    check = false;
                }
            }
            else {
                check = false;
                $("#err").html('Username or Password cannot be null').show();
                $("#Username").css("border", "1.5px solid red");
            }

            if ($("#Password").val() != "" && $("#Password").val() != null) {
                if (!Login.isValid($("#Password").val())) {
                    $("#err").html('Username or Password must not contain these characters \ / , . ^').show();
                    $("#Password").css("border", "1.5px solid red");
                    check = false;
                }
            }
            else {
                check = false;
                $("#err").html('Username or Password cannot be null').show();
                $("#Password").css("border", "1.5px solid red");
            }

            return check;
        }
    }

})