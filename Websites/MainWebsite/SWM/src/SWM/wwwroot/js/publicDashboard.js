﻿(function publicDashboard() {
    "use strict";
    var dates = [];
    var monthLabels = [];
    var products = [];
    var max_x_label = 8;
    var monthNames = [
                         "January", "February", "March",
                         "April", "May", "June", "July",
                         "August", "September", "October",
                         "November", "December"
    ];
    //Angular part
    //Defining a Module
    angular.module("public", []);
    //getting instance of module and adding controller to the modules
    angular.module("public").controller("chartsController", ["$scope", "$http", chartsController]);
    function chartsController($scope, $http) {
        $scope.isBusyChart1 = true;
        $http.get("/api/user_dates").then(function (response) {
            dates = response.data;
            for (var i = 0; i < dates.length; i++) {
                var date = new Date(dates[i])
                monthLabels.push(monthNames[date.getMonth()] + " " + date.getFullYear());
            }
            
            //decide min and max bound for charts
            if (dates.length >= max_x_label)
                var min_x = dates.length - max_x_label;
            else
                var min_x = 0;
            var max_x = dates.length;
            $scope.monthLabels = monthLabels;

            $scope.chart1StartDate = monthLabels[min_x];
            $scope.chart1EndDate = monthLabels[max_x - 1];

            $scope.displayChart1();
        }, function (error) {
            $scope.chart1Error = "Unable to display chart";
            $scope.isBusyChart1 = false;
        });

        $scope.displayChart1 = function () {
            if ($scope.chart1StartDate == $scope.chart1EndDate)
                return $scope.chart1Error = "Start month and end month must not have same value";
            var max_x, min_x;
            for (var i = 0; i < monthLabels.length; i++) {
                if (monthLabels[i] === $scope.chart1StartDate)
                    min_x = i;
                else if (monthLabels[i] === $scope.chart1EndDate)
                    max_x = i;
            }
            if (min_x > max_x)
                return $scope.chart1Error = "Start month must come before end month";
            $scope.isBusyChart1 = true;
            var startDate, endDate;
            for (var i = 0; i < dates.length; i++) {
                if (monthLabels[i] == $scope.chart1StartDate)
                    startDate = new Date(dates[i]);
                if (monthLabels[i] == $scope.chart1EndDate)
                    endDate = new Date(dates[i]);
            }
            $http.get("/api/product_info_month_wise/" + (startDate.getMonth() + 1) + "/" + startDate.getFullYear() + "/" + (endDate.getMonth() + 1) + "/" + endDate.getFullYear()).then(function (response) {
                $scope.isBusyChart1 = false;
                $scope.chart1Error = draw_chart1($scope.chart1StartDate, $scope.chart1EndDate, response.data);
            }, function (error) {
                $scope.isBusyChart1 = false;
                $scope.chart1Error = "Unable to display chart";
            });
        }
    }

    function draw_chart1(startDate, endDate, data) {
        var max_x, min_x;
        for (var i = 0; i < monthLabels.length; i++) {
            if (monthLabels[i] === startDate)
                min_x = i;
            else if (monthLabels[i] === endDate)
                max_x = i;
        }

        //credit to Christian Zosel on stack overflow. converts json data into chart data original was in ES6 converted to ES5
        function _toConsumableArray(arr) { if (Array.isArray(arr)) { for (var i = 0, arr2 = Array(arr.length) ; i < arr.length; i++) { arr2[i] = arr[i]; } return arr2; } else { return Array.from(arr); } }

        //credit to Christian Zosel on stack overflow. converts json data into chart data
        var uniq = function uniq(a) {
            return [].concat(_toConsumableArray(new Set(a)));
        };
        var flatten = function flatten(a) {
            return [].concat.apply([], a);
        };

        // step 1: find the distinct dates: ["2016-05-01T00:00:00", ... ]
        var dates = data.map(function (e) {
            return e.date;
        });

        // step 2: find the distinct labels: [Apple, Mango, ... ]
        var labels = uniq(flatten(data.map(function (e) {
            return e.productInformation;
        })).map(function (e) {
            return e.productName;
        }));

        // step 3: map the labels to entries containing their data by searching the original data array
        var result = labels.map(function (label) {
            return {
                data: dates.map(function (date) {
                    var hit = data.find(function (e) {
                        return e.date === date;
                    }).productInformation.find(function (p) {
                        return p.productName === label;
                    });
                    return hit ? hit.totalWeight : 0;
                })
            };
        });
        var chartLabels = [];
        for (var i = min_x; i <= max_x; i++) {
            chartLabels.push(monthLabels[i])
        }

        var datas = [];
        for (var i = 0; i < result[0].data.length; i++) {
            var total = 0;
            for (var j = 0; j < result.length; j++) {
                total += result[j].data[i];
            }
            datas.push(total);
        }


        var barChartData = {
            labels: chartLabels,
            datasets: [{
                label: "Weight in grams",
                data: datas,
                backgroundColor: "#ff7272"
            }]
        };
        var chartOptions = {
            maintainAspectRatio: false,
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Months'
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Weight In Grams'
                    }
                }]
            },
            legend: {
                display: false
            },
            tooltips: {
                enabled: true,
                mode: 'single',
                callbacks: {
                    label: function (tooltipItems, data) {
                        if (tooltipItems.xLabel <= 1000)
                            return tooltipItems.xLabel + ' gm';
                        else
                            return tooltipItems.xLabel / 1000 + ' kg';
                    }
                },
                displayColors: false
            }
        }

        $('#chart1div').empty();
        $('#chart1div').append('<canvas id="chart1" style="height:300px;" height="300"></canvas>');
        var ctx = document.getElementById("chart1").getContext("2d");
        var chart1 = new Chart(ctx, {
            type: 'bar',
            data: barChartData,
            options: chartOptions
        });
        return "";
    }
}
)();