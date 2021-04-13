using System;
using System.Threading;

namespace SpaceShooter
{
    class Program
    {
        static bool playing = true;
        static char[,] world;

        static float timer = 0;
        static float shotTimer = 0;
        static float shotDelay = .5f;

        static int width = 100;
        static int height = 20;
        static int playerPosition;
        static int playerMove = 0;
        static bool state = true;
        static string gameEndMessage = "";

        static int totalEnemies = 0;

        static void Main(string[] args)
        {
            StartGame();

            Thread inputThread = new Thread(Input);
            inputThread.Start();

            while (playing)
            {
                UpdateWorld();
                timer += .1f;
                shotTimer += .1f;
                Draw();
                playerPosition += playerMove;
                playerMove = 0;
                Thread.Sleep(1);
            }
            Console.Clear();
            Console.WriteLine(gameEndMessage);
            Console.ReadLine();
        }

        static void StartGame()
        {
            Console.WindowWidth = width;
            Console.WindowHeight = height;
            Console.BufferWidth = width;
            Console.BufferHeight = height;
            Console.CursorVisible = false;

            playerPosition = 50;
            world = new char[width, height];

            for (int x = 0; x < world.GetLength(0); x++)
            {
                for (int y = 0; y < world.GetLength(1); y++)
                {
                    world[x, y] = ' ';
                    if (y == 1 && x % 10 == 5)
                    {
                        world[x, y] = 'V';
                        totalEnemies++;
                    }
                }
            }
        }

        static void Input()
        {
            while (playing)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo k = Console.ReadKey(true);
                    if (k.Key == ConsoleKey.D)
                    {
                        playerMove = 1;
                    }
                    else if (k.Key == ConsoleKey.A)
                    {
                        playerMove = -1;
                    }
                    if (playerPosition < 0) playerPosition = 0;
                    if (playerPosition >= width) playerPosition = width - 1;
                    else if (k.Key == ConsoleKey.Spacebar)
                    {
                        Shoot();
                    }
                }
            }
        }

        static void Shoot()
        {
            if (shotTimer < shotDelay) return;
            Console.Beep(1000, 20);

            world[playerPosition, height - 2] = '|';
            shotTimer = 0;
        }

        static void Draw()
        {
            Console.Clear();
            for (int y = 0; y < world.GetLength(1); y++)
            {
                for (int x = 0; x < world.GetLength(0); x++)
                {
                    if (world[x, y] != ' ')
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(world[x, y]);
                    }
                }
            }
        }

        static void UpdateWorld()
        {
            for (int y = world.GetLength(1) - 1; y > -1; y--)
            {
                for (int x = 0; x < world.GetLength(0); x++)
                {
                    // Clear player
                    if (world[x, y] == 'A')
                    {
                        if (InWorld(x + 1, y))
                        {
                            if (world[x + 1, y] == 'V')
                            {
                                gameEndMessage = "You died!";
                                playing = false;
                            }
                        }
                        world[x, y] = ' ';
                    }

                    // IF enemy found
                    if (world[x, y] == 'V')
                    {
                        if (InWorld(x - 1, y))
                        {
                            // IF target move has a shot
                            if (world[x - 1, y] == '|' || world[x - 1, y] == '!')
                            {
                                world[x - 1, y] = '@';
                                EnemyHit();
                            }
                            else world[x - 1, y] = 'V';
                        }
                        else
                        {
                            // IF target move has a shot
                            if (world[width - 1, y + 1] == '|' || world[width - 1, y + 1] == '!')
                            {
                                world[width - 1, y + 1] = '@';
                                EnemyHit();
                            }
                            else world[width - 1, y + 1] = 'V';
                        }
                        world[x, y] = ' ';
                    }

                    // FX
                    if (world[x, y] == '-') world[x, y] = ' ';

                    if (world[x, y] == '-') world[x, y] = '.';

                    if (world[x, y] == '*') world[x, y] = '-';

                    if (world[x, y] == 'a') world[x, y] = '*';

                    if (world[x, y] == '@') world[x, y] = 'a';


                    // Check if shot will hit anything
                    if (world[x, y] == '|' || world[x, y] == '!')
                    {
                        if (InWorld(x, y - 1))
                        {
                            // IF above is enemy
                            if (world[x, y - 1] == 'V')
                            {
                                EnemyHit();
                                world[x, y - 1] = '@';
                                world[x, y] = ' ';
                            }
                        }
                    }

                    // Move shot forwards based on current state (current state can only move one of two representations)
                    // Changes to other rep to not be moved when found again.
                    if (world[x, y] == '|' && state || world[x, y] == '!' && !state)
                    {
                        if (InWorld(x, y - 1))
                        {
                            world[x, y - 1] = state ? '!' : '|';
                        }
                        world[x, y] = ' ';
                    }
                }
            }
            state = !state;

            //Player
            world[playerPosition, height - 1] = 'A';
        }

        static bool InWorld(int x, int y)
        {
            if (x < 0) return false;
            if (x >= world.GetLength(0)) return false;
            if (y < 0) return false;
            if (y >= world.GetLength(1)) return false;
            return true;
        }

        static void EnemyHit()
        {
            Console.Beep(500, 50);
            totalEnemies--;
            if (totalEnemies <= 0) playing = false;
            gameEndMessage = "Congratulations, you won!";
        }
    }
}
