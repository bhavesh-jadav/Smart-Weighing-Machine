function user_dashboard(username) {
    var chart1data = [];

    //Angular part
    //Defining a Module
    angular.module("app-dashboard", []);
    //getting instance of module and adding controller to the modules
    angular.module("app-dashboard").controller("chartsController", ["$scope","$http",chartsController]);
    function chartsController($scope, $http) {

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

    function draw_chart1() {
        var chartLabels = []
        var chartData = []

        for (var i = 0; i < chart1data.length; i++) {
            chartLabels.push(chart1data[i].productName);
            chartData.push(chart1data[i].totalWeight);
        }

        var areaChartData = {
            labels: ["January", "February", "March", "April", "May", "June", "July"],
            datasets: [
              {
                  label: "Electronics",
                  fillColor: "rgba(210, 214, 222, 1)",
                  strokeColor: "rgba(210, 214, 222, 1)",
                  pointColor: "rgba(210, 214, 222, 1)",
                  pointStrokeColor: "#c1c7d1",
                  pointHighlightFill: "#fff",
                  pointHighlightStroke: "rgba(220,220,220,1)",
                  data: [65, 59, 80, 81, 56, 55, 40]
              },
              {
                  label: "Digital Goods",
                  fillColor: "rgba(60,141,188,0.9)",
                  strokeColor: "rgba(60,141,188,0.8)",
                  pointColor: "#3b8bba",
                  pointStrokeColor: "rgba(60,141,188,1)",
                  pointHighlightFill: "#fff",
                  pointHighlightStroke: "rgba(60,141,188,1)",
                  data: [28, 48, 40, 19, 86, 27, 90]
              }
            ]
        };

        //-------------
        //- BAR CHART -
        //-------------
        var barChartCanvas = $("#chart1").get(0).getContext("2d");
       
        var barChartData = areaChartData;
        var ctx = document.getElementById("chart1").getContext("2d");
        window.myBar = new Chart(ctx, {
            type: 'bar',
            data: barChartData,
            options: {
                title: {
                    display: true,
                    text: "Chart.js Bar Chart - Stacked"
                },
                tooltips: {
                    mode: 'index',
                    intersect: false
                },
                responsive: true,
                scales: {
                    xAxes: [{
                        stacked: true,
                    }],
                    yAxes: [{
                        stacked: true
                    }]
                }
            }
        });
    }
};