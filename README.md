Silas
=====

##Dynamic Forecasting of Website Traffic using SignalR and WebAPI

Forecasting data, whether it be website traffic or retail sales, is considered by many to be a black box system built on advanced statistical models that are incomprehensible.  With the proliferation of dynamic infrastructures through services like Amazon Web Services, the ability to predict resource utilization becomes more important than ever.  A predictive infrastructure system creates an enormous advantage by reducing wasted resources.  This talk lays the groundwork for building a dynamic forecasting system of website traffic.  The traffic results will be loaded into the system through a RESTful service built with ASP.NET WebAPI.  The system will update predictions on the fly through a custom forecasting system.  Finally, the results of the forecast will be sent to a client side charting framework utilizing SignalR.  There will be a comparison and contrast of three different forecasting models and discussions of which forecasting techniques will help the most with varying types of data.  The three forecasting models will include a moving weighted average, single exponential smoothing, and finally double exponential smoothing.  The number one rule of forecasting is that your forecast will always be wrong; thus, an analysis of when and how to use confidence intervals will be provided.

##ToDo
1. Refactor and Clean
2. Confidence Intervals?
3. Setup feedback system to display error rate
4. Style
5. Prepare Talk
    1. Part 1 Forecasting in Excel
        1. Talk about the techniques
        2. Talk about Holdout data
    2. Part 2 Dynamic System based on live data.
        1. Go over the architecture of the system
