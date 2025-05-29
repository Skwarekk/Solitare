using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Solitare
{
    public static class ScoreBoard
    {
        public static ScoreBoardBase DeserializeObject() // Funkcja zamienia plik tekstowy na obiekt z wszystkimi wynikami.
        {
            string file = File.ReadAllText(@"C:\Users\oskar\Desktop\Projekty\C#\Solitare\Solitare\ScoreBoard.json");
            return JsonConvert.DeserializeObject<ScoreBoardBase>(file);
        }

        public static void SerializeObject(ScoreBoardBase obj) // Funkcja zamienia obiekt na plik tekstowy.
        {
            string file = JsonConvert.SerializeObject(obj);
            File.WriteAllText(@"C:\Users\oskar\Desktop\Projekty\C#\Solitare\Solitare\ScoreBoard.json", file);
        }
    }

    public class ScoreBoardBase // Klasa posiada listę wszystkich wyników.
    {
        public List<Score> scores;
    }

    public class Score // Klasa określa pojedynczy wynik w tabeli wyników.
    {
        public string username;
        public int score;
        public string difficultyLevel;
    }
}
