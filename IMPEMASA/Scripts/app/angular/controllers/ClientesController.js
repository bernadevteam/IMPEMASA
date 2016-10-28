/// <reference path="ClientesController.js" />
angular.module('impemasapp')
.controller('ClientesController', function ($scope, $mdSidenav, $mdDialog, $http, $mdToast) {
    $scope.clientes = [];
    $scope.cliente = {};

    var urlcliente = 'api/Clientes';

    $http.get(urlcliente).then(function (res) {
        $scope.clientes = res.data;
        enviarCambioClientes();
    });


    var manejocliente = function (ev, clienteSel) {
        var ctaPivot = {};
        angular.merge(ctaPivot, clienteSel);

        $mdDialog.show({
            locals: { modelo: ctaPivot },
            controller: DialogClienteController,
            templateUrl: 'Plantillas/FormCliente.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            clickOutsideToClose: false
        })
     .then(function (cliente) {
         //Resultado popup
         if (cliente.id != null) {
             $http.put(urlcliente, cliente).then(function (res) {
                 angular.merge(clienteSel, cliente);
                 $mdToast.show($mdToast.simple()
        .textContent('Modificado con exito!')
        .position('top right')
        .hideDelay(3000)
    );
                 enviarCambioClientes();
             });
         } else {
             $http.post(urlcliente, cliente).then(function (res) {
                 $scope.clientes.push(res.data);
                 $mdToast.show($mdToast.simple()
        .textContent('Creado con exito!')
        .position('top right')
        .hideDelay(3000)
    );
                 enviarCambioClientes();
             });
         }
     }).finally(function () {
         $scope.cliente = {};
     });
    };

    $scope.$on('Agregar', function (event, data) {
        if (data.modulo.toLowerCase() == 'clientes') {
            manejocliente(data.origen);
        }
    });

    $scope.editar = manejocliente;

    function enviarCambioClientes() {
        $scope.$emit('CambioClientes', $scope.clientes);
    }
});

function DialogClienteController($scope, $mdDialog, modelo) {
    $scope.cliente = modelo;

    $scope.cerrar = function () {
        $mdDialog.cancel();
    };

    $scope.cancel = function () {
        $mdDialog.cancel();
    };

    $scope.answer = function () {
        $scope.cliente.nombre = $scope.cliente.nombre.toUpperCase();
        $mdDialog.hide($scope.cliente);
    };
}