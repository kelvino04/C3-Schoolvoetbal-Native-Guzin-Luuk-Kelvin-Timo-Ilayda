using System;

namespace praC3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ======================================
            // FakeGokApp - PRA C3 project
            // ======================================
            // Projectdoel:
            // - Simuleer een gokspelletje (raad een nummer)
            // - Sla de resultaten op in een database zodat Laravel ze kan gebruiken
            //
            // Functionaliteiten (te implementeren):
            // 1. Vraag de gebruiker om een naam
            // 2. Laat de gebruiker een getal raden (bijvoorbeeld 1 t/m 5)
            // 3. Genereer een random getal als “winnende nummer”
            // 4. Vergelijk de gok van de gebruiker met het random nummer
            // 5. Geef een resultaat terug: "win" of "lose"
            // 6. (Later) Sla het resultaat op in de database
            //
            // Database:
            // - Tabel 'bets' met kolommen: id, username, guess, random_number, result, created_at
            //
            // Toekomstige uitbreidingen:
            // - Laravel kan de database uitlezen om resultaten te tonen
            // - Eventueel meerdere spelrondes
            //
            // ================================
            // Huidige status:
            // - Alleen een skeleton Program.cs met namespace en Main
            // - Nog geen echte functionaliteit geïmplementeerd
            //
            // ================================
            Console.WriteLine("Hello, World!"); // Placeholder
        }
    }
}