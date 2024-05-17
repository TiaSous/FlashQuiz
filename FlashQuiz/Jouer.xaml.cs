using FlashQuiz.ViewModels;

namespace FlashQuiz;

public partial class Jouer : ContentPage
{
    public Jouer()
    {
        InitializeComponent();

        var vm = BindingContext as JouerViewModel;
        vm.RotateCardUIAction = RotateUI;
    }

    private void RotateUI(int angle)
    {
        this.CardValue.RotateYTo(angle);
        this.CardValueOutside.RotateYTo(angle);

    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        var vm = BindingContext as JouerViewModel;
        vm.Accelerometer_Desactivate();
    }
}