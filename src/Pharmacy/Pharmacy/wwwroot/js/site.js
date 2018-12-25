$(document).ready(function () {
    var onlyExist = $("#OnlyExistence");
    if (onlyExist != null || onlyExist != undefined) {
        onlyExist.change(function (e) {
            if (this.checked) {
                $("#OnlyExistence").val("True");
            } else {
                $("#OnlyExistence").val("False");
            }
        });
    }
});
