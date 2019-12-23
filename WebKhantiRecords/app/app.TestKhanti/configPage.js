(function () {
    'use strict';

    angular
        .module('app')
        .controller('configPageCtrl', ['$scope', '$http', '$mdToast', '$mdDialog', '$sce', 'mainService', configPageCtrl]);    //['$scope', '$http', '$location', 'GlobalService', 'mainService', appCtrl]

    function configPageCtrl($scope, $http, $mdToast, $mdDialog, $sce, mainService) {


        $scope.Bonjour = "Salut de la page config";


        activate();
        function activate() {
        }


        $scope.UpdateConfig = function(action) {
            $scope.RepUpdateConfig = {};
            var param = {
                sAction: action,
                configParams: $scope.RepUpdateConfig.configParams
            };
            $http.post("api/Test/UpdateConfig", param).then(function (response) {
                $scope.RepUpdateConfig = response.data;

            });
        }

        

    }

})();
