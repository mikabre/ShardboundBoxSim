using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShardboundBoxSim
{
    static class Utils
    {
        private static StreamWriter w = File.AppendText(@"output.txt");

        public static List<Card> OpenABox(List<Card> cards, string rarity)
        {
            Random random = Constants.Rand();
            // get all cards of specified Rarity
            var results = from Card in cards
                          where Card.Rarity.Equals(rarity)
                          select Card;
            List<Card> cardList = results.ToList();

            // Poll for random numbers. These will be used to decide what cards get picked from the available Rarity.
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
            OutputLog("(" + packCardOne.Type + ") " + packCardOne.Name + " | " + "(" + packCardTwo.Type + ") " + packCardTwo.Name + " | " + "(" + packCardThree.Type + ") " + packCardThree.Name);

            return new List<Card> { packCardOne, packCardTwo, packCardThree };
        }

        public static BoxResultSet RandomBoxes(List<Card> cards, int numBoxes)
        {
            List<Box> boxes = new List<Box>();
            for (int i = 0; i < numBoxes; i++)
            {
                boxes.Add(new Box(cards));
            }
            BoxResultSet resultSet = new BoxResultSet(boxes);
            if (numBoxes > 1)
                Utils.OutputLog(Environment.NewLine + Environment.NewLine + "This set of boxes had " + resultSet.LegendaryCount + " legendaries, " + resultSet.EpicCount + " epics, " + resultSet.RareCount + " rares, and " + resultSet.CommonCount + " commons.");
            return resultSet;
        }

        // A wrapper for RandomBox that lets you open sets of random boxes
        public static void ManyRandomBoxes(int setsTotal, List<Card> cards)
        {
            List<BoxResultSet> boxSetList = new List<BoxResultSet>();
            int numBoxes = 0;
            double totalLegends = 0;
            double totalEpics = 0;
            double totalRares = 0;
            double totalCommons = 0;
            double averageLegends = 0.0;
            double averageEpics = 0;
            double averageRares = 0;
            double averageCommons = 0;

            Console.WriteLine("How many boxes?");
            try { numBoxes = Int32.Parse(Console.ReadLine()); }
            catch { IOException e; }
            { }

            for (int i = 0; i < setsTotal; i++)
            {
                BoxResultSet boxSet = RandomBoxes(cards, numBoxes);
                totalCommons += boxSet.CommonCount;
                totalRares += boxSet.RareCount;
                totalEpics += boxSet.EpicCount;
                totalLegends += boxSet.LegendaryCount;
                boxSetList.Add(boxSet);
            }
            averageCommons = totalCommons / boxSetList.Count;
            averageRares = totalRares / boxSetList.Count;
            averageEpics = totalEpics / boxSetList.Count;
            averageLegends = totalLegends / boxSetList.Count;

            OutputLog(Environment.NewLine + "-----" + Environment.NewLine + Environment.NewLine + "For this set of sets, the average amount of commons was " + averageCommons + " commons, " + averageRares + " rares, " + averageEpics + " epics, and " + averageLegends + " legendaries.");
        }

        // Method for handling the text file and console output.
        public static void OutputLog(string text)
        {
            w.AutoFlush = true;
            w.WriteLine(text);
            Console.WriteLine(text);
        }

        // static method that checks if we have enough dust to craft the rest of the cards in the set. returns true if we do.
        public static bool EnoughDustCheck(int currentDust, List<Card> cards, string[] colOne, string[] colTwo)
        {
            int dustNeeded = 0;

            // loop through cards to see which cards are missing. if a card is detected missing, we add its value to dustNeeded
            foreach (Card card in cards)
            {
                bool has = colOne.Contains(card.Name);

                if (!has)
                {
                    switch (card.Rarity)
                    {
                        case "Common":
                            dustNeeded += Constants.CommonBuy();
                            break;
                        case "Rare":
                            dustNeeded += Constants.RareBuy();
                            break;
                        case "Epic":
                            dustNeeded += Constants.EpicBuy();
                            break;
                        case "Legendary":
                            dustNeeded += Constants.LegendaryBuy();
                            break;
                        default:
                            break;
                    }
                }
            }

            foreach (Card card in cards)
            {
                bool has = colTwo.Contains(card.Name);

                if (!has)
                {
                    switch (card.Rarity)
                    {
                        case "Common":
                            dustNeeded += Constants.CommonBuy();
                            break;
                        case "Rare":
                            dustNeeded += Constants.RareBuy();
                            break;
                        case "Epic":
                            dustNeeded += Constants.EpicBuy();
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

        // Craft a Legendary Card. Gets its own method because it's a special case. Only one legendary in a set.
        // We circumvent this by simply automatically adding the second one as well.
        public static void CraftLegendaryCard(ref int currentDust, ref string[] collectionOne, ref string[] collectionTwo, Card card)
        {
            currentDust -= Constants.LegendaryBuy();
            int index = Array.IndexOf(collectionOne, "");
            collectionOne[index] = card.Name;
            index = Array.IndexOf(collectionTwo, "");
            collectionTwo[index] = card.Name;
            Utils.OutputLog(Environment.NewLine + "Crafted " + card.Rarity.ToLower() + " " + card.Name + ". I have " + currentDust + " now.");
        }

        // Craft a card.
        public static void CraftCard(ref int currentDust, ref string[] collection, Card card)
        {
            currentDust -= Constants.EpicBuy();
            int index = Array.IndexOf(collection, "");
            collection[index] = card.Name;
            Utils.OutputLog(Environment.NewLine + "Crafted " + card.Rarity.ToLower() + " " + card.Name + ". I have " + currentDust + " now.");
        }

        // Check what its dust value is, then dust it.
        public static void dustCard(ref int currentDust, Card card)
        {
            int sell = 0;
            if (card.Rarity.Equals(Constants.Common()))
                currentDust += sell = Constants.CommonSell();
            else if (card.Rarity.Equals(Constants.Rare()))
                currentDust += sell = Constants.RareSell();
            else if (card.Rarity.Equals(Constants.Epic()))
                currentDust += sell = Constants.EpicSell();
            else if (card.Rarity.Equals(Constants.Legendary()))
                currentDust += sell = Constants.LegendarySell();
            Utils.OutputLog("Dusted a " + card.Rarity + " for " + sell + ". I have " + currentDust + " dust.");
        }

        // Buy a card. When this method is called, we've decided we can buy a card. We start by checking the rarityTracker value to see what Rarity of card we currently want.
        // Priority is Legendary > Epic > Rare > Common. Once all of a Rarity is collected, we move down the chain. This is reflected by adding 1 to rarityTracker
        public static void BuyCard(List<Card> cards, ref int currentDust, ref string[] collectionOne, ref string[] collectionTwo, ref int rarityTracker)
        {
            string rarity = "";
            int buyVal = 0;

            // determine the Rarity we want
            switch (rarityTracker)
            {
                case 0:
                    rarity = Constants.Legendary();
                    buyVal = Constants.LegendaryBuy();
                    break;
                case 1:
                    rarity = Constants.Epic();
                    buyVal = Constants.EpicBuy();
                    break;
                case 2:
                    rarity = Constants.Rare();
                    buyVal = Constants.RareBuy();
                    break;
                case 3:
                    rarity = Constants.Common();
                    buyVal = Constants.CommonBuy();
                    break;
                default:
                    rarity = Constants.Legendary();
                    buyVal = Constants.LegendaryBuy();
                    break;
            }

            // get all cards of that Rarity
            var ll = from Card in cards
                     where Card.Rarity.Equals(rarity)
                     select Card;
            List<Card> theList = ll.ToList();

            // if we have enough dust, lets buy a card!
            if (currentDust >= buyVal)
            {
                foreach (Card card in theList)
                {
                    if ((Array.IndexOf(collectionOne, card.Name) == -1))
                    {
                        if (rarityTracker == 0)
                            Utils.CraftLegendaryCard(ref currentDust, ref collectionOne, ref collectionTwo, card);
                        else
                            Utils.CraftCard(ref currentDust, ref collectionOne, card);
                        if (currentDust < buyVal)
                            break;
                    }
                    if ((Array.IndexOf(collectionTwo, card.Name) == -1))
                    {
                        Utils.CraftCard(ref currentDust, ref collectionTwo, card);
                        if (currentDust < buyVal)
                            break;
                    }
                }
            }

            // Check if this has completed a set of rarities. If it has, we increment rarityTracker.
            bool checkIfDone = true;
            foreach (Card card in theList)
            {
                if ((Array.IndexOf(collectionOne, card.Name) == -1))
                    checkIfDone = false;
                if ((Array.IndexOf(collectionTwo, card.Name) == -1))
                    checkIfDone = false;
            }

            if (checkIfDone == true)
                rarityTracker++;
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
        public static void FillCollection(List<Card> cards, string[] collectionOne, string[] collectionTwo, int dustOption)
        {
            // Boilerplate.
            int currentDust = 0;
            int count = 0;
            int rarityTracker = 0;

            // Collection filling loop.
            while (true)
            {
                // This statement checks if all of the slots in the two arrays are full. If they are, the collection is complete
                // and we break out of the main loop.
                if (Array.IndexOf(collectionOne, "") == -1 && Array.IndexOf(collectionTwo, "") == -1)
                    break;

                // The count var is to track how many packs have been opened. RandomBoxCards supplies us with an opened box.
                count++;

                List<Card> results = Utils.RandomBoxes(cards, 1).Boxes.ElementAt(0).Cards;
                Card one = results.ElementAt(0);
                Card two = results.ElementAt(1);
                Card three = results.ElementAt(2);

                // Check if the first array has card one. If not, we want it, and then immediately go to the start of the loop.
                if (!collectionOne.Contains(one.Name))
                {
                    int index = Array.IndexOf(collectionOne, "");
                    collectionOne[index] = one.Name;
                    if (one.Rarity.Equals(Constants.Legendary())) // if it's a legendary, we simulate there being only one by adding it to both arrays at the same time. similar tradeoffs elsewhere too.
                    {
                        index = Array.IndexOf(collectionTwo, "");
                        collectionTwo[index] = one.Name;
                    }
                    Utils.OutputLog("Chose " + one.Name);
                    continue;
                }
                // Check if the second array has card one. If not, we want it, and then immediately go to the start of the loop.
                else if (!collectionTwo.Contains(one.Name))
                {
                    int index = Array.IndexOf(collectionTwo, "");
                    collectionTwo[index] = one.Name;
                    Utils.OutputLog("Chose " + one.Name);
                    continue;
                }
                // Check if the first array has card two. If not, we want it, and then immediately go to the start of the loop.
                if (!collectionOne.Contains(two.Name))
                {
                    int index = Array.IndexOf(collectionOne, "");
                    collectionOne[index] = two.Name;
                    if (two.Rarity.Equals(Constants.Legendary()))
                    {
                        index = Array.IndexOf(collectionTwo, "");
                        collectionTwo[index] = two.Name;
                    }
                    Utils.OutputLog("Chose " + two.Name);
                    continue;
                }
                // Check if the second array has card two. If not, we want it, and then immediately go to the start of the loop.
                else if (!collectionTwo.Contains(two.Name))
                {
                    int index = Array.IndexOf(collectionTwo, "");
                    collectionTwo[index] = two.Name;
                    Utils.OutputLog("Chose " + two.Name);
                    continue;
                }
                // Check if the first array has card three. If not, we want it, and then immediately go to the start of the loop.
                if (!collectionOne.Contains(three.Name))
                {
                    int index = Array.IndexOf(collectionOne, "");
                    collectionOne[index] = three.Name;
                    if (three.Rarity.Equals(Constants.Legendary()))
                    {
                        index = Array.IndexOf(collectionTwo, "");
                        collectionTwo[index] = three.Name;
                    }
                    Utils.OutputLog("Chose " + three.Name);
                    continue;
                }
                // Check if the second array has card three. If not, we want it, and then immediately go to the start of the loop.
                else if (!collectionTwo.Contains(three.Name))
                {
                    int index = Array.IndexOf(collectionTwo, "");
                    collectionTwo[index] = three.Name;
                    Utils.OutputLog("Chose " + three.Name);
                    continue;
                }

                // If we reach here, all we can do is dust a card. We have all three. It doesn't matter which we pick, as they all have the same Rarity.
                Utils.OutputLog(Environment.NewLine + "I have all these cards!");
                Utils.dustCard(ref currentDust, one);

                // dustOption one is the sequential ordering.
                if (dustOption == 1)
                {
                    // determine what Rarity of card we want. rarityTracker has a value of 0 for legendary cards, 1 for epics, 2 for rares, 3 for commons.
                    // we want to go in order of most rare to least
                    switch (rarityTracker)
                    {
                        case 0:
                            Utils.BuyCard(cards, ref currentDust, ref collectionOne, ref collectionTwo, ref rarityTracker);
                            break;
                        case 1:
                            Utils.BuyCard(cards, ref currentDust, ref collectionOne, ref collectionTwo, ref rarityTracker);
                            break;
                        case 2:
                            Utils.BuyCard(cards, ref currentDust, ref collectionOne, ref collectionTwo, ref rarityTracker);
                            break;
                        case 3:
                            Utils.BuyCard(cards, ref currentDust, ref collectionOne, ref collectionTwo, ref rarityTracker);
                            break;
                        default:
                            break;
                    }
                }

                // Dust option 2 calculates how much dust we need to complete the entire set, and then completes it when we can all at once.
                else if (dustOption == 2)
                {
                    // check if we have enough dust to do it
                    bool enoughDust = Utils.EnoughDustCheck(currentDust, cards, collectionOne, collectionTwo);

                    if (enoughDust)
                    {
                        // we have enough, lets do it
                        Utils.OutputLog(Environment.NewLine + "I have " + currentDust + " dust, which is enough to buy the remaining cards.");

                        // loop through and buy all of the missing cards
                        foreach (Card card in cards)
                        {
                            bool has1 = collectionOne.Contains(card.Name);
                            bool has2 = collectionTwo.Contains(card.Name);

                            if (!has1 || !has2)
                            {
                                switch (card.Rarity)
                                {
                                    case "Common":
                                        if (Array.IndexOf(collectionOne, card.Name) == -1)
                                            Utils.CraftCard(ref currentDust, ref collectionOne, card);
                                        if (Array.IndexOf(collectionTwo, card.Name) == -1)
                                            Utils.CraftCard(ref currentDust, ref collectionTwo, card);
                                        break;
                                    case "Rare":
                                        if (Array.IndexOf(collectionOne, card.Name) == -1)
                                            Utils.CraftCard(ref currentDust, ref collectionOne, card);
                                        if (Array.IndexOf(collectionTwo, card.Name) == -1)
                                            Utils.CraftCard(ref currentDust, ref collectionTwo, card);
                                        break;
                                    case "Epic":
                                        if (Array.IndexOf(collectionOne, card.Name) == -1)
                                            Utils.CraftCard(ref currentDust, ref collectionOne, card);
                                        if (Array.IndexOf(collectionTwo, card.Name) == -1)
                                            Utils.CraftCard(ref currentDust, ref collectionTwo, card);
                                        break;
                                    case "Legendary":
                                        if (Array.IndexOf(collectionOne, card.Name) == -1)
                                            Utils.CraftLegendaryCard(ref currentDust, ref collectionOne, ref collectionTwo, card);
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
            Utils.OutputLog(Environment.NewLine + "I finished! I have " + currentDust + " dust and opened " + count + " boxes.");
        }
    }
}
