﻿var postNavbar = function () {

    $className = $('.active').children("a").data("subclass");
    $classNameTitle = $('.active').children("a").data("subclass-title");

    //first if statement checks if $className has a value or is undefined
    if ($className) {
        if ($classNameTitle == "Correspondence") {
            $('#public_table').removeClass().addClass("correspondence");
        } else if ($classNameTitle == "Declaration/Endorsement") {
            $('#public_table').removeClass().addClass("declaration");
        } else {
            $('#public_table').removeClass().addClass($className);
        }
    }

    //need to check for form submit, then not do clearFields
    if ($(this).hasClass('navLink') || $(this).hasClass('button')) {

        clearFields();
    }

    updateCurrentCount();
}

//function to clear table/navbar/form to look like original content
function clearFields(id) {

    $form = $('form');
    $form.find("input[type=search]").val("");

    $form.find(".form-inputs select option").removeAttr('selected');

    $form.find("#IssueYearMinRange option:eq(0)").prop("selected", true);
    $form.find("#IssueYearMaxRange option:eq(0)").prop("selected", true);

    //may want to be more specific with check //having issues with this since updating the reset button
    if (id == "clearButton") {
        $form.find("#fromYear option:eq(0)").prop("selected", true);
        $(".active").removeClass('active');
    }

    $('#searchInputBox').focus();

    //need to figure out how to update status bar years
    //maybe change jquery connection for year status bar to look for selected option instead of current style
}

function updateCurrentCount() {
    //alert('updateCurrentCount is run');
    var currentCount = $('#public_table').data('currentrecord');
    var currentLowYear = $("select[name = 'IssueYearMinRange']:not(:disabled)").val();
    var currentHighYear = new Date().getFullYear();

    if ($("select[name = 'IssueYearMaxRange']:not(:disabled)")[0]) { //confirms in custom range

        if (currentLowYear == "") {
            currentLowYear = $("select[name = 'IssueYearMinRange']:not(:disabled) option:nth-child(2)").text();
        }

        if ($("select[name = 'IssueYearMaxRange']:not(:disabled)").val() != "") {
            currentHighYear = $("select[name = 'IssueYearMaxRange']:not(:disabled)").val();
        }
    }

    if (currentLowYear == -100) {
        currentLowYear = $("select[name = 'IssueYearMinRange']:disabled option:nth-child(2)").text();
    }
    else if (currentLowYear < 0) {
        currentLowYear = (new Date().getFullYear() + parseInt(currentLowYear)); //yearInput would be a negative value
    }

    if (currentLowYear > 0) {
        //takes user input

        $('#currentLowYear').text(currentLowYear);
    } else {

        //defaults back to original low
        currentLowYear = $('#IssueYearMinRange option:eq(1)').val();
        $('#currentLowYear').text(currentLowYear);
    }

    if (currentHighYear > 0) {

        $('#currentHighYear').text(currentHighYear);
    } else {

        currentHighYear = $('#IssueYearMaxRange option:last').val();
        $('#currentHighYear').text(currentHighYear);
    }

    $('#currentCount').text(currentCount);
}

function rememeberSort($this) {
    var ascending = '@ViewData["SortOrder"]';

    if (ascending == "True") {
        //not sure why $(this) had issues and couldnt replicate the effect I wanted
        $("#" + $this.children('i').attr('id')).removeClass("fa-sort").addClass("fa-sort-desc").parent('A').css('text-decoration', 'underline');
    } else {
        $("#" + $this.children('i').attr('id')).removeClass("fa-sort").addClass("fa-sort-asc").parent('A').css('text-decoration', 'underline');
    }
}

