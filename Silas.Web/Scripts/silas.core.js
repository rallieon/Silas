$(function() {
  var trueData = [],
    forecastData = [];
  
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

    $.connection.hub.start().done(function() {
      setupGraph('naieve', '.naieveGraphContainer');
    });
  };

  var setupHub=function(name,sendValueCallback) {
    //setup the hub objects
    var hubName=name+'DataHub';

    // Add a client-side hub method that the server will call
    $.connection.naieveDataHub.client.sendValue=sendValueCallback;

    $("."+name+"TickerStart").click(function() {
      debugger;
      //clear data before starting
      trueData = [];
      forecastData = [];
      $.connection[hubName].server.start();
    });
    $("."+name+"TickerStop").click(function() {
      $.connection[hubName].server.stop();
    });
  };

  var setupGraph=function(name,container) {
    debugger;
    //setup the d3 charting
    var n = 40;

    var margin={ top: 10,right: 10,bottom: 20,left: 40 },
        width=960-margin.left-margin.right,
        height=500-margin.top-margin.bottom;

    var x=d3.scale.linear()
        .domain([0,n-1])
        .range([0,width]);

    var y=d3.scale.linear()
        .domain([0,20000])
        .range([height,0]);

    var trueLine=d3.svg.line()
        .x(function(d,i) { return x(i); })
        .y(function(d,i) { return y(d); });
    var forecastLine=d3.svg.line()
        .x(function(d,i) { return x(i); })
        .y(function(d,i) { return y(d); });

    var svg=d3.select(container).append("svg")
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

    var truePath=svg.append("g")
        .attr("clip-path","url(#clip)")
        .append("path")
        .data([trueData])
        .attr("class","line")
        .attr("class","trueLine")
        .attr("d",trueLine);

    var forecastPath=svg.append("g")
        .attr("clip-path","url(#clip)")
        .append("path")
        .data([forecastData])
        .attr("class","line")
        .attr("class", name + "Line")
        .attr("d",forecastLine);

    // Add a client-side hub method that the server will call
    var sendValue=function(value) {
      debugger;
      console.log(name+" true value = "+value.DataEntry.Value);
      console.log(name+" forecast value = "+value.ForecastValue);
/*
      //draw true line
      trueData.push(value.DataEntry.Value);

      // redraw the line, and slide it to the left
      truePath
        .attr("d",trueLine)
        .attr("transform",null)
        .transition()
        .duration(500)
        .ease("linear");

      if(trueData.length>n)
        truePath.attr("transform","translate("+x(-1)+")");

      // pop the old data point off the front
      //if we are past our number of data points
      if(trueData.length>n)
        trueData.shift();

      //draw forecast line
      forecastData.push(value.ForecastValue);

      // redraw the line, and slide it to the left
      forecastPath
        .attr("d",forecastLine)
        .attr("transform",null)
        .transition()
        .duration(500)
        .ease("linear");

      if(forecastData.length>n)
        forecastPath.attr("transform","translate("+x(-1)+")");

      // pop the old data point off the front
      //if we are past our number of data points
      if(forecastData.length>n)
        forecastData.shift();*/
    };

    setupHub(name, sendValue);
  };

  init();
});
