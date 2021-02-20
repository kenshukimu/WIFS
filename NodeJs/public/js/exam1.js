var app1 = angular.model('app1',[]);

app1.contoller('ctrl1', function($scope) {
    $scope.first = 1;
    $scope.second = 1;

    $scope.updateValue = function() {
        $scope.calculation = $scope.first + '+' + $scope.second + '=' + (+$scope.first + +$scope.second);
    };
});