using System;
using System.Collections.Generic;
using System.Linq;

namespace Solitare
{
    public static class Display
    {
        private static readonly ConsoleColor colorDeckColor = ConsoleColor.DarkYellow;
        private static readonly ConsoleColor numberColor = ConsoleColor.DarkCyan;
        private static readonly ConsoleColor borderColor = ConsoleColor.DarkGray;
        private static readonly ConsoleColor HiddenCardColor = ConsoleColor.Green;
        private static readonly ConsoleColor defaultFontColor = ConsoleColor.White;
        private static readonly ConsoleColor defaultBackgroundColor = ConsoleColor.Black;
        private static readonly ConsoleColor selectedCardColor = ConsoleColor.Yellow;
        private static readonly ConsoleColor interfaceColor = ConsoleColor.Magenta;
        private static readonly ConsoleColor spareDeckColor = ConsoleColor.DarkRed;
        private static readonly ConsoleColor disabledCardColor = ConsoleColor.DarkGray;
        private static readonly ConsoleColor ScreenColor = ConsoleColor.Yellow;

        private static void DisplayBoard() // Funkcja wyświetla planszę
        {
            int maxRow = GameManager.board.Max(column => column.Count);
            Console.ForegroundColor = borderColor;
            Console.WriteLine("________________________________________");
            Console.Write("|");
            Console.ForegroundColor = numberColor;
            Console.Write("    1.   2.   3.   4.   5.   6.   7.  ");
            Console.ForegroundColor = borderColor;
            Console.Write("|");
            Console.Write("\n");

            int whichColumnIsSelectedOn = 0;
            bool wasSelected = false;
            for (int i = 0; i < maxRow; i++)
            {
                Console.ForegroundColor = borderColor;
                Console.Write("|");
                Console.ForegroundColor = numberColor;
                if ((i + 1).ToString().Length <= 1)
                {
                    Console.Write((i + 1) + ". ");
                }
                else
                {
                    Console.Write((i + 1) + ".");
                }

                for (int j = 0; j < 7; j++)
                {
                    if (i < GameManager.board[j].Count)
                    {
                        Card currentCard = GameManager.board[j][i];
                        Console.ForegroundColor = currentCard.color.color;
                        if (currentCard.isVisible)
                        {
                            if (currentCard.isSelected)
                            {
                                wasSelected = true;
                                whichColumnIsSelectedOn = j;
                            }

                            if (wasSelected && j == whichColumnIsSelectedOn)
                            {
                                Console.ForegroundColor = selectedCardColor;
                            }

                            Console.Write($"[{currentCard.value}{currentCard.color.code}]{"",-1}");
                        }
                        else
                        {
                            Console.ForegroundColor = HiddenCardColor;
                            Console.Write($"[##]{"",-1}");
                        }
                    }
                    else
                    {
                        Console.Write($"{"",-5}");
                    }
                }
                Console.ForegroundColor = borderColor;
                Console.Write("|");
                Console.Write("\n");
            }
            Console.ForegroundColor = borderColor;
            Console.WriteLine("----------------------------------------\n");
            ResetFontColor();
        }

        private static void DisplayColorDecks() // Funkcja wyświetla 4 stosy, w których będzie zbierać się karty od asa do króla. Każdy stos dla każdego koloru
        {
            foreach (List<Card> colorList in GameManager.colorDecks)
            {
                if (colorList != null)
                {
                    Card lastCard = colorList.Last();
                    Console.ForegroundColor = lastCard.color.color;
                    Console.Write($"[{lastCard.value}{lastCard.color.code}] ");
                }
                else
                {
                    Console.ForegroundColor = colorDeckColor;
                    Console.Write("[--] ");
                }
            }
            Console.Write("\n\n");
            ResetFontColor();
        }

        private static void DisplayDifficultyLevel() // Funkcja wyświetla poziom trudności w ramce. Ramka dostosowywuje się do długości tekstu
        {
            Console.ForegroundColor = interfaceColor;
            string middleLine = $"| Difficulty level: {GameManager.Difficulty} |";
            int length = middleLine.Length;
            for (int i = 0; i < length; i++)
            {
                Console.Write("-");
            }
            Console.Write("\n");
            Console.Write(middleLine);
            Console.Write("\n");
            for (int i = 0; i < length; i++)
            {
                Console.Write("-");
            }
            Console.Write("\n");
            ResetFontColor();
        }

        private static void DisplayUsername() // Funkcja wyświetla nazwę użytkownika. Bierze pod uwagę długość tekstu i dopasowuje rozmiar ramki.
        {
            Console.ForegroundColor = interfaceColor;
            string middleLine = $"| Username: {GameManager.Username} |";
            int length = middleLine.Length;
            for (int i = 0; i < length; i++)
            {
                Console.Write("-");
            }
            Console.Write("\n");
            Console.Write(middleLine);
            Console.Write("\n");
            for (int i = 0; i < length; i++)
            {
                Console.Write("-");
            }
            Console.Write("\n");
            ResetFontColor();
        }

