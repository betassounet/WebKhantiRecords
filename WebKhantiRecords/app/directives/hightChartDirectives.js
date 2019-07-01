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


    //--------------------------------------------------------------------------------------------------------------------------------------
    //    highChartCombinationChart
    //--------------------------------------------------------------------------------------------------------------------------------------

    app.directive('highChartCombinationChart', function ($compile) {
        return {
            restrict: 'C',
            replace: true,
            scope: {
                items: '=',
                idcontainer: '=',    // on se donne un attribut supplementaire
            },
            controller: function ($scope, $element, $attrs) {
                //console.log($element);
            },
            link: function (scope, element, attrs) {
                var myContainer = "container" + scope.items.idGraph;   // construction de notre id container
                var myheight = scope.items.height;
                // var template = '<div id="' + myContainer + '" style="margin: 0 auto">highChartLineIrregular not working</div>'   // le template inclu notre id container
                var template = '<div id="' + myContainer + '" style="margin: 0; height:' + myheight + ';">highChartLineIrregular not working</div>'   // le template inclu notre id container
                //Rendering template.
                element.html('').append($compile(template)(scope));   // on fait le rendu dans le DOM

                initChart(myContainer, scope.items);

                //var initChart = function (container, dataItems) {    // ce type de declaration de fonction ne permet pas l'appel anticipé
                function initChart(container, dataItems) {

                    var chart = new Highcharts.Chart({                    // puis on dessine avec Highchart

                        chart: {
                            renderTo: container,  // render to..       // sur notre container variable
                            marginBottom: 20,
                            zoomType: 'x',
                            height: 400,
                            width: 1000,
                        },
                        title: {
                            //text: dataItems.title   // 'Test combinaison charts'
                            text: null
                        },
                        subtitle: {
                            //text: dataItems.soustitre
                            text: null
                        },
                        xAxis: {
                            type: 'datetime',
                            dateTimeLabelFormats: { // don't display the dummy year
                                minute: '%H:%M',
                                hour: '%H:%M'       // voir http://stackoverflow.com/questions/7101464/how-to-get-highcharts-dates-in-the-x-axis
                                //month: '%e. %b',
                                //year: '%b'
                            },
                            title: {
                                text: 'Date'
                            }
                        },
                        yAxis: [
                            {
                                labels:{
                                    format:'{value} €'
                                },
                                title:{
                                    text:'Payable'
                                },
                                opposite: true
                            },
                            {
                                labels: {
                                    format: 'nb :{value} load'
                                },
                                title: {
                                    text: 'DownLoad'
                                }
                            }
                        ]
                        ,
                        labels: {
                            items: [{
                                //html: 'Total fruit consumption',
                                style: {
                                    left: '50px',
                                    top: '18px',
                                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'black'
                                }
                            }]
                        },
                        series: dataItems.serieCombine
                        ,
                        click: function (event) {
                            console.log("event");
                        },
                        tooltip: {     // voir pour action sur tooltip :        http://ahumbleopinion.com/customizing-highcharts-tooltip-visibility/
                            //  http://ahumbleopinion.com/customizing-highcharts-tooltip-positioning/
                            //positioner: function (labelWidth, labelHeight, point) {
                            //    var tooltipX, tooltipY;
                            //    tooltipX = point.plotX + 20;
                            //    tooltipY = point.plotY - 20;
                            //    return {
                            //        x: tooltipX,
                            //        y: tooltipY
                            //    };
                            //}
                            useHTML: true
                        }
                    });
                }

                //initChart(myContainer, scope.items);

                scope.$watch("items", function (newValue) {
                    initChart(myContainer, newValue);
                }, true);
            }
        }
    });




})();