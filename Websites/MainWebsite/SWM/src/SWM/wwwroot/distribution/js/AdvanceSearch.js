!function(){function a(a,s){a.showSearchResults=!1,a.isBusy=!1,a.searchData=null,a.search=function(){a.isBusy=!0;var e={FullNames:a.FullNames,Products:a.Products,Location:a.Location,States:a.States,Countries:a.Countries};s.post("/api/advance_search",e).then(function(s){a.searchData=s.data,a.showSearchResults=!0,a.isBusy=!1},function(a){console.log(a)})}}angular.module("advance-search",["datatables"]),angular.module("advance-search").controller("searchController",["$scope","$http",a])}();