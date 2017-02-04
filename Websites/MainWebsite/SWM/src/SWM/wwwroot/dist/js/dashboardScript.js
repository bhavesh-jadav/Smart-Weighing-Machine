function user_dashboard(username) {
    "use strict";
    var chart2data = [];
    var chart2Labels = [];
    var products = [];
    var locations = [];
    var max_x_label = 10;
    var locationsTotalWeight = [];
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
        
        //getting chart1 information
        $http.isBusyChart1 = true;
        $http.isBusyChart5 = true;
        $http.get("/api/" + username + "/location_info").then(function (response) {
            var data = response.data;
            for (var i = 0; i < data.length; i++) {
                locations.push(data[i].name);
                locationsTotalWeight.push(data[i].totalWeight);
            }
            if (locations.length > 1)
                locations.push("All");
            $scope.locations = locations;
            $scope.selectedLocation = locations[locations.length - 1];
            $scope.displayChart1();
            $http.isBusyChart5 = false;
            draw_chart5();
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
            if (chart2data.length >= max_x_label)
                var min_x = chart2data.length - max_x_label;
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

            $scope.chart4StartDate = chart2Labels[min_x];
            $scope.chart4EndDate = chart2Labels[max_x - 1];

            $scope.isBusyChart2 = false;
            $scope.chart2Error = draw_chart2($scope.chart2StartDate, $scope.chart2EndDate);
            $scope.chart3Error = draw_chart3($scope.chart3ProductName, $scope.chart3StartDate, $scope.chart3EndDate);
            $scope.chart4Error = draw_chart4($scope.chart4StartDate, $scope.chart4EndDate);
            
        }, function (error) {
            $scope.chart2Error = $scope.chart3Error = $scope.chart4Error = "Unable to display chart";
            $scope.isBusyChart2 = false;
        });
        $scope.displayChart1 = function () {
            var chart1data = [];
            $scope.isBusyChart1 = true;
            if ($scope.selectedLocation === locations[locations.length - 1]) {
                $http.get("/api/" + username + "/product_info").then(function (response) {
                    var data = response.data;
                    for (var i = 0; i < data.length; i++) {
                        chart1data.push(data[i]);
                    }
                    $scope.isBusyChart1 = false;
                    draw_chart1(chart1data);
                }, function (error) {
                    $scope.chart1Error = "Unable to display chart";
                });
            }
            else {
                for (var i = 0; i < locations.length; i++) {
                    if (locations[i] == $scope.selectedLocation) {
                        $http.get("/api/" + username + "/" + locations[i] + "/product_info").then(function (response) {
                            var data = response.data;
                            for (var i = 0; i < data.length; i++) {
                                chart1data.push(data[i]);
                            }
                            $scope.isBusyChart1 = false;
                            draw_chart1(chart1data);
                        }, function (error) {
                            $scope.chart1Error = "Unable to display chart"
                        });
                    }
                }
            }
        };
        $scope.displayChart2 = function () {
            $scope.chart2Error = draw_chart2($scope.chart2StartDate, $scope.chart2EndDate);
        };
        $scope.displayChart3 = function () {
            $scope.chart3Error = draw_chart3($scope.chart3ProductName, $scope.chart3StartDate, $scope.chart3EndDate);
        };
        $scope.displayChart4 = function () {
            $scope.chart4Error = draw_chart4($scope.chart4StartDate, $scope.chart4EndDate);
        };
    };

    //location wise total weight
    function draw_chart5() {
        var labels = [];
        var data = [];

        for (var i = 0; i < locations.length - 1; i++) {
            labels.push(locations[i]);
        }
        var barChartData = {
            labels: labels,
            datasets: [{
                data: locationsTotalWeight,
                backgroundColor: "#ff7272"
            }]
        };
        var chartOptions = {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Weight'
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Locations'
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
                        var weight = tooltipItems.yLabel;
                        if (weight <= 1000)
                            return tooltipItems.xLabel + ' gm';
                        else
                            return tooltipItems.xLabel / 1000 + ' kg';
                    }
                },
                displayColors: false
            }
        }

        var height = $('#locations').height() - 200;

        $('#chart5div').empty();
        $('#chart5div').append('<canvas id="chart5" style="height:' + height + '" height="' + height + '" width="787"></canvas>');
        var ctx = document.getElementById("chart5").getContext("2d");
        var chart4 = new Chart(ctx, {
            type: 'horizontalBar',
            data: barChartData,
            options: chartOptions
        });
    }

    //monthly total weight chart
    function draw_chart4(startDate, endDate) {

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
            return "Start month must come before end month";

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
                data: dates.map(function (date) {
                    var hit = chart2data.find(function (e) {
                        return e.date === date;
                    }).productInformation.find(function (p) {
                        return p.productName === label;
                    });
                    return hit ? hit.totalWeight : 0;
                }).slice(min_x, max_x + 1)
            };
        });

        var chartLabels = [];
        for (var i = min_x; i <= max_x; i++) {
            chartLabels.push(chart2Labels[i])
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
                        var weight = tooltipItems.yLabel;
                        if (weight <= 1000)
                            return tooltipItems.xLabel + ' gm';
                        else
                            return tooltipItems.xLabel / 1000 + ' kg';
                    }
                },
                displayColors: false
            }
        }

        $('#chart4div').empty();
        $('#chart4div').append('<canvas id="chart4" style="height: 250px;" height="250" width="787"></canvas>');
        var ctx = document.getElementById("chart4").getContext("2d");
        var chart4 = new Chart(ctx, {
            type: 'horizontalBar',
            data: barChartData,
            options: chartOptions
        });
        return "";
        
    }

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
            return "Start month must come before end month";
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
            backgroundColor: "#f7ab40",
            borderWidth: 5,
            borderColor: "#d68413",
            cubicInterpolationMode: "monotone"
        }];

        var barChartData = {
            labels: chartLabels,
            datasets: result
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
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            },
            legend: {
                display: false,
                labels: {
                    display: false
                }
            },
            tooltips: {
                enabled: true,
                mode: 'single',
                callbacks: {
                    label: function (tooltipItems, data) {
                        var weight = tooltipItems.yLabel;
                        if (weight <= 1000)
                            return tooltipItems.yLabel + ' gm';
                        else
                            return tooltipItems.yLabel / 1000 + ' kg';
                    }
                },
                displayColors: false
            }
        }

        $('#chart3div').empty();
        $('#chart3div').append('<canvas id="chart3" style="height:250px;" height="250"></canvas>');
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
        if (min_x > max_x)
            return "Start month must come before end month";

        function _toConsumableArray(arr) { if (Array.isArray(arr)) { for (var i = 0, arr2 = Array(arr.length) ; i < arr.length; i++) { arr2[i] = arr[i]; } return arr2; } else { return Array.from(arr); } }

        //credit to Christian Zosel on stack overflow. converts json data into chart data
        var uniq = function uniq(a) {
            return [].concat(_toConsumableArray(new Set(a)));
        };
        var flatten = function flatten(a) {
            return [].concat.apply([], a);
        };

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
            },
            tooltips: {
                enabled: true,
                mode: 'single',
                callbacks: {
                    label: function (tooltipItems, data) {
                        return data.datasets[tooltipItems.datasetIndex].label + ": " + tooltipItems.yLabel + ' gm';
                    }
                }
            }
        };

       
        $('#chart2div').empty();
        $('#chart2div').append('<canvas id="chart2" style="height:300px;" height="300"></canvas>');
        var ctx = document.getElementById("chart2").getContext("2d");
        var chart2 = new Chart(ctx, {
            type: 'bar',
            data: barChartData,
            options: chartOptions
        });
        return "";
    }

    //product to weight chart
    function draw_chart1(chart1data) {
        var chartLabels = []
        var chartData = []
        for (var i = 0; i < chart1data.length; i++) {
            chartLabels.push(chart1data[i].productName);
            chartData.push(chart1data[i].totalWeight);
        }
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
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            },
            legend: {
                display:false
            },
            tooltips: {
                enabled: true,
                mode: 'single',
                callbacks: {
                    label: function (tooltipItems, data) {
                        var weight = tooltipItems.yLabel;
                        if (weight <= 1000)
                            return tooltipItems.yLabel + ' gm';
                        else
                            return tooltipItems.yLabel/1000 + ' kg';
                    }
                },
                displayColors: false
            }
        }
        $('#chart1div').empty();
        $('#chart1div').append('<canvas id="chart1" style="height: 250px;" height="250" width="787"></canvas>');
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
}