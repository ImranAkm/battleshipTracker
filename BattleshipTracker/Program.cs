using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using BattleshipTracker.Interfaces;
using BattleshipTracker.Services;
using BattleshipTracker.Model;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace BattleshipTracker
{
    class Program
    {
        public static IConfigurationRoot configuration;
        public static async Task Main(string[] args)
        {
            
            // Initialize serilog logger
            Log.Logger = new LoggerConfiguration()
                 .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                 .MinimumLevel.Debug()
                 .Enrich.FromLogContext()
                 .CreateLogger();

            Log.Debug("Creating service collection");
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Create service provider
            Log.Debug("Building service provider");
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            while (true)
            {
                try
                {
                    try
                    {
                        var appsettings = configuration.Get<Appsettings>();
                        
                        Console.WriteLine("Starting Battle Ship Tracker Game");
                        
                        //Create board - will need number of players and board size
                        Console.Write($"Enter number of players (1 or 2) - Default will be {appsettings.PlayerCount} : ");
                        var inputPlayers = Console.ReadLine();
                        var playerCount = appsettings.PlayerCount;
                        if (string.IsNullOrEmpty(inputPlayers) || !int.TryParse(inputPlayers, out playerCount))
                        {
                            Console.WriteLine("Invalid number of players. The system will go ahead with default value of 1.");
                        }
                        else
                        {
                            playerCount = Convert.ToInt32(inputPlayers);
                        }

                        Console.Write($"Enter size of board - Example value = 10 will mean the board will be 10 x 10 - Default value will be {appsettings.BoardSize}: ");
                        var inputBoardSize = Console.ReadLine();
                        var boardSize = appsettings.BoardSize;
                        if(string.IsNullOrEmpty(inputBoardSize) || !int.TryParse(inputBoardSize,out boardSize))
                        {
                            Console.WriteLine("Invalid board size. The system will go ahead with default value of 10.");
                        }
                        else
                        {
                            boardSize = Convert.ToInt32(boardSize);
                        }

                        var battleShipService = serviceProvider.GetService<IBattleShipTrackerService>();

                        var boards = (List<Board>)battleShipService.CreateBoard(playerCount, boardSize);
                        if(boards != null && boards.Count > 0 )
                        {
                            Console.WriteLine("Board created!");
                            Console.WriteLine("All Board details");
                            for (int i = 1; i <= boards.Count; i++)
                            {
                                Console.WriteLine($" Board {i} -  {boards[i-1].BoardName}");
                            }

                            Console.Write("Select a board to start playing (Enter the number - Default will be first) : ");
                            var boardInput = Console.ReadLine();
                            var boardId = 1;
                            if(string.IsNullOrEmpty(boardInput) || !int.TryParse(boardInput, out boardId))
                            {
                                Console.WriteLine("Invalid board selection. The system has selected Board 1");
                            }
                            else
                            {
                                boardId = Convert.ToInt32(boardInput);
                            }

                            //Add Ship
                            Console.WriteLine("Add Ship on the board");
                            Console.Write("Enter Ship Name: ");
                            var shipName = Console.ReadLine();
                            if (string.IsNullOrEmpty(shipName))
                            {
                                shipName = appsettings.ShipName;
                                Console.WriteLine($"No name was entered. The system will use {shipName}");
                            }
                            Console.WriteLine("Place the battle ship on the board");

                            Console.WriteLine("Enter start position of ship: ");
                            Console.Write("X-Postion: ");
                            var startXInput = Console.ReadLine();
                            Console.Write("Y-Postion: ");
                            var startYInput = Console.ReadLine();

                            Console.WriteLine("Enter end position of ship: ");
                            Console.Write("X-Postion: ");
                            var endXInput = Console.ReadLine();
                            Console.Write("Y-Postion: ");
                            var endYInput = Console.ReadLine();

                            //validate the position
                            if(!int.TryParse(startXInput, out int startX) || !int.TryParse(startYInput, out int startY) ||
                                !int.TryParse(endXInput, out int endX) || !int.TryParse(endYInput, out int endY))
                            {
                                Console.WriteLine("Invalid number for the ship position");
                                return;
                            }

                            Console.Write("Enter direction of ship (Horizontal - 1 or Vertical - 2): Enter number -");
                            var directionInput = Console.ReadLine();
                            if (!int.TryParse(directionInput, out int direction) || !Enum.TryParse(typeof(Direction), directionInput, out _))
                            {
                                Console.WriteLine("Invalid direction");
                                return;
                            }

                            battleShipService.AddShip(shipName, boards[boardId - 1], new Position(startX, startY), new Position(endX, endY), (Direction)Enum.Parse(typeof(Direction), directionInput));

                            //Attack
                            Console.WriteLine("Lets attack now");
                            var endGame = false;
                            while (!endGame)
                            {
                                Console.WriteLine("Enter the attack position");
                                Console.Write("X-Postion: ");
                                var attackXInput = Console.ReadLine();
                                Console.Write("Y-Postion: ");
                                var attackYInput = Console.ReadLine();

                                if(!int.TryParse(attackYInput, out int attackY) || !int.TryParse(attackXInput, out int attackX))
                                {
                                    Console.WriteLine("Invalid attack position. Try again.");
                                }
                                else
                                {
                                    var result = battleShipService.Attack(boards[boardId - 1], new Position(attackX, attackY));
                                    Console.WriteLine(result);
                                    if(result == AttackResult.Miss)
                                    {
                                        Console.WriteLine("Try Again....");
                                    }
                                    endGame = result == AttackResult.Sunk;
                                    if (endGame)
                                    {
                                        Console.WriteLine("Game over");
                                    }
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex, "Error running service");
                        Console.WriteLine($"Error Occured - {ex.Message}");
                    }

                    Log.Debug("Ending service");
                }
                finally
                {
                    Log.CloseAndFlush();
                    Console.Write("Press any key to close window ...");
                    Console.Read();
                    Environment.Exit(0);
                }
            }
            
        }
        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Add logging
            serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder
                    .AddSerilog(dispose: true);
            }));
            serviceCollection.AddLogging();

            configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", true)
                 .Build();

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
            serviceCollection.Configure<Appsettings>(configuration);


            // Add Service
           serviceCollection.AddTransient<IBattleShipTrackerService, BattleShipTrackerService>();
        }
    }
}
