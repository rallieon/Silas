Silas
=====

##Forecasting of Website Traffic using SignalR and WebAPI

Forecasting data, whether it be website traffic or retail sales, is considered by many to be a black box system built on advanced statistical models that are incomprehensible.  With the proliferation of dynamic infrastructures through services like Amazon Web Services, the ability to predict resource utilization becomes more important than ever.  A predictive infrastructure system creates an enormous advantage by reducing wasted resources.  This talk lays the groundwork for building a forecasting system of website traffic.  The traffic results will be loaded into the system through a RESTful service built with ASP.NET WebAPI.  The system will update predictions through a custom forecasting system.  Finally, the results of the forecast will be sent to a client side charting framework utilizing SignalR.  There will be a comparison and contrast of four different forecasting models and discussions of which forecasting techniques will help the most with varying types of data.  The four forecasting models will include a moving average, single, double, and triple exponential smoothing.  The number one rule of forecasting is that your forecast will always be wrong; thus, an analysis of when and how to use confidence intervals will be provided.

##ToDo
1. Write Triple Exponential Smoothing strategy.
2. Introduce Holdout data set in the model
3. Introduce Confidence Intervals to the strategies.
4. Introduce Error parameters to strategies.
5. Setup Multiple DataSets for each strategy.
6. Write Solver Library to help minimize error by modifying parameters (http://msdn.microsoft.com/en-us/library/ff628587(v=vs.93).aspx)
7. Setup deck.js
8. Setup http://gridster.net/#usage
9. Setup a page for each strategy showing the Graph of data vs forecast model, error rates, confidence intervals (data table?)
10. Setup each Ticker to be Activated once that slide is active
11. Style Pages
12. Prepare Talk

##Talk
1. What is Forecasting?
2. What is Forecasting not?
3. How to Validate Forecast?
    1. Build Forecast model around current data first.
    2. Forecast Error Parameters (MAE, STDDEV, ETC)
    3. Holdout Data
4. Naieve Method
    1. Not good for far future predictions, always trails.
5. Moving Average
    1. Not good for far future predictions, only one or two periods.
6. Single Exponential Smoothing
    1. Not good for far future predictions, only one or two periods.
7. Double Exponential Smoothing
    1. Not good for far future predictions, only one or two periods.
8. Triple Exponential Smoothing
    1. Still doesn't work well for super far predictions, but can at least do a "season"
    2. Next steps, have the system readjust?
    3. Every season reevaluate the best parameters?
9. Where to start
    1. You need to look at your data first to determine what model you think the data will fit in.
10. Next Steps
     1. Have the system be able to adjust so you can update your predictions.
     2. Have the system determine the best parameters to use on the fly.