        private static void DisplaySpareDeck() // Funkcja wyświetla stos rezerwowy.
        {
            Console.ForegroundColor = spareDeckColor;
            Console.Write("Spare cards: ");
            if (GameManager.spareCards.Count > 0)
            {
                foreach (Card spareCard in GameManager.spareCards)
                {
                    if (spareCard.Equals(GameManager.spareCards.Last()))
                    {
                        if (spareCard.isSelected)
                        {
                            Console.ForegroundColor = selectedCardColor;
                        }
                        else
                        {
                            Console.ForegroundColor = spareCard.color.color;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = disabledCardColor;
                    }
                    Console.Write($"[{spareCard.value}{spareCard.color.code}]");
                }
            }
            else
            {
                Console.Write("[--]");
            }
            Console.ForegroundColor = spareDeckColor;
            Console.Write($"\nTotal cards in spare deck: {GameManager.spareDeck.Count}");
            Console.Write("\n\n");
            ResetFontColor();
        }

        public static void DisplayMoveCounter() // Funkcja wyświetla ilość wykonanych dotychczas ruchów.
        {
            Console.ForegroundColor = spareDeckColor;
            Console.Write($"You made {GameManager.howManyMoves} moves.\n");
            ResetFontColor();
        }

        public static void Clear() // Funkcja czyści konsolę i wyświetla na niej tytuł (tytuł wyświetla się zawsze)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\"SOLITARE\" by Oskar Rzońca");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("|\n--------------------------\n");
            ResetFontColor();
        }

        public static void Refresh() // Funkcja wyświetla wszystkie obiekty ponownie (Odświeża konsolę)
        {
            Clear();
            DisplayUsername();
            DisplayDifficultyLevel();
            DisplayMoveCounter();
            DisplayBoard();
            DisplaySpareDeck();
            DisplayColorDecks();
        }

        public static void ResetFontColor() // Funkcja przywraca podstawowy kolor czcionki
        {
            Console.ForegroundColor = defaultFontColor;
            Console.BackgroundColor = defaultBackgroundColor;
        }

        public static void Help() // Funkcja wyświetla menu pomocy z komendami ogólnymi
        {
            Clear();
            Console.ForegroundColor = interfaceColor;
            Console.Write("---------------------------------------------------\n" +
                          "|                       HELP                      |\n" +
                          "---------------------------------------------------\n" +
                          "|  Restart - Restart the game                     |\n" +
                          "|  Quit - Quit the game                           |\n" +
                          "|  Rename - Change username                       |\n" +
                          "|  Select - Select card on the board              |\n" +
                          "|  Draw - Draw card from spare deck               |\n" +
                          "|  Guide - Show selected card commands            |\n" +
                          "--------------------------------------------------- \n");
            ResetFontColor();
        }

        public static void SelectedCardGuide() // Funkcja wyświetla menu pomocy z komendami do zaznaczonej karty
        {
            Clear();
            Console.ForegroundColor = interfaceColor;
            Console.Write("---------------------------------------------------\n" +
                          "|                       GUIDE                     |\n" +
                          "---------------------------------------------------\n" +
                          "|  Move - Move card or cards onto column          |\n" +
                          "|  Color - Move Card onto it's color deck         |\n" +
                          "|  None - Do nothing                              |\n" +
                          "---------------------------------------------------\n");
            ResetFontColor();
        }

        public static void WinScreen(ScoreBoardBase scoreBoardBase, Score score) // Funkcja wyświetla menu wygranej
        {
            int place = 0;
            if (scoreBoardBase.scores != null)
            {
                foreach (Score currentScore in scoreBoardBase.scores)
                {
                    place++;
                    if (currentScore == score)
                    {
                        break;
                    }
                }

                Clear();
                Console.ForegroundColor = ScreenColor;
                Console.Write($"\n\nCONGRATULATIONS {score.username} !!!\nYou've made {score.score} moves.\nYou took {place} place on score board.\n\n");
                Console.Write("Top 3 on score board: \n\n");
                int counter = 0;
                foreach (Score currentScore in GameManager.GetFirstThreeScores(scoreBoardBase.scores.ToArray()))
                {
                    counter++;
                    Console.Write($"{counter}. {currentScore.username} - {currentScore.score}. Difficulty level: {currentScore.difficultyLevel}.\n\n");
                }
                Console.Write("Thanks for playing!");
                GameManager.Wait();
                ResetFontColor();
            }
        }

        public static void StartScreen()
        {
            Clear();
            Console.ForegroundColor = ScreenColor;
            Console.Write("Welcome! \n\n" +
                "Before you start here are some improtant differents from the original game:\n" +
                "1. In the game there is a T card instead of 10, but it's value hasn't changed.\n" +
                "2. You can type 'help' to see important commands.\n\n" +
                "Enjoy! :D\n");
            GameManager.Wait();
            ResetFontColor();
        }
    }
}
