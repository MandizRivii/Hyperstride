using System.Windows;
using System.Windows.Input;
using HyperStrideWPF.Game;
using HyperStrideWPF.UI;

namespace HyperStrideWPF
{
    public partial class MainWindow : Window
    {
        private MainMenu menu;
        private GameManager game;

        public MainWindow()
        {
            InitializeComponent();

            menu = new MainMenu();
            MenuLayer.Children.Add(menu.Root);

            game = new GameManager(GameCanvas);

            menu.StartClicked += () =>
            {
                menu.Hide();
                
                // Set difficulty multiplier based on selection
                double multiplier = menu.CurrentDifficulty switch
                {
                    MainMenu.Difficulty.Easy => 0.8,       // 20% slower
                    MainMenu.Difficulty.Normal => 1.0,     // Normal
                    MainMenu.Difficulty.Hard => 1.2,       // 20% faster
                    _ => 1.0
                };
                
                game.SetDifficulty(multiplier);
                game.Start();
            };

            game.GameOver += score =>
            {
                MessageBox.Show($"Game Over! Score: {score}");
                menu.Show();
            };
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            game?.HandleKeyDown(e);
        }
    }
}
