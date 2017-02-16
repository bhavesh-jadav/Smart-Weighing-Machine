(function publicDashboard() {
    "use strict";
    var dates = [];
    var monthLabels = [];
    var products = [];
    var chart2_max_x_label = 10;
    var chart1_max_x_label = 15;
    var monthNames = [
                         "January", "February", "March",
                         "April", "May", "June", "July",
                         "August", "September", "October",
                         "November", "December"
    ];
    //Angular part
    //Defining a Module
    angular.module("public", ['emguo.poller']);
    //getting instance of module and adding controller to the modules
    angular.module("public").controller("chartsController", ["$scope", "$http", "poller", chartsController]);

    function chartsController($scope, $http, poller) {

        var myPoller = poller.get("/api/public_dashboard", {
            catchError: true,
            delay: 10000
        });

        myPoller.promise.then(null, null, function (response) {
            if (response.data != null) {
                populateDashboard(response.data);
                $scope.gettingUserDetails = false;
            } else {
                $('#errorMessage').append('<div class="alert alert-danger alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>There was a problem while getting latest data. Make sure you are connected to the internet and Make sure that it is working fine.</div>');
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
            setTimeout(function () {
                $('#totalWeight').text(weight);
                $('#totalProducts').text(data.totalProducts);
                $('#totalUsers').text(data.totalUsers);
                $('#totalMachines').text(data.totalMachines);
                $('#totalUserLocations').text(data.totalUserLocations);
                if ($('#lastUserRegisterd').text() != data.lastUserRegisterd) {
                    $("#lastUserRegisterd").fadeToggle(function () {
                        $('#lastUserRegisterd').text(data.lastUserRegisterd);
                    });
                    $("#lastUserRegisterd").fadeToggle();
                }
            }, 10);
        }

        $scope.isBusyChart2 = true;
        $scope.isBusyChart1 = true;
        $http.get("/api/user_dates").then(function (response) {
            dates = response.data;
            for (var i = 0; i < dates.length; i++) {
                var date = new Date(dates[i])
                monthLabels.push(monthNames[date.getMonth()] + " " + date.getFullYear());
            }

            $scope.monthLabels = monthLabels;
            
            //decide min and max bound for chart1
            if (dates.length >= chart1_max_x_label)
                var min_x = dates.length - chart1_max_x_label;
            else
                var min_x = 0;
            var max_x = dates.length;
            $scope.chart1StartDate = monthLabels[min_x];
            $scope.chart1EndDate = monthLabels[max_x - 1];

            //decide min and max bound for chart2
            if (dates.length >= chart2_max_x_label)
                var min_x = dates.length - chart2_max_x_label;
            else
                var min_x = 0;
            var max_x = dates.length;
            $scope.chart2StartDate = monthLabels[min_x];
            $scope.chart2EndDate = monthLabels[max_x - 1];

            $http.get("/api/product_info").then(function (response) {
                populateProductsArray(response.data);
                $scope.products = products;
                $scope.chart2ProductName = products[0];
                $scope.displayChart2();
            }, function (error) {
                $scope.chart2Error = "Unable to display chart";
                $scope.isBusyChart2 = false;
            });
            $scope.displayChart1();
        }, function (error) {
            $scope.chart1Error = "Unable to display chart";
            $scope.isBusyChart1 = $scope.isBusyChart2 = false;
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

            $http.get("/api/product_info_month_wise/" + (startDate.getMonth() + 1) + "/" + startDate.getFullYear() + "/" + (endDate.getMonth() + 1) + "/" + endDate.getFullYear()).then(function (response) {
                $scope.isBusyChart2 = false;
                $scope.chart2Error = draw_chart2($scope.chart2ProductName, $scope.chart2StartDate, $scope.chart2EndDate, response.data);
            }, function (error) {
                $scope.isBusyChart2 = false;
                $scope.chart2Error = "Unable to display chart";
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
                display: false
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

    //single product monthly data chart
    function draw_chart2(productName, startDate, endDate, data) {
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

        $('#chart2div').empty();
        $('#chart2div').append('<canvas id="chart2" style="height:250px;" height="250"></canvas>');
        var ctx = document.getElementById("chart2").getContext("2d");
        var chart2 = new Chart(ctx, {
            type: 'line',
            data: barChartData,
            options: chartOptions
        });
        return "";
    }

    function populateProductsArray(data) {
        for (var i = 0; i < data.length; i++) {
            products.push(data[i].productName);
        }
    }
}
)();