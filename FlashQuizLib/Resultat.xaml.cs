using FlashQuiz.Models;
using System.Diagnostics;

namespace FlashQuiz;

public partial class Resultat : ContentPage
{
	private List<Card> cardsNotKnow = new List<Card>();

    // pour afficher les r�sultat sur la page
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
        Timer.Text = "Temps pris : " + timer.ToString() + " secondes";
        PourcentageText.Text = "M�morisation des cartes : " +pourcentageConnu.ToString() + "%";
        cardsCollectionView.ItemsSource = cardsNotKnow;
    }

    // veut retourner en arri�re
    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopToRootAsync();
    }
}