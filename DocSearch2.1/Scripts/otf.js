$(function () {

    //submits the search and date filter form
    var ajaxFormSubmit = function () {

        var $form = $(this);
        $category = $(".active a").data('subclass');
        $docType = $(".active a").data('subclass-title');

        if ($category != undefined && $docType != undefined) {
            var options = {
                url: $form.attr("action"),//maybe add to this to check for attributes somehow
                type: $form.attr("method"),
                data: $form.serialize() + "&subNav=" + $category + "&" + "prevNav=" + $docType //or add to the data somehow
            };
        } else {
            var options = {
                url: $form.attr("action"),//maybe add to this to check for attributes somehow
                type: $form.attr("method"),
                data: $form.serialize()//or add to the data somehow
            };
        }

        $.ajax(options).done(function (data) {

            var $target = $($form.attr("data-otf-target"));
            var $newHtml = $(data);

            $target.replaceWith($newHtml);
            $newHtml.children("table").effect("highlight");
            postNavbar();
            updateCurrentCount();
        });

        return false;
    };

    //pagination effect
    var getPage = function () {

        var $a = $(this); //this is the anchor

        if ($a.parent('li').hasClass('disabled') == true) {
            return false;
        }

        var options = {
            url: $a.attr("href"),
            data: $("form").serialize(),
            type: "get"
        };

        $.ajax(options).done(function (data) {
            var target = $a.parents("div.pagedList").attr("data-otf-target");
            $(target).replaceWith(data);
            //postNavbar();
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

    function createMiscTable(link) {
        var options = {
            url: link.attr("action"),
            type: link.attr("method"),
            data: link.serialize()
        };

        $.ajax(options).done(function (data) {

            var id = link.attr("data-otf-target");

            var $target = $(id);
            var $newHtml = $(data);
            $newHtml.attr("id", id.substring(1, id.length));

            $target.replaceWith($newHtml);
        })
    }

    function destroyMiscTable(link) {
        var id = link.attr("data-otf-target");
        var $target = $(id);

        $target.empty();
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
    $("#body").on("click", ".pagination a", getPage);

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

    $("#body").on("click", ".miscTableLink", function () {

        var $link = $(this);

        $link.toggleClass("showData");

        if ($link.hasClass("showData")) {
            createMiscTable($link);
        } else {
            destroyMiscTable($link);
        }
        return false;
    });

    $('select[name^="IssueYearMinRange"]').change(function () {
        var value = this.value;

        $('select[name^="IssueYearMinRange"] option').removeAttr('selected');
        $('select[name^="IssueYearMinRange"] option[value="' + value + '"]').attr("selected", "selected");
    });

    $('select[name^="IssueYearMaxRange"]').change(function () {
        var value = this.value;

        $('select[name^="IssueYearMaxRange"] option').removeAttr('selected');
        $('select[name^="IssueYearMaxRange"] option[value="' + value + '"]').attr("selected", "selected");
    });
});