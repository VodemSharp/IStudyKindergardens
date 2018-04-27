$(".toRecycleBin").on("click", function () {
    var current = this;
    $(current).parent().parent().parent().parent().css("display", "none");
    $(current).parent().parent().children('form').children('.labelToRecycleBin').parent().children('input').click();
});