(function () {

    angular.module("advance-search", ['datatables']);
    //getting instance of module and adding controller to the modules
    angular.module("advance-search").controller("searchController", ["$scope", "$http", searchController]);

    function searchController($scope, $http) {
        $scope.showSearchResults = false;
        $scope.isBusy = false;
        $scope.searchData = null;
        $scope.search = function () {
            $scope.isBusy = true;
            var data = {
                "FullNames": $scope.FullNames,
                "Products": $scope.Products,
                "Location": $scope.Location,
                "States": $scope.States,
                "Countries": $scope.Countries
            }
            $http.post("/api/advance_search", data).then(function (response) {
                $scope.searchData = response.data;
                $scope.showSearchResults = true;
                $scope.isBusy = false;
            }, function (error) {
                console.log(error);
            })
        };
    }
}

)();