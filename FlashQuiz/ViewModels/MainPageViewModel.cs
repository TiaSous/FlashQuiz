using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using FlashQuiz.Models;
using FlashQuiz.Services;
using System.Diagnostics;
using System.Security.AccessControl;

namespace FlashQuiz.ViewModels
{
    public partial class MainPageViewModel: ObservableObject
    {
        [RelayCommand]
        private async Task ModifyClicked()
        {
            await Shell.Current.Navigation.PushAsync(new Modifier());
        }

        [RelayCommand]
        private async Task JouerClicked()
        {
            await Shell.Current.Navigation.PushAsync(new Jouer());
        }
    }
}
