using CsvHelper;
using CsvHelper.Configuration;
using System.Diagnostics;
using System.Globalization;
using T2.PR1.ThreadsTasksPart2.SergiAlbalt;

namespace T2.PR1.ThreadsTasksPart2.SergiAlbalat
{
    public class Program
    {
        public static bool gameRunning = true;
        public static int shipX = 22;
        public static int shipY = 20;
        public static List<(int x, int y)> asteroids = new List<(int x, int y)>();
        public static object lockObject = new object();
        public static Record Record = new Record();
        public static async Task Main()
        {
            Record.TotalPlayTime.Start();
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
            Console.WriteLine($"Asteroids dodged: {Record.Score}\nDeaths: {Record.Deaths}\nTotal Play Time: {Record.TotalPlayTime.Elapsed.TotalSeconds} seconds");
            string filePath = "../../../records.csv";
            using StreamWriter sw = new StreamWriter(filePath, true);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = !File.Exists(filePath)
            };
            using (var csvWriter = new CsvWriter(sw, config))
            {
                
                csvWriter.WriteRecord(Record);
                csvWriter.NextRecord();
            }
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
                            Record.Score++;
                        }
                    });
                    asteroids.RemoveAll(a => a.y > Console.WindowHeight);  
                    if(asteroids.Any(a => a.x == shipX && a.y == shipY))
                    {
                        Record.Deaths++;
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
                    Record.TotalPlayTime.Stop();
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