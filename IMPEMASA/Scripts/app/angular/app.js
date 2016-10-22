angular.module('impemasapp', ['ngMaterial', 'ngMessages'])
    .config(function ($mdDateLocaleProvider) {
        $mdDateLocaleProvider.formatDate = function (date) {
            return moment(date).format('MM/DD/YYYY');
        };

        $mdDateLocaleProvider.parseDate = function (dateString) {
            var m = moment(dateString, 'MM/DD/YYYY');
            return m.isValid() ? m.toDate() : new Date(NaN);
        };
    })
.run(function ($http) {
    $http.defaults.headers.common.Authorization = 'Bearer ' + sessionStorage.getItem("accessToken");
});

function DialogController($scope, $mdDialog) {
    $scope.cerrar = function () {
        $mdDialog.cancel();
    };

    $scope.cancel = function () {
        $mdDialog.cancel();
    };

    $scope.answer = function () {
        $mdDialog.hide();
    };
}

