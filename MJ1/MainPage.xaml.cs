using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Algorithm;
using System.Diagnostics;

namespace MJ1;


public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel();
        (BindingContext as MainPageViewModel)!.TilesOne = TilesOne;
    }
}
