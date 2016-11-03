angular.module('impemasapp')
.controller('NavegacionController', function ($scope, $http, $mdSidenav, $document) {
    $scope.activarProgreso = false;
    $scope.modulos = ['Cuentas', 'Clientes', 'Ventas', 'Reportes'];
    $scope.moduloAct = 'Ventas';

    $scope.contactandoServidor = function () {
        return $http.pendingRequests.length > 0;
    };

    $scope.$watch($scope.contactandoServidor, function (v) {
        $scope.activarProgreso = v;
    });

    $scope.agregar = function (event) {
        $scope.$broadcast('Agregar', { origen: event, modulo: $scope.moduloAct });
    };

    $scope.$on('CambioClientes', function (ev, data) {
        $scope.$broadcast('CambioClientesBc', data);
    });
});