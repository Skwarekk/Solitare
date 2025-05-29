namespace Solitare
{
    public class Program
    {
        static void Main(string[] args)
        {
            GameManager.GameSetup();
            while (!GameManager.IsWin())
            {
                Display.Refresh();
                InputManager.CheckInput();
            }
            GameManager.EndGame();
        }
    }
}
