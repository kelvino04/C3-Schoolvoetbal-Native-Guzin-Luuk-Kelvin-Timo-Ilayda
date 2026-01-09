using System;
using System.Collections.Generic;
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
        static List<Bet> BetHistory = new List<Bet>(); // Stores all bets

        static void Main()
        {
            data = Services.DataService.Load();
            ShowAuthMenu();
        }

        // ---------- Auth Menu ----------
        static void ShowAuthMenu()
        {
            Console.Clear();
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.Write("Choose: ");
            string choice = Console.ReadLine();

            if (choice == "1") Login();
            else if (choice == "2") Register();
            else ShowAuthMenu();
        }

        // ---------- Register ----------
        static void Register()
        {
            Console.Clear();
            Console.Write("Username: ");
            string username = Console.ReadLine();

            foreach (var u in data.Users)
            {
                if (u.Username == username)
                {
                    Console.WriteLine("User already exists");
                    Console.ReadKey();
                    ShowAuthMenu();
                    return;
                }
            }

            User user = new User
            {
                Username = username,
                Balance = 50,
                Role = "user"
            };

            data.Users.Add(user);
            Services.DataService.Save(data);

            Console.WriteLine("Account created");
            Console.ReadKey();
            ShowAuthMenu();
        }

        // ---------- Login ----------
        static void Login()
        {
            Console.Clear();
            Console.Write("Username: ");
            string username = Console.ReadLine();

            foreach (var u in data.Users)
            {
                if (u.Username == username)
                {
                    currentUser = new User
                    {
                        Username = u.Username,
                        Balance = u.Balance,
                        Role = u.Role
                    };
                    ShowMainMenu();
                    return;
                }
            }

            if (username == "admin")
            {
                currentUser = new User
                {
                    Username = "admin",
                    Balance = 0,
                    Role = "admin"
                };
                ShowMainMenu();
                return;
            }

            Console.WriteLine("User not found");
            Console.ReadKey();
            ShowAuthMenu();
        }

        // ---------- Main Menu ----------
        static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine($"Logged in as: {currentUser.Username}");
            Console.WriteLine();

            Console.WriteLine("1. View profile");
            Console.WriteLine("2. View balance");
            Console.WriteLine("3. View bets");
            Console.WriteLine("4. Place bet");
            Console.WriteLine("5. View results");
            Console.WriteLine("6. Logout");

            if (currentUser.Role == "admin")
                Console.WriteLine("7. Admin panel");

            Console.Write("Choose: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": ViewProfile(); break;
                case "2": ViewBalance(); break;
                case "3": ViewBets(); break;
                case "4": PlaceBet().Wait(); break;
                case "5": ViewResults().Wait(); break;
                case "6": Logout(); break;
                case "7": if (currentUser.Role == "admin") AdminPanel(); break;
                default: ShowMainMenu(); break;
            }
        }

        // ---------- Profile ----------
        static void ViewProfile()
        {
            Console.Clear();
            Console.WriteLine("Profile");
            Console.WriteLine($"Username: {currentUser.Username}");
            Console.WriteLine($"Role: {currentUser.Role}");
            Console.ReadKey();
            ShowMainMenu();
        }

        // ---------- Balance ----------
        static void ViewBalance()
        {
            Console.Clear();
            Console.WriteLine($"Balance: {currentUser.Balance} 4S-dollars");
            Console.ReadKey();
            ShowMainMenu();
        }

        // ---------- Bets ----------
        static void ViewBets()
        {
            Console.Clear();
            Console.WriteLine("Your Bet History:");
            if (BetHistory.Count == 0)
            {
                Console.WriteLine("No bets placed yet.");
            }
            else
            {
                foreach (var b in BetHistory)
                {
                    string resultText = b.Result == null ? "Pending" : b.Result;
                    Console.WriteLine($"Match: {b.MatchName}, Team: {b.TeamName}, Amount: €{b.Amount}, Result: {resultText}");
                }
            }
            Console.ReadKey();
            ShowMainMenu();
        }

        // ---------- Place Bet ----------
        static async Task PlaceBet()
        {
            Console.Clear();
            Console.WriteLine("Loading matches...");
            var matches = await api.GetMatches();

            if (matches.Count == 0)
            {
                Console.WriteLine("No matches available yet.");
                Console.ReadKey();
                ShowMainMenu();
                return;
            }

            // Display matches
            for (int i = 0; i < matches.Count; i++)
            {
                var m = matches[i];
                Console.WriteLine($"{i + 1}. {m.Team1.Name} vs {m.Team2.Name} - {m.StartTime}");
            }

            Console.Write("Choose match to bet on (0 to go back): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > matches.Count)
            {
                ShowMainMenu();
                return;
            }
            if (choice == 0) { ShowMainMenu(); return; }

            var match = matches[choice - 1];
            Console.WriteLine($"1. {match.Team1.Name}");
            Console.WriteLine($"2. {match.Team2.Name}");
            Console.Write("Choose team to bet on: ");
            int teamChoice = int.Parse(Console.ReadLine());
            string chosenTeamName = teamChoice == 1 ? match.Team1.Name : match.Team2.Name;

            Console.Write("Bet amount: ");
            int amount = int.Parse(Console.ReadLine());

            if (amount > currentUser.Balance)
            {
                Console.WriteLine("Not enough balance.");
                Console.ReadKey();
                ShowMainMenu();
                return;
            }

            // Simulate placing bet
            Console.WriteLine($"Match chosen: {match.Team1.Name} vs {match.Team2.Name} - {match.StartTime}");
            Console.WriteLine($"Team chosen: {chosenTeamName}");
            Console.WriteLine($"Bet amount: {amount} euros");
            Console.WriteLine("Bet placed (simulation)!");


            currentUser.Balance -= amount;

            // Save bet in history
            BetHistory.Add(new Bet
            {
                MatchId = match.Id,
                MatchName = $"{match.Team1.Name} vs {match.Team2.Name}",
                TeamName = chosenTeamName,
                Amount = amount,
                Result = null // Pending
            });

            Console.ReadKey();
            ShowMainMenu();
        }

        // ---------- View Results ----------
        static async Task ViewResults()
        {
            Console.Clear();
            var results = await api.GetResults(); // simulate finished matches

            if (results.Count == 0)
            {
                Console.WriteLine("Awaiting results for all matches.");
            }
            else
            {
                foreach (var r in results)
                {
                    Console.WriteLine($"{r.Team1.Name} {r.Score} {r.Team2.Name}");
                }
            }

            // Show bet history
            Console.WriteLine("\nYour Bet History:");
            if (BetHistory.Count == 0)
            {
                Console.WriteLine("No bets placed yet.");
            }
            else
            {
                foreach (var b in BetHistory)
                {
                    string resultText = b.Result == null ? "Pending" : b.Result;
                    Console.WriteLine($"Match: {b.MatchName}, Team: {b.TeamName}, Amount: {b.Amount} euros, Result: {resultText}");
                }
            }

            Console.ReadKey();
            ShowMainMenu();
        }

        // ---------- Admin Panel ----------
        static void AdminPanel()
        {
            Console.Clear();
            Console.WriteLine("Admin panel");
            Console.WriteLine("1. Process results");
            Console.WriteLine("2. Back");
            Console.Write("Choose: ");

            string choice = Console.ReadLine();
            if (choice == "1")
            {
                Console.WriteLine("Results processed (demo)");
                Console.ReadKey();
            }

            ShowMainMenu();
        }

        // ---------- Logout ----------
        static void Logout()
        {
            currentUser = null;
            ShowAuthMenu();
        }
    }

    // ---------- Bet class ----------
    class Bet
    {
        public int MatchId { get; set; }
        public string MatchName { get; set; }
        public string TeamName { get; set; }
        public int Amount { get; set; }
        public string Result { get; set; } // Pending / Win / Lose
    }
}
