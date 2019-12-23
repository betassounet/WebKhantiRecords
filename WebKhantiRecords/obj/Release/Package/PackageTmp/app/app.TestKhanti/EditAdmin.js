(function () {
    'use strict';

    angular
        .module('app')
        .controller('EditAdminCtrl', EditAdminCtrl);

    function EditAdminCtrl($scope, $http, $mdToast, $mdDialog, $sce) {

        activate();
        function activate() {
            //GetDispoDataFromGlobal();
            //GetEntityArtists();
        }

        function GetDispoDataFromGlobal() {
            var param = {
                sAction:'',
            };
            $http.post("api/Test/GetDispoDataFromGlobal", param).then(function (response) {
                $scope.RepGetDispoDataFromGlobal = response.data;
            });
        }

        function GetEntityArtists() {
            var param = {
                sAction: '',
            };
            $http.post("api/Test/GetEntityArtists", param).then(function (response) {
                $scope.RepGetEntityArtists = response.data;
            });
        }

        

        //$scope.GetDispoDataFromGlobal = function () {
        //    GetDispoDataFromGlobal();
        //};

        $scope.SelectedArtistProduct = [];

        $scope.SelectArtistAndProduct = function (artistName, product) {
            var line = {
                artistName: artistName,
                product: product
            }
            $scope.SelectedArtistProduct.push(line);
        };

        $scope.UnSelectArtistAndProduct = function (selected) {

            _.remove($scope.SelectedArtistProduct, function (el) { return el == selected; });
        };


        $scope.GetNameResolutionPath = function(){
            var param = {
                sAction: '',
            };
            $http.post("api/Test/GetNameResolutionPath", param).then(function (response) {
                $scope.RepGetNameResolutionPath = response.data;
            });
        }
        
        $scope.GetArtisteAndResolutionPath = function () {
            var param = {
                sAction: '',
            };
            $http.post("api/Test/GetArtisteAndResolutionPath", param).then(function (response) {
                $scope.RepGetArtisteAndResolutionPath = response.data;
            });
        }

        //-----------------------------------------------------------------------------------------------

    }

})();
