(function () {
    'use strict';

    angular
        .module('app')
        .controller('Analyse2Ctrl', Analyse2Ctrl);

    function Analyse2Ctrl($scope, $http, $mdToast, $mdDialog, $sce) {

        activate();
        function activate() {
            Analyse2();
        }

        function Analyse2(action, artiste) {
            var param = {
                sAction: action,
                NomArtiste: artiste
            };
            $http.post("api/Test/Analyse2", param).then(function (response) {
                $scope.RepAnalyse2 = response.data;
            });
        }

        $scope.analyse2 = function (action, artiste) {
            Analyse2(action, artiste);
        };

    }

})();