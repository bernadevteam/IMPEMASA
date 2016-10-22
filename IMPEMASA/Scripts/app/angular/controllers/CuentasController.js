angular.module('impemasapp')
.controller('CuentasController', function ($scope, $mdSidenav, $mdDialog, $http, $mdToast) {
    $scope.cuentas = [];
    $scope.cuenta = {activo:true, idbanco:0};
    var urlcuenta = 'api/Cuentas';

    $http.get(urlcuenta).then(function (res) {
        $scope.cuentas = res.data;
    });


    var manejocuenta = function (ev, cuentaSel) {
        var ctaPivot = {activa:true};
        angular.merge(ctaPivot, cuentaSel);

        $mdDialog.show({
            locals: { modelo: ctaPivot },
            controller: DialogCuentasController,
            templateUrl: 'Plantillas/FormCuenta.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            clickOutsideToClose: false
        })
     .then(function (cuenta) {
         //Resultado popup
         if (cuenta.id != null) {
             $http.put(urlcuenta, cuenta).then(function (res) {
                 angular.merge(cuentaSel, res.data);
                 $mdToast.show($mdToast.simple()
        .textContent('Modificado con exito!')
        .position('top right')
        .hideDelay(3000)
    );
             });
         } else {
             $http.post(urlcuenta, cuenta).then(function (res) {
                 $scope.cuentas.push(res.data);
                 $mdToast.show($mdToast.simple()
        .textContent('Creado con exito!')
        .position('top right')
        .hideDelay(3000)
    );
             });
         }
     }).finally(function () {
         $scope.cuenta = {};
     });
    };

    $scope.$on('Agregar', function (event, data) {
        if (data.modulo.toLowerCase() == 'cuentas') {
            manejocuenta(data.origen);
        }
    });

    $scope.editar = manejocuenta;

});

function DialogCuentasController($scope, $mdDialog, $http, modelo) {
    $scope.bancos = [];
    $scope.cuenta = modelo;

    $http.get('api/Bancos').then(function (res) {
        $scope.bancos = res.data;
    });

    $scope.cerrar = function () {
        $mdDialog.cancel();
    };

    $scope.cancel = function () {
        $mdDialog.cancel();
    };

    $scope.answer = function () {
        $mdDialog.hide($scope.cuenta);
    };
}