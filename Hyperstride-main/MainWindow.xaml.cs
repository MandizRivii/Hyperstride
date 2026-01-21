using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media; 
using HyperStrideWPF.Game;
using HyperStrideWPF.UI;

namespace HyperStrideWPF
{
    public partial class MainWindow : Window
    {
        private MainMenu menu;
        private GameManager game;
        private MediaPlayer musicPlayer = new MediaPlayer(); // dodane

        public MainWindow()
        {
            InitializeComponent();

            PlayMusic();

            menu = new MainMenu();
            MenuLayer.Children.Add(menu.Root);

            game = new GameManager(GameCanvas);

            menu.StartClicked += () =>
            {
                menu.Hide();

                double multiplier = menu.CurrentDifficulty switch
                {
                    MainMenu.Difficulty.Easy => 0.8,
                    MainMenu.Difficulty.Normal => 1.1,
                    MainMenu.Difficulty.Hard => 2,
                    _ => 1.0
                };

                game.SetDifficulty(multiplier);
                game.Start();
            };

            game.GameOver += score =>
            {
                MessageBox.Show($"Koniec! Wynik: {score}");
                menu.Show();
            };
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            game?.HandleKeyDown(e);
        }
private void PlayMusic()
{
    musicPlayer.Open(new Uri("Audio/soundtrack.wav", UriKind.Relative));
    musicPlayer.Volume = 0.3;
    musicPlayer.MediaEnded += (s, e) => musicPlayer.Position = TimeSpan.Zero;
    musicPlayer.Play();
}

    }
}
