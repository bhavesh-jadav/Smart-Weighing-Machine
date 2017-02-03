function user_dashboard(username) {
    "use strict";
    var chart1data = [];
    var chart2data = [];

    var chart2Labels = [];
    var products = [];
    
    var monthNames = [
                          "January", "February", "March",
                          "April", "May", "June", "July",
                          "August", "September", "October",
                          "November", "December"
    ];

    //Angular part
    //Defining a Module
    angular.module("app-dashboard", []);
    //getting instance of module and adding controller to the modules
    angular.module("app-dashboard").controller("chartsController", ["$scope", "$http", chartsController]);

    function chartsController($scope, $http) {

        //getting chart1
        $scope.isBusyChart1 = true;
        $http.get("/api/" + username + "/product_info").then(function (response) {
            $scope.data = response.data;
            for (var i = 0; i < $scope.data.length; i++) {
                chart1data.push($scope.data[i]);
            }
            $scope.isBusyChart1 = false;
            draw_chart1();
        }, function (error) {
            $scope.chart1Error = "Unable to display chart"
        });

        //getting chart2
        $scope.isBusyChart2 = true;
        $http.get("/api/" + username + "/product_info_month_wise").then(function (response) {
            $scope.data = response.data;
            for (var i = 0; i < $scope.data.length; i++) {
                chart2data.push($scope.data[i]);
            }
            if (chart2data.length >= 6)
                var min_x = chart2data.length - 6;
            else
                var min_x = 0;
            var max_x = chart2data.length;
            for (var i = 0; i < chart2data.length; i++) {
                var date = new Date(chart2data[i].date)
                chart2Labels.push(monthNames[date.getMonth()] + " " + date.getFullYear());
            }
            populateProductsArray(chart2data);
            $scope.products = products;
            $scope.chart3ProductName = products[0];
            $scope.chart3StartDate = chart2Labels[min_x];
            $scope.chart3EndDate = chart2Labels[max_x - 1];

            $scope.chart2StartDate = chart2Labels[min_x];
            $scope.chart2EndDate = chart2Labels[max_x - 1];
            $scope.chart2Dates = chart2Labels;

            $scope.isBusyChart2 = false;
            $scope.chart2Error = draw_chart2($scope.chart2StartDate, $scope.chart2EndDate);
            $scope.chart3Error = draw_chart3($scope.chart3ProductName, $scope.chart3StartDate, $scope.chart3EndDate);
            
        }, function (error) {
            $scope.chart2Error = "Unable to display chart"
        });
        $scope.displayChart2 = function () {
            $scope.chart2Error = draw_chart2($scope.chart2StartDate, $scope.chart2EndDate);
        };
        $scope.displayChart3 = function () {
            $scope.chart3Error = draw_chart3($scope.chart3ProductName, $scope.chart3StartDate, $scope.chart3EndDate);
        };
    };

    //single product monthly data chart
    function draw_chart3(productName, startDate, endDate) {
        if (startDate == endDate)
            return "Start month and end month must not have same value";

        var max_x, min_x;

        for (var i = 0; i < chart2Labels.length; i++) {
            if (chart2Labels[i] === startDate)
                min_x = i;
            else if (chart2Labels[i] === endDate)
                max_x = i;
        }
        if (min_x > max_x)
            return "Start month must come before end month"
        var chartLabels = [];
        for (var i = min_x; i <= max_x; i++) {
            chartLabels.push(chart2Labels[i])
        }

        var productData = []
        var j;
        for (var i = 0; i < chart2data.length; i++) {
            for (j = 0; j < chart2data[i].productInformation.length; j++) {
                if (chart2data[i].productInformation[j].productName == productName) {
                    productData.push(chart2data[i].productInformation[j].totalWeight);
                    break;
                }
            }
            if (j == chart2data[i].productInformation.length)
                productData.push(0);
        }
        productData = productData.slice(min_x, max_x + 1);
        var result = [{
            label: productName,
            data: productData,
            backgroundColor: "#f7ab40"
        }];

        var barChartData = {
            labels: chartLabels,
            datasets: result
        };
        var chartOptions = {
            responsive: true,
            legend: {
                display: true,
                position: 'top'
            },
            scales: {
                xAxes: [{
                    display: true,
                    stacked: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Months'
                    }
                }],
                yAxes: [{
                    display: true,
                    stacked: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Weight In Grams'
                    }
                }]
            }
        }

        $('#chart3div').empty();
        $('#chart3div').append('<canvas id="chart3" height=400></canvas>');
        var ctx = document.getElementById("chart3").getContext("2d");
        var chart3 = new Chart(ctx, {
            type: 'line',
            data: barChartData,
            options: chartOptions
        });
        return "";
    }

    //all products monthly data chart
    function draw_chart2(startDate, endDate) {

        if (startDate == endDate)
            return "Start month and end month must not have same value";

        var max_x, min_x;

        for (var i = 0; i < chart2Labels.length; i++) {
            if (chart2Labels[i] === startDate)
                min_x = i;
            else if (chart2Labels[i] === endDate)
                max_x = i;
        }
        if(min_x > max_x)
            return "Start month must come before end month"

        //credit to Christian Zosel on stack overflow. converts json data into chart data
        const uniq = a =>[...new Set(a)]
        const flatten = a =>[].concat.apply([], a)

        // step 1: find the distinct dates: ["2016-05-01T00:00:00", ... ]
        var dates = chart2data.map(function (e) {
            return e.date;
        });

        // step 2: find the distinct labels: [Apple, Mango, ... ]
        var labels = uniq(flatten(chart2data.map(function (e) {
            return e.productInformation;
        })).map(function (e) {
            return e.productName;
        }));

        // step 3: map the labels to entries containing their data by searching the original data array
        var result = labels.map(function (label) {
            return {
                label: label,
                data: dates.map(function (date) {
                    var hit = chart2data.find(function (e) {
                        return e.date === date;
                    }).productInformation.find(function (p) {
                        return p.productName === label;
                    });
                    return hit ? hit.totalWeight : 0;
                }).slice(min_x, max_x + 1),
                backgroundColor: getRandomColor()
            };
        });

        var chartLabels = [];
        for (var i = min_x; i <= max_x; i++) {
            chartLabels.push(chart2Labels[i])
        }
        var barChartData = {
            labels: chartLabels,
            datasets: result
        };
        var chartOptions = {
            responsive:true,
            legend: {
                display: true,
                position: 'top'
            },
            scales: {
                xAxes: [{
                    display: true,
                    stacked: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Months'
                    }
                }],
                yAxes: [{
                    display: true,
                    stacked: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Weight In Grams'
                    }
                }]
            }
        }
        $('#chart2div').empty();
        $('#chart2div').append('<canvas id="chart2" height=400></canvas>');
        var ctx = document.getElementById("chart2").getContext("2d");
        var chart2 = new Chart(ctx, {
            type: 'bar',
            data: barChartData,
            options: chartOptions
        });
        return "";
    }

    //product to weight chart
    function draw_chart1() {
        var chartLabels = []
        var chartData = []
        for (var i = 0; i < chart1data.length; i++) {
            chartLabels.push(chart1data[i].productName);
            chartData.push(chart1data[i].totalWeight);
        }
        var barChartCanvas = $("#chart1").get(0).getContext("2d");
        var barChartData = {
            labels: chartLabels,
            datasets: [{
                label: "Weight in grams",
                data: chartData,
                backgroundColor: "#3c8dbc"
            }]
        };
        var chartOptions = {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Products'
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
                display:false
            }
        }
        var ctx = document.getElementById("chart1").getContext("2d");
        var chart1 = new Chart(ctx, {
            type: 'bar',
            data: barChartData,
            options: chartOptions
        });
    }

    function getRandomColor() {
        var letters = '0123456789ABCDEF';
        var color = '#';
        for (var i = 0; i < 6; i++ ) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    }

    function populateProductsArray(data) {
        var index, max = 0;
        for (var i = 0; i < data.length; i++) {
            if (data[i].productInformation.length > max) {
                max = data[i].productInformation.length;
                index = i;
            }
        }
        for (var i = 0; i < max; i++) {
            products.push(data[index].productInformation[i].productName);
        }
    }
};