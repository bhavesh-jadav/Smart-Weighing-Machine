function user_dashboard(username) {
    var chart1data = [];

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


    };

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
                label: "Weight In grams",
                data: chartData,
                backgroundColor: "#3c8dbc"
            }]
        };
        var chart1Options = {
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
            options: chart1Options
        });
    }
};