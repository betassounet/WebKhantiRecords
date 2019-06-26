(function () {
    'use strict';

    //var app = angular.module('app', ['ngRoute', 'ui.bootstrap', 'ngMaterial']);
    var app = angular.module('app', ['ngRoute', 'ui.bootstrap', 'ngSanitize', 'ngMaterial', 'ui.codemirror']);   // New ajout de 'ui.codemirror' pour test de l'editeur de code..
    //var app = angular.module('app', []);


    app.value('$', $);


    app.factory('GlobalService', function () {
        //var base_WebAPI_URL = "http://localhost:50163/";
        var base_WebAPI_URL = "http://localhost:50163/";
        return {
            base_WebAPI_URL: base_WebAPI_URL
        };
    });



    // A LIRE ABSOLUMENT...
    // Voir pour explication routage
    //http://www.tutoriel-angularjs.fr/tutoriel/2-utilisation-complete-d-angularjs/1-le-routage 
    //http://www.tutoriel-angularjs.fr/tutoriel/2-utilisation-complete-d-angularjs/1-le-routage

    // Config routes
    app.config(['$routeProvider', '$locationProvider', '$mdThemingProvider', function ($routeProvider, $locationProvider, $mdThemingProvider) {

        //black background color
        var black = $mdThemingProvider.extendPalette('grey', {'A100': '000000'});
        $mdThemingProvider.definePalette('black', black);

        //Themes
        $mdThemingProvider.theme('theme-maf').backgroundPalette('grey').dark();
        $mdThemingProvider.theme('theme-maf').primaryPalette('green').accentPalette('orange');

        $mdThemingProvider.theme('theme-ffl').backgroundPalette('grey').dark();
        $mdThemingProvider.theme('theme-ffl').primaryPalette('black').accentPalette('orange');

        $mdThemingProvider.theme('dark-orange').backgroundPalette('orange').dark();
        $mdThemingProvider.theme('dark-purple').backgroundPalette('deep-purple').dark();
        $mdThemingProvider.theme('dark-blue').backgroundPalette('blue').dark();

        $routeProvider.when('/', {
            templateUrl: 'app/app.welcome/welcome.html',
            controller: 'welcomeCtrl',
        });

        $routeProvider.when('/Config', {
            templateUrl: 'app/app.config/config.html',
            controller: 'configCtrl'
        });

        $routeProvider.when('/Maintenance', {
            templateUrl: 'app/app.Maintenance/Maintenance.html',
            controller: 'MaintenanceCtrl',
        });

        //Passage de parametres dans l'URL a l'appel :         // voir https://blog.hfarazm.com/angularjs-routeparams/         // voir https://namitamalik.github.io/routeParams-in-AngularJS/
        // Niveau Html exemple d'appel :
        //<a href="#help/toto/titi/tata/tutu">
        //<a href="#/details/{{guitarVariable.indexOf(item)}}">

        $routeProvider.when('/help/:showpdf/:pdf/:showTodo/:TodoFile', {   // c'est le controller appelé qui va décoder directement le ou les parametres passés dans l'URL
            templateUrl: 'app/app.help/help.html',
            controller: 'helpCtrl',                        // 
            resolve: { viewName: function () { return { url: 'Documents/TODO.pdf', titre: 'Titre Id' } } }  // TEST aussi pour passage d'info via le viewName
        });

        $routeProvider.when('/NoSQLEval', {
            templateUrl: 'app/app.NoSqlEval/nosqlEval.html',
            controller: 'NoSQLEvalCtrl',
        });

        $routeProvider.when('/testKhanti', {
            templateUrl: 'app/app.TestKhanti/TestKhanti.html',
            controller: 'testKhantiCtrl',
        });

        $routeProvider.when('/analyse2', {
            templateUrl: 'app/app.TestKhanti/Analyse2.html',
            controller: 'Analyse2Ctrl',
        });
        

        $routeProvider.otherwise({
            redirectTo: '/'
        });


        // ATTENTION : 
        // Depuis la version 1.6.0 d'angular il a été rajouté par defaut un hash-prefix '!' que l'on peut voir dans la barre d'adresse url
        // La routage tel qu'on l'utilisait avant ne marche plus.. : $location.path n'est plus mis a jour sur changement d'url (<a  href="newRoute"> ) 
        //Pour rétablir le fonctionnement precédent du routage angular il faut retirer le '!' par defaut :
        // SOLUTION : VOIR https://stackoverflow.com/questions/41211875/angularjs-1-6-0-latest-now-routes-not-working
        // https://github.com/angular/angular.js/commit/aa077e81129c740041438688dff2e8d20c3d7b52
        //https://developers.google.com/webmasters/ajax-crawling/docs/getting-started
        $locationProvider.hashPrefix('');         // Par defaut c'est un !

    }]);


   

    // Controller appCtrl au besoin..
    app.controller('appCtrl', ['$scope', '$http', '$location', 'GlobalService', appCtrl]);

    function appCtrl($scope, $http, $location, GlobalService) {
        $scope.myData = "Pas Init";
        $scope.VisuMenu = false;
        $scope.ImageLogo = "./img/LOGO_khanti_couleurs-e1558105770391.png";

        activate();
        function activate() {
            GetMyUserIdentity();
        }


        function GetMyUserIdentity() {
            var param = {
                action: ''
            }

            $http.post("api/UserIdentity/GetMyUserIdentity", param).then(function (response) {
                var Identity = response.data;
                if (Identity.IsAutoRoute) {
                    $location.path(Identity.AutoRouteURL);
                    console.log("route vers :" + Identity.AutoRouteURL);
                    alert("autoroute vers :" + Identity.AutoRouteURL);
                }
            }, function (data) {
                alert("Probleme appel Web API sur URL : " + BASE_URL_API +"\nAdapter URL Base Web API ou \n voir CORS policy ??");
            });
        }
    }


    app.run(function ($rootScope, $location) {
        // Pour tester le routage qui ne fonctionne pas comme il fonctionnait :
        // voir https://docs.angularjs.org/api/ngRoute/provider/$routeProvider
        // voir https://stackoverflow.com/questions/11784656/angularjs-location-not-changing-the-path
        // voir https://groups.google.com/forum/#!topic/angular/nFbtADyEHg8

        $rootScope.$on("$locationChangeStart", function (event, next, current) {
            //console.log("$locationChangeStart :-------------- ");
            //console.log($location);
        });
        $rootScope.$on("$locationChangeSuccess", function (event, next, current) {
            //console.log("$locationChangeSuccess :----------------- ");
            //console.log($location);
        });
    });


})();