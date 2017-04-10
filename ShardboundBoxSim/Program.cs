using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShardboundBoxSim
{
    class Program
    {
        // Setting various constants that are used regarding rarity values. The first set
        // are rng seed values, with the delta between two adjacent consts being the
        // chance out of 10000 of pulling a specific type of pack
        const int common = 10000;
        const int rare = 5219;
        const int epic = 1230;
        const int legendary = 342;

        // These values are the buy and sell costs for specific types of cards
        const int commonSell = 10;
        const int commonBuy = 50;
        const int rareSell = 25;
        const int rareBuy = 100;
        const int epicSell = 100;
        const int epicBuy = 400;
        const int legendarySell = 400;
        const int legendaryBuy = 1200;

        // RNG seed for the program, and streamwriter object for the log
        private static readonly Random random = new Random();
        private static FileStream fs = File.Create(@"output.txt");
        private static StreamWriter w;

        static void Main(string[] args)
        {
            // This bool kills the main program loop when set to true. Setting this to true will end execution.
            // on next looping.
            //
            // Other boilerplate code here as well. 
            bool kill = false;
            fs.Close();
            w = File.AppendText(@"output.txt");
            w.AutoFlush = true;
            int numBoxes = 0;
            string[] collectionOne = new string[275];
            string[] collectionTwo = new string[275];
            List<Card> cards = LoadJson();

            // String arrays start with null references. Let's initialize it with an empty string instead.
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
                if (c.rarity.Equals("Basic"))
                {
                    int index = Array.IndexOf(collectionOne,"");
                    collectionOne[index] = c.name;
                    collectionTwo[index] = c.name;
                }
            }

            // main loop
            while (kill == false) {
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
                        Console.WriteLine("How many boxes?");
                        try { numBoxes = Int32.Parse(Console.ReadLine()); }
                        catch { IOException e; } { }
                        RandomBox(cards, numBoxes);
                        break;
                    case "2":
                        // Code path for filling collections.
                        int dustOption = 0;
                        Console.WriteLine("1. Use sequential pack opening.");
                        Console.WriteLine("2. Save dust and craft entire remaining collection when enough is obtained.");
                        Console.WriteLine("3. Sim without factoring in dust");

                        string option = Console.ReadLine();
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
                        FillCollection(cards, collectionOne, collectionTwo, dustOption);
                        break;
                    default:
                        Console.WriteLine("Exiting.");
                        Console.ReadLine();
                        System.Environment.Exit(1);
                        break;
                }
            }

        }

        // Looks for a cards.json file and returns an object loaded with the Shardbound cards in a List<Card>.
        public static List<Card> LoadJson()
        {
            using (StreamReader r = new StreamReader("cards.json"))
            {
                string json = r.ReadToEnd();
                List<Card> cards = JsonConvert.DeserializeObject<List<Card>>(json);
                return cards;
            }
        }

        // Main collection filling method.
        private static void FillCollection(List<Card> cards, string[] collectionOne, string[] collectionTwo, int dustOption)
        {
            // Boilerplate.
            int currentDust = 0;
            int count = 0;
            int rarityTracker = 0;
            bool allLegends = false;
            bool allEpics = false;
            bool allRares = false;
            bool allCommons = false;

            // Collection filling loop.
            while (true)
            {
                // This statement checks if all of the slots in the two arrays are full. If they are, the collection is complete
                // and we break out of the main loop.
                if (Array.IndexOf(collectionOne, "") == -1 && Array.IndexOf(collectionTwo, "") == -1)
                    break;

                // The count var is to track how many packs have been opened. RandomBoxCards supplies us with an opened box.
                count++;
                List<Card> results = RandomBox(cards, 1);
                Card one = results.ElementAt(0);
                Card two = results.ElementAt(1);
                Card three = results.ElementAt(2);

                // Check if the first array has card one. If not, we want it, and then immediately go to the start of the loop.
                if (!collectionOne.Contains(one.name))
                {
                    int index = Array.IndexOf(collectionOne, "");
                    collectionOne[index] = one.name; 
                    if (one.rarity.Equals("Legendary")) // if it's a legendary, we simulate there being only one by adding it to both arrays at the same time. similar tradeoffs elsewhere too.
                    {
                        index = Array.IndexOf(collectionTwo, "");
                        collectionTwo[index] = one.name;
                    }
                    OutputLog("Chose " + one.name);
                    continue;
                }
                // Check if the second array has card one. If not, we want it, and then immediately go to the start of the loop.
                else if (!collectionTwo.Contains(one.name))
                {
                    int index = Array.IndexOf(collectionTwo, "");
                    collectionTwo[index] = one.name;
                    OutputLog("Chose " + one.name);
                    continue;
                }
                // Check if the first array has card two. If not, we want it, and then immediately go to the start of the loop.
                if (!collectionOne.Contains(two.name))
                {
                    int index = Array.IndexOf(collectionOne, "");
                    collectionOne[index] = two.name;
                    if (two.rarity.Equals("Legendary"))
                    {
                        index = Array.IndexOf(collectionTwo, "");
                        collectionTwo[index] = two.name;
                    }
                    OutputLog("Chose " + two.name);
                    continue;
                }
                // Check if the second array has card two. If not, we want it, and then immediately go to the start of the loop.
                else if (!collectionTwo.Contains(two.name))
                {
                    int index = Array.IndexOf(collectionTwo, "");
                    collectionTwo[index] = two.name;
                    OutputLog("Chose " + two.name);
                    continue;
                }
                // Check if the first array has card three. If not, we want it, and then immediately go to the start of the loop.
                if (!collectionOne.Contains(three.name))
                {
                    int index = Array.IndexOf(collectionOne, "");
                    collectionOne[index] = three.name;
                    if (three.rarity.Equals("Legendary"))
                    {
                        index = Array.IndexOf(collectionTwo, "");
                        collectionTwo[index] = three.name;
                    }
                    OutputLog("Chose " + three.name);
                    continue;
                }
                // Check if the second array has card three. If not, we want it, and then immediately go to the start of the loop.
                else if (!collectionTwo.Contains(three.name))
                {
                    int index = Array.IndexOf(collectionTwo, "");
                    collectionTwo[index] = three.name;
                    OutputLog("Chose " + three.name);
                    continue;
                }

                // If we reach here, all we can do is dust a card. We have all three. It doesn't matter which we pick, as they all have the same rarity.
                OutputLog(Environment.NewLine + "I have all these cards!");
                dustCard(ref currentDust, one);

                // dustOption one is the sequential ordering.
                if (dustOption == 1)
                {
                    // determine what rarity of card we want. rarityTracker has a value of 0 for legendary cards, 1 for epics, 2 for rares, 3 for commons.
                    // we want to go in order of most rare to least
                    switch (rarityTracker)
                    {
                        case 0:
                            BuyCard(cards, ref currentDust, ref collectionOne, ref collectionTwo, ref rarityTracker);
                            break;
                        case 1:
                            BuyCard(cards, ref currentDust, ref collectionOne, ref collectionTwo, ref rarityTracker);
                            break;
                        case 2:
                            BuyCard(cards, ref currentDust, ref collectionOne, ref collectionTwo, ref rarityTracker);
                            break;
                        case 3:
                            BuyCard(cards, ref currentDust, ref collectionOne, ref collectionTwo, ref rarityTracker);
                            break;
                        default:
                            break;
                    }
                }

                // Dust option 2 calculates how much dust we need to complete the entire set, and then completes it when we can all at once.
                else if (dustOption == 2)
                {
                    // check if we have enough dust to do it
                    bool enoughDust = EnoughDustCheck(currentDust, cards, collectionOne, collectionTwo);

                    if (enoughDust)
                    {
                        // we have enough, lets do it
                        OutputLog(Environment.NewLine + "I have " + currentDust + " dust, which is enough to buy the remaining cards.");

                        // loop through and buy all of the missing cards
                        foreach (Card card in cards)
                        {
                            bool has1 = collectionOne.Contains(card.name);
                            bool has2 = collectionTwo.Contains(card.name);

                            if (!has1 || !has2)
                            {
                                switch (card.rarity)
                                {
                                    case "Common":
                                        if (Array.IndexOf(collectionOne, card.name) == -1)
                                            CraftCard(ref currentDust, ref collectionOne, card);
                                        if (Array.IndexOf(collectionTwo, card.name) == -1)
                                            CraftCard(ref currentDust, ref collectionTwo, card);
                                        break;
                                    case "Rare":
                                        if (Array.IndexOf(collectionOne, card.name) == -1)
                                            CraftCard(ref currentDust, ref collectionOne, card);
                                        if (Array.IndexOf(collectionTwo, card.name) == -1)
                                            CraftCard(ref currentDust, ref collectionTwo, card);
                                        break;
                                    case "Epic":
                                        if (Array.IndexOf(collectionOne, card.name) == -1)
                                            CraftCard(ref currentDust, ref collectionOne, card);
                                        if (Array.IndexOf(collectionTwo, card.name) == -1)
                                            CraftCard(ref currentDust, ref collectionTwo, card);
                                        break;
                                    case "Legendary":
                                        if (Array.IndexOf(collectionOne, card.name) == -1)
                                            CraftLegendaryCard(ref currentDust, ref collectionOne, ref collectionTwo, card);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
                else if (dustOption == 3)
                {
                    // do nothing, no dust tracking.
                }
                //var colOne = from str in collectionOne
                //         where str.Equals("")
                //         select str;
                //var colTwo = from str in collectionTwo
                //             where str.Equals("")
                //             select str;

                //int emptySlots = colOne.Count() + colTwo.Count();
            }
            OutputLog(Environment.NewLine + "I finished! I have " + currentDust + " dust and opened " + count + " boxes.");
        }

        // Buy a card. When this method is called, we've decided we can buy a card. We start by checking the rarityTracker value to see what rarity of card we currently want.
        // Priority is Legendary > Epic > Rare > Common. Once all of a rarity is collected, we move down the chain. This is reflected by adding 1 to rarityTracker
        private static void BuyCard(List<Card> cards, ref int currentDust, ref string[] collectionOne, ref string[] collectionTwo, ref int rarityTracker)
        {
            string rarity = "";
            int buyVal = 0;

            // determine the rarity we want
            switch (rarityTracker)
            {
                case 0:
                    rarity = "Legendary";
                    buyVal = legendaryBuy;
                    break;
                case 1:
                    rarity = "Epic";
                    buyVal = epicBuy;
                    break;
                case 2:
                    rarity = "Rare";
                    buyVal = rareBuy;
                    break;
                case 3:
                    rarity = "Common";
                    buyVal = commonBuy;
                    break;
                default:
                    rarity = "Legendary";
                    buyVal = legendaryBuy;
                    break;
            }

            // get all cards of that rarity
            var ll = from Card in cards
                     where Card.rarity.Equals(rarity)
                     select Card;
            List<Card> theList = ll.ToList();

            // if we have enough dust, lets buy a card!
            if (currentDust >= buyVal)
            {
                foreach (Card card in theList)
                {
                    if ((Array.IndexOf(collectionOne, card.name) == -1))
                    {
                        if (rarityTracker == 0)
                            CraftLegendaryCard(ref currentDust, ref collectionOne, ref collectionTwo, card);
                        else
                            CraftCard(ref currentDust, ref collectionOne, card);
                        if (currentDust < buyVal)
                            break;
                    }
                    if ((Array.IndexOf(collectionTwo, card.name) == -1))
                    {
                        CraftCard(ref currentDust, ref collectionTwo, card);
                        if (currentDust < buyVal)
                            break;
                    }
                }
            }

            // Check if this has completed a set of rarities. If it has, we increment rarityTracker.
            bool checkIfDone = true;
            foreach (Card card in theList)
            {
                if ((Array.IndexOf(collectionOne, card.name) == -1))
                    checkIfDone = false;
                if ((Array.IndexOf(collectionTwo, card.name) == -1))
                    checkIfDone = false;
            }

            if (checkIfDone == true)
                rarityTracker++;
        }

        // Method for handling the text file and console output.
        private static void OutputLog(string text)
        {
            w.WriteLine(text);
            Console.WriteLine(text);
        }

        // Check what its dust value is, then dust it.
        private static void dustCard(ref int currentDust, Card card)
        {
            int sell = 0;
            if (card.rarity.Equals("Common"))
                currentDust += sell = commonSell;
            else if (card.rarity.Equals("Rare"))
                currentDust += sell = rareSell;
            else if (card.rarity.Equals("Epic"))
                currentDust += sell = epicSell;
            else if (card.rarity.Equals("Legendary"))
                currentDust += sell = legendarySell;
            OutputLog("Dusted a " + card.rarity + " for " + sell + ". I have " + currentDust + " dust.");
        }

        // Craft a card.
        private static void CraftCard(ref int currentDust, ref string[] collection, Card card)
        {
            currentDust -= epicBuy;
            int index = Array.IndexOf(collection, "");
            collection[index] = card.name;
            OutputLog(Environment.NewLine + "Crafted " + card.rarity.ToLower() + " " + card.name + ". I have " + currentDust + " now.");
        }

        // Craft a Legendary Card. Gets its own method because it's a special case. Only one legendary in a set.
        // We circumvent this by simply automatically adding the second one as well.
        private static void CraftLegendaryCard(ref int currentDust, ref string[] collectionOne, ref string[] collectionTwo, Card card)
        {
            currentDust -= legendaryBuy;
            int index = Array.IndexOf(collectionOne, "");
            collectionOne[index] = card.name;
            index = Array.IndexOf(collectionTwo, "");
            collectionTwo[index] = card.name;
            OutputLog(Environment.NewLine + "Crafted " + card.rarity.ToLower() + " " + card.name + ". I have " + currentDust + " now.");
        }

        // static method that checks if we have enough dust to craft the rest of the cards in the set. returns true if we do.
        private static bool EnoughDustCheck(int currentDust, List<Card> cards, string[] colOne, string[] colTwo)
        {
            int dustNeeded = 0;

            // loop through cards to see which cards are missing. if a card is detected missing, we add its value to dustNeeded
            foreach (Card card in cards)
            {
                bool has = colOne.Contains(card.name);

                if (!has)
                {
                    switch (card.rarity)
                    {
                        case "Common":
                            dustNeeded += commonBuy;
                            break;
                        case "Rare":
                            dustNeeded += rareBuy;
                            break;
                        case "Epic":
                            dustNeeded += epicBuy;
                            break;
                        case "Legendary":
                            dustNeeded += legendaryBuy;
                            break;
                        default:
                            break;
                    }
                }
            }

            foreach (Card card in cards)
            {
                bool has = colTwo.Contains(card.name);

                if (!has)
                {
                    switch (card.rarity)
                    {
                        case "Common":
                            dustNeeded += commonBuy;
                            break;
                        case "Rare":
                            dustNeeded += rareBuy;
                            break;
                        case "Epic":
                            dustNeeded += epicBuy;
                            break;
                        case "Legendary":
                            // Legendaries don't have a second copy
                            break;
                        default:
                            break;
                    }
                }
            }

            // Do we have enough?
            Console.WriteLine(currentDust + " | " + dustNeeded);
            if (currentDust >= dustNeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Wrapper for OpenABox that generates a random box.
        private static List<Card> RandomBox(List<Card> cards, int numBoxes)
        {
            List<Card> results = new List<Card>();

            for (int i = 0; i < numBoxes; i++)
            {
                int rand = random.Next(10000);
                if (rand < legendary)
                {
                    results = OpenABox(cards, "Legendary");
                }
                else if (rand < epic)
                {
                    results = OpenABox(cards, "Epic");
                }
                else if (rand < rare)
                {
                    results = OpenABox(cards, "Rare");
                }
                else
                {
                    results = OpenABox(cards, "Common");
                }
                
            }
            return results;
        }

        // Opens a box. Returns a list of three card objects. Uses the base card list and the rarity as a parameter.
        private static List<Card> OpenABox(List<Card> cards, string rarity)
        {
            // get all cards of specified rarity
            var results = from Card in cards
                          where Card.rarity.Equals(rarity)
                          select Card;
            List<Card> cardList = results.ToList();

            // Poll for random numbers. These will be used to decide what cards get picked from the available rarity.
            int randCardOne = random.Next(cardList.Count);
            int randCardTwo = random.Next(cardList.Count);
            int randCardThree = random.Next(cardList.Count);

            // Don't want them to be the same card!
            if (randCardOne == randCardTwo)
            {
                while (randCardOne == randCardTwo)
                {
                    randCardTwo = random.Next(cardList.Count);
                }
            }
            // Don't want them to be the same card!
            if (randCardOne == randCardThree || randCardTwo == randCardThree)
            {
                while (randCardOne == randCardTwo)
                {
                    randCardThree = random.Next(cardList.Count);
                }
            }
            // Fetch card objects
            Card packCardOne = cardList.ElementAt(randCardOne);
            Card packCardTwo = cardList.ElementAt(randCardTwo);
            Card packCardThree = cardList.ElementAt(randCardThree);
            // Report 
            OutputLog(Environment.NewLine + rarity.ToUpper() + " BOX");
            OutputLog("(" + packCardOne.type + ") " + packCardOne.name + " | " + "(" + packCardTwo.type + ") " + packCardTwo.name + " | " + "(" + packCardThree.type + ") " + packCardThree.name);
            
            return new List<Card> { packCardOne, packCardTwo, packCardThree };
        }
    }
}
