using Microsoft.Extensions.DependencyInjection;
using MyApp.ViewModels;
using Xamarin.Forms;

namespace MyApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = ((App) Application.Current).ServiceProvider.GetService<MainPageViewModel>();
        }
    }
}
