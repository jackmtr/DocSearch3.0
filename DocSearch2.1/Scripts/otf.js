$(function () {
    //**GLOBAL VARIABLES
    var editList = [];

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
        var id = $a.attr('id') + 'a';

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

            var imgWidth = document.getElementById('dynamic').clientWidth;

            img.css({ 'position': 'absolute', 'left': positionLeft - imgWidth, 'top': positionTop });
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

        //var newHeight = $('#public_table').height() + $('#form-div').height() + $('#status-bar').height() + $('.public_name_id').height();
        var newHeight = $('#form-div').height() + $('#public_table').height();
        var screenHeight = $(window).height() * 0.90;

        if (newHeight < screenHeight) {
            $('#public_navigation').css('height', screenHeight);
        } else {
            $('#public_navigation').css('height', newHeight);
        };
    }

    function persistEditCheckList() {
        var table = $('#public_table_inner table tbody');

        table.find('.main-rows').each(function (i, el) {

            var $this = $(this);
            var $tds = $(this).find('td');
            documentId = $tds.eq(11).text().trim();

            if ($.inArray(documentId, editList) >= 0) {

                $("#" + documentId + "edit").prop('checked', true);
            }
        });
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


    //image previewer off event, seems to work better with more opputunities to destroy itself
    $("#body").on("mouseleave", ".preview_image", function () {
        losePreview();
    });

    //$("#body").on("mouseleave", ".preview", function () {
    //    losePreview();
    //});


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
        persistEditCheckList();
    });

    $('#editList').on("click", function () {

        if (editList.length < 1) {
            alert('No documents have been selected.');

            return false;
        }

        //jQuery.ajaxSettings.traditional = true
        //localStorage.setItem("editList", JSON.stringify(editList));

        //$.get('/Admin/Edit', { vals: editList }, function (data) { })
        var options = {
            url: "/Admin/Edit",
            type: "get",
            traditional: true,
            data: {
                //EditList : JSON.stringify(editList)
                //EditList: editList.toString()
                EditList: editList
            }
            //,
            //success: function () {
            //    alert('success');
            //    $('.edit-issue').datepicker();
            //},
            //ajaxComplete: function () {
            //    alert('ajaxComplete');
            //    $('.edit-issue').datepicker();
            //}
        };


        $.ajax(options).done(function (data) {
            //alert('ajax done');
            $('#main-row').replaceWith($(data));
            //$('.edit-issue').datepicker();
            $(".edit-issue").each(function () {
                var displayedDate = $(this).val();
                var date = new Date(displayedDate);
                $(this).datepicker({ dateFormat: 'dd M yy'});
                //$(this).datepicker().datepicker("setDate", date);

            });
        });

        return false;
    });


    $("#body").on("change", "#public_table input[type=checkbox]", function () {
        if ($(this).prop('checked')) {

            editList.push($(this).val().trim());
        } else {
            var removeItem = $(this).val();
            editList.splice($.inArray(removeItem, editList), 1);
        }
    });

    //$('#public_table input[type=checkbox]').on("change", function () {

    //    //var found = $.inArray()

    //    if ($(this).prop('checked')) {

    //        editList.push($(this).val());
    //    } else {
    //        var removeItem = $(this).val();
    //        editList.splice($.inArray(removeItem, editList), 1);
    //    }
    //});

    $('#text-fill').textfill({ widthOnly: true, minFontPixels: 4, innerTag: "span" }); //trying to ensure the status line will shrink if needed

    //$(".edit-issue").datepicker();

    $(".edit-issue").datepicker();

    //if (!Modernizr.inputtypes.date) {

    //    $(".edit-issue").each(function () {
    //        var displayedDate = $(this).val();
    //        var date = new Date(displayedDate);
    //        $(this).datepicker().datepicker("setDate", date);
    //    });
    //}

    //$("body").on("change", ".edit-issue", function () {
    //    alert('hi');
    //            var displayedDate = $(this).val();
    //            var date = new Date(displayedDate);
    //            $(this).datepicker().datepicker("setDate", date);
    //});

    //$("#main-row").on("change", function () {
    //    $(this).datepicker();
    //        $(".edit-issue").each(function () {
    //            var displayedDate = $(this).val();
    //            var date = new Date(displayedDate);
    //            $(this).datepicker().datepicker("setDate", date);
    //        });
    //});
});