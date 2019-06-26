(function () {
    'use strict';

    var app = angular.module('app');


    // directive utile pour utilisation du textarea avec le binding angular...  voir http://stackoverflow.com/questions/22783268/angular-js-display-and-edit-model-in-textarea

    app.directive('objEdit', function () {
        return {
            restrict: 'A',
            scope: {
                obj: '=obj',
                id: '=id',
                trigger:'=trigger'
            },
            link: function (scope, element, attrs) {
                var Cpt = 0;
                init();
                function init() {
                    element.text(JSON.stringify(scope.obj, undefined, 2));

                    element.change(function (e) {
                        //console.log(e.currentTarget.value);
                        scope.$apply(function () {                             // ATTENTION : prise en compte des changements dans l'editeur.. mais 
                            scope.obj = JSON.parse(e.currentTarget.value);     // ATTENTION : A partir de là pb de binding niveau entrant.. plus de mise a jour sur le watch !!
                        });
                        //console.log(scope.obj);
                    })
                }

                // RAJOUT pour remise a jour de l'obj si le html n'est pas reconstruit.....
                scope.$watch("obj", function (newValue, oldValue) {
                    if (newValue != oldValue) {
                        init();
                        Cpt++;
                        //console.log("Id :" + scope.id + " : NEW watch Obj : " + Cpt);
                    }
                }, true);

                // Rajout de ce code pour tentative de reinit... mais marche pas..

                scope.$watch("trigger", function (newValue, oldValue) { 
                    if (newValue != oldValue) {
                        init();
                        Cpt++;
                        //console.log("Id :" + scope.id + " : NEW watch Trigger : " + Cpt);
                    }
                }, true);
            }
        }
    });

    // AUtrea façon de faire :
    // https://stackoverflow.com/questions/17893708/angularjs-textarea-bind-to-json-object-shows-object-object

    // Ne fonctionne pas sur changements...

    app.directive('jsonText', function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attr, ngModel) {
                function into(input) {
                    console.log(JSON.parse(input));
                    return JSON.parse(input);
                }
                function out(data) {
                    return JSON.stringify(data, undefined, 2);
                }
                ngModel.$parsers.push(into);
                ngModel.$formatters.push(out);
            }
        };
    });



})();

