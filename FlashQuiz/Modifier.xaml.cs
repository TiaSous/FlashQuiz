namespace FlashQuiz;

public partial class Modifier : ContentPage
{
	public Modifier()
	{
		InitializeComponent();

        var cards = new List<Card>
            {
            new Card { Terme = "avoir", Definition = "haben" },
            new Card { Terme = "l'objet", Definition = "der Gegenstand, -¨e" },
            new Card { Terme = "Salut", Definition = "hallo" },
            };

        cardsListView.ItemsSource = cards;
    }
}

public class Card
{
    public string Terme { get; set; }
    public string Definition { get; set; }
}