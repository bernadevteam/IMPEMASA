angular.module('impemasapp')
    .filter('ventaspendientes', function () {
        return function (ventas, dias) {
            var pendientes = [];

            angular.forEach(ventas, function (venta) {
                if (venta.pagoPendiente) {
                    var minDia = dias - 30;
                    if (dias > 91 && venta.diasPendientes > 91 || venta.diasPendientes >= minDia && venta.diasPendientes <= dias) {
                        pendientes.push(venta);
                    }
                }
            });

            return pendientes;
        };
    })
      .directive('tarjetaVenta', function () {
          return {
              restrict: 'E',
              templateUrl: 'Plantillas/TarjetaVenta.html?V1.2'
          }
      })
.controller('VentasController', function ($scope, $filter, $mdSidenav, $mdDialog, $http, $mdToast) {
    $scope.ventas = [];
    $scope.venta = {};
    var tiposDeps = [],
        clientes = [],
        tiposVentas = [],
    cuentas = [];

    var urlventa = 'api/Ventas',
        urlCuentas = 'api/Cuentas',
        urltipos = 'api/VentaTipos',
        urldeposito = 'api/Depositos',
    urlDepTipos = 'api/DepositoTipos';

    $http.get(urlventa + '/VentasPendientes').then(function (res) {
        $scope.ventas = res.data;
        $http.get(urlDepTipos).then(function (tiposdatas) {
            tiposDeps = tiposdatas.data;
            $http.get(urlCuentas).then(function (cuentasdata) {
                cuentas = cuentasdata.data;
                $http.get('api/VentaTipos').then(function (restipos) {
                    tiposVentas = restipos.data;
                });
            });
        });
    });

    $scope.eliminarVenta = function (ev, venta) {
        var confirm = $mdDialog.confirm()
                 .title('Deseas eliminar esta venta?')
                 .textContent('Todos los depositos relacionados también se eliminarán.')
                 .targetEvent(ev)
                 .ok('Adelante!')
                 .cancel('Mejor no');

        $mdDialog.show(confirm)
.then(function () {
    $http.delete(urlventa + '?id=' + venta.id).then(function () {
        var i = $scope.ventas.indexOf(venta);
        $scope.ventas.splice(i, 1);
        toast('Venta eliminada con exito!');
    }, function () {
        toast('No se pudo eliminar la venta. Intente mas tarde o contacte al administrador.');
    });
});

    };

    $scope.verEstadoCuenta = function (ev, idCliente) {
        var pendientes = $filter('filter')($scope.ventas, { idCliente: idCliente, pagoPendiente: true });
        var estadoCliente = $filter('filter')(clientes, { id: idCliente })[0];

        var sumaPendientes = 0.0;

        for (var i in pendientes) {
            sumaPendientes = sumaPendientes + pendientes[i].totalPendiente;
        }

        $scope.estadoCuenta = { pendientes: pendientes, cliente: estadoCliente, totalPendiente: sumaPendientes };

        $mdDialog.show({
            locals: { estadoCuenta: $scope.estadoCuenta },
            controller: DialogEstadoCuentaController,
            templateUrl: 'Plantillas/EstadoCuenta.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            clickOutsideToClose: false
        }).finally(function () {
            $scope.estadoCuenta = null;
            $scope.estadoCliente = null;

        });
    }

    $scope.totalDepositos = function (depositos) {
        var total = 0.0;

        if (depositos) {
            for (var i = 0; i < depositos.length; i++) {
                total += depositos[i].monto;
            }
        }

        return total;
    }

    var manejoventa = function (ev, ventaSel) {
        var ctaPivot = { pagoPendiente: true };
        angular.merge(ctaPivot, ventaSel);

        $mdDialog.show({
            locals: { modelo: ctaPivot, clientes: clientes, tipoventas: tiposVentas },
            controller: DialogVentaController,
            templateUrl: 'Plantillas/FormVenta.html?V1.1',
            parent: angular.element(document.body),
            targetEvent: ev,
            clickOutsideToClose: false
        })
     .then(function (venta) {
         //Resultado popup
         if (venta.id != null) {
             $http.put(urlventa, venta).then(function (res) {
                 angular.merge(ventaSel, res.data);
                 toast('Modificado con exito!');
             });
         } else {
             $http.post(urlventa, venta).then(function (res) {
                 $scope.ventas.push(res.data);
                 toast('Creado con exito!');
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

    $scope.$on('CambioClientesBc', function (ev, data) {
        clientes = data;
    });

    var manejodeposito = function (ev, venta, depSel) {
        var ctaPivot = { idVenta: venta.id };
        angular.merge(ctaPivot, depSel);

        $mdDialog.show({
            locals: { modelo: ctaPivot, tipos: tiposDeps, cuentas: cuentas },
            controller: DialogDepositoController,
            templateUrl: 'Plantillas/FormDeposito.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            clickOutsideToClose: false
        })
     .then(function (deposito) {
         //Resultado popup
         if (deposito.id != null) {
             $http.put(urldeposito, deposito).then(function (res) {
                 angular.merge(depSel, res.data);
                 toast('Modificado con exito!');
             });
         } else {
             $http.post(urldeposito, deposito).then(function (res) {
                 if (venta.depositos == null) {
                     venta.depositos = [];
                 }
                 venta.depositos.push(res.data);
                 toast('Creado con exito!');
             });
         }
     });
    };

    $scope.editar = manejoventa;
    $scope.editarDeposito = manejodeposito

    function toast(msg) {
        $mdToast.show($mdToast.simple()
     .textContent(msg)
     .position('bottom right')
     .hideDelay(5000)
 );
    }

});

function DialogVentaController($scope, $mdDialog, $http, $filter, modelo, clientes, tipoventas) {
    $scope.venta = modelo;
    $scope.tiposVenta = ['Cargando...'];
    $scope.cliente = {};
    $scope.fechaFactura = {};
    $scope.tiposVenta = tipoventas;

    $scope.$watch('cliente', function (n) {
        if (n.id) {
            $scope.venta.idCliente = n.id;
            $scope.venta.rnc = n.rnc;
        }
    });

    $scope.clientes = clientes;
    if (modelo.id) {
        $scope.cliente = $filter('filter')($scope.clientes, { id: modelo.idCliente })[0];
        $scope.fechaFactura = moment(modelo.fecha, 'MM/DD/YYYY').toDate();
    }

    $scope.calcular = function () {
        var st = $scope.venta.subTotal;
        var itbis = $scope.venta.idVentaTipo == 3 ? 0.0 : 0.18;

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

function DialogDepositoController($scope, $mdDialog, $http, $filter, modelo, tipos, cuentas) {
    $scope.deposito = modelo;
    $scope.cuentas = cuentas;
    $scope.tipos = tipos;
    if (modelo.id) {
        $scope.fechaDeposito = moment(modelo.fecha, 'MM/DD/YYYY').toDate();
    }

    $scope.cerrar = function () {
        $mdDialog.cancel();
    };

    $scope.cancel = function () {
        $mdDialog.cancel();
    };

    $scope.answer = function () {
        $mdDialog.hide($scope.deposito);
    };
}

function DialogEstadoCuentaController($scope, $mdDialog, estadoCuenta) {
    $scope.pendientes = estadoCuenta.pendientes;
    $scope.cliente = estadoCuenta.cliente;

    $scope.fecha = moment().format('MM/DD/YYYY');
    $scope.fechaVencimiento = moment().format('MM/DD/YYYY');

    $scope.cerrar = function () {
        $mdDialog.cancel();
    };

    $scope.imprimir = function () {
        window.print();
    };

    $scope.cancel = function () {
        $mdDialog.cancel();
    };

    $scope.sumaPendientes = estadoCuenta.totalPendiente;

    $scope.answer = function () {
        $mdDialog.hide($scope.deposito);
    };
}