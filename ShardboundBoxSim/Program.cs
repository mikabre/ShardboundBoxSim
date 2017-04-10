using System;
using System.Collections.Generic;
using System.IO;

namespace ShardboundBoxSim
{
    class Program
    {





        // RNG seed for the program, and streamwriter object for the log
        private static FileStream fs = File.Create(@"output.txt");

        static void Main(string[] args)
        {
            // This bool kills the main program loop when set to true. Setting this to true will end execution.
            // on next looping.
            //
            // Other boilerplate code here as well. 
            bool kill = false;
            fs.Close();
            int numBoxes = 0;
            int numSets = 0;
            string[] collectionOne = new string[275];
            string[] collectionTwo = new string[275];
            List<Card> cards = Utils.LoadJson();

            // main loop
            while (kill == false)
            {
                // String arrays start with null references. Let's initialize it with an empty string instead.
                // doing this within the main loop also resets it after doing a full collection add
                for (int i = 0; i < cards.Count; i++)
                {
                    collectionOne[i] = "";
                    collectionTwo[i] = "";
                }

                // The Shardbound card collection starts with a number of basic cards. I could have just 
                // pretended they didn't exist, but they might come in handy some day. For now, I'll
                // simply assume the player has all basic cards (they do).
                foreach (Card c in cards)
                {
                    if (c.Rarity.Equals("Basic"))
                    {
                        int index = Array.IndexOf(collectionOne,"");
                        collectionOne[index] = c.Name;
                        collectionTwo[index] = c.Name;
                    }
                }
                
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("Choose an option.");
                Console.WriteLine("1. Open a set number of boxes");
                Console.WriteLine("2. Fill collection");
                Console.WriteLine("");
                string menuOption = Console.ReadLine();
            
                // initial menu
                switch (menuOption)
                {
                    case "1":
                        // This option leads down a code path that has no involvement in the overall collection. 
                        // Choose this just to open a few boxes and see what you get. Not useful for much else.
                        Console.WriteLine("1. Open a single set of boxes.");
                        Console.WriteLine("2. Open multiple sets of boxes.");

                        string option = Console.ReadLine();
                        switch (option)
                        {
                            case "1":
                                Console.WriteLine("How many boxes?");
                                try { numBoxes = Int32.Parse(Console.ReadLine()); }
                                catch { IOException e; }
                                { }
                                Utils.RandomBoxes(cards, numBoxes);
                                break;
                            case "2":
                                Console.WriteLine("How many sets of boxes?");
                                try { numSets = Int32.Parse(Console.ReadLine()); }
                                catch { IOException e; }
                                { }
                                Utils.ManyRandomBoxes(numSets, cards);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "2":
                        // Code path for filling collections.
                        int dustOption = 0;
                        Console.WriteLine("1. Use sequential pack opening.");
                        Console.WriteLine("2. Save dust and craft entire remaining collection when enough is obtained.");
                        Console.WriteLine("3. Sim without factoring in dust");

                        option = Console.ReadLine();
                        switch(option)
                        {
                            case "1":
                                // Sequential pack opening. Choosing this will attempt to craft cards as it goes along,
                                // with the crafting priority being Legendaries > Epics > Rares > Commons.
                                dustOption = 1;
                                break;
                            case "2":
                                // Batch card crafting. Choosing this will save dust and get as many card packs as it 
                                // can before it has enough to complete the collection with dust alone.
                                dustOption = 2;
                                break;
                            case "3":
                                // No dust interaction, pure card pack opening to complete.
                                dustOption = 3;
                                break;
                            default:
                                Console.WriteLine("Defaulting to not factoring dust.");
                                dustOption = 3;
                                break;
                        }
                        // Execute the main collection filling method.
                        Utils.FillCollection(cards, collectionOne, collectionTwo, dustOption);
                        break;
                    case "3":

                        break;
                    default:
                        Console.WriteLine("Exiting.");
                        Console.ReadLine();
                        System.Environment.Exit(1);
                        break;
                }
            }

        }
    }
}