$(function () {

    //**GLOBAL VARIABLES
    var editList = [];
    var IssueYearMinRange = 1;

    $.validator.addMethod(
        "regex",
        function (value, element, regexp) {
            var re = new RegExp(regexp);
            return this.optional(element) || re.test(value);
        },
        "Please check your input."
    );

    $.validator.addMethod('daterange', function (value, element, arg) {
        if (this.optional(element) && !value) {
            return true;
        }

        var startDate = Date.parse(arg[0]),
            endDate = Date.parse(arg[1]),
            enteredDate = Date.parse(value);

        if (isNaN(enteredDate)) {
            return false;
        }

        return ((isNaN(startDate) || (startDate <= enteredDate)) &&
                 (isNaN(endDate) || (enteredDate <= endDate)));


    }, $.validator.format("Please specify a date between 01 Jan 1990 and today.")); //the daterange validator isnt working properly, but enough

    //**FUNCTIONS

    function postNavbar1() {

        $className = $('.active').children("a").data("subclass");
        $classNameTitle = $('.active').children("a").data("subclass-title");

        //first if statement checks if $className has a value or is undefined
        if ($className) {
            if ($classNameTitle == "Correspondence") {
                $('#public_table').removeClass().addClass("correspondence");
            } else if ($classNameTitle == "Declaration/Endorsement") {
                $('#public_table').removeClass().addClass("declaration");
            } else {
                $('#public_table').removeClass().addClass($className);
            }
        }

        //need to check for form submit, then not do clearFields
        if ($(this).hasClass('navLink') || $(this).hasClass('button')) {

            clearFields();
        }

        updateCurrentCount();
    }



    //submits the search and date filter form asynchronously
    var ajaxFormSubmit = function () {

        var $selectName = $("select[name = 'IssueYearMinRange']:not(:disabled)");

        if (!$selectName.val() && !$selectName.parent().next().find("select[name = 'IssueYearMaxRange']:not(:disabled)").val()) { //checks for inputs for both custom year dropdowns
            alert("Please input a Starting and Ending Year.");
            return false;
        } 

        var $form = $('form');

        $category = $(".active a").data('subclass');
        $docType = $(".active a").data('subclass-title');

        if ($category != undefined && $docType != undefined) {
            var options = {
                url: $form.attr("action"),//maybe add to this to check for attributes somehow
                type: $form.attr("method"),
                data: $form.serialize() + "&navBarGroup=" + $category + "&navBarItem=" + $docType
            };
        } else {
            var options = {
                url: $form.attr("action"),//maybe add to this to check for attributes somehow
                type: $form.attr("method"),
                data: $form.serialize()
            };
        }

        $.ajax(options).done(function (data) {

            var $target = $($form.attr("data-otf-target"));
            var $newHtml = $(data);

            $target.replaceWith($newHtml);
            $newHtml.children("table").effect("highlight");
            postNavbar();
            //updateCurrentCount(); may cause an issue removing this and relying on it within current postNavbar
        });

        return false;
    };

    //pagination function
    var getPage = function () {

        var $a = $(this); //this is the anchor
        var filter = "";

        if ($a.parent('li').hasClass('disabled') == true) {
            return false;
        }

        if ($('.fa-sort-asc')[0]){

            var thisA = $('.fa-sort-asc').parent('A');
            filter = $('.fa-sort-asc').attr('id');
        } else if ($('.fa-sort-desc')[0]) {

            var thisA = $('.fa-sort-desc').parent('A');
            filter = $('.fa-sort-desc').attr('id');
        } else {
            var thisA = $('#issue').parent('A');
        }

        var options = {
            url: $a.attr("href"),
            data: $("form").serialize() + "&filter=" + filter,
            type: "get"
        };

        //***CURRENTLY ONLY WORKING RIGHT WITH DOC+

        $.ajax(options).done(function (data) {
            var target = $a.parents("div.pagedList").attr("data-otf-target");
            $(target).replaceWith(data);
            rememeberSort(thisA);
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

            var positionLeft = e.clientX - 30;
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

        var formTableHeight = $('#form-div').height() + $('#public_table').height();
        var policyBarHeight = $('.div-table').height() + $('#policy_section').height();
        var screenHeight = $(window).height() * 0.90;
        var newHeight;

        (formTableHeight > policyBarHeight) ? newHeight = formTableHeight : newHeight = policyBarHeight;

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

    var modifyEditList = function () {

        if (editList.length < 1) {
            alert('No documents have been selected.');

            return false;
        }

        //JACKIE
        var options = {
            url: "/Admin/Edit",
            type: "get",
            traditional: true,
            data: {
                EditList: editList,
                publicId: $('#search').val()
            },
            cache: false
        };

        $.ajax(options).done(function (data) {

            $('#main-row').replaceWith($(data));

            $(".edit-issue").each(function () {

                var displayedDate = $(this).val();
                var date = new Date(displayedDate);
                $(this).datepicker({ dateFormat: 'dd M yy' });
            });

            $(".Correspondence").each(function () {

                $(this).prop('disabled',false);
            });
        });

        return false;
    }

    //**EVENTS

    //will adjust left banner bar on load to dynamically match table size
    adjustSideBanner();

    if ($('#issue')[0]) {
        var thisA = $('#issue').parent('A');
        rememeberSort(thisA);
    }


    //will load the user to have focus inside search bar
    $('#searchInputBox').focus();

    //event for form submit
    $("form[data-otf-ajax='true']").submit(ajaxFormSubmit);

    //event for pagination buttons
    $("#body").on("click", ".pagination a", getPage);


    //image previewer on event
    $("#body").on("mousedown", ".preview_image", function (e) { //mouseenter before

        $this = $(this).children("a");
        showPreview($this, e);
    });


    //image previewer off event, seems to work better with more opputunities to destroy itself
    $("#body").on("mouseup", ".preview_image", function (e) { //mouseleave before
        losePreview();
    });

    $("#body").on("click", ".preview", function () {
        return false;
    })

    //functionality of category/policy slider
    $('#category_policy_toggle').click(function () {
        $("#category_section, #policy_section").toggleClass("hidden");
    })

    //active navbar class addition ***MAYBE COMBINE WITH ANOTHER
    $(".nav_lists a").on("click", function () {
        $(".nav_lists").find(".active").removeClass("active");
        $(this).parent().addClass("active");
    });

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

    //may end up dropping pagination
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
        } else if (e.which == '192') { // `, this is used to toggle styling for navbar TEMP
            $('.nav_lists').toggleClass("test-style");
        }
        else if (e.which == '220') { // '\' button

            var $yearInputMin = $("select[name = 'IssueYearMinRange']:not(:disabled)").val();
            var $yearInputMax = new Date().getFullYear();

            if ($("select[name = 'IssueYearMaxRange']:not(:disabled)")[0]) { //confirms in custom range

                if ($yearInputMin == "") {
                    $yearInputMin = $("select[name = 'IssueYearMinRange']:not(:disabled) option:nth-child(2)").text();
                }
                
                if ($("select[name = 'IssueYearMaxRange']:not(:disabled)").val() != "") {
                    $yearInputMax = $("select[name = 'IssueYearMaxRange']:not(:disabled)").val();
                }
            }

            if ($yearInputMin == -100) {
                $yearInputMin = $("select[name = 'IssueYearMinRange']:disabled option:nth-child(2)").text();
            }
            else if ($yearInputMin < 0) {
                $yearInputMin = (new Date().getFullYear() + parseInt($yearInputMin)); //yearInput would be a negative value
            }
        }
    });

    $(document).ajaxComplete(function () {

        adjustSideBanner();
        persistEditCheckList();

        if (!$('.fa-sort-asc')[0] && !$('.fa-sort-desc')[0]) { //will only run if sort wasnt run right before this request
            var thisA = $('#issue').parent('A');
            rememeberSort(thisA);
        }
        //might honestly be i need to add a parameter to ajaxComplete to know who called it.
    });

    $("#body").on("change", "#public_table input[type=checkbox]", function () {

        if ($(this).prop('checked')) {
            editList.push($(this).val().trim());
        } else {
            var removeItem = $(this).val();
            editList.splice($.inArray(removeItem, editList), 1);
        }


    });


    $(".edit-issue").datepicker({ dateFormat: 'dd M yy' }); //not sure if needed

    $('#clearButton').on("click", function () {

        $this = $(this);

        var options = {
            url: $this.attr('href'),
            data: "IssueYearMinRange =" + "2016", //not being sent to controller, but I need a data attribute it seems
            type: "GET",
        };


        $.ajax(options).done(function (data) {

            var $target = $('#public_table');
            var $newHtml = $(data);


            $target.replaceWith($newHtml);

            complete: updateCurrentCount();
            success: clearFields($this.prop('id'));      
        });

        return false;
    });

    var downloadAllDocuments = function () {

        var clientId = $(".public_name_id").html().match(/\d+/);

        $('#ClientId').val(clientId);

        $("#downloadForm").submit();

        return false;
    };

    $("#body").on("click", "#editOptionsSubmit", function () {

        $choice = $('#editOptions option:selected').text();

        if ($choice == "Edit These Files") {
            modifyEditList();
        } else if ($choice == "Download all Public Documents") {
            downloadAllDocuments();
        } else {
            alert("No option was selected");
        }

        return false;
    });

    $("#body").on("submit", "form#updateListSubmit", function (event) {

        $('form#updateListSubmit').validate();

        $('.edit-rows :not(.input_class) input').each(function () {

            $(this).prop("tagName");

            $(this).rules("add", {
                required: true,
            });

            $(".edit-issue").each(function () {

                var today = new Date();
                var dd = today.getDate();
                var mm = today.getMonth() + 1; //January is 0!
                var yyyy = today.getFullYear();

                if (dd < 10) {
                    dd = '0' + dd;
                }
                if (mm < 10) {
                    mm = '0' + mm;
                }

                var today = dd + '/' + mm + '/' + yyyy;

                $(this).rules("add", { regex: "^[0-3][0-9]\\s[A-Z][a-z]{2}\\s[1-2][0-9]{3}$" }),
                $(this).rules("add", {
                    daterange: ['01/01/1990', today], //Kinda working regex, today isnt today but a day is the near future
                });
            });
        });

        if ($("form#updateListSubmit").validate().form()) {
            console.log("validates");
        } else {
            console.log("does not validate");
            event.preventDefault();
        }
    });

    $("#body").on("click", ".selectAll", function () {

        if (!$(this).is(':checked')) {
            $("table.table tbody td:last-child() input:checkbox:checked").click();
        } else {
            $("table.table tbody td:last-child() input:checkbox:not(:checked)").click();
        }
    });

    //event for selecting a 'records per page' from dropdown
    $('#fromYear').change(function () {
        var value = this.value;

        $('select[name^="IssueYearMinRange"] option').removeAttr('selected');
        $('select[name^="IssueYearMinRange"] option[value="' + value + '"]').attr("selected", "selected");

        IssueYearMinRange = $('#fromYear option:selected').val();

        ajaxFormSubmit();
    });

    $('#customDates').on("click", function () {

        if ($('#customDates').is(':checked')) {
            $('#fromYear').prop("disabled", true);
            $('.customDates').children().prop("disabled", false).children().addBack().css("display", "block");
        } else {
            $('#fromYear').prop("disabled", false);
            $('.customDates').children().prop("disabled", true).children().addBack().css("display", "none");
        }
    });

    $('.navLink').on("click", function () {
        $this = $(this);

        var $selectName = $("select[name = 'IssueYearMinRange']:not(:disabled)");

        if (!$selectName.val()) {
            alert("Please input a Starting and Ending Year.");
            return false;
        }

        var options = {
            url: $this.attr('href'),
            data: "&IssueYearMinRange=" + $selectName.val(),
            type: "GET",
        };

        $.ajax(options).done(function (data) {

            var $target = $('#public_table');
            var $newHtml = $(data);

            $target.replaceWith($newHtml);

            complete: postNavbar();
            success: clearFields($this.prop('id'));
        });

        return false;
    });

    $("#body").on("click", "#allDocs", function () {
        $('#fromYear').val('-100');
    });
});