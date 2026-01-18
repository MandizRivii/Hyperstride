using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HyperStrideWPF.Game
{
    public class MovingEnemy
    {
        public Rectangle? Rect { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double VelocityY { get; set; }
        public const double Width = 40;
        public const double Height = 40;
        public const double Speed = 2;
        public const double MinY = 50;
        public const double MaxY = 340;
    }

    public class TrapEnemy
    {
        public Rectangle? Rect { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double VelocityY { get; set; }
        public const double Width = 40;
        public const double Height = 40;
        public const double Speed = 2;
        public const double MinY = 50;
        public const double MaxY = 340;
    }

    public class GameManager
    {
        private readonly Canvas canvas;
        private readonly DispatcherTimer timer;

        private Rectangle? player;
        private Rectangle? attackRect;

        private double playerY = 300;
        private double gravity = 5;
        private double playerSpeed = 5;

        private const int obstacleCount = 15;
        private Rectangle?[] obstacles = new Rectangle?[obstacleCount];
        private double[] obstacleX = new double[obstacleCount];

        private List<MovingEnemy> movingEnemies = new List<MovingEnemy>();

        private List<TrapEnemy> trapEnemies = new List<TrapEnemy>();

        private double baseSpeed = 7;
        private double difficultyMultiplier = 1.0;
        private double groundX;

        private bool canAttack = true;
        private bool isGameOver;
        private int score;
        private long elapsedMilliseconds;
        private TextBlock? scoreText;

        private Random rnd = new Random();

        public event Action<int>? GameOver;

        public GameManager(Canvas canvas)
        {
            this.canvas = canvas;

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            timer.Tick += GameLoop;
        }

        public void Start()
        {
            ClearGame();

            score = 0;
            elapsedMilliseconds = 0;
            groundX = 0;
            playerY = 200;
            canAttack = true;
            isGameOver = false;

            player = new Rectangle { Width = 40, Height = 40, Fill = Brushes.Red };
            Canvas.SetLeft(player, 50);
            Canvas.SetTop(player, playerY);
            canvas.Children.Add(player);

            attackRect = new Rectangle
            {
                Width = 60,
                Height = 80,
                Fill = Brushes.Yellow,
                Opacity = 0.5,
                Visibility = Visibility.Hidden
            };
            canvas.Children.Add(attackRect);

            // Add gray bars on top and bottom (cave effect)
            var topBand = new Rectangle { Width = 800, Height = 50, Fill = Brushes.Gray };
            Canvas.SetLeft(topBand, 0);
            Canvas.SetTop(topBand, 0);
            canvas.Children.Add(topBand);

            var bottomBand = new Rectangle { Width = 800, Height = 80, Fill = Brushes.Gray };
            Canvas.SetLeft(bottomBand, 0);
            Canvas.SetTop(bottomBand, 450 - 70);
            canvas.Children.Add(bottomBand);

            for (int i = 0; i < obstacleCount; i++)
                SpawnObstacle(i);

            // Spawn moving enemies
            for (int i = 0; i < 2; i++)
                SpawnMovingEnemy(800 + i * 500);

            // Spawn trap enemies
            for (int i = 0; i < 2; i++)
                SpawnTrapEnemy(1000 + i * 600);

            scoreText = new TextBlock
            {
                Text = "Score: 0",
                FontSize = 20,
                Foreground = Brushes.White
            };
            Canvas.SetLeft(scoreText, 10);
            Canvas.SetTop(scoreText, 10);
            canvas.Children.Add(scoreText);

            timer.Start();
        }

        public void SetDifficulty(double multiplier)
        {
            difficultyMultiplier = multiplier;
        }

        private void ClearGame()
        {
            timer.Stop();
            canvas.Children.Clear();
            movingEnemies.Clear();
            trapEnemies.Clear();
        }


        private void SpawnObstacle(int i)
        {
            obstacles[i] = new Rectangle { Width = 40, Height = 40, Fill = Brushes.Gray };
            obstacleX[i] = 800 + i * 250;
            
            // Find a Y position that doesn't collide with other obstacles or walls
            double spawnY;
            bool validPosition = false;
            
            do
            {
                spawnY = rnd.Next(50, 380);
                validPosition = true;
                
                Rect newObstacleRect = new Rect(obstacleX[i], spawnY, 40, 40);
                
                for (int j = 0; j < obstacleCount; j++)
                {
                    if (obstacles[j] == null) continue;
                    
                    Rect existingObstacleRect = new Rect(
                        obstacleX[j],
                        Canvas.GetTop(obstacles[j]!),
                        40,
                        40);
                    
                    if (newObstacleRect.IntersectsWith(existingObstacleRect))
                    {
                        validPosition = false;
                        break;
                    }
                }
            } while (!validPosition);
            
            Canvas.SetLeft(obstacles[i]!, obstacleX[i]);
            Canvas.SetTop(obstacles[i]!, spawnY);
            canvas.Children.Add(obstacles[i]!);
        }

        private void SpawnMovingEnemy(double startX)
        {
            var enemy = new MovingEnemy
            {
                Rect = new Rectangle { Width = 40, Height = 40, Fill = Brushes.Orange },
                X = startX,
                Y = rnd.Next(50, 320),
                VelocityY = MovingEnemy.Speed
            };

            canvas.Children.Add(enemy.Rect);
            Canvas.SetLeft(enemy.Rect, enemy.X);
            Canvas.SetTop(enemy.Rect, enemy.Y);

            movingEnemies.Add(enemy);
        }

        private void SpawnTrapEnemy(double startX)
        {
            var enemy = new TrapEnemy
            {
                Rect = new Rectangle { Width = 40, Height = 40, Fill = Brushes.SkyBlue },
                X = startX,
                Y = rnd.Next(50, 320),
                VelocityY = TrapEnemy.Speed
            };

            canvas.Children.Add(enemy.Rect);
            Canvas.SetLeft(enemy.Rect, enemy.X);
            Canvas.SetTop(enemy.Rect, enemy.Y);

            trapEnemies.Add(enemy);
        }

private void GameLoop(object? sender, EventArgs e)
{
    if (player == null) return;

    // Track elapsed time
    elapsedMilliseconds += 16;
    score = (int)(elapsedMilliseconds / 1000);
    scoreText!.Text = $"Score: {score}s";

    // RUCH GRACZA
    playerY += Keyboard.IsKeyDown(Key.Space) ? -playerSpeed : gravity;
    playerY = Math.Clamp(playerY, 50, 360);
    Canvas.SetTop(player, playerY);

    Rect playerRect = new Rect(50, playerY, 40, 40);

    // Check collision with cave bars
    Rect topBarRect = new Rect(0, 0, 800, 50);
    Rect bottomBarRect = new Rect(0, 380, 800, 70);
    
    if (playerRect.IntersectsWith(topBarRect) || playerRect.IntersectsWith(bottomBarRect))
    {
        EndGame();
        return;
    }

    bool attackActive = attackRect != null &&
                        attackRect.Visibility == Visibility.Visible;

    Rect attackRectBox = Rect.Empty;
    if (attackActive)
    {
        attackRectBox = new Rect(
            Canvas.GetLeft(attackRect!),
            Canvas.GetTop(attackRect!),
            attackRect!.Width,
            attackRect!.Height);
    }

    for (int i = 0; i < obstacleCount; i++)
    {
        if (obstacles[i] == null) continue;

        obstacleX[i] -= baseSpeed * difficultyMultiplier;
        Canvas.SetLeft(obstacles[i]!, obstacleX[i]);

        Rect obstacleRect = new Rect(
            obstacleX[i],
            Canvas.GetTop(obstacles[i]!),
            40,
            40);

        // ATAK → ZNISZCZENIE PRZESZKODY
        if (attackActive && attackRectBox.IntersectsWith(obstacleRect))
        {
            canvas.Children.Remove(obstacles[i]!);
            obstacles[i] = null;

            // Respawn new obstacle
            SpawnObstacle(i);
            continue;
        }

        // GRACZ → ŚMIERĆ
        if (playerRect.IntersectsWith(obstacleRect))
        {
            EndGame();
            return;
        }

        // USUNIĘCIE GDY WYJDZIE ZA EKRAN
        if (obstacleX[i] < -40)
        {
            canvas.Children.Remove(obstacles[i]!);
            obstacles[i] = null;
            
            // Respawn new obstacle
            SpawnObstacle(i);
        }
    }

    // Update moving enemies
    for (int i = movingEnemies.Count - 1; i >= 0; i--)
    {
        var enemy = movingEnemies[i];
        if (enemy.Rect == null) continue;

        // Move horizontally
        enemy.X -= baseSpeed * difficultyMultiplier;
        Canvas.SetLeft(enemy.Rect, enemy.X);

        // Move vertically (up and down)
        enemy.Y += enemy.VelocityY;
        if (enemy.Y <= MovingEnemy.MinY || enemy.Y >= MovingEnemy.MaxY)
            enemy.VelocityY *= -1;
        Canvas.SetTop(enemy.Rect, enemy.Y);

        Rect enemyRect = new Rect(enemy.X, enemy.Y, MovingEnemy.Width, MovingEnemy.Height);

        // ATAK → ZNISZCZENIE PRZESZKODY
        if (attackActive && attackRectBox.IntersectsWith(enemyRect))
        {
            canvas.Children.Remove(enemy.Rect);
            movingEnemies.RemoveAt(i);
            SpawnMovingEnemy(800);
            continue;
        }

        // GRACZ → ŚMIERĆ
        if (playerRect.IntersectsWith(enemyRect))
        {
            EndGame();
            return;
        }

        // USUNIĘCIE GDY WYJDZIE ZA EKRAN
        if (enemy.X < -40)
        {
            canvas.Children.Remove(enemy.Rect);
            movingEnemies.RemoveAt(i);
            SpawnMovingEnemy(800);
        }
    }

    // Update trap enemies
    for (int i = trapEnemies.Count - 1; i >= 0; i--)
    {
        var enemy = trapEnemies[i];
        if (enemy.Rect == null) continue;

        // Move horizontally
        enemy.X -= baseSpeed * difficultyMultiplier;
        Canvas.SetLeft(enemy.Rect, enemy.X);

        // Move vertically (up and down)
        enemy.Y += enemy.VelocityY;
        if (enemy.Y <= TrapEnemy.MinY || enemy.Y >= TrapEnemy.MaxY)
            enemy.VelocityY *= -1;
        Canvas.SetTop(enemy.Rect, enemy.Y);

        Rect enemyRect = new Rect(enemy.X, enemy.Y, TrapEnemy.Width, TrapEnemy.Height);

        // ATAK → PLAYER DIES (hitting trap enemy with attack kills you)
        if (attackActive && attackRectBox.IntersectsWith(enemyRect))
        {
            EndGame();
            return;
        }

        // Player can fly through without dying

        // USUNIĘCIE GDY WYJDZIE ZA EKRAN
        if (enemy.X < -40)
        {
            canvas.Children.Remove(enemy.Rect);
            trapEnemies.RemoveAt(i);
            SpawnTrapEnemy(800);
        }
    }
}



        public async void HandleKeyDown(KeyEventArgs e)
        {
            if (e.Key != Key.D || !canAttack || attackRect == null || isGameOver) return;

            canAttack = false;
            Canvas.SetLeft(attackRect, 90);
            Canvas.SetTop(attackRect, playerY - 20);
            attackRect.Visibility = Visibility.Visible;

            await Task.Delay(250);
            attackRect.Visibility = Visibility.Hidden;

            await Task.Delay(2000);
            canAttack = true;
        }

        private void EndGame()
        {
            if (isGameOver) return;
            isGameOver = true;

            timer.Stop();
            GameOver?.Invoke(score);
        }
    }
}
