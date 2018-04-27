$('.div-hide').on("click", function () {
    if ($(this).children('button').children('span').hasClass('glyphicon-plus')) {
        $(this).children('button').children('span').removeClass("glyphicon-plus");
        $(this).children('button').children('span').addClass("glyphicon-minus");
    }
    else {
        $(this).children('button').children('span').removeClass("glyphicon-minus");
        $(this).children('button').children('span').addClass("glyphicon-plus");
    }
});