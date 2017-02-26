(function () {
    angular.module("app-allUsers", [])
    angular.module("app-allUsers").controller("allUserController", ["$scope", "$http", allUserController]);
    var pageNo = 1;
    var datatable;
    function allUserController($scope, $http) {
        $scope.initial = true;
        $scope.loadingMessage = "Load More Users";
        $scope.showLoadingIcon = false;
        $scope.disableLoadMoreButton = false;
        load_more_users();
        $scope.loadMoreUsers = load_more_users;
        function load_more_users() {
            $scope.loadingMessage = "Getting More Users...";
            $scope.showLoadingIcon = true;
            $scope.disableLoadMoreButton = true;
            $http.get("/api/get_all_users/" + pageNo).then(function (response) {
                if (pageNo === 1) {
                    $scope.initial = false;
                    $scope.userData = response.data.users;
                    $scope.totalUsers = response.data.totalUsers;
                    $(function () {
                        datatable = $("#data-table").DataTable({
                            "responsive": true,
                            "paging": false,
                            "language": {
                                "info": "Showing 1 to _TOTAL_ of " + response.data.totalUsers + " users"
                            }
                        });
                    });
                    $scope.loadingMessage = "Load More Users";
                    $scope.showLoadingIcon = false;
                    $scope.disableLoadMoreButton = false;
                }
                else {
                    var data = response.data.users;
                    if (data.length == 0) {
                        $scope.loadingMessage = "No More Users To display.";
                        $scope.showLoadingIcon = false;
                    }
                    else {
                        for (var i = 0; i < data.length; i++) {
                            datatable.row.add([
                                data[i].no,
                                '<a href="/Public/User/' + data[i].subId + '">' + data[i].fullName + '</a>',
                                data[i].productsIntoAccount,
                                data[i].state,
                                data[i].country
                            ]).draw(false);
                        }
                        $scope.loadingMessage = "Load More Users";
                        $scope.showLoadingIcon = false;
                        $scope.disableLoadMoreButton = false;
                    }
                }
                pageNo += 1;
            }, function (error) {
                $scope.initial = false;
                $scope.totalUsers = 0;
                $scope.apiError = "There is a problem while getting results. Try again later.";
                $scope.loadingMessage = "Load More Users";
                $scope.showLoadingIcon = false;
                $scope.disableLoadMoreButton = true;
            });
        };
    }
})();