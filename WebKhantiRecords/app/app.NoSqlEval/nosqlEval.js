(function () {
    'use strict';

    angular
        .module('app')
        .controller('NoSQLEvalCtrl', NoSQLEvalCtrl);

    function NoSQLEvalCtrl($scope, $http, $mdToast, $mdDialog, $sce, GlobalService) {

        //var BASE_URL_API = GlobalService.base_WebAPI_URL;
        var BASE_URL_API = "";

        activate();
        function activate() {
        }


        $scope.TestNoSQL = function () {
            var param = { sAction: ''};
            $http.post(BASE_URL_API + "api/NoSql/TestNoSQL", param).then(function (response) {
                $scope.RepTestNoSQL = response.data;
            });
        }


        $scope.TestMongoDB = function (action) {
            var param = { sAction: action };
            $http.post(BASE_URL_API + "api/NoSql/TestMongoDB", param).then(function (response) {
                $scope.RepTestMongoDB = response.data;
            });
        }
    }

})();