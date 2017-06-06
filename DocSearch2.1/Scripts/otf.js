﻿$(function () {
    //**FUNCTIONS

    //submits the search and date filter form asynchronously
    var ajaxFormSubmit = function () {

        var $pageSize = $('#pageSize option:selected').val();
        var $form = $('form');

        $category = $(".active a").data('subclass');
        $docType = $(".active a").data('subclass-title');

        if ($category != undefined && $docType != undefined) {
            var options = {
                url: $form.attr("action"),//maybe add to this to check for attributes somehow
                type: $form.attr("method"),
                data: $form.serialize() + "&subNav=" + $category + "&prevNav=" + $docType + "&pageSize=" + $pageSize //or add to the data somehow
            };
        } else {
            var options = {
                url: $form.attr("action"),//maybe add to this to check for attributes somehow
                type: $form.attr("method"),
                data: $form.serialize() + "&pageSize=" + $pageSize//or add to the data somehow
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

    //pagination function
    var getPage = function () {

        var $pageSize = $('#pageSize option:selected').val(); 
        var $a = $(this); //this is the anchor

        if ($a.parent('li').hasClass('disabled') == true) {
            return false;
        }

        var options = {
            url: $a.attr("href"),
            data: $("form").serialize() + "&pageSize=" + $pageSize,
            type: "get"
        };

        $.ajax(options).done(function (data) {
            var target = $a.parents("div.pagedList").attr("data-otf-target");
            $(target).replaceWith(data);
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

    //function to create the misc table and append it to the table
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
            $newHtml.addClass("misc_table");

            $target.replaceWith($newHtml);
        })
    }

    //function to destroy the misc table
    function destroyMiscTable(link) {
        var id = link.attr("data-otf-target");
        var $target = $(id);

        $target.empty();
    }

    function adjustSideBanner() {
        var newHeight = $('#public_table').height() + $('#form-div').height() + $('#status-bar').height() + $('.public_name_id').height();
        var screenHeight = $(window).height() * 0.90;

        if (newHeight < screenHeight) {
            $('#public_navigation').css('height', screenHeight);
        } else {
            $('#public_navigation').css('height', newHeight);
        };
    }


    //**EVENTS

    //will adjust left banner bar on load to dynamically match table size
    adjustSideBanner();

    //will load the user to have focus inside search bar
    $('#searchInputBox').focus();

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

    //doesnt seem to be needed
    //allows category list item hover to highlight every doctype in it
    //$(".category_nav > div > a").hover( function () {
    //    $(this).parent().toggleClass("nav_cate_hover");
    //});

    //event for click on 'MORE', which brings up a misc table
    $("#body").on("click", ".miscTableLink", function () {

        var $link = $(this);

        $link.parents('tr').siblings().find('a').each(function () {
            destroyMiscTable($(this));
            $(this).removeClass('showData').closest('tr').removeClass("misc_table");
        });

        $link.toggleClass("showData").closest('tr').toggleClass("misc_table");

        if ($link.hasClass("showData")) {
            createMiscTable($link);
        } else {
            destroyMiscTable($link);
        }

        return false;
    });

    //event for selecting a min start year from 'Starting Year' dropdown
    $('select[name^="IssueYearMinRange"]').change(function () {
        var value = this.value;

        $('select[name^="IssueYearMinRange"] option').removeAttr('selected');
        $('select[name^="IssueYearMinRange"] option[value="' + value + '"]').attr("selected", "selected");
    });

    //event for selecting a min start year from 'Ending Year' dropdown
    $('select[name^="IssueYearMaxRange"]').change(function () {
        var value = this.value;

        $('select[name^="IssueYearMaxRange"] option').removeAttr('selected');
        $('select[name^="IssueYearMaxRange"] option[value="' + value + '"]').attr("selected", "selected");
    });

    //event for selecting a 'records per page' from dropdown
    $('#pageSize').change(function () {
        var value = this.value;

        $('select[name^="pageSize"] option').removeAttr('selected');
        $('select[name^="pageSize"] option[value="' + value + '"]').attr("selected", "selected");
        ajaxFormSubmit();
    });

    
    //event for clicking the backspace or enter key in select scenerios
    $(document).bind("keydown", function (e) {

        var rx = /INPUT|SELECT|TEXTAREA/i; //will be used to check which markup you are in while clicking on backspace

        if (e.which == 8) { // 8 is 'backspace' in ASCII
            if (rx.test(e.target.tagName) && e.target.id == "formSubmitId" || !rx.test(e.target.tagName) || e.target.disabled || e.target.readOnly) {
                
                $('#clearButton').click();
                $('#searchInputBox').focus();
                e.preventDefault();
            }
        } else if (e.which == 13) { // 13 is 'enter' in ASCII
            if (e.target.id != "searchInputBox") {

                $('#formSubmitId').click();
                e.preventDefault();
            }
        }
    });

    $(document).ajaxComplete(function () {

        adjustSideBanner();
    });
});