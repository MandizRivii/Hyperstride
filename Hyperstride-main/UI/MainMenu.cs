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
                Background = Brushes.Black
            };

            var titleText = new TextBlock
            {
                Text = "HYPERSTRIDE",
                FontSize = 48,
                FontWeight = System.Windows.FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, -250, 0, 0)
            };
            Root.Children.Add(titleText);

            var startButton = new Button
            {
                Content = "Start",
                Width = 120,
                Height = 50,
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, -120, 0, 0)
            };

            startButton.Click += (_, _) => StartClicked?.Invoke();
            Root.Children.Add(startButton);

            var difficultyButton = new Button
            {
                Content = "Poziom: Normal",
                Width = 150,
                Height = 50,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0)
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
                difficultyButton.Content = $"Poziom: {CurrentDifficulty}";
            };

            Root.Children.Add(difficultyButton);

            var infoButton = new Button
            {
                Content = "Info",
                Width = 100,
                Height = 50,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 120, 0, 0)
            };

            infoButton.Click += (_, _) => ShowInfoDialog();
            Root.Children.Add(infoButton);
        }

        public void Show() => Root.Visibility = Visibility.Visible;
        public void Hide() => Root.Visibility = Visibility.Collapsed;

        private void ShowInfoDialog()
        {
            var infoWindow = new Window
            {
                Title = "Game Info",
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = Brushes.DarkGray
            };

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(20),
                VerticalAlignment = VerticalAlignment.Top
            };

            var title = new TextBlock
            {
                Text = "Instrukcje Gry",
                FontSize = 20,
                FontWeight = System.Windows.FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 15)
            };
            stackPanel.Children.Add(title);

            var info1 = new TextBlock
            {
                Text = "• SPACJA - Wzlatuj (skok)",
                FontSize = 14,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 5, 0, 5),
                TextWrapping = TextWrapping.Wrap
            };
            stackPanel.Children.Add(info1);

            var info2 = new TextBlock
            {
                Text = "• D - Atakuj",
                FontSize = 14,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 5, 0, 5),
                TextWrapping = TextWrapping.Wrap
            };
            stackPanel.Children.Add(info2);

            var info3 = new TextBlock
            {
                Text = "• Przetrwaj jak najdłużej!",
                FontSize = 14,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 5, 0, 5),
                TextWrapping = TextWrapping.Wrap
            };
            stackPanel.Children.Add(info3);

            var info4 = new TextBlock
            {
                Text = "• NIEBIESCY - ignoruj!",
                FontSize = 14,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 5, 0, 5),
                TextWrapping = TextWrapping.Wrap
            };
            stackPanel.Children.Add(info4);

            var closeButton = new Button
            {
                Content = "Zamknij",
                Width = 100,
                Height = 40,
                FontSize = 14,
                Margin = new Thickness(0, 20, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            closeButton.Click += (_, _) => infoWindow.Close();
            stackPanel.Children.Add(closeButton);

            infoWindow.Content = stackPanel;
            infoWindow.ShowDialog();
        }
    }
}
