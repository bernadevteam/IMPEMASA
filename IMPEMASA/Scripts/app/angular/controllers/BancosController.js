angular.module('impemasapp')
.controller('BancosController', function ($scope, $mdSidenav, $mdDialog, $http, $mdToast) {
    $scope.bancos = [];
    $scope.banco = {};

    $http.get('api/Bancos').then(function (res) {
        $scope.bancos = res.data;
    });

    var urlBanco = 'api/Bancos';
    var manejoBanco = function (ev, bancoSel) {
        angular.merge($scope.banco, bancoSel || {});
        $mdDialog.show({
            scope: $scope,
            preserveScope: true,
            controller: DialogController,
            templateUrl: 'Plantillas/FormBanco.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            clickOutsideToClose: false
        })
     .then(function () {
         var banco = $scope.banco;

         if (banco.id != null) {
             $http.put(urlBanco, banco).then(function () {
                 angular.merge(bancoSel, banco);
                 $mdToast.show($mdToast.simple()
        .textContent('Modificado con exito!')
        .position('top right')
        .hideDelay(3000)
    );
             });
         } else {
             $http.post(urlBanco, banco).then(function () {
                 $scope.bancos.push(banco);
                 $mdToast.show($mdToast.simple()
        .textContent('Creado con exito!')
        .position('top right')
        .hideDelay(3000)
    );
             });
         }
     }).finally(function () {
         $scope.banco = {};
     });
    };

    $scope.$on('Agregar', function (event, data) {
        if (data.modulo.toLowerCase() == 'bancos') {
            manejoBanco(data.origen);
        }
    });

    $scope.editar = manejoBanco;

});

function DialogBancosController() {
}