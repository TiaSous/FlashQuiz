using FlashQuiz.Models;
using System.Diagnostics;

namespace FlashQuiz;

public partial class Resultat : ContentPage
{
	private List<Card> cardsNotKnow = new List<Card>();

    public Resultat(List<Card> cardsNotKnowImport, int pourcentageConnu, int timer, int cardsKnow)
	{
		InitializeComponent();
		if(cardsNotKnowImport != null )
		{
            foreach (Card card in cardsNotKnowImport)
            {
                cardsNotKnow.Add(card);
            }
        }
        Connu.Text = "Connu : " + cardsKnow.ToString();
        Erreur.Text = "Erreur : " + cardsNotKnow.Count.ToString();
        Timer.Text = "Temps prix : " + timer.ToString() + " secondes";
        PourcentageText.Text = "Mémorisation des cartes : " +pourcentageConnu.ToString() + "%";
        cardsCollectionView.ItemsSource = cardsNotKnow;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopToRootAsync();
    }
}