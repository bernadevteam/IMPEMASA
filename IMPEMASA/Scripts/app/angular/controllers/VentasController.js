angular.module('impemasapp')
.controller('VentasController', function ($scope, $mdSidenav, $mdDialog, $http, $mdToast) {
    $scope.ventas = [];
    $scope.venta = {};

    var urlventa = 'api/Ventas';

    $http.get(urlventa).then(function (res) {
        $scope.ventas = res.data;
    });

    var manejoventa = function (ev, ventaSel) {
        var ctaPivot = { pagoPendiente: true };
        angular.merge(ctaPivot, ventaSel);

        $mdDialog.show({
            locals: { modelo: ctaPivot },
            controller: DialogVentaController,
            templateUrl: 'Plantillas/FormVenta.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            clickOutsideToClose: false
        })
     .then(function (venta) {
         //Resultado popup
         if (venta.id != null) {
             $http.put(urlventa, venta).then(function (res) {
                 angular.merge(ventaSel, res.data);
                 $mdToast.show($mdToast.simple()
        .textContent('Modificado con exito!')
        .position('top right')
        .hideDelay(3000)
    );
             });
         } else {
             $http.post(urlventa, venta).then(function (res) {
                 $scope.ventas.push(res.data);
                 $mdToast.show($mdToast.simple()
        .textContent('Creado con exito!')
        .position('top right')
        .hideDelay(3000)
    );
             });
         }
     }).finally(function () {
         $scope.venta = {};
     });
    };

    $scope.$on('Agregar', function (event, data) {
        if (data.modulo.toLowerCase() == 'ventas') {
            manejoventa(data.origen);
        }
    });

    $scope.editar = manejoventa;

});

function DialogVentaController($scope, $mdDialog, $http, $filter, modelo) {
    $scope.venta = modelo;
    $scope.clientes = ['Cargando...'];
    $scope.tiposVenta = ['Cargando...'];
    $scope.cargandoMsg = 'Cargando clientes...';
    $scope.cliente = {};
    $scope.fechaFactura = {};

    $scope.$watch('cliente', function (n) {
        if (n.id) {
            $scope.venta.idCliente = n.id;
            $scope.venta.rnc = n.rnc;
        }
    });

    $http.get('api/Clientes').then(function (res) {
        $scope.clientes = res.data;
        if (modelo.id) {
            $scope.cliente = $filter('filter')($scope.clientes, { id: modelo.idCliente })[0];
            $scope.fechaFactura = moment(modelo.fecha,'MM/DD/YYYY').toDate();

        }
        $scope.cargandoMsg = 'Cargando tipo ventas...';
        $http.get('api/VentaTipos').then(function (restipos) {
            $scope.tiposVenta = restipos.data;
            $scope.cargandoMsg = null;
        });
    });

    $scope.calcular = function () {
        var st = $scope.venta.subTotal;
        var itbis = 0.18;
        if (isNaN(st)) {
            $scope.venta.itbis = 0.0;
            $scope.venta.total = 0.0;
        } else {
            $scope.venta.itbis = (st * itbis).toFixed(2);
            $scope.venta.total = ((st * itbis) + st).toFixed(2);
        }
    };

    $scope.cerrar = function () {
        $mdDialog.cancel();
    };

    $scope.cancel = function () {
        $mdDialog.cancel();
    };

    $scope.answer = function () {
        $mdDialog.hide($scope.venta);
    };
}