namespace FlashQuiz
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void ModifyClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Modifier());
        }
    }

}
