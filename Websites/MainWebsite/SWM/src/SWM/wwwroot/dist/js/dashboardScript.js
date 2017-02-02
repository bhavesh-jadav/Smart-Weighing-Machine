function user_dashboard(username) {
    var chart1data = [];
    var chart2data = [];

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
            $scope.isBusyChart2 = false;
            draw_chart2();
        }, function (error) {
            $scope.chart2Error = "Unable to display chart"
        });
    };

    //month wise data chart
    function draw_chart2() {
        var monthNames = [
                          "January", "February", "March",
                          "April", "May", "June", "July",
                          "August", "September", "October",
                          "November", "December"
                          ];

        var chartLabels = []
        var max_x, min_x = 0, max_product=0, index;

        var firstDate = chart2data[0].date;
        var lastDate = chart2data[chart2data.length - 1].date;

        //credit to Christian Zosel on stack overflow. converts json data into chart data
        const uniq = a =>[...new Set(a)]
        const flatten = a =>[].concat.apply([], a)

        // step 1: find the distinct dates: ["2016-05-01T00:00:00", ... ]
        const dates = chart2data.map(e => e.date)

        // step 2: find the distinct labels: [Apple, Mango, ... ]
        const labels = uniq(
          flatten(chart2data.map(e => e.productInformation))
          .map(e => e.productName))

        // step 3: map the labels to entries containing their data by searching the original data array
        const result = labels.map(label => {
            return {
                label,
                data: dates.map(date => {
                    const hit = chart2data.find(e => e.date === date)
                      .productInformation
                      .find(p => p.productName === label)
                    return hit ? hit.totalWeight : 0
                }),
                backgroundColor: getRandomColor()
            }
        })
        console.log(result);
        if (chart2data.length >= 6)
            max_x = 6;
        else
            max_x = chart2data.length;

        for (var i = max_x-1; i >= min_x; i--) {
            var date = new Date(chart2data[i].date)
            chartLabels.push(monthNames[date.getMonth()] + " " + date.getFullYear());
        }

        var barChartData = {
            labels: ["May 2016", "September 2016", "January 2017"],
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
        var ctx = document.getElementById("chart2").getContext("2d");
        var myBar = new Chart(ctx, {
            type: 'bar',
            data: barChartData,
            options: chartOptions
        });
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
        var myBar = new Chart(ctx, {
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
};