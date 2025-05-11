using System.Diagnostics;

namespace T2.PR1.ThreadsTasksPart2.SergiAlbalt
{
    public class Record
    {
        public int Score { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public Stopwatch TotalPlayTime { get; set; } = new Stopwatch();
    }
}
