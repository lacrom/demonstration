var RebatesSummaryReportController = function ($scope, ngTableParams, $location, $routeParams) {

	var self = this;

	//get all traders
	jQuery.ajax({
	    type: "POST",
	    url: 'RebatesSummary/GetTradersForIB',
	    data: JSON.stringify({}),
	    contentType: "application/json",
	    success: function (result) {
	        self.userList = [];
	        self.userList.push({ UserName: "All Traders", UserID: "All" });
	        for (i = 0; i < result.length; i++) {
	            self.userList.push(result[i]);
	        };

	        self.selectedTraderItem = self.userList[0];
	    },
	    async: false
	});

    //query all symbols
	jQuery.ajax({
	    type: "POST",
	    url: 'RebatesSummary/GetSymbols',
	    contentType: "application/json",
	    success: function (result) {
	        self.symbolList = [];
	        self.symbolList.push({ name: "All Symbols", value: undefined });
	        for (i = 0; i < result.length; i++) {
	            self.symbolList.push({ name: result[i], value: result[i] });
	        };

	        self.selectedSymbol = self.symbolList[0];
	    },
	    async: false
	});

    //if parameters came from URL
	if ($routeParams.selectedTraderItem !== undefined) {
	    //get current user
	    var currentUser = self.userList.filter(function (item) {
	        return item.UserID == $routeParams.selectedTraderItem;
	    })[0];

	    self.selectedTraderItem = currentUser;
	}

	if ($routeParams.Symbol !== undefined) {
	    //get current symbol
	    var currentSymbol = self.symbolList.filter(function (item) {
	        return item.name == $routeParams.Symbol;
	    })[0];

	    self.selectedSymbol = currentSymbol;
	}

	$scope.DownloadAsCSV = function () {
		var params = $scope.reportParameters;

		if (params.PeriodFrom == "" || params.PeriodTo == "") {
			return;
		}

		//get UserID for request
		var userID;
		if (self.selectedTraderItem === undefined || self.selectedTraderItem.UserID == "All") {
			userID = undefined;
		} else {
			userID = self.selectedTraderItem.UserID;
		}
	    //get symbol for request
		var symbol;
		if (self.selectedSymbol === undefined) {
		    symbol = undefined;
		} else {
		    symbol = self.selectedSymbol.value;
		}

		var data = { reportParams: { PeriodFrom: $scope.reportParameters.PeriodFrom, PeriodTo: $scope.reportParameters.PeriodTo}, UserID: userID, Symbol: symbol };

		$.fileDownload('RebatesSummary/DownloadAsCSV', {
			httpMethod: "POST",
			data: data,
		});
	}

	var updateViewModel = function ($defer, tableParams) {

		//get UserID for request
		var userID;
		if (self.selectedTraderItem === undefined || self.selectedTraderItem.UserID == "All" ) {
			userID = undefined;
		} else {
			userID = self.selectedTraderItem.UserID;
		}
	    //get symbol for request
		var symbol;
		if (self.selectedSymbol === undefined) {
		    symbol = undefined;
		} else {
		    symbol = self.selectedSymbol.value;
		}

		var data = { reportParams: { PeriodFrom: $scope.reportParameters.PeriodFrom, PeriodTo: $scope.reportParameters.PeriodTo }, tableParams: tableParams.$params, UserID: userID, Symbol: symbol };

		busyAjaxRequest($scope, "box", {
			type: "POST",
			url: "RebatesSummary/GetJsonData",
			data: JSON.stringify(data),
			contentType: "application/json",
			success: function (result) {
				$scope.result = result;
				tableParams.total(result.TotalCount);
				if ($defer != null) {
					$defer.resolve(result.RebatesSummary);
				}

				//draw header for table
				drawFrozenHeader();
			},
			async: true
		});

	}

	self.getReport = function () {
		var UserID;

		if (self.selectedTraderItem === undefined || self.selectedTraderItem == {} || self.selectedTraderItem.UserID == "All") {
			UserID = undefined;
		} else {
			UserID = self.selectedTraderItem.UserID;
		}

	    //get symbol for request
		if (self.selectedSymbol === undefined) {
		    self.selectedSymbol = { value: undefined };
		}

		$location.search("PeriodFrom", $scope.reportParameters.PeriodFrom).search("PeriodTo", $scope.reportParameters.PeriodTo).search("selectedTraderItem", UserID).search("Symbol", self.selectedSymbol.value);

		self.showReport();
	}

	self.showReport = function () {
		if ($scope.tableParams === undefined) {
			$scope.tableParams = new ngTableParams({
				page: 1,            // show first page
				count: 100,          // count per page
				sorting: {
					Rebates: 'desc'     // initial sorting
				}
			}, {
				total: 0, // length of data
				getData: function ($defer, tableParams) {
					updateViewModel($defer, tableParams);
				}
			});
		} else {
			$scope.tableParams.reload();
		}
	}

	if ($routeParams.PeriodFrom !== undefined
		&& $routeParams.PeriodTo !== undefined) {

		$scope.reportParameters = {
			PeriodFrom: $routeParams.PeriodFrom,
			PeriodTo: $routeParams.PeriodTo,
		}

		self.showReport();
	}
}