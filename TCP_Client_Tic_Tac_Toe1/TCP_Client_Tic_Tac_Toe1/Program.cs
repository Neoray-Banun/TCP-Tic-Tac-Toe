using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Client_Tic_Tac_Toe2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient("127.0.0.1", 5000);
            NetworkStream stream = client.GetStream();
            Console.WriteLine("Connected to the server");

            byte[] buffer = new byte[1024];
            int byteRead = stream.Read(buffer, 0, buffer.Length);
            string symbol = UTF8Encoding.UTF8.GetString(buffer, 0, byteRead);

            Console.WriteLine($"You are ({symbol})");

            Recive(stream);

        }

        public static void Recive(NetworkStream stream)
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                int byteRead = stream.Read(buffer, 0, buffer.Length);
                string message = UTF8Encoding.UTF8.GetString(buffer, 0, byteRead);

                if (message.StartsWith("BOARD:"))
                {
                    message = message.Substring(6);
                    char[,] board = StringToBoard(message);
                    DisplayBoardAndSend(board, stream);
                }
                else if (message.StartsWith("MSG:"))
                {
                    message = message.Substring(4);
                    Console.WriteLine("");
                    Console.WriteLine(message);
                }
                else if (message.StartsWith("RES:"))
                {
                    message = message.Substring(4);
                    Console.WriteLine(message);
                    break;
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }

        public static char[,] StringToBoard(string stringBoard)
        {
            int index = 0;
            char[,] board = { {' ', ' ', ' '}
                             ,{' ', ' ', ' '}
                             ,{' ', ' ', ' '} };

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    board[row, col] = stringBoard[index];
                    index++;
                }
            }

            return board;
        }

        public static void DisplayBoardAndSend(char[,] board, NetworkStream stream)
        {
            Console.WriteLine("");
            Console.WriteLine($"    [1]   [2]   [3]");
            Console.WriteLine("");
            Console.WriteLine($"[1]  {board[0, 0]}  │  {board[0, 1]}  │  {board[0, 2]}");
            Console.WriteLine("    ─── + ─── + ───");
            Console.WriteLine($"[2]  {board[1, 0]}  │  {board[1, 1]}  │  {board[1, 2]}");
            Console.WriteLine("    ─── + ─── + ───");
            Console.WriteLine($"[3]  {board[2, 0]}  │  {board[2, 1]}  │  {board[2, 2]}");

            Console.WriteLine("\nWrite the position you would like to place");

            int pos;

            while (true)
            {
                Console.WriteLine("Write only two numbers like (X position) (Y position) starting at 1");
                string message = Console.ReadLine();
                string result = message.Trim();

                if (int.TryParse(result, out pos))
                {
                    int posX = pos / 10;
                    int posY = pos % 10;

                    if (posX < 1 || posX > 3 || posY < 1 || posY > 3 || pos < 10 || pos > 99)
                    {
                        Console.WriteLine("\nNot in the compounds of the board!");
                    }
                    else
                    {
                        if (board[posY - 1, posX - 1] != ' ')
                        {
                            Console.WriteLine("\nThe position is already taken!");
                        }
                        else
                        {
                            Console.WriteLine("\nWaiting for opponent response...");
                            break;
                        }

                    }
                }
                else
                {
                    Console.WriteLine("\nNot a valid position!");
                }
            }

            string posStr = pos.ToString();
            byte[] buffer = UTF8Encoding.UTF8.GetBytes(posStr);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
