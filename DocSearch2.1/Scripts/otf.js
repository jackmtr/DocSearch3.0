$(function () {

    //submits the search and date filter form
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
            updateCurrentCount();
        });

        return false;
    };

    //pagination effect
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
            postNavbar();
        });

        return false;
    };

    //function to preview image (jpeg)
    function showPreview($this, e) {
        var $a = $this;
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

    //function to destroy preview image
    function losePreview() {
        $('.previewImg').remove();
    }


    if (!Modernizr.inputtypes.date) {

        $(".datefield").each(function () {
            var displayedDate = $(this).val();
            var date = new Date(displayedDate);
            $(this).datepicker().datepicker("setDate", date);
        });
    }

    //event for form submit
    $("form[data-otf-ajax='true']").submit(ajaxFormSubmit);

    //event for pagination buttons
    $("#body").on("click", ".pagedList a", getPage);

    //image previewer on event
    $("#body").on("mouseenter", ".preview_image", function (e) {

        $this = $(this).children("a");
        showPreview($this, e);
    });

    //image previewer off event
    $("#body").on("mouseleave", ".preview_image", function () {
        losePreview();
    });

    //functionality of category/policy slider
    $('#category_policy_toggle').click(function () {
        $("#category_section, #policy_section").toggleClass("hidden");
    })

    //active navbar class addition
    $(".nav_lists a").on("click", function () {
        $(".nav_lists").find(".active").removeClass("active");
        $(this).parent().addClass("active");
    });

    //allows category list item hover to highlight every doctype in it
    $(".category_nav > div > a").hover( function () {
        $(this).parent().toggleClass("nav_cate_hover");
    });
});