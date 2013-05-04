Silas
=====

##Dynamic Forecasting of Website Traffic using SignalR and WebAPI

Forecasting data, whether it be website traffic or retail sales, is considered by many to be a black box system built on advanced statistical models that are incomprehensible.  With the proliferation of dynamic infrastructures through services like Amazon Web Services, the ability to predict resource utilization becomes more important than ever.  A predictive infrastructure system creates an enormous advantage by reducing wasted resources.  This talk lays the groundwork for building a dynamic forecasting system of website traffic.  The traffic results will be loaded into the system through a RESTful service built with ASP.NET WebAPI.  The system will update predictions on the fly through a custom forecasting system.  Finally, the results of the forecast will be sent to a client side charting framework utilizing SignalR.  There will be a comparison and contrast of three different forecasting models and discussions of which forecasting techniques will help the most with varying types of data.  The three forecasting models will include a dynamic running average, trending, and finally trending with seasonality.  The number one rule of forecasting is that your forecast will always be wrong; thus, an analysis of when and how to use confidence intervals will be provided.

##ToDo
1. Setup default empty database that feed will populate.
2. Setup default seeded database for comparison with true data.
3. Setup DataFeed project to import from CSV every second (runs for 16 minutes).
4. Setup Strategy pattern in Forecasting project.
5. Create Basic Strategy (Naieve for now).
6. Setup Signal R to pull from forecasted data.
7. Setup Default website to use charting system.
8. Setup Signal R to push forecasted Data.
9. Finalize the forecast techniques.