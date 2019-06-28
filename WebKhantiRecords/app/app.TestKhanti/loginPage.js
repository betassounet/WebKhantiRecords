(function () {
    'use strict';

    angular
        .module('app')
        .controller('loginPageCtrl', ['$scope', '$http', '$mdToast', '$mdDialog', '$sce', 'mainService', loginPageCtrl]);    //['$scope', '$http', '$location', 'GlobalService', 'mainService', appCtrl]

    function loginPageCtrl($scope, $http, $mdToast, $mdDialog, $sce, mainService) {

        $scope.user = {
            title: '',
            email: '',
            password: ''
        };

        activate();
        function activate() {
        }

        $scope.Login = function () {
            mainService.Login($scope.user);
        }
    }

})();
