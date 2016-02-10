var BackOfficeAngularMVCApp = angular.module('BackOfficeAngularMVCApp', ['ngRoute', 'ngTable', 'BackOfficeFilters', 'ngDialog', 'ui.bootstrap', 'ui.select', 'ngSanitize', 'nvd3ChartDirectives']);

BackOfficeAngularMVCApp.controller('DashboardController', DashboardController);
BackOfficeAngularMVCApp.controller('ChartController', ChartController);
BackOfficeAngularMVCApp.controller('AccountHistoryController', AccountHistoryController);
BackOfficeAngularMVCApp.controller('RebatesSummaryReportController', RebatesSummaryReportController);
BackOfficeAngularMVCApp.controller('RebatesSummaryByUserReportController', RebatesSummaryByUserReportController);
BackOfficeAngularMVCApp.controller('TradeHistoryForIBReportController', TradeHistoryForIBReportController);
BackOfficeAngularMVCApp.controller('RebatesAmountPerSymbolController', RebatesAmountPerSymbolController);
BackOfficeAngularMVCApp.controller('SettingsController', SettingsController);
BackOfficeAngularMVCApp.controller('ContactUsController', ContactUsController);

BackOfficeAngularMVCApp.directive('periodSelector', periodSelector);

var configFunction = function ($routeProvider, $locationProvider) {
	var getCurrentUserDefaultUrl = function () {
		var defaultUrl = '';

		jQuery.ajax({
			type: "POST",
			url: "/Account/GetCurrentUserDefaultUrl",
			contentType: "application/json",
			success: function (result) {
				defaultUrl = result.DefaultUrl;
			},
			async: false
		});

		return defaultUrl;
	}


	$routeProvider.when('/Dashboard', { templateUrl: 'Dashboard' }).
		when('/AccountHistory', { templateUrl: 'AccountHistory' }).
        when('/RebatesSummaryReport', { templateUrl: 'RebatesSummary/RebatesSummaryReport', reloadOnSearch: false }).
		when('/RebatesSummaryByUserReport', { templateUrl: 'RebatesSummary/RebatesSummaryByUserReport', reloadOnSearch: false }).
        when('/TradeHistory/Transactions', { templateUrl: 'TradeHistoryForIB/FullTradeHistory', reloadOnSearch: false }).
        when('/TradeHistory/ClosedOnly', { templateUrl: 'TradeHistoryForIB/ClosedOnlyTradeHistory', reloadOnSearch: false }).
		when('/RebatesCumulativeChart', { templateUrl: 'Chart/RebatesCumulative' }).
        when('/RebatesAmountPerSymbol', { templateUrl: 'Chart/RebatesAmountPerSymbol' }).
        when('/Settings', { templateUrl: 'Settings/Settings' }).
        when('/ContactUs', { templateUrl: 'ContactUs/ContactUs' }).
        when('/WithdrawalRequest', { templateUrl: 'WithdrawalRequest/WithdrawalRequest' }).
		otherwise({ redirectTo: getCurrentUserDefaultUrl });
}
configFunction.$inject = ['$routeProvider', '$locationProvider'];

BackOfficeAngularMVCApp.config(configFunction);

BackOfficeAngularMVCApp.directive('jqdatepicker', function () {
	return {
		restrict: 'A',
		require: 'ngModel',
		link: function (scope, element, attrs, ngModelCtrl) {
			var format = "yy-mm-dd";
			scope.$watch(attrs.ngModel, function (date) {
				var result = "";
				if (date !== "" && date !== null && date !== undefined) {
					result = $.datepicker.formatDate(format, new Date(date));
				}
				element.val(result);
			});

			$(element).datepicker({
				numberOfMonths: 3,
				dateFormat: format,
				showButtonPanel: true,
				showCurrentAtPos: 2,
				onSelect: function (date) {
					var ngModelName = this.attributes['ng-model'].value;

					// if value for the specified ngModel is a property of
					// another object on the scope
					if (ngModelName.indexOf(".") != -1) {
						var objAttributes = ngModelName.split(".");
						var lastAttribute = objAttributes.pop();
						var partialObjString = objAttributes.join(".");
						var partialObj = eval("scope." + partialObjString);

						partialObj[lastAttribute] = date;
					}
						// if value for the specified ngModel is directly on the scope
					else {
						scope[ngModelName] = date;
					}
					scope.$apply();
				}

			});
		}
	};
});

BackOfficeAngularMVCApp.directive(
    'formattedDateInput',
    function (dateFilter) {
    	return {
    		require: 'ngModel',
    		template: '<input type="text"></input>',
    		replace: true,
    		link: function (scope, elm, attrs, ngModelCtrl) {
    			ngModelCtrl.$formatters.unshift(function (modelValue) {
    				return dateFilter(modelValue, 'yyyy-MM-dd');
    			});

    			ngModelCtrl.$parsers.unshift(function (viewValue) {
    				if (viewValue !== "" && viewValue !== null && viewValue !== undefined) {
    					var date = new Date(viewValue);
    					if (isNaN(date.getDate())) {
    						return undefined;
    					}
    					return new Date(viewValue);
    				}
    			});
    		},
    	};
    });

BackOfficeAngularMVCApp.directive("onlyDigits", function () {

	return {
		restrict: 'A',
		require: '?ngModel',
		link: function (scope, element, attrs, ngModel) {
			if (!ngModel) return;
			ngModel.$parsers.unshift(function (inputValue) {
				var digits = inputValue.split('').filter(function (s) { return (!isNaN(s) && s != ' '); }).join('');
				ngModel.$viewValue = digits;
				ngModel.$render();
				return digits;
			});
		}
	};
});

//filter for multiple filtration in select2
var propsFilter = function () {
    return function (items, props) {
        var out = [];

        if (angular.isArray(items)) {
            items.forEach(function (item) {
                var itemMatches = false;

                var keys = Object.keys(props);
                for (var i = 0; i < keys.length; i++) {
                    var prop = keys[i];
                    var text = props[prop].toLowerCase();
                    if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                        itemMatches = true;
                        break;
                    }
                }

                if (itemMatches) {
                    out.push(item);
                }
            });
        } else {
            // Let the output be the input untouched
            out = items;
        }

        return out;
    };
};

BackOfficeAngularMVCApp.filter('propsFilter', propsFilter);