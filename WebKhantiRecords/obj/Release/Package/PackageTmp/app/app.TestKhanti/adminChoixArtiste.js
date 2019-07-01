(function () {
    'use strict';

    angular
        .module('app')
        .controller('adminChoixArtisteCtrl', ['$scope', '$http', '$mdToast', '$mdDialog', '$sce', 'mainService', adminChoixArtisteCtrl]);

    function adminChoixArtisteCtrl($scope, $http, $mdToast, $mdDialog, $sce, mainService) {

        activate();
        function activate() {
            Analyse2();
        }


        function Analyse2(action, artiste) {
            $scope.RepAnalyse2 = null;
            var param = {
                sAction: action,
                NomArtiste: artiste
            };
            $http.post("api/Test/Analyse2", param).then(function (response) {
                $scope.RepAnalyse2 = response.data;
            });
        }

        $scope.typeTri = "None";
        $scope.onTypeTriChange = function (typeTri) {
            console.log(typeTri);
            Analyse2(typeTri);
        }

        $scope.clickLien = function (nom) {
            console.log(nom);
            mainService.selectArtiste(nom);
        }

    }

})();
