(function () {
    'use strict';
    //var appMain = angular.module('app.main', []);    // Attention : il est important de mettre [] plutot que rien même s'il n'y a pas de dépendances sinon des comportements
    // bizarres sont a redouter, style le module app.main n'est pas déclaré..
    // https://docs.angularjs.org/error/$injector/nomod?p0=app.main

    var app = angular.module('app');  // Dans ce cas ou on ne fait que reprendre la definition de app ( déjà defini dans app.js ).. on veut coller un service au module app

    app.service('mainService', function ($rootScope) {

        // Pour events ( emit, broadcast...) :
        // voir https://toddmotto.com/all-about-angulars-emit-broadcast-on-publish-subscribing/



        return {
            EnterPage: function (data) {                 // la version générique pour toutes les pages
                console.log("Enter Page");
                console.log(data);
                $rootScope.$emit("onEnterPage", data);          // l'idée ici etant d'envoyer event aux controlleur actifs... pour signaler une autre page ouverte..cependant au passage d'un scope à l'autre             
                $rootScope.$broadcast("onEnterPage", data);     // Le scope de la page quitté ne doit plus etre actif.. lui et ses handler events.. c'est le cas sur le test des events ici 
            },
            EnterPageLayout: function () {
                console.log("In page Layout");
            },
            Login: function (user) {
                console.log("Login");
                $rootScope.$emit("onLogin", user);          // l'idée ici etant d'envoyer event aux controlleur actifs... pour signaler une autre page ouverte..cependant au passage d'un scope à l'autre             
                $rootScope.$broadcast("onLogin", user);     // Le scope de la page quitté ne doit plus etre actif.. lui et ses handler events.. c'est le cas sur le test des events ici 
            }
        };

    });


})();