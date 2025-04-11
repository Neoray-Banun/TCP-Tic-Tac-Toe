using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TCP_Server_Tic_Tac_Toe
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool Win = false;

            char[,] board = { {' ', ' ', ' '}
                             ,{' ', ' ', ' '}
                             ,{' ', ' ', ' '} };


            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Server started");

            TcpClient playerX = server.AcceptTcpClient();
            NetworkStream xStream = playerX.GetStream();

            Console.WriteLine("Player 1 connected");
            Send("X", xStream);
            Send("Waiting for the other player to join...", xStream);

            TcpClient playerO = server.AcceptTcpClient();
            NetworkStream oStream = playerO.GetStream();

            Console.WriteLine("Player 2 connected");
            Send("O", oStream);

            server.Stop();

            for (int i = 0; i < 9; i++)
            {
                DisplayBoard(board);
                if (i % 2 == 0)
                {
                    SendBoard(board, xStream);

                    string strPos = Recive(xStream);
                    int pos = int.Parse(strPos);
                    int posY = (pos / 10) - 1;
                    int posX = (pos % 10) - 1;

                    board[posX, posY] = 'X';
                    Win = CheckWin(board, 'X');

                    if (Win)
                    {
                        Console.WriteLine("X won");

                        Send("RES:You won!", xStream);
                        Send("RES:You lost :(", oStream);
                        break;
                    }
                }
                else if (i % 2 == 1)
                {
                    SendBoard(board, oStream);

                    string strPos = Recive(oStream);
                    int pos = int.Parse(strPos);
                    int posY = (pos / 10) - 1;
                    int posX = (pos % 10) - 1;

                    board[posX, posY] = 'O';
                    Win = CheckWin(board, 'O');

                    if (Win)
                    {
                        Console.WriteLine("\nO won");

                        Send("RES:You won!", oStream);
                        Send("RES:You lost :(", xStream);
                        break;
                    }
                }
            }

            if (!Win)
            {
                Console.WriteLine("It is a tie");
                Send("RES:It is a tie", oStream);
                Send("RES:It is a tie", xStream);

            }
        }

        public static bool CheckWin(char[,] board, char player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (board[0, i] == player && board[1, i] == player && board[2, i] == player)
                {
                    return true;
                }
                else if (board[i, 0] == player && board[i, 1] == player && board[i, 2] == player)
                {
                    return true;
                }
            }

            if (board[0, 0] == player && board[1, 1] == player && board[2, 2] == player)
            {
                return true;
            }
            else if (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player)
            {
                return true;
            }

            return false;
        }

        public static void DisplayBoard(char[,] board)
        {
            Console.WriteLine("");
            Console.WriteLine($"    [1]   [2]   [3]");
            Console.WriteLine("");
            Console.WriteLine($"[1]  {board[0, 0]}  │  {board[0, 1]}  │  {board[0, 2]}");
            Console.WriteLine("    ─── + ─── + ───");
            Console.WriteLine($"[2]  {board[1, 0]}  │  {board[1, 1]}  │  {board[1, 2]}");
            Console.WriteLine("    ─── + ─── + ───");
            Console.WriteLine($"[3]  {board[2, 0]}  │  {board[2, 1]}  │  {board[2, 2]}");
        }

        public static void Send(string message, NetworkStream stream)
        {
            if (message == "X" || message == "O")
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                string formattedMessage = "MSG:" + message;
                byte[] buffer = Encoding.UTF8.GetBytes(formattedMessage);
                stream.Write(buffer, 0, buffer.Length);
            }

        }


        public static string Recive(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int byteRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, byteRead);
        }

        public static void SendBoard(char[,] board, NetworkStream stream)
        {
            string stringBoard = string.Concat(board.Cast<char>());
            string formattedBoard = "BOARD:" + stringBoard;
            byte[] buffer = Encoding.UTF8.GetBytes(formattedBoard);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
