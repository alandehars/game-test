using System;
using System.Collections.Generic;
using System.Linq;

namespace OuterRimRun
{
    internal sealed class Sabrina
    {
        public int Health = 12;
        public int Credits = 430;
        public int Heat = 1;
        public int Time = 19 * 60 + 42;
        public readonly List<string> Gear = new List<string> { "Blaster pistol", "Holdout derringer", "Marked sabacc deck", "Ship: Lucky Comet (impounded)" };
        public readonly Random Dice = new Random();

        public bool Roll(int bonus, int target)
        {
            int face = Dice.Next(1, 21);
            int total = face + bonus;
            Console.WriteLine("(d20 " + face + " + " + bonus + " = " + total + " vs " + target + ")");
            return face == 20 || total >= target;
        }
        public void Advance(int minutes) { Time += minutes; }
        public string Clock { get { return String.Format("{0:00}:{1:00}", (Time / 60) % 24, Time % 60); } }
    }

    internal static class Program
    {
        private static readonly Sabrina Hero = new Sabrina();
        private static bool fled = false;
        private static bool clue = false;
        private static bool debtPaid = false;

        private static void Main()
        {
            Console.Title = "Outer Rim Run — Sabrina's Story";
            Console.WriteLine("OUTER RIM RUN | A small, original d20-inspired smuggler adventure\n");
            Scene();
            while (Hero.Health > 0 && !fled)
            {
                Choices();
                Console.Write("Choose 1–5, or type sheet / quit: ");
                string input = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
                if (input == "quit") break;
                if (input == "sheet") { Sheet(); continue; }
                int n;
                if (!Int32.TryParse(input, out n) || n < 1 || n > 5) { Console.WriteLine("The neon sign buzzes. That is not one of the offered moves.\n"); continue; }
                Hero.Advance(2 + Hero.Dice.Next(2, 7));
                Resolve(n);
                if (Hero.Health <= 0) Console.WriteLine("\nSabrina falls beneath the docking gantry. The Empire takes the ship; Nar Shaddaa keeps the rest of the story. GAME OVER.");
                else if (debtPaid && clue) { Console.WriteLine("\nWith the ledger and a clean berth chit, the Lucky Comet's clamps release. Somewhere above, a Star Destroyer turns toward the moon. TO BE CONTINUED."); fled = true; }
                else Console.WriteLine("\n" + Hero.Clock + " | Health " + Hero.Health + "/12 | Credits " + Hero.Credits + " | Imperial heat " + Hero.Heat + "\n");
            }
            if (!fled && Hero.Health > 0) Console.WriteLine("Campaign paused. Your progress lasts for this session.");
        }

        private static void Scene()
        {
            Console.WriteLine("Nar Shaddaa, Smuggler's Moon — Docking Ring 94, local time " + Hero.Clock + ". Acid rain rattles the transparisteel canopy; below it, a thousand signs smear red and blue across the puddles. The Lucky Comet hangs in an impound cradle while an Imperial customs cordon searches freighters for stolen Alliance medical codes.");
            Console.WriteLine("A familiar voice crackles through a battered comlink: Venn Ralo, the slicer who once sold Sabrina a 'lucky' sabacc deck, now claims he has proof the Empire is diverting relief shipments to bait a Rebel cell. He wants a meeting at the old gantry. A debt collector wants 300 credits. Both are already here.");
            Console.WriteLine("Sabrina is a human smuggler: quick hands, quicker lies, and an inconvenient belief that one more game will fix the last one. Type sheet to inspect her. Enter a number to act; type quit to pause.\n");
        }
        private static void Sheet()
        {
            Console.WriteLine("\nSABRINA | Human smuggler | Background: gambler in recovery (allegedly)");
            Console.WriteLine("Health " + Hero.Health + "/12 | Credits " + Hero.Credits + " | Time " + Hero.Clock + " | Imperial heat " + Hero.Heat);
            Console.WriteLine("Skills: Deception +6, Piloting +5, Ranged +4, Streetwise +5, Slicing +2, Insight +1");
            Console.WriteLine("Gear: " + String.Join(", ", Hero.Gear.ToArray()) + "\n");
        }
        private static void Choices()
        {
            Console.WriteLine("What do you do?");
            Console.WriteLine("1. {Slip through the cordon with a forged manifest (Deception +6).}");
            Console.WriteLine("2. {Confront Venn and demand the ledger before the collector spots you (Insight +1).}");
            Console.WriteLine("3. {Stake 50 credits on a fast sabacc hand to distract the debt collector (Gambling +4; risky).}");
            Console.WriteLine("4. {Slice the public dock terminal for the impound release code (Slicing +2).}");
            Console.WriteLine("5. {Fire a blaster bolt into the gantry's ancient sprinkler valve and improvise a rainstorm (Ranged +4; dangerous).}");
        }
        private static void Resolve(int n)
        {
            switch (n)
            {
                case 1:
                    if (Hero.Roll(6, 14)) { Hero.Heat = Math.Max(0, Hero.Heat - 1); debtPaid = true; Console.WriteLine("The customs officer stamps the manifest without looking up. The impound clerk releases the berth; your unpaid collector, unfortunately, is not part of the paperwork."); }
                    else { Hero.Heat++; Hero.Health--; Console.WriteLine("The manifest's ink runs in the rain. A stun bolt clips your shoulder as the cordon closes ranks."); }
                    break;
                case 2:
                    if (Hero.Roll(1, 12)) { clue = true; Console.WriteLine("Venn slides over a data wafer: convoy routes, plus a hidden Imperial trap. 'I did not sell this one,' he says. 'Probably.'"); }
                    else { Hero.Heat++; Console.WriteLine("Venn bolts, leaving a dropped wafer. The collector sees your face and names the old debt aloud."); clue = true; }
                    break;
                case 3:
                    if (Hero.Credits < 50) { Console.WriteLine("You check your pockets. The pockets check back: empty."); break; }
                    Hero.Credits -= 50;
                    if (Hero.Roll(4, 15)) { Hero.Credits += 100; debtPaid = true; Console.WriteLine("A perfect sequence. The collector grudgingly accepts his 300 credits from your winnings and pretends not to admire the bluff."); }
                    else { Hero.Heat++; Console.WriteLine("The house wins, the collector laughs, and somewhere a tiny sensible voice asks why you keep doing this."); }
                    break;
                case 4:
                    if (Hero.Roll(2, 13)) { clue = true; debtPaid = true; Console.WriteLine("You find the release code and a cached shipment ledger showing Imperial bait routes. Venn's warning was real."); }
                    else { Hero.Heat++; Hero.Health--; Console.WriteLine("The terminal locks you out and pings security. A guard's stun baton catches your ribs."); }
                    break;
                case 5:
                    if (Hero.Roll(4, 16)) { clue = true; Console.WriteLine("The valve bursts. Rain and steam veil the gantry; Venn tosses you the ledger while the cordon scrambles. 'Subtle,' he says. 'Like a falling moon.'"); }
                    else { Hero.Health -= 3; Hero.Heat += 2; Console.WriteLine("The bolt ricochets off the valve housing. The sprinkler stays dry; the blaster team does not. You hit the deck under a burst of fire."); }
                    break;
            }
        }
    }
}


