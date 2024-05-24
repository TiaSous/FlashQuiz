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
        private string wordShow;                            // le mot qui est actuellemnt afficher (soit terme, soit définition)

        [ObservableProperty]
        private int validate = 0;                           // nombre de mot validé 

        [ObservableProperty]
        private int notValidate = 0;                        // nombre de mot non validé

        private int totalCardAtStart;

        private List<Card> cards = new List<Card>();        // liste des cartes

        private List<Card> cardsNotKnow = new List<Card>(); // Liste des cartes non connus

        private int cardsKnow = 0;                          // Nombre de cartes connus

        private List<Card> cardsNotKnowFinal = new List<Card>(); // liste des cartes non connus qui sera envoyée dans le résultat

        private int actualCard;                             // numéro de placement de la cart actuelle

        private bool isTerme;                               // définit s'il affiche le terme ou la définition

        private int angle = 0;                              // quel est l'angle actuelle de la carte (0 ou 180)

        [ObservableProperty]
        private string nombreDeCarte;

        private int nombreCarteTotal;

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
                nombreCarteTotal = cards.Count;
                totalCardAtStart = cards.Count;
                MiseAJourNombre();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
            }
        }

        // pour désactiver l'acceleromètre
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
            using (var dbContext = context ?? new FlashQuizzContext())
            {
                foreach (Card card in dbContext.Cards)
                {
                    cards.Add(card);
                }
            }
        }

        // lorsque la carte est retournée
        [RelayCommand]
        private void ChangeSideCard()
        {
            angle = angle == 0 ? 180 : 0;
            RotationCard(angle);
            WordShow = isTerme == true ? cards[actualCard].Definition : cards[actualCard].Terme;
            isTerme = isTerme == true ? false : true;
        }

        // pour valider la carte
        [RelayCommand]
        private async Task ValidateCard()
        {
            Validate += 1;
            cardsKnow += 1;
            ChangeCard();
        }

        // lorsqu'il secoue le téléphone met la carte dans non appris
        private void Accelerometer_ShakeDetected(object? sender, EventArgs e)
        {
            NotValidate += 1;
            cardsNotKnow.Add(cards[actualCard]);
            ChangeCard();
        }

        // pour la rotation de la carte
        private void RotationCard(int angle)
        {
            if (RotateCardUIAction != null)
            {
                RotateCardUIAction.Invoke(angle);
            }
        }

        private void MiseAJourNombre()
        {
            NombreDeCarte = (actualCard + 1).ToString() + "/" + nombreCarteTotal;
        }

        private async void ChangeCard()
        {
            actualCard += 1;
            if(actualCard == nombreCarteTotal && cardsNotKnow.Count == 0)
            {
               await Finish(cards);
            }
            else if (actualCard == nombreCarteTotal && cardsNotKnow.Count != 0)
            {
                RefreshGame();
            }
            else
            {
                WordShow = cards[actualCard].Terme;
                isTerme = true;
                MiseAJourNombre();
            }
            
        }

        private async Task Finish(List<Card> cards)
        {
            double number = (double)cardsKnow / totalCardAtStart * 100;
            int pourcentageConnu = (int)Math.Round(number);
    
            await Shell.Current.Navigation.PushModalAsync(new Resultat(cards, pourcentageConnu));
        }

        private void RefreshGame()
        {
            cards.Clear();
            cardsNotKnowFinal.Clear();
            foreach(Card card in cardsNotKnow) 
            { 
                cards.Add(card);
                cardsNotKnowFinal.Add(card);
            }
            cardsNotKnow.Clear();
            actualCard = 0;
            WordShow = cards[actualCard].Terme;
            isTerme = true;
            nombreCarteTotal = cards.Count;
            MiseAJourNombre();
            NotValidate = 0;
            Validate = 0;  
        }

        [RelayCommand]
        private async Task StopGame()
        {
            await Finish(cardsNotKnow);
        }
    }
}
