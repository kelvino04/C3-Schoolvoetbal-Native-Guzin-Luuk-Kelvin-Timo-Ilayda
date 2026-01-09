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

            if (data.Users.Any(u => u.Username == username))
            {
                Console.WriteLine("User already exists");
                Console.ReadKey();
                ShowAuthMenu();
                return;
            }

            data.Users.Add(new User
            {
                Username = username,
                Balance = 50,
                Role = "user"
            });

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

            var user = data.Users.FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                currentUser = new User
                {
                    Username = user.Username,
                    Balance = user.Balance,
                    Role = user.Role
                };
                ShowMainMenu();
                return;
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
            Console.WriteLine("3. View bets/results");
            Console.WriteLine("4. Place bet");
            Console.WriteLine("5. Logout");

            if (currentUser.Role == "admin")
                Console.WriteLine("6. Admin panel");

            Console.Write("Choose: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": ViewProfile(); break;
                case "2": ViewBalance(); break;
                case "3": ViewBetsAndResults().Wait(); break;
                case "4": PlaceBet().Wait(); break;
                case "5": Logout(); break;
                case "6": if (currentUser.Role == "admin") AdminPanel().Wait(); break;
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
            Console.WriteLine($"Balance: €{currentUser.Balance}");
            Console.ReadKey();
            ShowMainMenu();
        }

        // ---------- Unified View Bets + Results ----------
        static async Task ViewBetsAndResults()
        {
            Console.Clear();
            Console.WriteLine("Your Bets and Results:\n");

            if (BetHistory.Count == 0)
            {
                Console.WriteLine("No bets placed yet.");
            }
            else
            {
                foreach (var bet in BetHistory)
                {
                    string result = bet.Result ?? "Pending";
                    Console.WriteLine(
                        $"Match: {bet.MatchName}, Team: {bet.TeamName}, Amount: €{bet.Amount}, Result: {result}"
                    );
                }
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            ShowMainMenu();
        }

        // ---------- Place Bet ----------
        static async Task PlaceBet()
        {
            Console.Clear();
            var matches = await api.GetMatches();

            if (matches.Count == 0)
            {
                Console.WriteLine("No matches available.");
                Console.ReadKey();
                ShowMainMenu();
                return;
            }

            for (int i = 0; i < matches.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {matches[i].Team1.Name} vs {matches[i].Team2.Name}");
            }

            Console.Write("Choose match (0 = back): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice == 0)
            {
                ShowMainMenu();
                return;
            }

            var match = matches[choice - 1];

            Console.WriteLine($"1. {match.Team1.Name}");
            Console.WriteLine($"2. {match.Team2.Name}");
            Console.Write("Choose team: ");
            int teamChoice = int.Parse(Console.ReadLine());

            var chosenTeam = teamChoice == 1 ? match.Team1 : match.Team2;

            Console.Write("Bet amount: ");
            int amount = int.Parse(Console.ReadLine());

            if (amount > currentUser.Balance)
            {
                Console.WriteLine("Not enough balance.");
                Console.ReadKey();
                ShowMainMenu();
                return;
            }

            currentUser.Balance -= amount;

            BetHistory.Add(new Bet
            {
                MatchId = match.Id,
                TeamId = chosenTeam.Id,
                MatchName = $"{match.Team1.Name} vs {match.Team2.Name}",
                TeamName = chosenTeam.Name,
                Amount = amount,
                Result = null // Pending until admin updates
            });

            Console.WriteLine("Bet placed successfully!");
            Console.ReadKey();
            ShowMainMenu();
        }

        // ---------- Admin Panel ----------
        static async Task AdminPanel()
        {
            Console.Clear();
            Console.WriteLine("Admin panel");
            Console.WriteLine("1. Update results from Laravel API");
            Console.WriteLine("2. Back");
            Console.Write("Choose: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                await UpdateResults();
            }

            ShowMainMenu();
        }

        // ---------- Admin: Fetch results ----------
        static async Task UpdateResults()
        {
            Console.Clear();
            Console.WriteLine("Fetching latest results from Laravel...");

            var results = await api.GetResults();

            foreach (var bet in BetHistory)
            {
                var matchResult = results.FirstOrDefault(r => r.Match_Id == bet.MatchId);

                if (matchResult == null || matchResult.Winner_Team_Id == 0)
                    bet.Result = "Pending";
                else if (matchResult.Winner_Team_Id == bet.TeamId)
                    bet.Result = "Won";
                else
                    bet.Result = "Lost";
            }

            Console.WriteLine("Results updated for all bets!");
            Console.ReadKey();
        }

        // ---------- Logout ----------
        static void Logout()
        {
            currentUser = null;
            ShowAuthMenu();
        }
    }

    // ---------- Bet ----------
    class Bet
    {
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public string MatchName { get; set; }
        public string TeamName { get; set; }
        public int Amount { get; set; }
        public string Result { get; set; } // Pending / Won / Lost
    }
}
