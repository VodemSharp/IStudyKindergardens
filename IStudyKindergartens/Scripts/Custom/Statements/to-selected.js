$(".toSelected").on("click", function () {
    var current = this;
    if ($(current).hasClass("btn-primary")) {
        $(current).removeClass("btn-primary");
        $(current).addClass("btn-tumblr");
        $(current).html("Забрати заявку з вибраних");
        $(current).parent().parent().children('form').children('.labelToSelected').parent().children('input').click();
    }
    else {
        $(current).removeClass("btn-tumblr");
        $(current).addClass("btn-primary");
        $(current).html("Добавити заявку у вибрані");
        $(current).parent().parent().children('form').children('.labelFromSelected').parent().children('input').click();
    }
});