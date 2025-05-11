using System.Diagnostics;

namespace T2.PR1.ThreadsTasksPart2.SergiAlbalat
{
    public class Program
    {
        public static bool gameRunning = true;
        public static int shipX = 22;
        public static int shipY = 20;
        public static List<(int x, int y)> asteroids = new List<(int x, int y)>();
        public static object lockObject = new object();
        public static int score = 0;
        public static int deaths = 0;
        public static Stopwatch stopwatch = new Stopwatch();
        public static async Task Main()
        {
            stopwatch.Start();
            Console.CursorVisible = false;
            Console.SetWindowSize(45, 20);
            Task.Run(UpdateAsteroids);
            Task.Run(ReadUserInput);
            while (gameRunning)
            {
                DrawGame();
                await Task.Delay(50);
            }
            Console.Clear();
            Console.WriteLine($"Asteroids dodged: {score}\nDeaths: {deaths}\nTotal Play Time: {stopwatch.Elapsed.TotalSeconds} seconds");
        }

        public static async Task UpdateAsteroids()
        {
            Random rand = new Random();
            while (gameRunning)
            {
                lock (lockObject)
                {
                    asteroids.Add((rand.Next(Console.WindowWidth), 0));
                    for (int i = 0; i < asteroids.Count; i++)
                    {
                        asteroids[i] = (asteroids[i].x, asteroids[i].y + 1);
                    }
                    asteroids.ForEach(a =>
                    {
                        if(a.y > Console.WindowHeight)
                        {
                            score++;
                        }
                    });
                    asteroids.RemoveAll(a => a.y > Console.WindowHeight);  
                    if(asteroids.Any(a => a.x == shipX && a.y == shipY))
                    {
                        deaths++;
                        shipX = 22;
                        shipY = 20;
                        asteroids.Clear();
                    }
                }
                await Task.Delay(20);
            }
        }
        public static async Task ReadUserInput()
        {
            while (gameRunning)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.A && shipX != 0)
                {
                    shipX--;
                }
                if (key == ConsoleKey.D && shipX != 45)
                {
                    shipX++;
                }
                if (key == ConsoleKey.Q)
                {
                    stopwatch.Stop();
                    gameRunning = false;
                }
                await Task.Delay(50);
            }
        }
        public static void DrawGame()
        {
            lock (lockObject)
            {
                Console.Clear();
                Console.SetCursorPosition(shipX, shipY);
                Console.Write("^");
            
                foreach (var asteroid in asteroids)
                {
                    Console.SetCursorPosition(asteroid.x, asteroid.y);
                    Console.Write("*");
                }
            }
        }
    }
}