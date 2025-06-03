using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Тесктовая RPG-игра";

            AudioManager.Instance.SetMusicEnabled(true);
            AudioManager.Instance.SetVolume(0.7f);
            Console.WriteLine("=================================");
            Console.WriteLine("         Приключения героя");
            Console.WriteLine("=================================");
            Console.WriteLine("\nДобро пожаловать в мир приключений!");
            Console.WriteLine("\nНажмите на любую клавишу для продолжения...");
            Console.ReadKey(true);

            Game game = new Game();
            game.Start();
        }
    }
}
