//-------------------------------------------------------------------------------
//-------------------------------------------------------------------------------
// ON traite ICI la config pour la partie Input STOCK
//-------------------------------------------------------------------------------
//-------------------------------------------------------------------------------


(function () {
    'use strict';

    angular
        .module('app')
        .controller('configCtrl', configCtrl);

    function configCtrl($scope, $http, $q, GlobalService) {

        //var BASE_URL_API = GlobalService.base_WebAPI_URL;
        var BASE_URL_API = "";

        $scope.Bonjour = "Configs";

        $scope.ConfigFiles = {
            environnementProjet: null,
            environnementGlobal:null,
            configEnvironnementSession: null,
            configEnvironnementClient: null
        }

        $scope.LastMessage = "";

        activate();
        function activate() {
            ActionOnConfigEnvironnementSession('Get');
        }

        function ActionOnConfigEnvironnementSession(sAction, newClientName) {
            var param = { sAction: sAction, newClientName:newClientName, configEnvironnementSession: $scope.ConfigFiles.configEnvironnementSession };
            $http.post(BASE_URL_API + "api/Config/ActionOnConfigEnvironnementSession", param).then(function (response) {
                $scope.ConfigFiles.environnementProjet = response.data.environnementProjet;
                $scope.ConfigFiles.environnementGlobal = response.data.environnementGlobal;
                $scope.ConfigFiles.configEnvironnementSession = response.data.configEnvironnementSession;
                $scope.ConfigFiles.configEnvironnementClient = response.data.configEnvironnementClient;
                $scope.LastMessage = response.data.environnementProjet.sMessage;

                if ($scope.ConfigFiles.environnementProjet.listTypeFileConfigNeeded) {
                    //Attention .. ici..  promises en boucle !!!
                    var chain = $q.when();
                    $scope.ConfigFiles.environnementProjet.listTypeFileConfigNeeded.forEach(function (el) {
                        chain = chain.then(function () {
                            return ActionGetFichierConfig(el);
                        });
                    });
                }

            });
        }

        $scope.ActionOnConfigEnvironnementSession = function (sAction, newClientName) {
            ActionOnConfigEnvironnementSession(sAction, newClientName);
        }

        // Fonction retournant une promise, orientée pour récupèrer les fichiers de config ..  
        function ActionGetFichierConfig(fileDef) {
            var param = { sAction: "Get", sTypeConfig: fileDef.TypeFileConfig, NameFileConfig: fileDef.NameFileConfig };
            return $http.post(BASE_URL_API + "api/Config/ActionFichierConfig", param).then(function (response) {
                fileDef.objConfig = response.data.objConfig;
            });
        }

        // Fonction pour set fichiers de config  
        $scope.ActionFichierConfig = function (sTypeConfig, NameFileConfig, jsonObjConfig) {
            var param = { sAction: "Set", sTypeConfig: sTypeConfig, NameFileConfig: NameFileConfig, jsonObjConfig: jsonObjConfig };
            $http.post(BASE_URL_API + "api/Config/ActionFichierConfig", param).then(function (response) {
                $scope.RepActionFichierConfig = response.data;
                $scope.LastMessage = response.data.sMessage;
            });
        }

    }

})();