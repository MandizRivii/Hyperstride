using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HyperStrideWPF.UI
{
    public class MainMenu
    {
        public Grid Root { get; }
        public event Action? StartClicked;
        public enum Difficulty { Easy, Normal, Hard }
        public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Normal;

        public MainMenu()
        {
            Root = new Grid
            {
                Width = 800,
                Height = 450,
                Background = Brushes.DarkBlue
            };

            var startButton = new Button
            {
                Content = "Start",
                Width = 120,
                Height = 50,
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, -60, 0, 0)
            };

            startButton.Click += (_, _) => StartClicked?.Invoke();
            Root.Children.Add(startButton);

            var difficultyButton = new Button
            {
                Content = "Difficulty: Normal",
                Width = 150,
                Height = 50,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 60, 0, 0)
            };

            difficultyButton.Click += (_, _) =>
            {
                CurrentDifficulty = CurrentDifficulty switch
                {
                    Difficulty.Easy => Difficulty.Normal,
                    Difficulty.Normal => Difficulty.Hard,
                    Difficulty.Hard => Difficulty.Easy,
                    _ => Difficulty.Normal
                };
                difficultyButton.Content = $"Difficulty: {CurrentDifficulty}";
            };

            Root.Children.Add(difficultyButton);
        }

        public void Show() => Root.Visibility = Visibility.Visible;
        public void Hide() => Root.Visibility = Visibility.Collapsed;
    }
}
