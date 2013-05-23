$(function() {
  var trueData=[],
    forecastData=[],
    errorVMInstance;

  var ErrorViewModel=function() {
    this.meanAbsoluteError=ko.observable(0);
    this.percentError=ko.observable(0);
    this.confidenceHigh=ko.observable(0);
    this.confidenceLow=ko.observable(0);
    this.entries=ko.observableArray([]);
  };

  var init=function() {
    //gridster initialization
    $(".gridster > ul").gridster({
      widget_margins: [10,10],
      widget_base_dimensions: [140,140],
      min_cols: 12,
      extra_cols: 4
    });

    // Deck initialization
    $.deck('.slide');

    //model init
    errorVMInstance=new ErrorViewModel();
    ko.applyBindings(errorVMInstance);

    //setup graphs
    setupGraph('naieve','.naieveGraphContainer');

    $.connection.hub.start();
  };

  var setupHub=function(name,sendValueCallback) {
    //setup the hub objects
    var hubName=name+'DataHub';

    // Add a client-side hub method that the server will call
    $.connection[hubName].client.sendValue=sendValueCallback;

    $("."+name+"TickerStart").click(function() {
      //clear data before starting
      trueData=[];
      forecastData=[];
      $.connection[hubName].server.start();
    });
    $("."+name+"TickerStop").click(function() {
      $.connection[hubName].server.stop();
    });
  };

  var setupGraph=function(name,container) {
    var n=30,isInit,x,y,trueLine,forecastLine,svg,truePath,forecastPath,margin;

    var initGraph=function() {
      margin={ top: 10,right: 10,bottom: 20,left: 40 },
              width=740-margin.left-margin.right,
              height=450-margin.top-margin.bottom;

      x=d3.scale.linear()
          .domain([0,n-1])
          .range([0,width]);

      y=d3.scale.linear()
          .domain([800,2700])
          .range([height,0]);

      trueLine=d3.svg.line()
          .x(function(d,i) { return x(i); })
          .y(function(d,i) { return y(d); });
      forecastLine=d3.svg.line()
          .x(function(d,i) { return x(i); })
          .y(function(d,i) { return y(d); });

      svg=d3.select(container).append("svg")
          .attr("width",width+margin.left+margin.right)
          .attr("height",height+margin.top+margin.bottom)
          .append("g")
          .attr("transform","translate("+margin.left+","+margin.top+")");

      svg.append("defs").append("clipPath")
          .attr("id","clip")
          .append("rect")
          .attr("width",width)
          .attr("height",height);

      svg.append("g")
          .attr("class","x axis")
          .attr("transform","translate(0,"+height+")")
          .call(d3.svg.axis().scale(x).orient("bottom"));

      svg.append("g")
          .attr("class","y axis")
          .call(d3.svg.axis().scale(y).orient("left"));

      truePath=svg.append("g")
          .attr("clip-path","url(#clip)")
          .append("path")
          .data([trueData])
          .attr("class","line")
          .attr("class","trueLine")
          .attr("d",trueLine);

      forecastPath=svg.append("g")
          .attr("clip-path","url(#clip)")
          .append("path")
          .data([forecastData])
          .attr("class","line")
          .attr("class","forecastLine")
          .attr("d",forecastLine);
    };

    // Add a client-side hub method that the server will call
    var sendValue=function(value) {

      value.DataEntry.Value = value.DataEntry.Value.toFixed(0);
      value.ForecastValue = value.ForecastValue.toFixed(0);
      
      //update model
      errorVMInstance.entries.push(value);

      //cause it to shift if over 5
      if(errorVMInstance.entries().length>3)
        errorVMInstance.entries.shift();

      errorVMInstance.confidenceLow(value.ConfidenceIntervalLow.toFixed(0));
      errorVMInstance.confidenceHigh(value.ConfidenceIntervalHigh.toFixed(0));
      errorVMInstance.meanAbsoluteError(value.ModelMeanAbsoluteError.toFixed(0));
      errorVMInstance.percentError(value.ModelPercentError.toFixed(2));

      if(!isInit) {
        isInit=true;
        initGraph();
      }

      //update true graph line
      trueData.push(value.DataEntry.Value);

      truePath
        .attr("d",trueLine)
        .attr("transform",null)
        .transition()
        .duration(500)
        .ease("linear");

      if(trueData.length>n)
        truePath.attr("transform","translate("+x(-1)+")");

      if(trueData.length>n)
        trueData.shift();

      //update forecast graph
      forecastData.push(value.ForecastValue);

      forecastPath
        .attr("d",forecastLine)
        .attr("transform",null)
        .transition()
        .duration(500)
        .ease("linear");

      if(forecastData.length>n)
        forecastPath.attr("transform","translate("+x(-1)+")");

      if(forecastData.length>n)
        forecastData.shift();
    };

    setupHub(name,sendValue);
  };

  init();
});
