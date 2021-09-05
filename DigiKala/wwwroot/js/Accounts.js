function SendSmsAndMailCode() {
    var fd = new FormData();
    fd.append("mobile", document.getElementById("mobile").value);
    fd.append("mail", document.getElementById("mail").value);
    $.ajax({
        type: "Post",
        url: "/Accounts/ActivateStore",
        data: fd,
        processData: false,
        contentType: false
    })
        .success(function (result) {
            toastr.success(data.message)
        })
        .error(function (result) {
            toastr.error(data.message);
        });
}
function SendSmsCode() {
    var fd = new FormData();
    fd.append("mobile", document.getElementById("mobile").value);
    $.ajax({
        type: "Post",
        url: "/Accounts/Activate",
        data: fd,
        processData: false,
        contentType: false
    })
        .success(function (result) {
            toastr.success(data.message)
        })
        .error(function (result) {
            toastr.error(data.message);
        });
}
