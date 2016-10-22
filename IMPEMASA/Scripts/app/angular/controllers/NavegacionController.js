angular.module('impemasapp')
.controller('NavegacionController', function ($scope, $http, $mdSidenav, $document) {
    $scope.activarProgreso = false;
    $scope.modulos = ['Bancos', 'Cuentas', 'Clientes', 'Ventas'];
    $scope.moduloAct = null;

    $scope.toggleNav = function () {
        $mdSidenav('left').toggle();
    };

    $scope.contactandoServidor = function () {
        return $http.pendingRequests.length > 0;
    };

    $scope.$watch($scope.contactandoServidor, function (v) {
        $scope.activarProgreso = v;
    });

    $scope.mostrarModulo = function (modulo) {
        $scope.moduloAct = modulo;
    };

    $scope.agregar = function (event) {
        $scope.$broadcast('Agregar', { origen: event, modulo: $scope.moduloAct });
    };
});