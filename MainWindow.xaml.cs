using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HyperStrideWPF
{
    public partial class MainWindow : Window
    {
        private Rectangle player;
        private Rectangle attackRect;
        private DispatcherTimer gameTimer;
        private double playerY;
        private double playerSpeed = 7;
        private double gravity = 5;

        private double groundX = 0;
        private Rectangle ground1;
        private Rectangle ground2;

        private int score = 0;
        private TextBlock scoreText;

        private Random rnd = new Random();
        private const int obstacleCount = 12;
        private Rectangle[] obstacles = new Rectangle[obstacleCount];
        private double[] obstacleX = new double[obstacleCount];
        private double baseObstacleSpeed = 7;
        private double obstacleSpeed;

        private bool canAttack = true;

        public MainWindow()
        {
            InitializeComponent();
            obstacleSpeed = baseObstacleSpeed;

            player = new Rectangle { Width = 40, Height = 40, Fill = Brushes.Red };
            Canvas.SetLeft(player, 50);
            playerY = 300;
            Canvas.SetTop(player, playerY);
            GameCanvas.Children.Add(player);

            attackRect = new Rectangle { Width = 50, Height = 80, Fill = Brushes.Yellow, Visibility = Visibility.Hidden, Opacity = 0.5 };
            GameCanvas.Children.Add(attackRect);

            ground1 = new Rectangle { Width = 800, Height = 100, Fill = Brushes.Green };
            Canvas.SetLeft(ground1, 0);
            Canvas.SetTop(ground1, 350);
            GameCanvas.Children.Add(ground1);

            ground2 = new Rectangle { Width = 800, Height = 100, Fill = Brushes.Green };
            Canvas.SetLeft(ground2, 800);
            Canvas.SetTop(ground2, 350);
            GameCanvas.Children.Add(ground2);

            for (int i = 0; i < obstacleCount; i++)
            {
                obstacles[i] = new Rectangle { Width = 40, Height = 40, Fill = Brushes.Black };
                obstacleX[i] = 800 + i * 100;
                Canvas.SetLeft(obstacles[i], obstacleX[i]);
                Canvas.SetTop(obstacles[i], rnd.Next(0, 310));
                GameCanvas.Children.Add(obstacles[i]);
            }

            scoreText = new TextBlock { Text = "Score: 0", FontSize = 20, Foreground = Brushes.White };
            Canvas.SetLeft(scoreText, 10);
            Canvas.SetTop(scoreText, 10);
            GameCanvas.Children.Add(scoreText);

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            obstacleSpeed = (playerY <= 0 || playerY >= 310) ? baseObstacleSpeed / 2 : baseObstacleSpeed;

            playerY += Keyboard.IsKeyDown(Key.Space) ? -playerSpeed : gravity;
            playerY = Math.Clamp(playerY, 0, 310);
            Canvas.SetTop(player, playerY);

            groundX -= obstacleSpeed;
            if (groundX <= -800) groundX = 0;
            Canvas.SetLeft(ground1, groundX);
            Canvas.SetLeft(ground2, groundX + 800);

            for (int i = 0; i < obstacleCount; i++)
            {
                if (obstacles[i] == null) continue;

                obstacleX[i] -= obstacleSpeed;
                if (obstacleX[i] < -40)
                {
                    obstacleX[i] = 800 + rnd.Next(0, 200);
                    Canvas.SetTop(obstacles[i], rnd.Next(0, 310));
                    score++;
                    scoreText.Text = "Score: " + score;
                }
                Canvas.SetLeft(obstacles[i], obstacleX[i]);

                if (new Rect(Canvas.GetLeft(player), playerY, player.Width, player.Height)
                    .IntersectsWith(new Rect(obstacleX[i], Canvas.GetTop(obstacles[i]), obstacles[i].Width, obstacles[i].Height)))
                {
                    gameTimer.Stop();
                    MessageBox.Show("Game Over! Score: " + score);
                    Application.Current.Shutdown();
                }
            }
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D && canAttack)
            {
                canAttack = false;

                Canvas.SetLeft(attackRect, Canvas.GetLeft(player) + player.Width);
                Canvas.SetTop(attackRect, playerY - 20);
                attackRect.Visibility = Visibility.Visible;

                for (int i = 0; i < obstacles.Length; i++)
                {
                    if (obstacles[i] == null) continue;
                    if (new Rect(Canvas.GetLeft(attackRect), Canvas.GetTop(attackRect), attackRect.Width, attackRect.Height)
                        .IntersectsWith(new Rect(obstacleX[i], Canvas.GetTop(obstacles[i]), obstacles[i].Width, obstacles[i].Height)))
                    {
                        GameCanvas.Children.Remove(obstacles[i]);
                        obstacles[i] = null;
                    }
                }

                await Task.Delay(250);
                attackRect.Visibility = Visibility.Hidden;

                await Task.Delay(2750);
                canAttack = true;
            }
        }
    }
}
