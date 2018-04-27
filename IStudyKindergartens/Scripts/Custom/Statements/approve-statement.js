$(".approveStatement").on("click", function () {
    var current = this;
    $(current).parent().parent().parent().parent().css("display", "none");
    $(current).parent().parent().children('form').children('.labelApproveStatement').parent().children('input').click();
});