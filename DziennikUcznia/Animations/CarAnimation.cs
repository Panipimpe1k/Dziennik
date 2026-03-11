using Microsoft.Maui.Controls.Shapes;

namespace DziennikUcznia.Animations
{
    public class CarAnimation
    {
        private BoxView carBody;
        private Ellipse wheelLeft;
        private Ellipse wheelRight;
        private Label numberLabel;

        private double carWidth = 180;
        private double screenWidth = 2000;

        public CarAnimation(BoxView carBody, Ellipse wheelLeft, Ellipse wheelRight, Label numberLabel)
        {
            this.carBody = carBody;
            this.wheelLeft = wheelLeft;
            this.wheelRight = wheelRight;
            this.numberLabel = numberLabel;
        }

        public async Task PlayAsync(int luckyNumber)
        {
            double startX = -carWidth;
            double centerX = (screenWidth - carWidth) / 2;
            double endX = screenWidth + carWidth;

            // Reset
            carBody.TranslationX = startX;
            wheelLeft.TranslationX = startX;
            wheelRight.TranslationX = startX;
            numberLabel.IsVisible = false;

            // Pączątkowy wyjazd autobusu(samobus)
            await Task.WhenAll(
                carBody.TranslateTo(centerX, 0, 1500, Easing.CubicInOut),
                wheelLeft.TranslateTo(centerX, 0, 1500, Easing.CubicInOut),
                wheelRight.TranslateTo(centerX, 0, 1500, Easing.CubicInOut)
            );

            // Pauza niby na środku
            await Task.Delay(500);
            numberLabel.IsVisible = true;

            // Pojawienie się numerka który tego nie robi
            numberLabel.Text = luckyNumber.ToString();
            numberLabel.IsVisible = true;

            // Miganie niewidocznego numerka
            for (int i = 0; i < 3; i++)
            {
                await numberLabel.FadeTo(0, 200);
                await numberLabel.FadeTo(1, 200);
            }

            // Samobus odjeżdża w dal
            await Task.WhenAll(
                carBody.TranslateTo(endX, 0, 1200, Easing.CubicInOut),
                wheelLeft.TranslateTo(endX, 0, 1200, Easing.CubicInOut),
                wheelRight.TranslateTo(endX, 0, 1200, Easing.CubicInOut)
            );

            // Ukrywamy numerek juz ukryty
            numberLabel.IsVisible = false;

            // Reset drugi
            carBody.TranslationX = startX;
            wheelLeft.TranslationX = startX;
            wheelRight.TranslationX = startX;
        }
    }
}