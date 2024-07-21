﻿(function ($) {
    "use strict";

    // Spinner
    var spinner = function () {
        setTimeout(function () {
            if ($('#spinner').length > 0) {
                $('#spinner').removeClass('show');
            }
        }, 1);
    };
    spinner();

    
    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $('.back-to-top').fadeIn('slow');
        } else {
            $('.back-to-top').fadeOut('slow');
        }
    });
    $('.back-to-top').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 1500, 'easeInOutExpo');
        return false;
    });


    //// Sidebar Toggler
    $('.sidebar-toggler').click(function () {
        $('.sidebar, .content').toggleClass("open");
        return false;
    });

    
    // Progress Bar
    $('.pg-bar').waypoint(function () {
        $('.progress .progress-bar').each(function () {
            $(this).css("width", $(this).attr("aria-valuenow") + '%');
        });
    }, { offset: '80%' });


    // Calender
    $('#calender').datetimepicker({
        inline: true,
        format: 'L'
    });


    // Testimonials carousel
    $(".testimonial-carousel").owlCarousel({
        autoplay: true,
        smartSpeed: 1000,
        items: 1,
        dots: true,
        loop: true,
        nav: false
    });

    var dataListElement = document.getElementById('dataList');
    var dataListElementTrans = document.getElementById('dataListTransaction');
    var dataListElementPost = document.getElementById('dataListPost');
    var dataListElementComp = document.getElementById('dataListCompleted');
    var dataListTotalPostExchange = document.getElementById('dataListTotalPostExchange');
    var dataListTotalPostSale = document.getElementById('dataListTotalPostSale');

    var dataList = $(dataListElement).data("list");
    var dataListTrans = $(dataListElementTrans).data("list");
    var dataListPost = $(dataListElementPost).data("list");
    var dataListComp = $(dataListElementComp).data("list");
    var dataListPostExchange = $(dataListTotalPostExchange).data("list");
    var dataListPostSale = $(dataListTotalPostSale).data("list");

    var dataListTransName = $(dataListElementTrans).data("name");
    var dataListPostName = $(dataListElementPost).data("name");
    var dataListCompName = $(dataListElementComp).data("name");
    var dataListPostExchangeName = $(dataListTotalPostExchange).data("name");
    var dataListPostSaleName = $(dataListTotalPostSale).data("name");

    var ctx1 = $("#worldwide-sales").get(0).getContext("2d");

    var myChart1 = new Chart(ctx1, {
        type: "bar",
        data: {
            labels: dataList,

            datasets: [{
                label: dataListPostName,
                data: dataListPost,
                backgroundColor: "rgba(0, 156, 255, .7)"
            },
            {
                label: dataListTransName,
                data: dataListTrans,
                backgroundColor: "rgba(0, 156, 255, .5)"
            },
            {
                label: dataListCompName,
                data: dataListComp,
                backgroundColor: "rgba(0, 156, 255, .3)"
            }
            ]
        },
        options: {
            responsive: true
        }
    });

    // Salse & Revenue Chart
    var ctx2 = $("#salse-revenue").get(0).getContext("2d");
    var myChart2 = new Chart(ctx2, {
        type: "line",
        data: {
            labels: dataList,
            datasets: [{
                label: dataListPostExchangeName,
                data: dataListPostExchange,
                backgroundColor: "rgba(0, 156, 255, .5)",
                fill: true
            },
            {
                label: dataListPostSaleName,
                data: dataListPostSale,
                backgroundColor: "rgba(0, 156, 255, .3)",
                fill: true
            }
            ]
        },
        options: {
            responsive: true
        }
    });

    var dataDayListElement = document.getElementById('dataDayList');
    var dataDayListElementTrans = document.getElementById('dataDayListTransaction');
    var dataDayListElementPost = document.getElementById('dataDayListPost');
    var dataDayListElementComp = document.getElementById('dataDayListCompleted');
    var dataDayListTotalPostExchange = document.getElementById('dataDayListTotalPostExchange');
    var dataDayListTotalPostSale = document.getElementById('dataDayListTotalPostSale');

    var dataDayList = $(dataDayListElement).data("list");
    var dataDayListTrans = $(dataDayListElementTrans).data("list");
    var dataDayListPost = $(dataDayListElementPost).data("list");
    var dataDayListComp = $(dataDayListElementComp).data("list");
    var dataDayListPostExchange = $(dataDayListTotalPostExchange).data("list");
    var dataDayListPostSale = $(dataDayListTotalPostSale).data("list");

    var dataDayListTransName = $(dataDayListElementTrans).data("name");
    var dataDayListPostName = $(dataDayListElementPost).data("name");
    var dataDayListCompName = $(dataDayListElementComp).data("name");
    var dataDayListPostExchangeName = $(dataDayListTotalPostExchange).data("name");
    var dataDayListPostSaleName = $(dataDayListTotalPostSale).data("name");
    var ctx1Day = $("#worldwide-salesDay").get(0).getContext("2d");

    var myChart1 = new Chart(ctx1Day, {
        type: "bar",
        data: {
            labels: dataDayList,

            datasets: [{
                label: dataDayListPostName,
                data: dataDayListPost,
                backgroundColor: "rgba(0, 156, 255, .7)"
            },
            {
                label: dataDayListTransName,
                data: dataDayListTrans,
                backgroundColor: "rgba(0, 156, 255, .5)"
            },
            {
                label: dataDayListCompName,
                data: dataDayListComp,
                backgroundColor: "rgba(0, 156, 255, .3)"
            }
            ]
        },
        options: {
            responsive: true
        }
    });
    var ctx2Day = $("#salse-revenueDay").get(0).getContext("2d");

    // Salse & Revenue Chart
    var myChart2 = new Chart(ctx2Day, {
        type: "line",
        data: {
            labels: dataDayList,
            datasets: [{
                label: dataDayListPostExchangeName,
                data: dataDayListPostExchange,
                backgroundColor: "rgba(0, 156, 255, .5)",
                fill: true
            },
            {
                label: dataDayListPostSaleName,
                data: dataDayListPostSale,
                backgroundColor: "rgba(0, 156, 255, .3)",
                fill: true
            }
            ]
        },
        options: {
            responsive: true
        }
    });

})(jQuery);

