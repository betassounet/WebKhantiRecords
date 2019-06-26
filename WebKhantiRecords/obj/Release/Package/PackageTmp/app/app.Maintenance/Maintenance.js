

(function () {
    'use strict';

    angular
        .module('app')
        .controller('MaintenanceCtrl', MaintenanceCtrl);

    function MaintenanceCtrl($scope, $http, GlobalService) {

        //var BASE_URL_API = GlobalService.base_WebAPI_URL;
        var BASE_URL_API = "";

        $scope.Bonjour = "Maintenance";

        $scope.dtFiltre = new Date();
        $scope.SenderFiltre = "";
        $scope.IsFiltreDate = false;
        $scope.IsFiltreSender = false;
        $scope.SenderExclureFiltre = "";

        activate();
        function activate() {
            GetLogInXml(true);
        }

        function GetLogInXml(IsGetSenders) {
            var param = { IsGetSenders: IsGetSenders, IsFiltreDate: $scope.IsFiltreDate, IsFiltreSender: $scope.IsFiltreSender, dtFiltre: $scope.dtFiltre, SenderFiltre: $scope.SenderFiltre, SenderExclureFiltre: $scope.SenderExclureFiltre };
            $http.post(BASE_URL_API + "api/Maintenance/GetLogsFiltre", param).then(function (response) {
                $scope.ReponseGetLogInXml = response.data;
            });
        }

        $scope.GetLogInXml = function () {
            GetLogInXml(false);
        }

        $scope.OperationLog = function (sAction) {
            var param = { sAction: sAction };
            $http.post(BASE_URL_API + "api/Maintenance/OperationLog", param).then(function (response) {
                $scope.ReponseOperationLog = response.data;
            });
        }

        $scope.GetSetPerformanceLogger = function (Action, Param) {
            var param = { Action: Action, Param: Param };
            $http.post(BASE_URL_API + "api/Maintenance/GetSetPerformanceLogger", param).then(function (response) {
                $scope.ReponseGetSetPerformanceLogger = response.data;
            });
        }
    }

})();