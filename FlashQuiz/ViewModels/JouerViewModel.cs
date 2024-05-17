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
    public partial class JouerViewModel : ObservableObject
    {
        [ObservableProperty]
        private string wordShow;

        [ObservableProperty]
        private int validate = 0;

        [ObservableProperty]
        private int notValidate = 0;

        private List<Card> cards = new List<Card>();

        private int actualCard;

        private bool isTerme;

        private int angle = 0;


        public Action<int>? RotateCardUIAction { set; private get; }

        public JouerViewModel() 
        {
            try
            {
                RefreshCards();
                actualCard = 0;
                WordShow = cards[actualCard].Terme;
                isTerme = true;
                Accelerometer.Default.ShakeDetected += Accelerometer_ShakeDetected;
                Accelerometer.Default.Start(SensorSpeed.Default);
            }
            catch (Exception ex)
            { 
                Debug.WriteLine(ex.InnerException);
            }
        }

        public void Accelerometer_Desactivate()
        {
            if (Accelerometer.Default.IsMonitoring)
            {
                Accelerometer.Default.Stop();
                Accelerometer.Default.ShakeDetected -= Accelerometer_ShakeDetected;
            }
        }
        private void RefreshCards(FlashQuizzContext? context = null)
        {
            cards.Clear();
            using(var dbContext = context?? new FlashQuizzContext())
            {
                foreach(Card card in dbContext.Cards)
                {
                    cards.Add(card);
                }
            }
        }

        [RelayCommand]
        private void ChangeSideCard()
        {
            angle = angle == 0 ? 180 : 0;
            RotationCard(angle);
            WordShow = isTerme == true ? cards[actualCard].Definition : cards[actualCard].Terme;
            isTerme = isTerme == true ? false : true;
        }

        [RelayCommand]
        private void ValidateCard()
        {
            Validate += 1;
            actualCard += 1;
            WordShow = cards[actualCard].Terme;
            isTerme = true;
        }

        private void RotationCard(int angle)
        {
            if (RotateCardUIAction != null)
            {
                RotateCardUIAction.Invoke(angle);
            }
        }

        private void Accelerometer_ShakeDetected(object?sender, EventArgs e)
        {
            NotValidate += 1;
            actualCard += 1;
            WordShow = cards[actualCard].Terme;
            isTerme = true;
        }
    }
}
