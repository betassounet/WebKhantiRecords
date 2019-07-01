(function () {
    'use strict';

    angular
        .module('app')
        .controller('welcomeCtrl', ['$scope', '$http', '$mdToast', '$mdDialog', '$sce', 'mainService', welcomeCtrl]);    //['$scope', '$http', '$location', 'GlobalService', 'mainService', appCtrl]

    function welcomeCtrl($scope, $http, $mdToast, $mdDialog, $sce, mainService) {
       
        $scope.ImageLogo = "./img/LOGO_khanti_couleurs-e1558105770391.png";
        $scope.ImageWelcome = "./img/slider_homepage.jpg";

        activate();
        function activate() {
            mainService.logOut();
        }

        $scope.Test = function () {
            mainService.EnterPage('toto');
        }
   }

})();
