function searchUser (fullName) {
    angular.module('app-searchUser', []);
    angular.module('app-searchUser').controller('searchUserController', ['$scope', '$http', searchUserController]);
    function searchUserController($scope, $http) {
        $scope.initial = true;
        $http.get('/api/search_user/' + fullName).then(function (response) {
            $scope.userData = response.data.users;
            $scope.totalUsers = response.data.totalUsers;
            $(function () {
                $("#data-table").DataTable({
                    "responsive": true,
                    "iDisplayLength": 25
                });
            });
            $scope.initial = false;
        }, function (error) {
            $scope.initial = false;
            $scope.totalUsers = 0;
            $scope.apiError = "There is a problem while getting search results. Try again later.";
        });
    }
}