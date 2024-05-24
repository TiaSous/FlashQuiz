using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashQuiz.Models;
using FlashQuiz.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashQuiz.ViewModels
{
    public sealed partial class ModifierViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Card> cards = new ObservableCollection<Card>() {};

        // dernière id de carte dans la db
        private int lastIdDb = -1;

        // dernière id de carte dans l'application
        private int lastId = -1;

        // list de carte retirer
        private List<Card> cardRemoved = new List<Card>();

        public ModifierViewModel()
        {
            try
            {
                RefreshCards();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.InnerException}");
            }


        }
        
        // permet d'aller chercher les cartes dans la db 
        private void RefreshCards(FlashQuizzContext? context = null)
        {
            Cards.Clear();
            using (var dbContext = context?? new FlashQuizzContext()) 
            {
                foreach (var dbCard in dbContext.Cards) 
                {
                    Cards.Add(dbCard);
                }
                lastIdDb = Cards.Last().Id;
                lastId = lastIdDb;
            }
        }

        // lorsqu'il appuie sur le boutton +, cela va ajouter une carte dans l'application
        [RelayCommand]
        private void CreateCard()
        {
            lastId += 1;
            Cards.Add(new Card {Id=lastId});
        }

        // détruit la carte
        [RelayCommand]
        private void DeleteCard(Card card)
        {
            cardRemoved.Add(card);
            Cards.Remove(card);
        }

        // quand il clique sur save
        [RelayCommand]
        private async Task ButtonSave() 
        {
            using (var dbContext = new FlashQuizzContext())
            {
                // permet d'ajouter les cartes à la db
                foreach (var card in Cards)
                {
                    if (card.Id > lastIdDb)
                    {
                        if ((card.Definition == null || card.Definition == "") && (card.Terme == null || card.Terme == ""))
                        { }
                        else
                        {
                            await CreateCardDB(card);
                        }

                    }
                    else
                    {
                        var dbCard = dbContext.Find<Card>(card.Id);
                        dbCard.Definition = card.Definition;
                        dbCard.Terme = card.Terme;
                    }
                }

                // détruit les cartes
                foreach (var card in cardRemoved)
                {
                    await dbContext.Cards.Where(dbwish => dbwish.Id == card.Id).ExecuteDeleteAsync();
                }
                await dbContext.SaveChangesAsync();
            }
            cardRemoved.Clear();
            await Shell.Current.Navigation.PopAsync();
        }

        // ajoute les carte dans la db
        private async Task CreateCardDB(Card card)
        {
            using(var dbContext = new FlashQuizzContext())
            {
                dbContext.Add(card);
                await dbContext.SaveChangesAsync();
            }
        }



    }
}
