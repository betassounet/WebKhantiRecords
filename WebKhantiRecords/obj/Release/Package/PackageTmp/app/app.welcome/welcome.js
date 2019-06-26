(function () {
    'use strict';

    angular
        .module('app')
        .controller('welcomeCtrl', welcomeCtrl);

    function welcomeCtrl($scope, $http, $mdToast, $mdDialog, $sce) {
       
        $scope.ImageLogo = "./img/LOGO_khanti_couleurs-e1558105770391.png";
        $scope.ImageWelcome = "./img/slider_homepage.jpg";

        activate();
        function activate() {
        }


   }

})();
