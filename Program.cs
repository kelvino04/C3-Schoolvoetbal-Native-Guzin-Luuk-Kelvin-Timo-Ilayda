using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using praC3.Models;
using praC3.Services;

namespace praC3
{
    class Program
    {
        static AppData data;
        static User currentUser;
        static ApiService api = new ApiService();
        static List<Bet> BetHistory = new List<Bet>();

        static void Main()
        {
            Console.Title = "Betting System";
            data = DataService.Load();
            ShowAuthMenu();
        }

        static int GetValidChoice(int min, int max, string errorMessage = "Invalid choice!")
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= min && choice <= max)
                    return choice;

                Console.WriteLine(errorMessage);
                Console.Write("Please enter a valid choice: ");
            }
        }

        static string GetValidString(string prompt, bool allowSpecialChars = false, int minLength = 1, int maxLength = 50)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input cannot be empty!");
                    continue;
                }

                if (input.Length < minLength || input.Length > maxLength)
                {
                    Console.WriteLine($"Input must be between {minLength} and {maxLength} characters!");
                    continue;
                }

                if (!allowSpecialChars && input.Any(c => !char.IsLetterOrDigit(c) && c != ' ' && c != '_'))
                {
                    Console.WriteLine("Input contains invalid characters! Use only letters, numbers, spaces, or underscores.");
                    continue;
                }

                return input;
            }
        }

        static int GetValidAmount(int min = 1, int max = 10000)
        {
            while (true)
            {
                Console.Write($"Enter amount ({min}-{max}): ");
                if (int.TryParse(Console.ReadLine(), out int amount) && amount >= min && amount <= max)
                    return amount;

                Console.WriteLine($"Invalid amount! Please enter a number between {min} and {max}.");
            }
        }

        static void ShowAuthMenu()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== WELCOME TO BETTING SYSTEM ===");
                Console.WriteLine("\n1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Exit");
                Console.WriteLine("\n" + new string('=', 40));
                Console.Write("\nChoose option (1-3): ");

                int choice = GetValidChoice(1, 3, "Please enter 1, 2, or 3!");

                switch (choice)
                {
                    case 1: Login(); break;
                    case 2: Register(); break;
                    case 3: Environment.Exit(0); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
                ShowAuthMenu();
            }
        }

        static void Register()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== REGISTER NEW ACCOUNT ===");
                Console.WriteLine(new string('=', 30));

                string username = GetValidString("Enter username: ", false, 3, 20);

                if (data.Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("\nUsername already exists!");
                    Console.WriteLine("Press any key to try again...");
                    Console.ReadKey();
                    ShowAuthMenu();
                    return;
                }

                var newUser = new User
                {
                    Username = username,
                    Balance = 100,
                    Role = "user"
                };

                data.Users.Add(newUser);
                DataService.Save(data);

                Console.WriteLine($"\nAccount created successfully!");
                Console.WriteLine($"Welcome, {username}! You have {newUser.Balance} credits.");
                Console.WriteLine("\nPress any key to continue to login...");
                Console.ReadKey();
                Login();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                Console.ReadKey();
                ShowAuthMenu();
            }
        }

        static void Login()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== LOGIN ===");
                Console.WriteLine(new string('=', 30));

                string username = GetValidString("Username: ", true);

                if (username.ToLower() == "admin")
                {
                    Console.Write("Admin password: ");
                    string password = Console.ReadLine();

                    if (password == "admin123")
                    {
                        currentUser = new User
                        {
                            Username = "admin",
                            Balance = 0,
                            Role = "admin"
                        };
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid admin password!");
                        Console.ReadKey();
                        ShowAuthMenu();
                        return;
                    }
                }
                else
                {
                    var user = data.Users.FirstOrDefault(u =>
                        u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

                    if (user == null)
                    {
                        Console.WriteLine("\nUser not found!");
                        Console.WriteLine("Press any key to try again...");
                        Console.ReadKey();
                        ShowAuthMenu();
                        return;
                    }

                    currentUser = new User
                    {
                        Username = user.Username,
                        Balance = user.Balance,
                        Role = user.Role
                    };
                }

                data = DataService.Load();

                BetHistory = data.BetHistory
                    .Where(b => currentUser.Role == "admin" ||
                           b.Username.Equals(currentUser.Username, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                Console.WriteLine($"\nWelcome, {currentUser.Username}!");
                ShowMainMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                Console.ReadKey();
                ShowAuthMenu();
            }
        }

        static void ShowMainMenu()
        {
            try
            {
                Console.Clear();
                Console.WriteLine($"=== MAIN MENU ===");
                Console.WriteLine($"Logged in as: {currentUser.Username} ({currentUser.Role})");
                Console.WriteLine(new string('=', 40));
                Console.WriteLine("\n1. View Profile");
                Console.WriteLine("2. View Balance");
                Console.WriteLine("3. View Bets/Results");
                Console.WriteLine("4. Place Bet");
                Console.WriteLine("5. Logout");

                if (currentUser.Role == "admin")
                    Console.WriteLine("6. Admin Panel");

                Console.WriteLine("\n" + new string('-', 40));
                Console.Write("\nChoose option (1-6): ");

                int choice = GetValidChoice(1, currentUser.Role == "admin" ? 6 : 5);

                switch (choice)
                {
                    case 1: ViewProfile(); break;
                    case 2: ViewBalance(); break;
                    case 3: ViewBetsOrResults(); break;
                    case 4: PlaceBet().Wait(); break;
                    case 5: Logout(); break;
                    case 6: if (currentUser.Role == "admin") AdminPanel().Wait(); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Menu error: {ex.Message}");
                Console.ReadKey();
                ShowMainMenu();
            }
        }

        static void ViewProfile()
        {
            Console.Clear();
            Console.WriteLine("=== YOUR PROFILE ===");
            Console.WriteLine(new string('=', 30));
            Console.WriteLine($"Username: {currentUser.Username}");
            Console.WriteLine($"Role: {currentUser.Role}");
            Console.WriteLine($"Balance: {currentUser.Balance} credits");

            var userStats = data.BetHistory
                .Where(b => b.Username == currentUser.Username)
                .GroupBy(b => b.Result)
                .ToDictionary(g => g.Key ?? "Pending", g => g.Count());

            if (userStats.Any())
            {
                Console.WriteLine("\n=== BETTING STATISTICS ===");
                foreach (var stat in userStats)
                {
                    Console.WriteLine($"{stat.Key}: {stat.Value}");
                }
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            ShowMainMenu();
        }

        static void ViewBalance()
        {
            Console.Clear();
            Console.WriteLine("=== YOUR BALANCE ===");
            Console.WriteLine(new string('=', 30));
            Console.WriteLine($"Available: {currentUser.Balance} credits");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            ShowMainMenu();
        }

        static void ViewBetsOrResults()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== YOUR BETS & RESULTS ===");
                Console.WriteLine(new string('=', 40));

                data = DataService.Load();
                BetHistory = data.BetHistory
                    .Where(b => currentUser.Role == "admin" ||
                           b.Username.Equals(currentUser.Username, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!BetHistory.Any())
                {
                    Console.WriteLine("\nNo bets placed yet.");
                }
                else
                {
                    int pending = 0, won = 0, lost = 0;

                    foreach (var bet in BetHistory.OrderByDescending(b => b.MatchId))
                    {
                        string resultText = bet.Result ?? "Pending";
                        Console.WriteLine($"\n[Match #{bet.MatchId}]");
                        Console.WriteLine($"Match: {bet.MatchName}");
                        Console.WriteLine($"Team: {bet.TeamName}");
                        Console.WriteLine($"Amount: €{bet.Amount}");

                        if (resultText == "Won")
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            won++;
                        }
                        else if (resultText == "Lost")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            lost++;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            pending++;
                        }

                        Console.WriteLine($"Status: {resultText}");
                        Console.ResetColor();
                        Console.WriteLine(new string('-', 40));
                    }

                    Console.WriteLine($"\nSummary: {won} won, {lost} lost, {pending} pending");
                }

                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading bets: {ex.Message}");
                Console.ReadKey();
            }

            ShowMainMenu();
        }

        static async Task PlaceBet()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== PLACE A BET ===");
                Console.WriteLine(new string('=', 30));

                if (currentUser.Balance <= 0)
                {
                    Console.WriteLine("\nInsufficient balance! You need at least 1 credit to bet.");
                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                    ShowMainMenu();
                    return;
                }

                Console.WriteLine("\nFetching available matches...");
                var matches = await api.GetMatches();

                if (!matches.Any())
                {
                    Console.WriteLine("\nNo matches available for betting at the moment.");
                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                    ShowMainMenu();
                    return;
                }

                Console.WriteLine("\n=== AVAILABLE MATCHES ===");
                for (int i = 0; i < matches.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {matches[i].Team1.Name} vs {matches[i].Team2.Name}");
                }

                Console.WriteLine($"0. Cancel and return to menu");
                Console.WriteLine(new string('-', 40));
                Console.Write($"\nSelect match (1-{matches.Count}): ");

                int matchChoice = GetValidChoice(0, matches.Count, $"Please enter 0-{matches.Count}!");

                if (matchChoice == 0)
                {
                    ShowMainMenu();
                    return;
                }

                var selectedMatch = matches[matchChoice - 1];

                Console.WriteLine($"\n=== SELECT TEAM ===");
                Console.WriteLine($"1. {selectedMatch.Team1.Name}");
                Console.WriteLine($"2. {selectedMatch.Team2.Name}");
                Console.WriteLine($"0. Cancel");
                Console.WriteLine(new string('-', 40));
                Console.Write("\nChoose team (1 or 2): ");

                int teamChoice = GetValidChoice(0, 2, "Please enter 1, 2, or 0!");

                if (teamChoice == 0)
                {
                    ShowMainMenu();
                    return;
                }

                var chosenTeam = teamChoice == 1 ? selectedMatch.Team1 : selectedMatch.Team2;

                Console.WriteLine($"\n=== ENTER BET AMOUNT ===");
                Console.WriteLine($"Available balance: {currentUser.Balance} credits");
                Console.WriteLine($"Selected: {chosenTeam.Name}");
                Console.WriteLine(new string('-', 40));

                int betAmount = GetValidAmount(1, currentUser.Balance);

                Console.WriteLine($"\n=== CONFIRM BET ===");
                Console.WriteLine($"Match: {selectedMatch.Team1.Name} vs {selectedMatch.Team2.Name}");
                Console.WriteLine($"Your pick: {chosenTeam.Name}");
                Console.WriteLine($"Amount: {betAmount} credits");
                Console.WriteLine($"Potential win: {betAmount * 2} credits");
                Console.WriteLine(new string('-', 40));
                Console.Write("\nConfirm bet? (Y/N): ");

                string confirm = Console.ReadLine()?.Trim().ToUpper();
                if (confirm != "Y" && confirm != "YES")
                {
                    Console.WriteLine("\nBet cancelled.");
                    Console.ReadKey();
                    ShowMainMenu();
                    return;
                }

                currentUser.Balance -= betAmount;

                var newBet = new Bet
                {
                    Username = currentUser.Username,
                    MatchId = selectedMatch.Match_Id,
                    TeamId = chosenTeam.Id,
                    MatchName = $"{selectedMatch.Team1.Name} vs {selectedMatch.Team2.Name}",
                    TeamName = chosenTeam.Name,
                    Amount = betAmount,
                    Result = null,
                    PlacedAt = DateTime.Now
                };

                BetHistory.Add(newBet);
                data.BetHistory.Add(newBet);

                var userInData = data.Users.FirstOrDefault(u =>
                    u.Username.Equals(currentUser.Username, StringComparison.OrdinalIgnoreCase));
                if (userInData != null)
                {
                    userInData.Balance = currentUser.Balance;
                }

                DataService.Save(data);

                Console.WriteLine($"\nBet placed successfully!");
                Console.WriteLine($"Remaining balance: {currentUser.Balance} credits");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError placing bet: {ex.Message}");
                Console.ReadKey();
            }

            ShowMainMenu();
        }

        static async Task AdminPanel()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== ADMIN PANEL ===");
                Console.WriteLine(new string('=', 30));
                Console.WriteLine("\n1. Fetch match results from Laravel API");
                Console.WriteLine("2. View all user bets");
                Console.WriteLine("3. View system statistics");
                Console.WriteLine("4. Return to main menu");
                Console.WriteLine(new string('-', 40));
                Console.Write("\nChoose option (1-4): ");

                int choice = GetValidChoice(1, 4);

                switch (choice)
                {
                    case 1: await FetchAndProcessResults(); break;
                    case 2: ViewAllBets(); break;
                    case 3: ViewSystemStats(); break;
                    case 4: ShowMainMenu(); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Admin panel error: {ex.Message}");
                Console.ReadKey();
                ShowMainMenu();
            }
        }

        static async Task FetchAndProcessResults()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== FETCHING RESULTS FROM LARAVEL ===");
                Console.WriteLine(new string('=', 40));

                Console.WriteLine("\nConnecting to API...");
                var results = await api.GetResults();

                if (!results.Any())
                {
                    Console.WriteLine("\nNo results found or API is unavailable.");
                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                    AdminPanel().Wait();
                    return;
                }

                Console.WriteLine($"\nRetrieved {results.Count} match result(s)");
                Console.WriteLine(new string('-', 40));

                int updatedBets = 0;
                int pendingBets = 0;

                foreach (var result in results)
                {
                    Console.WriteLine($"\nProcessing Match #{result.Match_Id}:");
                    Console.WriteLine($"{result.Team1} vs {result.Team2}");
                    Console.WriteLine($"Score: {result.Score1}-{result.Score2}");

                    var winnerId = result.Winner_Team_Id;
                    if (winnerId == 0)
                    {
                        Console.WriteLine("Result: Draw (no winner)");
                    }
                    else
                    {
                        Console.WriteLine($"Winner Team ID: {winnerId}");
                    }

                    var matchBets = data.BetHistory
                        .Where(b => b.MatchId == result.Match_Id && b.Result == null)
                        .ToList();

                    if (matchBets.Any())
                    {
                        Console.WriteLine($"Found {matchBets.Count} pending bet(s) for this match");

                        foreach (var bet in matchBets)
                        {
                            if (winnerId == 0)
                            {
                                bet.Result = "Draw";
                                var user = data.Users.FirstOrDefault(u => u.Username == bet.Username);
                                if (user != null)
                                {
                                    user.Balance += bet.Amount;
                                    Console.WriteLine($"  {bet.Username}: Draw - bet returned");
                                }
                            }
                            else if (bet.TeamId == winnerId)
                            {
                                bet.Result = "Won";
                                var user = data.Users.FirstOrDefault(u => u.Username == bet.Username);
                                if (user != null && user.Username != "admin")
                                {
                                    int winnings = bet.Amount * 2;
                                    user.Balance += winnings;
                                    Console.WriteLine($"  {bet.Username}: WON - +{winnings} credits");
                                }
                                updatedBets++;
                            }
                            else
                            {
                                bet.Result = "Lost";
                                Console.WriteLine($"  {bet.Username}: Lost");
                                updatedBets++;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No pending bets for this match");
                        pendingBets++;
                    }
                }

                DataService.Save(data);

                if (currentUser.Role != "admin")
                {
                    BetHistory = data.BetHistory
                        .Where(b => b.Username == currentUser.Username)
                        .ToList();
                }

                Console.WriteLine("\n" + new string('-', 40));
                Console.WriteLine($"Processing complete!");
                Console.WriteLine($"Updated bets: {updatedBets}");
                Console.WriteLine($"Matches with no bets: {pendingBets}");

                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError fetching results: {ex.Message}");
                Console.WriteLine("Make sure Laravel API is running at http://127.0.0.1:8000");
                Console.ReadKey();
            }

            AdminPanel().Wait();
        }

        static void ViewAllBets()
        {
            Console.Clear();
            Console.WriteLine("=== ALL USER BETS ===");
            Console.WriteLine(new string('=', 30));

            var allBets = data.BetHistory
                .OrderByDescending(b => b.MatchId)
                .ThenBy(b => b.Username)
                .ToList();

            if (!allBets.Any())
            {
                Console.WriteLine("\nNo bets placed in the system yet.");
            }
            else
            {
                var groupedBets = allBets.GroupBy(b => b.MatchId);

                foreach (var matchGroup in groupedBets)
                {
                    Console.WriteLine($"\n[Match #{matchGroup.Key}]");
                    Console.WriteLine($"Match: {matchGroup.First().MatchName}");
                    Console.WriteLine(new string('-', 40));

                    foreach (var bet in matchGroup)
                    {
                        Console.WriteLine($"User: {bet.Username}");
                        Console.WriteLine($"Team: {bet.TeamName}");
                        Console.WriteLine($"Amount: {bet.Amount} credits");
                        Console.WriteLine($"Result: {bet.Result ?? "Pending"}");
                        Console.WriteLine();
                    }
                }
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            AdminPanel().Wait();
        }

        static void ViewSystemStats()
        {
            Console.Clear();
            Console.WriteLine("=== SYSTEM STATISTICS ===");
            Console.WriteLine(new string('=', 30));

            int totalUsers = data.Users.Count;
            int totalBets = data.BetHistory.Count;
            decimal totalBetAmount = data.BetHistory.Sum(b => b.Amount);

            Console.WriteLine($"\nTotal Users: {totalUsers}");
            Console.WriteLine($"Total Bets Placed: {totalBets}");
            Console.WriteLine($"Total Bet Amount: {totalBetAmount} credits");

            if (totalBets > 0)
            {
                var statusGroups = data.BetHistory
                    .GroupBy(b => b.Result ?? "Pending")
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .OrderByDescending(g => g.Count);

                Console.WriteLine("\n=== BET STATUS DISTRIBUTION ===");
                foreach (var group in statusGroups)
                {
                    Console.WriteLine($"{group.Status}: {group.Count} bets");
                }

                var userActivity = data.BetHistory
                    .GroupBy(b => b.Username)
                    .Select(g => new { User = g.Key, Bets = g.Count(), Total = g.Sum(b => b.Amount) })
                    .OrderByDescending(g => g.Bets)
                    .Take(5);

                Console.WriteLine("\n=== TOP 5 ACTIVE USERS ===");
                foreach (var user in userActivity)
                {
                    Console.WriteLine($"{user.User}: {user.Bets} bets, {user.Total} credits total");
                }
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            AdminPanel().Wait();
        }

        static void Logout()
        {
            Console.Clear();
            Console.WriteLine($"Goodbye, {currentUser.Username}!");
            Console.WriteLine("Logging out...");

            currentUser = null;
            BetHistory.Clear();

            System.Threading.Thread.Sleep(1500);
            ShowAuthMenu();
        }
    }
}