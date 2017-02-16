function user_dashboard(username) {
    "use strict";
    var dates = [];
    var chart2data = [];
    var monthLabels = [];
    var products = [];
    var locations = [];
    var max_x_label = 8;
    var locationsTotalWeight = [];
    var monthNames = [
                          "January", "February", "March",
                          "April", "May", "June", "July",
                          "August", "September", "October",
                          "November", "December"
    ];

    //Angular part
    //Defining a Module
    angular.module("app-dashboard", ['emguo.poller']);
    //getting instance of module and adding controller to the modules
    angular.module("app-dashboard").controller("chartsController", ["$scope", "$http", chartsController]);
    angular.module("app-dashboard").controller("userController", ["$scope", "$http", "poller", userController]);

    function userController($scope, $http, poller) {

        $scope.gettingUserDetails = true;
        var myPoller = poller.get('/api/' + username, {
            catchError: true,
            delay: 10000
        });

        myPoller.promise.then(null, null, function (response) {
            if (response.data != null) {
                populateDashboard(response.data);
                $scope.userData = response.data;
                $scope.gettingUserDetails = false;
            } else {
                $('#errorMessage').append('<div class="alert alert-danger alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>There was a problem while getting latest data. Make sure you are connected to the internet and Make sure that it is working fine.</div>');
                $scope.gettingUserDetails = false;
                poller.stopAll();
            }
        });

        var populateDashboard = function (data) {
            var weight = 0.0;
            if (data.totalWeight >= 1000000) {
                weight = data.totalWeight / 1000000.0;
                $scope.unit = "Tonne(s)"
            }
            else if (data.totalWeight >= 1000) {
                weight = data.totalWeight / 1000.0;
                $scope.unit = "KG(s)"
            }
            else {
                weight = data.totalWeight;
                $scope.unit = "Gram(s)";
            }
            $scope.gettingUserDetails = false;
            setTimeout(function () {
                $('#TotalWeight').text(weight);
                $('#TotalProducts').text(data.totalProducts);
                $('#TotalLocation').text(data.totalLocation);

                if ($('#LastUpdatedProduct').text() != data.lastUpdatedProduct){
                    $("#LastUpdatedProduct").fadeToggle(function () {
                        $('#LastUpdatedProduct').text(data.lastUpdatedProduct);
                    });
                    $("#LastUpdatedProduct").fadeToggle();
                }
            }, 10);
        }
    }


    function chartsController($scope, $http) {

        //getting chart1 information and drawing chart 1 and 5
        $scope.isBusyChart1 = true;
        $scope.isBusyChart5 = true;
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
            $scope.isBusyChart5 = false;
            draw_chart5();
        }, function (error) {
            $scope.chart1Error = "Unable to display chart"
        });

        //getting user months and calling chart 2, 3 and 4 drawing funcitions
        $scope.showMonthlyCharts = true;
        $scope.chartClass = "col-md-6";
        $scope.isBusyChart2 = $scope.isBusyChart4 = $scope.isBusyChart3 = true;
        $http.get("/api/" + username + "/user_dates").then(function (response) {
            dates = response.data;
            if (dates.length > 1) {
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

                $scope.chart2StartDate = monthLabels[min_x];
                $scope.chart2EndDate = monthLabels[max_x - 1];

                //pupulate the product array for chart3
                $http.get("/api/" + username + "/product_info").then(function (response) {
                    populateProductsArray(response.data);
                    $scope.products = products;
                    $scope.chart3ProductName = products[0];
                    $scope.chart3StartDate = monthLabels[min_x];
                    $scope.chart3EndDate = monthLabels[max_x - 1];
                    $scope.displayChart3();
                }, function (error) {
                    $scope.chart3Error = "Unable to display chart";
                })

                $scope.chart4StartDate = monthLabels[min_x];
                $scope.chart4EndDate = monthLabels[max_x - 1];

                $scope.displayChart2();
                $scope.displayChart4();
            }
            else {
                $scope.showMonthlyCharts = false;
                $scope.chartClass = "col-md-12";
            }

        }, function (error) {
            $scope.chart2Error = $scope.chart3Error = $scope.chart4Error = "Unable to display chart";
            $scope.isBusyChart2 = $scope.isBusyChart3 = $scope.isBusyChart4 = false;
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
            if ($scope.chart2StartDate == $scope.chart2EndDate)
                return $scope.chart2Error = "Start month and end month must not have same value";
            var max_x, min_x;
            for (var i = 0; i < monthLabels.length; i++) {
                if (monthLabels[i] === $scope.chart2StartDate)
                    min_x = i;
                else if (monthLabels[i] === $scope.chart2EndDate)
                    max_x = i;
            }
            if (min_x > max_x)
            return $scope.chart2Error = "Start month must come before end month";

            $scope.isBusyChart2 = true;
            var startDate, endDate;
            for (var i = 0; i < dates.length; i++) {
                if (monthLabels[i] == $scope.chart2StartDate)
                    startDate = new Date(dates[i]);
                if (monthLabels[i] == $scope.chart2EndDate)
                    endDate = new Date(dates[i]);
            }
            $http.get("/api/" + username + "/product_info_month_wise/" + (startDate.getMonth()+1) + "/" + startDate.getFullYear() + "/" + (endDate.getMonth()+1) + "/" + endDate.getFullYear()).then(function (response) {
                $scope.isBusyChart2 = false;
                $scope.chart2Error = draw_chart2($scope.chart2StartDate, $scope.chart2EndDate, response.data);
            }, function (error) {
                $scope.isBusyChart2 = false;
                $scope.chart2Error = "Unable to display chart";
            });
        };
        $scope.displayChart3 = function () {
            if ($scope.chart3StartDate == $scope.chart3EndDate)
                return $scope.chart3Error = "Start month and end month must not have same value";
            var max_x, min_x;
            for (var i = 0; i < monthLabels.length; i++) {
                if (monthLabels[i] === $scope.chart3StartDate)
                    min_x = i;
                else if (monthLabels[i] === $scope.chart3EndDate)
                    max_x = i;
            }
            if (min_x > max_x)
                return $scope.chart3Error = "Start month must come before end month";

            $scope.isBusyChart3 = true;
            var startDate, endDate;
            for (var i = 0; i < dates.length; i++) {
                if (monthLabels[i] == $scope.chart3StartDate)
                    startDate = new Date(dates[i]);
                if (monthLabels[i] == $scope.chart3EndDate)
                    endDate = new Date(dates[i]);
            }
            $http.get("/api/" + username + "/product_info_month_wise/" + (startDate.getMonth() + 1) + "/" + startDate.getFullYear() + "/" + (endDate.getMonth() + 1) + "/" + endDate.getFullYear()).then(function (response) {
                $scope.isBusyChart3 = false;
                $scope.chart3Error = draw_chart3($scope.chart3ProductName, $scope.chart3StartDate, $scope.chart3EndDate, response.data);
            }, function (error) {
                $scope.isBusyChart3 = false;
                $scope.chart3Error = "Unable to display chart";
            });
        };
        $scope.displayChart4 = function () {
            if ($scope.chart2StartDate == $scope.chart2EndDate)
                return $scope.chart4Error = "Start month and end month must not have same value";
            var max_x, min_x;
            for (var i = 0; i < monthLabels.length; i++) {
                if (monthLabels[i] === $scope.chart2StartDate)
                    min_x = i;
                else if (monthLabels[i] === $scope.chart2EndDate)
                    max_x = i;
            }
            if (min_x > max_x)
                return $scope.chart4Error = "Start month must come before end month";

            $scope.isBusyChart4 = true;
            var startDate, endDate;
            for (var i = 0; i < dates.length; i++) {
                if (monthLabels[i] == $scope.chart4StartDate)
                    startDate = new Date(dates[i]);
                if (monthLabels[i] == $scope.chart4EndDate)
                    endDate = new Date(dates[i]);
            }
            $http.get("/api/" + username + "/product_info_month_wise/" + (startDate.getMonth()+1) + "/" + startDate.getFullYear() + "/" + (endDate.getMonth()+1) + "/" + endDate.getFullYear()).then(function (response) {
                $scope.isBusyChart4 = false;
                $scope.chart4Error = draw_chart4($scope.chart4StartDate, $scope.chart4EndDate, response.data);
            }, function (error) {
                $scope.isBusyChart4 = false;
                $scope.chart4Error = "Unable to display chart";
            });
        };
    };
    
    //monthly total weight chart
    function draw_chart5() {
        var labels = [];
        var data = [];

        if (locations.length != 1) {
            for (var i = 0; i < locations.length - 1; i++) {
                labels.push(locations[i]);
            }
        }
        else
            labels.push(locations[0]);
        var barChartData = {
            labels: labels,
            datasets: [{
                data: locationsTotalWeight,
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
                        if (tooltipItems.xLabel <= 1000)
                            return tooltipItems.xLabel + ' gm';
                        else
                            return tooltipItems.xLabel / 1000 + ' kg';
                    }
                },
                displayColors: false
            }
        }

        //var height = $('#locations').height() - Math.floor($('#locations').height() * 0.30);
        var height = (locationsTotalWeight.length * 80);
        $('#chart5div').empty();
        $('#chart5div').append('<canvas id="chart5" style="height:' + height + 'px;" height="' + height + '" width="787"></canvas>');
        var ctx = document.getElementById("chart5").getContext("2d");
        var chart4 = new Chart(ctx, {
            type: 'horizontalBar',
            data: barChartData,
            options: chartOptions
        });
    }

    //location wise total weight
    function draw_chart4(startDate, endDate, data) {

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
        var height = 0;
        if (data.length > 15)
            height = 250 + Math.floor((data.length * 100) * 0.09);
        else
            height = 250;

        $('#chart4div').empty();
        $('#chart4div').append('<canvas id="chart4" style="height: ' + height + 'px;" height="' + height + '" width="787"></canvas>');
        var ctx = document.getElementById("chart4").getContext("2d");
        var chart4 = new Chart(ctx, {
            type: 'horizontalBar',
            data: barChartData,
            options: chartOptions
        });
        return "";
        
    }

    //single product monthly data chart
    function draw_chart3(productName, startDate, endDate, data) {
        var max_x, min_x;
        for (var i = 0; i < monthLabels.length; i++) {
            if (monthLabels[i] === startDate)
                min_x = i;
            else if (monthLabels[i] === endDate)
                max_x = i;
        }

        var chartLabels = [];
        for (var i = min_x; i <= max_x; i++) {
            chartLabels.push(monthLabels[i])
        }

        var productData = []
        var j;
        for (var i = 0; i < data.length; i++) {
            for (j = 0; j < data[i].productInformation.length; j++) {
                if (data[i].productInformation[j].productName == productName) {
                    productData.push(data[i].productInformation[j].totalWeight);
                    break;
                }
            }
            if (j == data[i].productInformation.length)
                productData.push(0);
        }
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
            maintainAspectRatio: false,
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
                        if (tooltipItems.yLabel <= 1000)
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
    function draw_chart2(startDate, endDate, data) {
        
        var max_x, min_x;
        for (var i = 0; i < monthLabels.length; i++) {
            if (monthLabels[i] === startDate)
                min_x = i;
            else if (monthLabels[i] === endDate)
                max_x = i;
        }

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
                label: label,
                data: dates.map(function (date) {
                    var hit = data.find(function (e) {
                        return e.date === date;
                    }).productInformation.find(function (p) {
                        return p.productName === label;
                    });
                    return hit ? hit.totalWeight : 0;
                }),
                backgroundColor: getRandomColor()
            };
        });

        var chartLabels = [];
        for (var i = min_x; i <= max_x; i++) {
            chartLabels.push(monthLabels[i])
        }
        var barChartData = {
            labels: chartLabels,
            datasets: result
        };
        var chartOptions = {
            maintainAspectRatio: false,
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
                        if (tooltipItems.yLabel <= 1000)
                            return data.datasets[tooltipItems.datasetIndex].label + ": " + tooltipItems.yLabel + ' gm';
                        else
                            return data.datasets[tooltipItems.datasetIndex].label + ": " + tooltipItems.yLabel/1000 + ' kg';
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
            maintainAspectRatio: false,
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
        for (var i = 0; i < data.length; i++) {
            products.push(data[i].productName);
        }
    }
}