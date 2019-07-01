(function () {
    'use strict';

    angular
        .module('app')
        .controller('loginPageCtrl', ['$scope', '$http', '$mdToast', '$mdDialog', '$sce', 'mainService', loginPageCtrl]);    //['$scope', '$http', '$location', 'GlobalService', 'mainService', appCtrl]

    function loginPageCtrl($scope, $http, $mdToast, $mdDialog, $sce, mainService) {

        $scope.user = new LoginUser();

        activate();
        function activate() {
        }

        $scope.Login = function () {
            if ($scope.user.title == "admin") {
                $scope.user.droit = "admin";
            }
            else {
                $scope.user.droit = "user";
            }
            mainService.Login($scope.user);
        }
    }

})();
