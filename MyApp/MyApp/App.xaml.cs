using System;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Helpers;
using MyApp.Interfaces;
using MyApp.ViewModels;
using WebSiteRetriever.Interfaces;
using WebSiteRetriever.Services;
using Xamarin.Forms;

namespace MyApp
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; set; }

        public App()
        {
            InitializeComponent();

            ServiceProvider = new ServiceCollection()
                .AddTransient<MainPageViewModel>()
                .AddScoped<IReportBuilder, ReportBuilder>()
                .AddScoped<IWebSiteService, WebSiteService>()
                .BuildServiceProvider();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
