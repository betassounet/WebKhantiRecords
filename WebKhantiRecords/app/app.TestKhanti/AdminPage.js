(function () {
    'use strict';

    angular
        .module('app')
        .controller('AdminPageCtrl', AdminPageCtrl);

    function AdminPageCtrl($scope, $http, $location, $mdToast, $mdDialog, $sce) {

        $scope.ImageLogo = "./img/LOGO_khanti_couleurs-e1558105770391.png";

        activate();
        function activate() {
            var urlBase = $location.$$absUrl;
            urlBase = urlBase.replace('#/AdminPage', '');
            console.log('urlBase : ', urlBase);
            var uploadUrl = urlBase + 'UpLoadHandler.ashx';// Url du Handler generique ashx niveau du serveur
            $scope.urlBase = urlBase;
            $scope.uploadUrl = uploadUrl;
            //var uploadUrl = "http://localhost:50360//UpLoadHandler.ashx"; 
        }

        $scope.ListFileToUpload = [];

        $scope.PreUploadFile = function (files) {
            $scope.$apply(function () {
                if (files[0]) {
                    var itemFile = {
                        formDataUpLoad: new FormData(),
                        name: files[0].name,
                        lastmodified : files[0].lastModified,
                        size : files[0].size
                    }
                    itemFile.formDataUpLoad.append("file", files[0]);
                    $scope.ListFileToUpload.push(itemFile);
                }
            });
        }

        $scope.UploadFile = function (filesToUpload) {
            if (filesToUpload && filesToUpload.formDataUpLoad) {
                filesToUpload.formDataUpLoad.append("forceUpLoad", filesToUpload.isForceUpLoad);     // on peut essayer de passer des infos supplémentaires..
                filesToUpload.formDataUpLoad.append("SelectedMonth", filesToUpload.selectedMonth);
                $http.post($scope.uploadUrl, filesToUpload.formDataUpLoad, {
                    withCredentials: true,
                    headers: { 'Content-Type': undefined },
                    transformRequest: angular.identity
                }).then(function (response) {
                    filesToUpload.reponseUpLoad = response.data;
                }, function (responseError) {
                    filesToUpload.reponseUpLoad = responseError.data;
                });
            }
        }



        $scope.AnalyseFichiers = function () {
            var param = {
                sAction: '',
            };
            $http.post("api/Test/AnalyseFichiers", param).then(function (response) {
                $scope.RepAnalyseFichiers = response.data;
            });
        }

        
    }

})();