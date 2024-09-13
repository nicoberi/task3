using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RockPaperScissors
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3 || args.Length % 2 == 0 || args.Distinct().Count() != args.Length)
            {
                Console.WriteLine("Error: Provide an odd number of at least 3 unique moves.");
                Console.WriteLine("Example: rock paper scissors lizard Spock");
                return;
            }

            string[] moves = args;
            var random = new Random();

            // Генерация криптографического ключа (256 бит)
            var key = GenerateKey();
            string computerMove = moves[random.Next(moves.Length)];
            string hmac = ComputeHMAC(computerMove, key);

            // Показ HMAC для хода компьютера
            Console.WriteLine("HMAC: " + hmac);

            while (true)
            {
                Console.WriteLine("Available moves:");
                for (int i = 0; i < moves.Length; i++)
                {
                    Console.WriteLine($"{i + 1} - {moves[i]}");
                }
                Console.WriteLine("0 - Exit");
                Console.WriteLine("? - Help");

                Console.Write("Enter your move: ");
                string input = Console.ReadLine();

                if (input == "0")
                {
                    break;
                }
                else if (input == "?")
                {
                    PrintHelpTable(moves);
                    continue;
                }

                if (int.TryParse(input, out int playerChoice) && playerChoice >= 1 && playerChoice <= moves.Length)
                {
                    string playerMove = moves[playerChoice - 1];
                    Console.WriteLine("Your move: " + playerMove);
                    Console.WriteLine("Computer move: " + computerMove);

                    var result = DetermineWinner(moves, playerMove, computerMove);
                    Console.WriteLine(result);

                    // Показ секретного ключа
                    Console.WriteLine("HMAC key: " + BitConverter.ToString(key).Replace("-", ""));
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }
            }
        }

        // Метод для генерации криптографического ключа
        static byte[] GenerateKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] key = new byte[32]; // 256 бит
                rng.GetBytes(key);
                return key;
            }
        }

        // Метод для вычисления HMAC с использованием SHA256
        static string ComputeHMAC(string message, byte[] key)
        {
            using (var hmac = new HMACSHA256(key))
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] hmacBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hmacBytes).Replace("-", "");
            }
        }

        // Метод для определения победителя
        static string DetermineWinner(string[] moves, string playerMove, string computerMove)
        {
            int playerIndex = Array.IndexOf(moves, playerMove);
            int computerIndex = Array.IndexOf(moves, computerMove);
            int half = moves.Length / 2;

            if (playerIndex == computerIndex)
                return "It's a draw!";
            
            if ((playerIndex > computerIndex && playerIndex - computerIndex <= half) ||
                (computerIndex > playerIndex && computerIndex - playerIndex > half))
                return "You win!";
            else
                return "Computer wins!";
        }

        // Метод для вывода таблицы помощи
        static void PrintHelpTable(string[] moves)
        {
            Console.WriteLine("Help table:");
            Console.Write("\t");
            foreach (var move in moves)
            {
                Console.Write(move + "\t");
            }
            Console.WriteLine();

            for (int i = 0; i < moves.Length; i++)
            {
                Console.Write(moves[i] + "\t");
                for (int j = 0; j < moves.Length; j++)
                {
                    if (i == j)
                    {
                        Console.Write("Draw\t");
                    }
                    else
                    {
                        string result = DetermineWinner(moves, moves[i], moves[j]);
                        if (result == "You win!")
                            Console.Write("Win\t");
                        else
                            Console.Write("Lose\t");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
