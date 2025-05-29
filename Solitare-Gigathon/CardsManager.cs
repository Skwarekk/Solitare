using System.Collections.Generic;
using System.Linq;
using System;

namespace Solitare
{
    public static class CardsManager //Klasa zawierająca wszystkie funkcje związane z talią kart
    {
        private static char[] values = new char[] { 'A', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K' }; //Lista zawierająca wszystkie figury kart. T oznacza liczbę 10. Zapisałem to tak, bo liczba 10 ma 2 znaki i karta była by dłuższa od innych
        private static CardColor[] cardColors = new CardColor[] {
      new CardColor("\u2660", ConsoleColor.White),
      new CardColor("\u2663", ConsoleColor.White),
      new CardColor("\u2666", ConsoleColor.Red),
      new CardColor("\u2665", ConsoleColor.Red)
    }; //Lista zawierająca wszystkie kolory kart.

        private static int drawedSpareCards; // Określa ile do tej pory kart zostało dobranych z stosu rezerwowego. Wartość się zeruje w przypadku dojścia do końca stosu.

        private static List<Card> DeckSetup() //Funkcja zwraca tablicę posiadającą całą talię kart.
        {
            List<Card> cards = new List<Card>();

            foreach (CardColor color in cardColors)
            {
                foreach (char value in values)
                {
                    cards.Add(new Card(value, color));
                }
            }
            return cards;
        }

        private static List<Card> ShuffleCards(List<Card> cards) //Funkcja tasuje wszystkie karty obecne w talii
        {
            Random random = new Random();

            int numberOfCarts = cards.Count;

            for (int i = numberOfCarts - 1; i > 0; i--)
            {
                int randomCard = random.Next(0, i + 1);

                Card temp = cards[i];
                cards[i] = cards[randomCard];
                cards[randomCard] = temp;
            }

            return cards;
        }

        private static List<List<Card>> boardSetup(List<Card> deck) //Funkcja zwraca 2 wymiarową listę reprezentującą układ kart na planszy
        {
            List<List<Card>> board = new List<List<Card>>();

            for (int i = 1; i <= 7; i++)
            {
                List<Card> column = new List<Card>();

                for (int j = 0; j < i; j++)
                {
                    Card cardToAdd = deck.First();

                    deck.Remove(deck.First());
                    if (j != i - 1) //Jeśli jest to ostatnia iteracja pętli
                    {
                        cardToAdd.isVisible = false;
                    }
                    column.Add(cardToAdd);
                }
                board.Add(column);
            }

            return board;
        }

        public static void SetupDecks() //Funkcja tworzy talię kart i dzieli ją na 2 kupki: Kupkę rezerwową oraz kupkę do zbudowania startowej planszy
        {
            List<Card> shuffledDeck = ShuffleCards(DeckSetup());
            List<Card> cardsToBoard = new List<Card>();
            int howManyCardsOnStarterBoard = 28;

            for (int i = 0; i < howManyCardsOnStarterBoard; i++)
            {
                cardsToBoard.Add(shuffledDeck.First());
                shuffledDeck.Remove(shuffledDeck.First());
            }

            GameManager.board = boardSetup(cardsToBoard);
            GameManager.spareDeck = shuffledDeck;
            GameManager.colorDecks = new List<Card>[4];
            GameManager.spareCards.Clear();
        }

        public static void SelectCard() // Funkcja odpowiada za wybór karty
        {
        restart:
            int row, column;
            int[] boardSize = GameManager.GetBoardSize();

            GameManager.Write("From where do you want to select card?\n1. Board\n2. Deck");
            string input = GameManager.Ask().ToUpper().Trim();

            Display.Refresh();

            if (input == "1" || input == "BOARD")
            {
                column = GameManager.SetDimension(boardSize[1], "column");
                row = GameManager.SetDimension(boardSize[0], "row");

                if (GameManager.board[column].Count <= row)
                {
                    Display.Refresh();
                    GameManager.Write("Card doesn't exists. Please select another.");
                    goto restart;
                }

                if (!GameManager.board[column][row].isVisible)
                {
                    Display.Refresh();
                    GameManager.Write("Can't select hidden card. Please select another.");
                    goto restart;
                }

                Card card = GameManager.board[column][row];
                card.isSelected = true;
                GameManager.board[column][row] = card;
                Display.Refresh();

            }
            else if (input == "2" || input == "DECK")
            {
                if (GameManager.spareCards.Count > 0)
                {
                    Card card = GameManager.spareCards.Last();

                    GameManager.spareCards.Remove(card);
                    GameManager.spareDeck.Remove(card);

                    card.isSelected = true;
                    GameManager.spareCards.Add(card);
                    GameManager.spareDeck.Add(card);
                }
                else
                {
                    Display.Refresh();
                    GameManager.Write("There is no cards draw!");
                    GameManager.Wait();
                    Display.Refresh();
                    goto restart;
                }
            }
            else
            {
                Display.Refresh();
                GameManager.Write("Incorrect input. Please write number or name of location.");
                goto restart;
            }
            Display.Refresh();
        }

        public static void MoveCardsOntoAnotherColumn() // Funkcja porusza karty z jednej kolumny na inną
        {
            int boardWidth = GameManager.GetBoardSize()[1];

            int toWhitchColumn = GameManager.SetDimension(boardWidth, "column");

            Card[] currentSelectedCards = GetSelectedCards();

            int columnIndex = GetSelectedCardsColumnIndex();
            int selectedCardsIndex = GetSelectedCardsColumnIndex();

            if (GameManager.board[toWhitchColumn].Count > 0)
            {
                Display.Refresh();

                GameManager.Write("Where to move: ");

                int indexOfFirstSelectedCardValue = GetIndexOfCardValue(currentSelectedCards.First());
                int indexOfLastCardValueInSelectedColumn = GetIndexOfCardValue(GetLastCardFromColumn(toWhitchColumn));

                // Selected column is not empty

                if (currentSelectedCards.First().color.color == GetLastCardFromColumn(toWhitchColumn).color.color ||
        indexOfFirstSelectedCardValue != indexOfLastCardValueInSelectedColumn - 1)
                {
                    // Move is not possible

                    Display.Refresh();
                    GameManager.Write("Can't do this move.");
                    GameManager.Wait();
                    return;
                }
                else
                {
                    // Move is possible

                    foreach (Card card in currentSelectedCards)
                    {
                        GameManager.board[toWhitchColumn].Add(card);
                        Display.Refresh();
                    }
                    if (GameManager.ContainsSpareCard(currentSelectedCards))
                    {
                        // Remove card from spare deck and selected spare cards

                        Card cardToRemove = GameManager.spareCards.Last();

                        GameManager.spareDeck.Remove(cardToRemove);
                        GameManager.spareCards.Remove(cardToRemove);
                    }
                    else
                    {
                        foreach (Card card in currentSelectedCards)
                        {
                            GameManager.board[selectedCardsIndex].Remove(card);
                        }
                    }

                    GameManager.howManyMoves++;
                }
            }
            else
            {
                // Selected column is empty

                if (currentSelectedCards.First().value == 'K')
                {
                    // Selected card is King card

                    foreach (Card card in currentSelectedCards)
                    {
                        GameManager.board[toWhitchColumn].Add(card);
                    }

                    if (GameManager.ContainsSpareCard(currentSelectedCards))
                    {
                        // Remove card from spare deck and selected spare cards
                        Card cardToRemove = GameManager.spareCards.Last();

                        GameManager.spareDeck.Remove(cardToRemove);
                        GameManager.spareCards.Remove(cardToRemove);
                    }
                    else
                    {
                        foreach (Card card in currentSelectedCards)
                        {
                            GameManager.board[selectedCardsIndex].Remove(card);
                        }
                    }

                    GameManager.howManyMoves++;
                }
                else
                {
                    // Selected card is not King card
                    // Move is not possible

                    Display.Refresh();
                    GameManager.Write("Can't do this move.");
                    GameManager.Wait();
                    return;
                }
            }

            GameManager.UpdateBoard();
            Display.Refresh();
        }

        public static void MoveCardOntoColorDeck() // Funkcja przenosi wybr
        {
            Card[] selectedCards = GetSelectedCards();

            if (selectedCards.Count() != 1)
            {
                // There is more than one selected card

                Display.Refresh();
                GameManager.Write("Can't move more than one card onto color deck.");
                GameManager.Wait();
                return;
            }
            else
            {
                // There is only one card

                Card selectedCard = selectedCards.First();

                if (selectedCard.value == 'A')
                {
                    // Card is As card

                    for (int i = 0; i < GameManager.colorDecks.Length; i++)
                    {
                        if (GameManager.colorDecks[i] == null)
                        {
                            // Color deck is empty

                            GameManager.colorDecks[i] = new List<Card>() { selectedCard };

                            if (GameManager.ContainsSpareCard(selectedCards))
                            {
                                // Remove card from spare deck and selected spare card
                                Card cardToRemove = GameManager.spareCards.Last();

                                GameManager.spareDeck.Remove(cardToRemove);
                                GameManager.spareCards.Remove(cardToRemove);
                            }
                            else
                            {
                                GameManager.board[GetSelectedCardsColumnIndex()].Remove(selectedCard);
                            }

                            GameManager.howManyMoves++;
                            Display.Refresh();
                            return;
                        }
                    }
                }
                else
                {
                    // Card is not As card
                    foreach (List<Card> list in GameManager.colorDecks)
                    {
                        if (list == null)
                        {
                            // Color deck is empty
                            Display.Refresh();
                            GameManager.Write("Can't move this card onto color deck yet.");
                            GameManager.Wait();

                            return;
                        }
                        else
                        {
                            // Color deck has some cards

                            if (list.Last().color.code == selectedCard.color.code && GetIndexOfCardValue(list.Last()) == GetIndexOfCardValue(selectedCard) - 1)
                            {
                                // Card matches deck
                                list.Add(selectedCard);

                                if (GameManager.ContainsSpareCard(selectedCards))
                                {
                                    // Remove card from spare deck and selected spare cards
                                    Card cardToRemove = GameManager.spareCards.Last();

                                    GameManager.spareDeck.Remove(cardToRemove);
                                    GameManager.spareCards.Remove(cardToRemove);
                                }
                                else
                                {
                                    GameManager.board[GetSelectedCardsColumnIndex()].Remove(selectedCard);
                                }

                                GameManager.howManyMoves++;
                                return;
                            }
                            else
                            {
                                // Card does not match
                                continue;
                            }
                        }
                    }
                    Display.Refresh();
                    GameManager.Write("Can't move this card onto color deck yet.");
                    GameManager.Wait();
                    return;
                }
            }
        }

        public static void DrawCardFromSpareDeck() // Funkcja w zależności od poziomu trudności dobiera odpowiednią ilość kart. 
        {
            UnselectSelectedCard();

            GameManager.spareCards.Clear();

            int everyHowManyCards;

            switch (GameManager.Difficulty)
            {
                case Difficulties.Easy:
                    everyHowManyCards = 1;
                    break;
                case Difficulties.Hard:
                    everyHowManyCards = 3;
                    break;
                default:
                    everyHowManyCards = 1;
                    break;
            }

            if (GameManager.spareDeck.Count > 0)
            {
                for (int i = 0; i < everyHowManyCards; i++)
                {
                    int whitchCard = drawedSpareCards + everyHowManyCards;

                    if (GameManager.spareDeck.Count < whitchCard)
                    {
                        drawedSpareCards = 0;
                        ShuffleCards(GameManager.spareDeck);
                        GameManager.Write("end of the spare deck. Shullfing...");
                        GameManager.Wait();
                        return;
                    }
                    GameManager.spareCards.Add(GameManager.spareDeck[whitchCard - 1]);

                    drawedSpareCards++;
                }
            }
            else
            {
                GameManager.Write("There is no more cards on spare deck.");
                GameManager.Wait();
                Display.Refresh();
            }
        }

        private static Card GetLastCardFromColumn(int index) // Funkcja pobiera ostatnią kartę z kolumny o danym indeksie
        {
            List<Card> column = GameManager.board[index];

            return column.Last();
        }

        public static Card[] GetSelectedCards() // Funkcja zwraca aktualnie wybraną przez użytkownika kartę. W przypadku gdy żadna karta nie jest wybrana, funkcja zwraca pustą kartę, bo funkcja nie może zwrócić null
        {
            List<Card> cards = new List<Card>();

            foreach (List<Card> list in GameManager.board)
            {
                bool wasSelected = false;

                foreach (Card card in list)
                {
                    if (card.isSelected)
                    {
                        wasSelected = true;
                    }

                    if (wasSelected)
                    {
                        cards.Add(card);
                    }
                }

                if (wasSelected)
                {
                    break;
                }
            }

            if (GameManager.spareCards != null)
            {
                // There is at least one card drawed from spare deck

                foreach (Card card in GameManager.spareCards)
                {
                    if (card.isSelected)
                    {
                        cards.Add(card);
                    }
                }
            }

            return cards.ToArray();
        }

        private static int GetSelectedCardsColumnIndex() // Funkcja zwraca kolumnę, w której są wszystkie wybrane karty
        {
            for (int i = 0; i < GameManager.board.Count; i++)
            {
                foreach (Card card in GetSelectedCards())
                {
                    if (GameManager.board[i].Contains(card))
                    {
                        Console.WriteLine($"selected cards column: {i}");
                        return i;
                    }
                }
            }
            return 0;
        }

        public static void UnselectSelectedCard() // Funkcja pobiera akutalnie zaznaczoną kartę i odznacza ją.
        {
            for (int i = 0; i < GameManager.board.Count; i++)
            {
                for (int j = 0; j < GameManager.board[i].Count; j++)
                {
                    Card boardCard = GameManager.board[i][j];
                    if (boardCard.isSelected)
                    {
                        boardCard.isSelected = false;
                        GameManager.board[i][j] = boardCard;
                    }
                }
            }

            if (GameManager.spareCards != null)
            {
                List<Card> cardsToEdit = new List<Card>();

                foreach (Card card in GameManager.spareCards)
                {
                    cardsToEdit.Add(card);
                }

                foreach (Card card in cardsToEdit)
                {
                    GameManager.spareCards.Remove(card);
                    Card tempCard = card;
                    tempCard.isSelected = false;
                    GameManager.spareCards.Add(tempCard);
                }
            }
        }

        private static int GetIndexOfCardValue(Card card) // Funkcja pobiera indeks figury karty
        {
            char value = card.value;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == value)
                {
                    return i;
                }
            }
            return 0;
        }
    }

    public struct Card //Struktura określająca pojedynczą kartę do gry.
    {
        public char value;
        public CardColor color;
        public bool isVisible;
        public bool isSelected;

        public Card(char value, CardColor color)
        {
            this.value = value;
            this.color = color;
            isVisible = true;
            isSelected = false;
        }
    }

    public struct CardColor //Struktura określająca kolory kart
    {
        public string code;
        public ConsoleColor color;

        public CardColor(string code, ConsoleColor color)
        {
            this.code = code;
            this.color = color;
        }
    }
}