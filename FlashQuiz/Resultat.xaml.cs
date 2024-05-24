using FlashQuiz.Models;
using System.Diagnostics;

namespace FlashQuiz;

public partial class Resultat : ContentPage
{
	private List<Card> cardsNotKnow = new List<Card>();

    private int pourcentageConnu;
    public Resultat(List<Card> cardsNotKnowImport, int pourcentageConnu)
	{
		InitializeComponent();
		if(cardsNotKnowImport != null )
		{
            foreach (Card card in cardsNotKnowImport)
            {
                cardsNotKnow.Add(card);
            }
        }

        PourcentageText.Text = "Mémorisation des cartes : " +pourcentageConnu.ToString() + "%";
        cardsCollectionView.ItemsSource = cardsNotKnow;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopToRootAsync();
    }
}