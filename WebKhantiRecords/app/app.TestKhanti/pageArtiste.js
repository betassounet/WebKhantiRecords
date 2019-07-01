(function () {
    'use strict';

    angular
        .module('app')
        .controller('pageArtisteCtrl', ['$scope', '$http', '$mdToast', '$mdDialog', '$sce', 'mainService', pageArtisteCtrl]); 

    function pageArtisteCtrl($scope, $http, $mdToast, $mdDialog, $sce, mainService) {

        $scope.statut = {};

        $scope.View = {
            ModeViewResult: 'ViewResults'
        }

        $scope.GraphHisto = {
            idGraph: 100,
            title: "Histo Periodes",
            height: '400px',
            serieCombine: []
        }
        var markerSpline = {
            lineWidth: 2,
            lineColor: Highcharts.getOptions().colors[3],
            fillColor: 'white'
        };
        var formatTooltip = {
            pointFormat: "<span>{point.libelleTooltip}</span>",
            positioner: function (labelWidth, labelHeight, point) {
                var tooltipX = point.plotX + 100;
                var tooltipY = point.plotX + 100;
                // ... Calculations go here ... //
                return { x: tooltipX, y: tooltipY };
            }
        };

        activate();
        function activate() {
            $scope.statut = mainService.getCurrentStatut();
            //console.log("ENter page Artiste : " + $scope.statut.currentNameArtiste);
            GetDetailArtisteV2($scope.statut.currentNameArtiste);
        }


        $scope.ListePeriode = [];

        $scope.GetDetailArtisteV2 = function () {
            GetDetailArtisteV2($scope.statut.currentNameArtiste);
        }

        function GetDetailArtisteV2(nomArtiste) {
            $scope.RepGetDetailArtisteV2 = null;
            var param = {
                NomArtiste: nomArtiste,
                listPeriode: $scope.ListePeriode
            };
            $http.post("api/Test/GetDetailArtisteV2", param).then(function (response) {
                $scope.RepGetDetailArtisteV2 = response.data;
                $scope.ListePeriode = $scope.RepGetDetailArtisteV2.listPeriode;

                $scope.GraphData = [];
                var graph1 = { id: 1000, data: [], libelle: 'by DMS' };
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


                
                var dataPayable = [];
                var dataDownload = [];
                response.data.resultHisto.listHistoPeriode.forEach(function (d) {
                    dataPayable.push({ x: d.dtlong, y: d.TotalPayable, libelleTooltip: d.sPeriode });
                    dataDownload.push({ x: d.dtlong, y: d.NbDownLoad, libelleTooltip: d.libelleTooltip });
                });
                $scope.GraphHisto.serieCombine.push({ type: 'spline', name: 'payable', data: dataPayable, marker: markerSpline, id: 'dataseries' , yAxis:0 });
                $scope.GraphHisto.serieCombine.push({ type: 'spline', name: 'download', data: dataDownload, marker: markerSpline, id: 'dataseries', yAxis: 1 });
            });
        }


        //$scope.ReponseGetSetPerformanceLogger.listLogCallPerformanceItem.forEach(function (el) {
        //    el.LocalGraph = {
        //        idGraph: el.callIdAndStatsItem.Id,
        //        title: "Graph test Id :" + el.callIdAndStatsItem.Id,
        //        height: '200px',
        //        serieCombine: []
        //    };
        //    var dataDelay = [];
        //    var dataFlags = [];
        //    el.listHisto.forEach(function (d) {
        //        dataDelay.push({ x: d.x, y: d.y, libelleTooltip: d.libelleTooltip });
        //        //dataDelay.push({ x: d.x, y: d.y, libelleTooltip: d.libelleTooltip, events: { "click": flagClickPointSpline } });
        //        if (d.flag) {
        //            dataFlags.push({ x: d.x, y: d.y, libelleTooltip: d.libelleTooltip });
        //        }
        //    });
        //    el.LocalGraph.serieCombine.push({ type: 'spline', name: 'response delay', data: dataDelay, marker: markerSpline, id: 'dataseries' });
        //    el.LocalGraph.serieCombine.push({ type: 'flags', name: 'call', data: dataDelay, marker: markerSpline, tooltip: formatTooltip, id: 'dataseries' });
        //    el.LocalGraph.serieCombine.push({ type: 'flags', name: 'flags', tooltip: formatTooltip, data: dataFlags, shape: "url(./images/Warning.png)" });
        //});


    }

})();
