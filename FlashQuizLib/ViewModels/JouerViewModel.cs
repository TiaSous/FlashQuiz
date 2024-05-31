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

        private int totalCardAtStart;                       // nombre de carte total depuis le début

        public List<Card> cards = new List<Card>();        // liste des cartes 

        public List<Card> cardsNotKnow = new List<Card>(); // Liste des cartes non connus

        private int cardsKnow = 0;                          // Nombre de cartes connus

        private List<Card> cardsNotKnowFinal = new List<Card>(); // liste des cartes non connus qui sera envoyée dans le résultat

        private int actualCard;                             // nombre de carte actuellement jouer

        private bool isTerme;                               // définit s'il affiche le terme ou la définition

        public int angle = 0;                              // quel est l'angle actuelle de la carte (0 ou 180) (il est en public pour les test)

        [ObservableProperty]
        private string nombreDeCarte;                       // text (carte actuel / nombre total carte)

        private int nombreCarteTotal;                       // nombre de carte total selon le refresh

        public Action<int>? RotateCardUIAction { set; private get; }    // action de rotation

        public IDispatcherTimer timer;                      // timer

        private int totalSeconds = 0;                       // nombre total de secondes écouler
                
        Random random = new Random();                       // random pour la carte suivante

        private int randomcard = 0;                         // permet de faire l'aléatoire lorsque on joue (séléction des cartes)

        public JouerViewModel()
        {
            try
            {
                RefreshCards();
                actualCard = 0;
                isTerme = true;
                nombreCarteTotal = cards.Count;
                randomcard = random.Next(nombreCarteTotal);
                WordShow = cards[randomcard].Terme;
                totalCardAtStart = cards.Count;
                MiseAJourNombre();
                Accelerometer.Default.ShakeDetected += Accelerometer_ShakeDetected;
                Accelerometer.Default.Start(SensorSpeed.Default);
                timer = Application.Current.Dispatcher.CreateTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += OnTimerTick;
                timer.Start();
            }
            catch (Exception ex)
            {
                NoCards();
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
        // va chercher les cartes dans la db
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
        public void ChangeSideCard()
        {
            if(angle == 0)
            {
                angle = 180;
            }
            else
            {
                angle = 0;
            }
            RotationCard(angle);
            WordShow = isTerme == true ? cards[randomcard].Definition : cards[randomcard].Terme;
            isTerme = isTerme == true ? false : true;
        }

        // pour valider la carte
        [RelayCommand]
        private async Task ValidateCard()
        {
            Validate += 1;
            cardsKnow += 1;
            await ChangeCard();
        }

        // carte non apprises
        [RelayCommand]
        private async Task NotValidateCard()
        {
            NotValidate += 1;
            cardsNotKnow.Add(cards[randomcard]);
            await ChangeCard();
        }

        // lorsqu'il secoue le téléphone met la carte dans non appris
        private async void Accelerometer_ShakeDetected(object? sender, EventArgs e)
        {
            await NotValidateCard();
        }

        // pour la rotation de la carte
        private void RotationCard(int angle)
        {
            if (RotateCardUIAction != null)
            {
                RotateCardUIAction.Invoke(angle);
            }
        }

        // met à jour le champ qui affiche le carte actuel / nombre total
        public void MiseAJourNombre()
        {
            NombreDeCarte = (actualCard + 1).ToString() + "/" + nombreCarteTotal;
        }

        // lorsqu'il change de carte
        private async Task ChangeCard()
        {
            actualCard += 1;
            cards.RemoveAt(randomcard);
            randomcard = random.Next(cards.Count);
            if(actualCard == nombreCarteTotal && cardsNotKnow.Count == 0)
            {
               cards.Clear();
               await Finish(cards);
            }
            else if (actualCard == nombreCarteTotal && cardsNotKnow.Count != 0)
            {
                RefreshGame();
            }
            else
            {
                WordShow = cards[randomcard].Terme;
                isTerme = true;
                MiseAJourNombre();
            }
            
        }

        // lorsqu'il a finit
        private async Task Finish(List<Card> cards)
        {
            double number = (double)cardsKnow / totalCardAtStart * 100;
            int pourcentageConnu = (int)Math.Round(number);
            timer.Stop();

            await Shell.Current.Navigation.PushModalAsync(new Resultat(cards, pourcentageConnu, totalSeconds, cardsKnow));
        }

        // lorsqu'il a finit la liste et qu'il ne connaît pas les cartes remet tous à zéro avec une nouvelle liste
        public void RefreshGame()
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
            isTerme = true;
            nombreCarteTotal = cards.Count;
            randomcard = random.Next(nombreCarteTotal);
            WordShow = cards[randomcard].Terme;
            MiseAJourNombre();
            NotValidate = 0;
            Validate = 0;  
        }

        // lorsqu'il décide de finir le jeu lui même
        [RelayCommand]
        private async Task StopGame()
        {
            await Finish(cardsNotKnow);
        }

        // écoule le timer
        private void OnTimerTick(object sender, EventArgs e)
        {
            totalSeconds++;
        }

        // s'il n'y a pas de carte
        private async Task NoCards()
        {
            await Shell.Current.DisplayAlert("Acune Carte", "Vous n'avez aucune carte", "Retour");
            await Shell.Current.Navigation.PopAsync();
        }
    }
}
