$(function() {
    var errorVMInstance;

    ko.bindingHandlers.knob = {
        init: function(element, valueAccessor) {
            var value = valueAccessor()();
            $(element).knob({ width: 120, height: 70, inputColor: '#fff', fgColor: '#12b0c5', min: 0, max: 100, angleArc: 100, angleOffset: -50, readOnly: true });
        },
        update: function(element, valueAccessor, allBindingsAccessor) {
            var value = valueAccessor()();
            $(element).val(parseFloat(value)).trigger('change');
        }
    };

    var ParametersViewModel = function () {
        var self = this;
        this.token = ko.observable("");
        this.strategy = ko.observable("Naieve");
        this.periodCount = ko.observable();
        this.alpha = ko.observable();
        this.beta = ko.observable();
        this.gamma = ko.observable();
        this.periodsPerSeason = ko.observable();
        this.seasonsForRegression = ko.observable();
        this.start = function() {
            $.connection.forecastingDataHub.server.register({
                Token: self.token()
            });
            $.connection.forecastingDataHub.server.modifyParameters(
                {
                    Token: self.token()
                },
                {
                    Strategy: strategy(),
                    PeriodCount: periodCount(),
                    Alpha: alpha(),
                    Beta: beta(),
                    Gamma: gamma(),
                    PeriodsPerSeason: periodsPerSeason(),
                    SeasonsForRegression: seasonsForRegression(),
                    State: 'Started'
                });
        };
        this.stop = function () {
            $.connection.forecastingDataHub.server.unregister({
                Token: self.token()
            });
        };
        return this;
    };
    
    var ErrorViewModel = function () {
        this.parameters = ParametersViewModel();
        this.meanAbsoluteError = ko.observable(0);
        this.percentError = ko.observable(0);
        this.confidenceHigh = ko.observable(0);
        this.confidenceLow = ko.observable(0);
        this.entries = ko.observableArray([{ DataEntry: { Period: 0, Value: 0 }, ForecastValue: 0 },
            { DataEntry: { Period: 0, Value: 0 }, ForecastValue: 0 },
            { DataEntry: { Period: 0, Value: 0 }, ForecastValue: 0 }]);
        this.status = ko.observable('Off');
        this.statusClass = ko.computed(function() {
            return this.status() === 'Off' ? "green" : "red";
        }, this);
    };

    $(document).bind('deck.change', function(event, from, to) {
        errorVMInstance.meanAbsoluteError(0);
        errorVMInstance.percentError(0);
        errorVMInstance.confidenceHigh(0);
        errorVMInstance.confidenceLow(0);
        errorVMInstance.entries([{ DataEntry: { Period: 0, Value: 0 }, ForecastValue: 0 },
            { DataEntry: { Period: 0, Value: 0 }, ForecastValue: 0 },
            { DataEntry: { Period: 0, Value: 0 }, ForecastValue: 0 }]);
        errorVMInstance.status('Off');
    });

    var init = function() {
        //gridster initialization
        $(".gridster > ul").gridster({
            widget_margins: [5, 5],
            widget_base_dimensions: [150, 150],
            min_cols: 8,
            width: 900
        });

        // Deck initialization
        $.deck('.slide');

        //model init
        errorVMInstance = new ErrorViewModel();
        ko.applyBindings(errorVMInstance);

        setupGraph('.graphContainer');

        $.connection.hub.url = "http://localhost:8080/signalr";
        $.connection.hub.start();
    };

    var setupHub = function(hubName, sendValueCallback) {
        // Add a client-side hub method that the server will call
        $.connection[hubName].client.sendValue = sendValueCallback;
    };

    var setupGraph = function(container) {
        var trueData = [], forecastData = [];
        var n = 15, isInit, x, y, trueLine, forecastLine, svg, truePath, forecastPath, xAxis;
        var period = 0;
        var margin = { top: 30, right: 30, bottom: 20, left: 80 },
            width = 600 - margin.left - margin.right,
            height = 400 - margin.top - margin.bottom;
        var startX = 0;
        var endX = n - 1;
        var startXRange = 0;
        var endXRange = width;

        var initGraph = function() {
            x = d3.scale.linear()
                .domain([startX, endX])
                .range([startXRange, endXRange]);

            y = d3.scale.linear()
                .domain([33500, 40000])
                .range([height, 0]);

            trueLine = d3.svg.line()
                .x(function(d, i) { return x(d[0] - 1); })
                .y(function(d, i) { return y(d[1]); });

            forecastLine = d3.svg.line()
                .x(function(d, i) { return x(d[0] - 1); })
                .y(function(d, i) { return y(d[1]); });

            svg = d3.select(".deck-current " + container).append("svg")
                .attr("id", "forecastGraph")
                .attr("width", width + margin.left + margin.right)
                .attr("height", height + margin.top + margin.bottom + 20)
                .append("g")
                .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

            svg.append("defs").append("clipPath")
                .attr("id", "clip")
                .append("rect")
                .attr("width", width)
                .attr("height", height);

            xAxis = d3.svg.axis().scale(x).orient("bottom");

            svg.append("g")
                .attr("class", "x axis")
                .attr("transform", "translate(0," + height + ")")
                .call(xAxis);

            svg.append("g")
                .attr("class", "y axis")
                .call(d3.svg.axis().scale(y).orient("left"));

            truePath = svg.append("g")
                .attr("clip-path", "url(#clip)")
                .append("path")
                .data([trueData])
                .attr("class", "line")
                .attr("class", "trueLine")
                .attr("d", trueLine);

            forecastPath = svg.append("g")
                .attr("clip-path", "url(#clip)")
                .append("path")
                .data([forecastData])
                .attr("class", "line")
                .attr("class", "forecastLine")
                .attr("d", forecastLine);
        };

        var updateGraph = function() {

            if (period > n) {
                startX += 1;
                endX += 1;
                trueData.shift();
                forecastData.shift();

                x = d3.scale.linear().domain([startX, endX]).range([startXRange, endXRange]);
                xAxis = d3.svg.axis().scale(x).orient("bottom");
                svg.select(".x.axis").transition().duration(500).ease("linear").call(xAxis);
            }

            truePath
                .attr("d", trueLine)
                .attr("transform", null);

            forecastPath
                .attr("d", forecastLine)
                .attr("transform", null);
        };

        // Add a client-side hub method that the server will call
        var sendValue = function (value) {
            console.log(value);
            period++;

            value.DataEntry.Value = value.DataEntry.Value.toFixed(0);
            value.ForecastValue = value.ForecastValue.toFixed(0);

            //update model
            errorVMInstance.entries.push(value);

            //cause it to shift if over 3
            if (errorVMInstance.entries().length > 3)
                errorVMInstance.entries.shift();

            errorVMInstance.confidenceLow(value.ConfidenceIntervalLow.toFixed(0));
            errorVMInstance.confidenceHigh(value.ConfidenceIntervalHigh.toFixed(0));
            errorVMInstance.meanAbsoluteError(value.ModelMeanAbsoluteError.toFixed(0));
            errorVMInstance.percentError((value.ModelPercentError * 100).toFixed(2));

            if (!isInit) {
                isInit = true;
                initGraph();
            }

            trueData.push([period, value.DataEntry.Value]);
            forecastData.push([period, value.ForecastValue]);
            updateGraph();
        };
        
        setupHub('forecastingDataHub', sendValue);
        
        $(document).bind('deck.change', function (event, from, to) {
            d3.selectAll("#forecastGraph").remove();
            isInit = false; 
        });
    };

    init();
});