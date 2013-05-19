$(function() {
  //gridster initialization
  $(".gridster > ul").gridster({
    widget_margins: [10, 10],
    widget_base_dimensions: [140, 140],
    min_cols: 12,
    extra_cols: 4
  });
  
  // Deck initialization
  $.deck('.slide');
  
  //setup the hub objects
  var naieveDataHub = $.connection.naieveDataHub;
  var trueDataHub = $.connection.trueDataHub;
  
  // Add a client-side hub method that the server will call
  trueDataHub.client.sendValue = function(value) {
    console.log("true value = " + value.DataEntry.Value);
  };
  
  naieveDataHub.client.sendValue = function(value) {
    console.log("naieve value = " + value.ForecastValue);
  };

  $.connection.hub.start().done(function() {
    $(".naieveTickerStart").click(function() {
      naieveDataHub.server.start();
      trueDataHub.server.start();
    });
    $(".naieveTickerStop").click(function() {
      naieveDataHub.server.stop();
      trueDataHub.server.stop();
    });
  });

});
