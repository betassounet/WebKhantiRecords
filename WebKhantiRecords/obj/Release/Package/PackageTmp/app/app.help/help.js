(function () {
    'use strict';

    angular
        .module('app')
        .controller('helpCtrl', ['$scope', '$http', 'viewName', '$routeParams', 'GlobalService', helpCtrl]);

    // ICI on a un prototype qui récupère des données du routage via le route provider avec le viewname et via le routeParam qui relais les parametres passés directmeent dans l'URL
    function helpCtrl($scope, $http, viewName, $routeParams, GlobalService) {

        var BASE_URL_API = GlobalService.base_WebAPI_URL;

        //------------------------------------------------------------------------------------------------
        // Identification Ligne a partir menu
        //------------------------------------------------------------------------------------------------
        $scope.view = {                        // Le viewname permet de recup des donnees differentes en fonction du lien clické
            url: viewName.url,             // le meme controler et la meme view html sont invoqué a chaque fois avec des données différentes 
            titre: viewName.titre,
            showpdf: $routeParams.showpdf ==="true",
            pdf: $routeParams.pdf,
            url2: 'Documents/'+ $routeParams.pdf,           // decodage des parametres passé dans directement dans l'URL
            showTodo: $routeParams.showTodo ==="true",
            TodoFile:$routeParams.TodoFile
        };

        activate();
        function activate() {
            if ($scope.view.showTodo) {
                UpdateTodoText("Get", "");
            }
        }

        $scope.UpdateTodoText = function (texte) {
            UpdateTodoText("Set", texte);
        }

        function UpdateTodoText(sAction, texte) {
            var param = { sAction: sAction, sTexte: texte, FileName: $scope.view.TodoFile };
            $http.post(BASE_URL_API + "api/Config/ActionTodoFile", param).then(function (response) {
                $scope.RepActionTodoFile = response.data;
            });
        }


        
        // TEST EDITEUR DE CODE WEB codemirror
        // voir http://angular-ui.github.io/ui-codemirror/
        //https://codemirror.net/
        //http://plnkr.co/edit/?p=preview
        // Ne pas oublier l'injection de 'ui.codemirror'  dans le module app angular..


        // The ui-codemirror option
        $scope.cmOption = {
            lineNumbers: true,
            indentWithTabs: true,
            onLoad: function (_cm) {
                // HACK to have the codemirror instance in the scope...
                $scope.modeChanged = function () {
                    _cm.setOption("mode", $scope.mode.toLowerCase());
                };
            }
        };

        // The modes
        $scope.modes = ['Scheme', 'XML', 'Javascript', 'clike', 'css', 'htmlmixed', 'php', 'python'];
        $scope.mode = $scope.modes[0];

        // Initial code content...
        $scope.cmModel = ';; Scheme code in here.\n' +
          '(define (double x)\n\t(* x x))\n\n\n' +
          '<!-- XML code in here. -->\n' +
          '<root>\n\t<foo>\n\t</foo>\n\t<bar/>\n</root>\n\n\n' +
          '// Javascript code in here.\n' +
          'function foo(msg) {\n\tvar r = Math.random();\n\treturn "" + r + " : " + msg;\n}';
    }

})();
