$(function () {

    var ajaxFormSubmit = function () {
        var $form = $(this);
        var options = {
            url: $form.attr("action"),
            type: $form.attr("method"),
            data: $form.serialize()
        };

        $.ajax(options).done(function (data) {

            var $target = $($form.attr("data-otf-target"));
            var $newHtml = $(data);

            $target.replaceWith($newHtml);
            $newHtml.children("table").effect("highlight");
        });

        return false;
    };

    var getPage = function () {

        var $a = $(this); //this is the anchor

        var options = {
            url: $a.attr("href"),
            data: $("form").serialize(),
            type: "get"
        };

        $.ajax(options).done(function (data) {
            var target = $a.parents("div.pagedList").attr("data-otf-target");
            $(target).replaceWith(data);
        });
        return false;
    };

    var showPreview = function (e) {

        var $a = $(this);
        var id = $a.attr('id');

        var options = {
            url: $a.attr("href"),
            data: id,
            type: "get"
        };

        $.ajax(options).done(function (data) {

            var img = $('<img id="dynamic" class="previewImg">');

            var positionLeft = e.clientX;
            var positionTop = e.clientY;

            img.attr('src', options.url);
            img.appendTo('body');
            img.css({ 'position': 'absolute', 'left': positionLeft, 'top': positionTop });
        });
    }

    var losePreview = function () {
        
        $('#dynamic').remove();
    }
    

    if (!Modernizr.inputtypes.date) {

        $(".datefield").each(function () {
            var displayedDate = $(this).val();
            var date = new Date(displayedDate);
            $(this).datepicker().datepicker("setDate", date);
        });
    }

    //$("#category_list").menu();

    //$("#category_list").menu(
    //"focus", null, $("#category_list").menu().find(".ui-menu-item:last"));
    //$(menu).mouseleave(function () {
    //    menu.menu('collapseAll');
    //});

    //$("#category_list").accordion({
    //    collapsible: true,
    //    navigation: true,
    //    clearStyle: true,
    //    event: "mouseover"
    //});

    $("form[data-otf-ajax='true']").submit(ajaxFormSubmit);

    $("#body").on("click", ".pagedList a", getPage);

    $("#body").on({ click: function () { $(this).toggleClass("active"); }, mouseenter: showPreview, mouseleave: losePreview }, ".preview");

    $('#category_policy_toggle').click(function () {
        $("#category_section, #policy_section").toggleClass("hidden");
    })

});