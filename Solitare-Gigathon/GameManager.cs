using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solitare
{
    public class GameManager //Klasa odpowiadająca za ogólne ustawienia gry
    {
        public static Difficulties Difficulty { get; private set; }
        public static string Username { get; private set; }
        public static ScoreBoardBase scoreBoard { get; private set; }
        public static List<Card> spareDeck;
        public static List<List<Card>> board;
        public static List<Card>[] colorDecks = new List<Card>[4];
        public static List<Card> spareCards = new List<Card>();
        public static int howManyMoves;
        public static int drawedSpareCards;

        private static void SetDifficulty() //Funkcja odpowiada za ustawienie poziomu trudności
        {
            Write("Select Difficulty:\n1. Easy\n2. Hard\n");
            string input = Ask().ToUpper().Trim();
            if (input == "1" || input == "EASY")
            {
                Difficulty = Difficulties.Easy;
            }
            else if (input == "2" || input == "HARD")
            {
                Difficulty = Difficulties.Hard;
            }
            else
            {
                Display.Clear();
                Write("Incorrect input. Please write number or name of difficulty level.");
                SetDifficulty();
            }
            Display.Clear();
        }

        public static void SetUsername() //Funkcja odpowiada za ustawianie nazwy użytkownika
        {
        startFunc:
            Write("Enter username: ");
            string input = Ask();
            if (scoreBoard.scores != null)
            {
                foreach (Score score in scoreBoard.scores)
                {
                    if (input == score.username)
                    {
                        Display.Clear();
                        Write("Username taken. Please select another.");
                        goto startFunc;
                    }
                }
            }
            Username = input;
            Display.Clear();
        }

        public static void GameSetup() //Funkcja przygotowuje grę 
        {
            scoreBoard = new ScoreBoardBase();
            scoreBoard = ScoreBoard.DeserializeObject();
            Display.Clear();
            Console.OutputEncoding = Encoding.UTF8;
            Display.StartScreen();
            Display.Refresh();
            SetDifficulty();
            SetUsername();
            CardsManager.SetupDecks();
            howManyMoves = 0;
        }

        public static void Stop() // Zatrzymuje program
        {
            Environment.Exit(0);
        }

        public static int[] GetBoardSize()
        {
            int[] boardSize = new int[2];
            boardSize[0] = board.Max(column => column.Count); // Pobieranie najdłuższej listy (liczba rzędów)
            boardSize[1] = board.Count(); // Pobieranie ilości kolumn (liczba kolumn)
            return boardSize;
        }


        private static bool IsInRange(int minValue, int maxValue, int value) // Sprawdza czy input jest w podanym przedziale
        {
            bool isValid = false;
            for (int i = minValue; i <= maxValue; i++)
            {
                if (value == i)
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        public static void Wait() // Funkcja która zatrzymuje program do czasu aż użyktkownik nie wciśnie przycisku.
        {
            Write("Press any key to continue...");
            Console.ReadKey();
        }

        public static void Write<T>(T text) // Funkcja ułatwia wypisywanie tekstu na konsoli.
        {
            Console.WriteLine(text);
        }

        public static int SetDimension(int max, string name) // Fuckcja pyta użytkownika o kolumnę lub rząd
        {
        restart:
            Write($"Select {name} (1 - {max}):\n");
            string input = Ask();

            int value;
            if (!int.TryParse(input, out value))
            {
                Display.Refresh();
                Write($"Input has to be an integer number between 1 - {max}.");
                goto restart;
            }

            if (!IsInRange(1, max, value))
            {
                Display.Refresh();
                Write($"This {name} doesn't exists. Enter number between  1 - {max}.");
                goto restart;
            }

            Display.Refresh();
            return value - 1;
        }

        public static void UpdateBoard() // Funkcja sprawdza, czy w jakiejś kolumnie przynajmniej 1 karta jest odkryta. Jeśli nie to odkrywa ostatnią.
        {
            foreach (List<Card> list in board)
            {
                bool IsVisibleCard = false;
                foreach (Card card in list)
                {
                    if (card.isVisible)
                    {
                        IsVisibleCard = true;
                    }
                }
                if (!IsVisibleCard && list.Count > 0)
                {
                    Card card = list.Last();
                    card.isVisible = true;
                    list.Remove(list.Last());
                    list.Add(card);
                }
            }
        }

        public static string Ask() //Funkcja pyta o input
        {
            Console.Write("Command: ");
            return Console.ReadLine();
        }

        public static bool ContainsSpareCard(Card[] array)
        {
            foreach (Card card in array)
            {
                foreach (Card spareCard in GameManager.spareCards)
                {
                    if (card.value == spareCard.value && card.color.code == spareCard.color.code)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsWin() // Funkcja sprawdza czy na stosach końcowych są tylko króle.
        {
            foreach (List<Card> list in colorDecks)
            {
                if (list != null)
                {
                    if (list.Last().value == 'K')
                    {
                        // Deck is full
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static Score[] SortScores(Score[] scores) // Funkcja sortuje tabele wyników. Od najniższego do najwyższego (im mniej ruchów, tym lepiej)
        {
            Score[] scoreArray = scores;

            int length = scoreArray.Length;
            bool changed;

            for (int i = 0; i < length - 1; i++)
            {
                changed = false;
                for (int j = 0; j < length - i - 1; j++)
                {
                    if (scoreArray[j].score > scoreArray[j + 1].score)
                    {
                        Score temp = scoreArray[j];
                        scoreArray[j] = scoreArray[j + 1];
                        scoreArray[j + 1] = temp;
                        changed = true;
                    }
                }


                if (!changed)
                {
                    break;
                }
            }

            return scoreArray;
        }

        public static Score[] GetFirstThreeScores(Score[] scoreArray) // Funkcja pobiera ostatnie 3 wyniki z tabeli wyników
        {
            int length = scoreArray.Length;
            int howManyCards = 3;

            List<Score> scores = new List<Score>();

            for (int i = 0; i < length; i++)
            {
                if (i < howManyCards)
                {
                    scores.Add(scoreArray[i]);
                }
            }

            return scores.ToArray();
        }

        public static void EndGame() // Funkcja zakańcza grę.
        {

            ScoreBoardBase scoreBoardBase = scoreBoard;

            Score score = new Score();
            score.username = Username;
            score.score = howManyMoves;
            score.difficultyLevel = Difficulty.ToString();

            if (scoreBoardBase.scores == null)
            {
                scoreBoardBase.scores = new List<Score>();
            }
            scoreBoardBase.scores.Add(score);

            scoreBoardBase.scores = SortScores(scoreBoardBase.scores.ToArray()).ToList();

            ScoreBoard.SerializeObject(scoreBoardBase);

            Display.WinScreen(scoreBoardBase, score);
        }
    }
}

public enum Difficulties // Typ enum zawierający wszystkie możliwe poziomy trudności.
{
    Easy,
    Hard
}
