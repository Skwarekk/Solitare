using System;

namespace Solitare
{
    public static class InputManager
    {
        public static void CheckInput() // Funkcja pobiera input i wykonuje funkcję przypisaną do pobranego inputa
        {
            GameManager.Write("Write 'help' for help.\n");
            string input = GameManager.Ask();
            Display.Refresh();

            switch (input.ToUpper().Trim())
            {
                case "RESTART":
                    GameManager.GameSetup();
                    break;
                case "QUIT":
                    GameManager.Stop();
                    break;
                case "SELECT":
                    CardsManager.SelectCard();
                    SelectedCardCommands();
                    break;
                case "HELP":
                    Display.Help();
                    GameManager.Wait();
                    break;
                case "GUIDE":
                    Display.SelectedCardGuide();
                    GameManager.Wait();
                    break;
                case "DRAW":
                    CardsManager.DrawCardFromSpareDeck();
                    break;
                case "RENAME":
                    Display.Clear();
                    GameManager.SetUsername();
                    break;
                default:
                    GameManager.Write("Incorrect input.");
                    CheckInput();
                    break;

            }
        }

        private static void SelectedCardCommands() // Funkcja odpowiada za inputy związane z wybraną kartą
        {
            Card[] cards = CardsManager.GetSelectedCards();
            Console.Write("What do you want to do with these cards? ");
            foreach (Card card in cards)
            {
                Console.ForegroundColor = card.color.color;
                Console.Write($"[{card.value}{card.color.code}]");
            }
            Console.Write("\n");
            Display.ResetFontColor();
            GameManager.Write("Write 'guide' to see all features.\n");
            string input = GameManager.Ask();
            switch (input.ToUpper().Trim())
            {
                case "MOVE":
                    Display.Refresh();
                    CardsManager.MoveCardsOntoAnotherColumn();
                    break;
                case "NONE":
                    break;
                case "GUIDE":
                    Display.SelectedCardGuide();
                    GameManager.Wait();
                    Display.Refresh();
                    SelectedCardCommands();
                    break;
                case "COLOR":
                    CardsManager.MoveCardOntoColorDeck();
                    break;
                default:
                    Display.Refresh();
                    GameManager.Write("Incorrect input!");
                    SelectedCardCommands();
                    break;
            }

            GameManager.UpdateBoard();
            CardsManager.UnselectSelectedCard();
        }
    }
}
