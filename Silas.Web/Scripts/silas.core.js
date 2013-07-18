$(function () {
  var trueData = [],
    forecastData = [],
    errorVMInstance;

  ko.bindingHandlers.knob = {
    init: function (element, valueAccessor) {
      var value = valueAccessor()();
      $(element).knob({ width: 200, height: 140, inputColor: '#fff', fgColor: '#12b0c5', min: 0, max: 100, angleArc: 100, angleOffset: -50, readOnly: true });
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
      var value = valueAccessor()();
      $(element).val(parseFloat(value)).trigger('change');
    }
  };

  var ErrorViewModel = function () {
    this.meanAbsoluteError = ko.observable(0);
    this.percentError = ko.observable(0);
    this.confidenceHigh = ko.observable(0);
    this.confidenceLow = ko.observable(0);
    this.entries = ko.observableArray([{ DataEntry: { Period: 0, Value: 0 }, ForecastValue: 0 }, 
                                       { DataEntry: { Period: 0, Value: 0 }, ForecastValue: 0 }, 
                                       { DataEntry: { Period: 0, Value: 0 }, ForecastValue: 0 }]);
    this.status = ko.observable('Off');
    this.statusClass = ko.computed(function () {
      return this.status() === 'Off' ? "green" : "red";
    }, this);
  };

  $(document).bind('deck.change', function (event, from, to) {
    errorVMInstance = new ErrorViewModel();
  });

  var init = function () {
    //gridster initialization
    $(".gridster > ul").gridster({
      widget_margins: [10, 10],
      widget_base_dimensions: [260, 260],
      min_cols: 8,
      width: 1280
    });

    // Deck initialization
    $.deck('.slide');

    //model init
    errorVMInstance = new ErrorViewModel();
    ko.applyBindings(errorVMInstance);

    //setup graphs
    setupGraph('naieve', '.naieveGraphContainer');

    $.connection.hub.start();
  };

  var setupHub = function (name, sendValueCallback) {
    //setup the hub objects
    var hubName = name + 'DataHub';

    // Add a client-side hub method that the server will call
    $.connection[hubName].client.sendValue = sendValueCallback;

    $("." + name + "TickerStart").click(function () {
      errorVMInstance.status('On');
      $.connection[hubName].server.start();
    });
    $("." + name + "TickerStop").click(function () {
      errorVMInstance.status('Off');
      $.connection[hubName].server.stop();
    });
  };

  var setupGraph = function (name, container) {
    var n = 15, isInit, x, y, trueLine, forecastLine, svg, truePath, forecastPath, xAxis;
    var period = 0;
    var margin = { top: 30, right: 30, bottom: 20, left: 80 },
              width = 850 - margin.left - margin.right,
              height = 700 - margin.top - margin.bottom;
    var startX = 0;
    var endX = n - 1;
    var startXRange = 0;
    var endXRange = width;

    var initGraph = function () {
      x = d3.scale.linear()
          .domain([startX, endX])
          .range([startXRange, endXRange]);

      y = d3.scale.linear()
          .domain([800, 2700])
          .range([height, 0]);

      trueLine = d3.svg.line()
          .x(function (d, i) { return x(d[0] - 1); })
          .y(function (d, i) { return y(d[1]); });

      forecastLine = d3.svg.line()
          .x(function (d, i) { return x(d[0] - 1); })
          .y(function (d, i) { return y(d[1]); });

      svg = d3.select(container).append("svg")
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

    var updateGraph = function () {

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

    setupHub(name, sendValue);
  };

  init();
});
