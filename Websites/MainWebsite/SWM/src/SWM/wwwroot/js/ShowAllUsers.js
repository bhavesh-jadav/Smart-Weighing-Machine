(function () {

    angular.module("app-allUsers", [])
    angular.module("app-allUsers").controller("allUserController", ["$scope", "$http", allUserController]);
    var pageNo = 1;
    var datatable;
    function allUserController($scope, $http) {
        $scope.initial = true;
        $scope.loadingMessage = "Load More Users";
        $scope.showLoadingIcon = false;
        load_more_users();
        $scope.loadMoreUsers = load_more_users;
        function load_more_users() {
            $scope.loadingMessage = "Getting More Users...";
            $scope.showLoadingIcon = true;
            $http.get("/api/get_all_users/" + pageNo).then(function (response) {
                if (pageNo === 1) {
                    $scope.initial = false;
                    $scope.userData = response.data.users;
                    $(function () {
                        datatable = $("#data-table").DataTable({
                            "responsive": true,
                            "paging": false,
                            "language": {
                                "info": "Showing _START_ to _END_ of " + response.data.totalUsers +" users"
                            }
                        });
                    });
                }
                else {
                    var data = response.data.users;
                    for (var i = 0; i < data.length; i++) {
                        datatable.row.add([
                            data[i].no,
                            '<a href="/Public/User/' + data[i].subId + '">' + data[i].fullName + '</a>',
                            data[i].productsIntoAccount,
                            data[i].state,
                            data[i].country
                        ]).draw(false);
                    }
                }
                pageNo += 1;
                $scope.loadingMessage = "Load More Users";
                $scope.showLoadingIcon = false;
            }, function (error) {
                $scope.initial = false;
                $scope.apiError = "Error loading data.";
            });
        };
    }

})();