(function () {
    'use strict';

    angular
        .module('app')
        .controller('testKhantiCtrl', testKhantiCtrl);

    function testKhantiCtrl($scope, $http, $mdToast, $mdDialog, $sce) {

        $scope.ImageLogo = "./img/LOGO_khanti_couleurs-e1558105770391.png";

        activate();
        function activate() {
        }


        $scope.AnalyseFichiers = function () {
            var param = {
                sAction: '',
            };
            $http.post("api/Test/AnalyseFichiers", param).then(function (response) {
                $scope.RepAnalyseFichiers = response.data;
            });
        }

        $scope.AnalyseFichier = function (NomFichier) {
            $scope.RepTest = null;
            var param = {
                sAction:'',
                WithDetails: true,
                NomFichier: NomFichier
            };
            $http.post("api/Test/AnalyseFichier", param).then(function (response) {
                $scope.RepTest = response.data;
            });
        }

        $scope.getDetailArtiste = function (NomArtiste) {
            $scope.RepGetDetailArtiste = null;
            var param = {
                NomArtiste: NomArtiste
            };
            $http.post("api/Test/GetDetailArtiste", param).then(function (response) {
                $scope.RepGetDetailArtiste = response.data;

                $scope.GraphData = [];
                var graph1 = { id: 1000, data: [], libelle:'by DMS' };
                response.data.listKeyTotalQuantityCritereDNS.forEach(function (el) {
                    var datagraph = { name: el.key + " : " + el.Quantity, y: el.Quantity };
                    graph1.data.push(datagraph);
                });
                $scope.GraphData.push(graph1)
                
                var graph2 = { id: 1001, data: [], libelle: 'by TERRITORY' };
                response.data.listKeyTotalQuantityCritereTerritory.forEach(function (el) {
                    var datagraph = { name: el.key + " : " + el.Quantity, y: el.Quantity };
                    graph2.data.push(datagraph);
                });
                $scope.GraphData.push(graph2)

                var graph3 = { id: 1002, data: [], libelle: 'by FORMAT TYPE' };
                response.data.listKeyTotalQuantityCritereFormatType.forEach(function (el) {
                    var datagraph = { name: el.key + " : " + el.Quantity, y: el.Quantity };
                    graph3.data.push(datagraph);
                });
                $scope.GraphData.push(graph3)

                var graph4 = { id: 1003, data: [], libelle: 'by PRICE CATEGORY' };
                response.data.listKeyTotalQuantityCritereCategorie.forEach(function (el) {
                    var datagraph = { name: el.key + " : " + el.Quantity, y: el.Quantity };
                    graph4.data.push(datagraph);
                });
                $scope.GraphData.push(graph4)

                //$scope.GraphData = { id: 1000, data: [] };
                //response.data.listKeyTotalQuantityCritereDNS.forEach(function (el) {
                //    var datagraph = { name: el.key + " : " + el.Quantity, y: el.Quantity };
                //    $scope.GraphData.data.push(datagraph);
                //});

                //$scope.GraphData1 = { id: 1001, data: [] };
                //response.data.listKeyTotalQuantityCritereTerritory.forEach(function (el) {
                //    var datagraph = { name: el.key + " : " + el.Quantity, y: el.Quantity };
                //    $scope.GraphData1.data.push(datagraph);
                //});

                //$scope.GraphData2 = { id: 1002, data: [] };
                //response.data.listKeyTotalQuantityCritereFormatType.forEach(function (el) {
                //    var datagraph = { name: el.key + " : " + el.Quantity, y: el.Quantity };
                //    $scope.GraphData2.data.push(datagraph);
                //});

                //$scope.GraphData3 = { id: 1003, data: [] };
                //response.data.listKeyTotalQuantityCritereCategorie.forEach(function (el) {
                //    var datagraph = { name: el.key + " : " + el.Quantity, y: el.Quantity };
                //    $scope.GraphData3.data.push(datagraph);
                //});
            });
        }

    }

})();