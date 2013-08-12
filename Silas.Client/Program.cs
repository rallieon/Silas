﻿using System;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace Silas.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Initialize();
        }

        private static void Initialize()
        {
            var hubConnection = new HubConnection("http://localhost:8080/");
            hubConnection.TraceLevel = TraceLevels.All;
            hubConnection.TraceWriter = Console.Out;
            IHubProxy proxy = hubConnection.CreateHubProxy("MyHub");
            hubConnection.Start().Wait();
            proxy.Invoke<string>("Send", "Keith", "Hello1!").Wait();
            proxy.Invoke<string>("Send", "Keith", "Hello2!").Wait();
            proxy.Invoke<string>("Send", "Keith", "Hello3!").Wait();
        }
    }
}