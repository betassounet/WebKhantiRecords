(function () {
    'use strict';

    var app = angular.module('app');


    // directive utile pour utilisation du textarea avec le binding angular...  voir http://stackoverflow.com/questions/22783268/angular-js-display-and-edit-model-in-textarea

    app.directive('highChartPie2', function ($compile) {
        return {
            restrict: 'C',
            replace: true,
            scope: {
                items: '=',
                idcontainer: '='    // on se donne un attribut supplementaire
            },
            controller: function ($scope, $element, $attrs) {
                //console.log($element);
            },
            //template: '<div id="container" style="margin: 0 auto">not working</div>',  // Faut rendre ce template dynamique et rajouter container+Id!! ou
            // definir le container a l'exterieur...
            link: function (scope, element, attrs) {
                //console.log(scope.items);
                var myContainer = "container" + scope.idcontainer;   // construction de notre id container
                var template = '<div id="' + myContainer + '" style="margin: 0 auto ;">not working</div>';   // le template inclu notre id container
                //console.log(myContainer);
                //console.log(template);

                //Rendering template.
                element.html('').append($compile(template)(scope));   // on fait le rendu dans le DOM

                initChart(myContainer, scope.items);

                function initChart(container, dataItems) {
                    var chart = new Highcharts.Chart({                    // puis on dessine avec Highchart
                        chart: {
                            renderTo: container,  // render to..       // sur notre container variable
                            plotBackgroundColor: null,
                            plotBorderWidth: null,
                            plotShadow: false,
                            type: 'pie'
                        },
                        title: {
                            text: ''
                        },
                        tooltip: {
                            pointFormat: '{point.y}',
                            percentageDecimals: 1
                        },
                        plotOptions: {
                            pie: {
                                allowPointSelect: true,
                                cursor: 'pointer',
                                dataLabels: {
                                    enabled: true
                                },
                                showInLegend: false
                            }
                        },
                        series: [{
                            //name: 'Browser share',
                            data: dataItems
                        }]
                    });
                }

                scope.$watch("items", function (newValue) {
                    initChart(myContainer, newValue);
                }, true);
            }
        }
    });

})();