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
            bool kill = false;
            fs.Close();
            w = File.AppendText(@"output.txt");
            w.AutoFlush = true;
            string output = "";
            int numBoxes = 0;
            string[] collectionOne = new string[275];
            string[] collectionTwo = new string[275];

                List<Card> cards = LoadJson();

            for (int i = 0; i < cards.Count; i++)
            {
                collectionOne[i] = "";
                collectionTwo[i] = "";
            }
            foreach (Card c in cards)
            {
                if (c.rarity.Equals("Basic"))
                {
                    int index = Array.IndexOf(collectionOne,"");
                    collectionOne[index] = c.name;
                    collectionTwo[index] = c.name;
                }
            }

            while (kill == false) {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("Choose an option.");
                Console.WriteLine("1. Open a set number of boxes");
                Console.WriteLine("2. Fill collection");
                Console.WriteLine("");
                string menuOption = Console.ReadLine();
            
                switch (menuOption)
                {
                    case "1":
                        Console.WriteLine("How many boxes?");
                        try { numBoxes = Int32.Parse(Console.ReadLine()); }
                        catch { IOException e; } { }
                        OpenBoxes(cards, numBoxes, ref output);
                        break;
                    case "2":
                        int dustOption = 0;
                        Console.WriteLine("1. Use sequential pack opening.");
                        Console.WriteLine("2. Save dust and craft entire remaining collection when enough is obtained.");
                        Console.WriteLine("3. Sim without factoring in dust");

                        string option = Console.ReadLine();
                        switch(option)
                        {
                            case "1":
                                dustOption = 1;
                                break;
                            case "2":
                                dustOption = 2;
                                break;
                            case "3":
                                dustOption = 3;
                                break;
                            default:
                                Console.WriteLine("Defaulting to not factoring dust.");
                                dustOption = 3;
                                break;
                        }

                        FillCollection(cards, collectionOne, collectionTwo, dustOption, ref output);
                        break;
                    default:
                        Console.WriteLine("Exiting.");
                        Console.ReadLine();
                        System.Environment.Exit(1);
                        break;
                }
            }

        }

        public static List<Card> LoadJson()
        {
            using (StreamReader r = new StreamReader("cards.json"))
            {
                string json = r.ReadToEnd();
                List<Card> cards = JsonConvert.DeserializeObject<List<Card>>(json);
                return cards;
            }
        }

        private static void FillCollection(List<Card> cards, string[] collectionOne, string[] collectionTwo, int dustOption, ref string output)
        {
            int currentDust = 0;
            int count = 0;
            bool allLegends = false;
            bool allEpics = false;
            bool allRares = false;
            bool allCommons = false;
            while (true)
            {
                if (Array.IndexOf(collectionOne, "") == -1 && Array.IndexOf(collectionTwo, "") == -1)
                {
                    break;
                }
                count++;
                List<Card> results = RandomBoxCards(cards, 1, ref output);
                Card one = results.ElementAt(0);
                Card two = results.ElementAt(1);
                Card three = results.ElementAt(2);

                if (!collectionOne.Contains(one.name))
                {
                    int index = Array.IndexOf(collectionOne, "");
                    collectionOne[index] = one.name;
                    if (one.rarity.Equals("Legendary"))
                    {
                        index = Array.IndexOf(collectionTwo, "");
                        collectionTwo[index] = one.name;
                    }
                    OutputLog("Chose " + one.name);
                    continue;
                }
                else if (!collectionTwo.Contains(one.name))
                {
                    int index = Array.IndexOf(collectionTwo, "");
                    collectionTwo[index] = one.name;
                    OutputLog("Chose " + one.name);
                    continue;
                }

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
                else if (!collectionTwo.Contains(two.name))
                {
                    int index = Array.IndexOf(collectionTwo, "");
                    collectionTwo[index] = two.name;
                    OutputLog("Chose " + two.name);
                    continue;
                }

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
                else if (!collectionTwo.Contains(three.name))
                {
                    int index = Array.IndexOf(collectionTwo, "");
                    collectionTwo[index] = three.name;
                    OutputLog("Chose " + three.name);
                    continue;
                }

                output = output + Environment.NewLine + "I have all these cards!";
                dustCard(ref currentDust, one, ref output);

                if (dustOption == 1)
                {
                    var ll = from Card in cards
                             where Card.rarity.Equals("Legendary")
                             select Card;
                    List<Card> legendList = ll.ToList();

                    if (currentDust >= legendaryBuy)
                    {
                        foreach (Card legendary in legendList)
                        {
                            if ((Array.IndexOf(collectionOne, legendary.name) == -1))
                            {
                                CraftLegendaryCard(ref currentDust, ref collectionOne, ref collectionTwo, legendary, ref output);
                                if (currentDust < legendaryBuy)
                                    break;
                            }
                        }
                    }

                    bool legendCheckIfDone = true;
                    if (!allLegends)
                    {
                        foreach (Card legendary in legendList)
                        {
                            if ((Array.IndexOf(collectionOne, legendary.name) == -1))
                                legendCheckIfDone = false;
                        }
                    }

                    if (legendCheckIfDone == true)
                        allLegends = true;

                    if (allLegends)
                    {
                        var el = from Card in cards
                                 where Card.rarity.Equals("Epic")
                                 select Card;
                        List<Card> epicList = el.ToList();

                        if (currentDust >= epicBuy)
                        {
                            foreach (Card epic in epicList)
                            {
                                if ((Array.IndexOf(collectionOne, epic.name) == -1))
                                    CraftCard(ref currentDust, ref collectionOne, epic, ref output);
                                else if ((Array.IndexOf(collectionTwo, epic.name) == -1))
                                    CraftCard(ref currentDust, ref collectionTwo, epic, ref output);
                                if (currentDust < epicBuy)
                                    break;
                            }
                        }

                        bool epicCheckIfDone = true;
                        if (!allEpics)
                        {
                            foreach (Card epic in epicList)
                            {
                                if ((Array.IndexOf(collectionOne, epic.name) == -1))
                                    epicCheckIfDone = false;
                            }
                        }

                        if (epicCheckIfDone == true)
                            allEpics = true;
                    }
                    if (allLegends && allEpics)
                    {
                        var rl = from Card in cards
                                 where Card.rarity.Equals("Rare")
                                 select Card;
                        List<Card> rareList = rl.ToList();

                        if (currentDust >= rareBuy)
                        {
                            foreach (Card rare in rareList)
                            {
                                if ((Array.IndexOf(collectionOne, rare.name) == -1) && currentDust >= rareBuy)
                                    CraftCard(ref currentDust, ref collectionOne, rare, ref output);
                                if ((Array.IndexOf(collectionTwo, rare.name) == -1) && currentDust >= rareBuy)
                                    CraftCard(ref currentDust, ref collectionTwo, rare, ref output);

                                if (currentDust < rareBuy)
                                    break;
                            }
                        }
                        bool rareCheckIfDone = true;
                        if (!allRares)
                        {
                            foreach (Card rare in rareList)
                            {
                                if ((Array.IndexOf(collectionOne, rare.name) == -1))
                                    rareCheckIfDone = false;
                            }
                        }
                        if (rareCheckIfDone == true)
                            allRares = true;
                    }
                    if (allLegends && allEpics && allRares)
                    {
                        var cl = from Card in cards
                                 where Card.rarity.Equals("common")
                                 select Card;
                        List<Card> commonList = cl.ToList();

                        if (currentDust >= commonBuy)
                        {
                            foreach (Card common in commonList)
                            {
                                if ((Array.IndexOf(collectionOne, common.name) == -1) && currentDust >= commonBuy)
                                {
                                    CraftCard(ref currentDust, ref collectionTwo, common, ref output);
                                }
                                if ((Array.IndexOf(collectionTwo, common.name) == -1) && currentDust >= commonBuy)
                                {
                                    CraftCard(ref currentDust, ref collectionTwo, common, ref output);
                                }
                                if (currentDust < commonBuy)
                                    break;
                            }
                        }

                        bool commonCheckIfDone = true;
                        if (!allCommons)
                        {
                            foreach (Card common in commonList)
                            {
                                if ((Array.IndexOf(collectionOne, common.name) == -1))
                                    commonCheckIfDone = false;
                            }
                        }
                        if (commonCheckIfDone == true)
                            allCommons = true;
                    }
                } else if (dustOption == 2)
                {
                    bool enoughDust = EnoughDustCheck(currentDust, cards, collectionOne, collectionTwo);

                    if (enoughDust)
                    {
                        OutputLog(Environment.NewLine + "I have " + currentDust + " dust, which is enough to buy the remaining cards.");
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
                                            CraftCard(ref currentDust, ref collectionOne, card, ref output);
                                        if (Array.IndexOf(collectionTwo, card.name) == -1)
                                            CraftCard(ref currentDust, ref collectionTwo, card, ref output);
                                        break;
                                    case "Rare":
                                        if (Array.IndexOf(collectionOne, card.name) == -1)
                                            CraftCard(ref currentDust, ref collectionOne, card, ref output);
                                        if (Array.IndexOf(collectionTwo, card.name) == -1)
                                            CraftCard(ref currentDust, ref collectionTwo, card, ref output);
                                        break;
                                    case "Epic":
                                        if (Array.IndexOf(collectionOne, card.name) == -1)
                                            CraftCard(ref currentDust, ref collectionOne, card, ref output);
                                        if (Array.IndexOf(collectionTwo, card.name) == -1)
                                            CraftCard(ref currentDust, ref collectionTwo, card, ref output);
                                        break;
                                    case "Legendary":
                                        if (Array.IndexOf(collectionOne, card.name) == -1)
                                            CraftLegendaryCard(ref currentDust, ref collectionOne, ref collectionTwo, card, ref output);
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

        private static void OutputLog(string text)
        {
            w.WriteLine(text);
            Console.WriteLine(text);
        }

        private static void dustCard(ref int currentDust, Card card, ref string output)
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

        private static void CraftCard(ref int currentDust, ref string[] collection, Card card, ref string output)
        {
            currentDust -= epicBuy;
            int index = Array.IndexOf(collection, "");
            collection[index] = card.name;
            OutputLog(Environment.NewLine + "Crafted " + card.rarity.ToLower() + " " + card.name + ". I have " + currentDust + " now.");
        }

        private static void CraftLegendaryCard(ref int currentDust, ref string[] collectionOne, ref string[] collectionTwo, Card card, ref string output)
        {
            currentDust -= legendaryBuy;
            int index = Array.IndexOf(collectionOne, "");
            collectionOne[index] = card.name;
            index = Array.IndexOf(collectionTwo, "");
            collectionTwo[index] = card.name;
            OutputLog(Environment.NewLine + "Crafted " + card.rarity.ToLower() + " " + card.name + ". I have " + currentDust + " now.");
        }

        private static bool EnoughDustCheck(int currentDust, List<Card> cards, string[] colOne, string[] colTwo)
        {
            int dustNeeded = 0;
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
                            //dustNeeded += legendaryBuy;
                            break;
                        default:
                            break;
                    }
                }
            }
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

        private static void RandomBox(List<Card> cards, int numBoxes, ref string output)
        {
            for (int i = 0; i < numBoxes; i++)
            {
                int rand = random.Next(10000);

                if (rand < legendary)
                {
                    OpenABox(cards, "Legendary", ref output);
                }
                else if (rand < epic)
                {
                    OpenABox(cards, "Epic", ref output);
                }
                else if (rand < rare)
                {
                    OpenABox(cards, "Rare", ref output);
                }
                else
                {
                    OpenABox(cards, "Common", ref output);
                }
                Console.ReadLine();
            }
        }

        private static List<Card> RandomBoxCards(List<Card> cards, int numBoxes, ref string output)
        {
            List<Card> results = new List<Card>();

            for (int i = 0; i < numBoxes; i++)
            {
                int rand = random.Next(10000);
                if (rand < legendary)
                {
                    results = OpenABox(cards, "Legendary", ref output);
                }
                else if (rand < epic)
                {
                    results = OpenABox(cards, "Epic", ref output);
                }
                else if (rand < rare)
                {
                    results = OpenABox(cards, "Rare", ref output);
                }
                else
                {
                    results = OpenABox(cards, "Common", ref output);
                }
                
            }
            return results;
        }

        private static void OpenBoxes(List<Card> cards, int numBoxes, ref string output)
        {
            RandomBox(cards, numBoxes, ref output);
        }

        private static List<Card> OpenABox(List<Card> cards, string type, ref string output)
        {
            var results = from Card in cards
                          where Card.rarity.Equals(type)
                          select Card;
            List<Card> cardList = results.ToList();

            int randCardOne = random.Next(cardList.Count);
            int randCardTwo = random.Next(cardList.Count);
            int randCardThree = random.Next(cardList.Count);

            if (randCardOne == randCardTwo)
            {
                while (randCardOne == randCardTwo)
                {
                    randCardTwo = random.Next(cardList.Count);
                }
            }

            if (randCardOne == randCardThree || randCardTwo == randCardThree)
            {
                while (randCardOne == randCardTwo)
                {
                    randCardThree = random.Next(cardList.Count);
                }
            }
            Card packCardOne = cardList.ElementAt(randCardOne);
            Card packCardTwo = cardList.ElementAt(randCardTwo);
            Card packCardThree = cardList.ElementAt(randCardThree);
            OutputLog(Environment.NewLine + type.ToUpper() + " BOX");
            OutputLog("(" + packCardOne.type + ") " + packCardOne.name + " | " + "(" + packCardTwo.type + ") " + packCardTwo.name + " | " + "(" + packCardThree.type + ") " + packCardThree.name);
            return new List<Card> { packCardOne, packCardTwo, packCardThree };
        }
    }
}
