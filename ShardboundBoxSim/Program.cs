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
        // is the chance of drawing a certain type of pack. if the random number generator
        // roles that number or 
        const int common = 10000;
        const int rare = 5219;
        const int epic = 1230;
        const int legendary = 342;

        const int commonSell = 10;
        const int commonBuy = 50;
        const int rareSell = 25;
        const int rareBuy = 100;
        const int epicSell = 100;
        const int epicBuy = 400;
        const int legendarySell = 400;
        const int legendaryBuy = 1200;
        private static readonly Random random = new Random();

        static void Main(string[] args)
        {
            bool kill = false;

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
                Console.WriteLine("Choose an option.");
                Console.WriteLine("1. Open a set number of boxes");
                Console.WriteLine("2. Fill collection");
                Console.WriteLine("");
                string menuOption = Console.ReadLine();
            
                switch (menuOption)
                {
                    case "1":
                        Console.WriteLine("How many boxes?");
                        try
                        {
                            numBoxes = Int32.Parse(Console.ReadLine());
                        } catch { IOException e; }
                        {
                            
                        }
                        OpenBoxes(cards, numBoxes);
                        break;
                    case "2":
                        FillCollection(cards, collectionOne, collectionTwo);
                        break;
                    default:
                        Console.WriteLine("Exiting.");
                        Console.ReadLine();
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

        private static void FillCollection(List<Card> cards, string[] collectionOne, string[] collectionTwo)
        {
            bool kill = false;
            string output = "";
            int currentDust = 0;
            int count = 0;
            bool allLegends = false;
            bool allEpics = false;
            bool allRares = false;
            bool allCommons = false;
            while (kill == false)
            {
                count++;
                List<Card> results = RandomBoxCards(cards, 1);
                Card one = results.ElementAt(0);
                Card two = results.ElementAt(1);
                Card three = results.ElementAt(2);


                output = output + Environment.NewLine + Environment.NewLine + one.rarity.ToUpper() + " BOX";
                output = output + Environment.NewLine + "(" + one.type + ") " + one.name + " | " + "(" + two.type + ") " + two.name + " | " + "(" + three.type + ") " + three.name;

                if (!collectionOne.Contains(one.name))
                {
                    int index = Array.IndexOf(collectionOne, "");
                    collectionOne[index] = one.name;
                    if (one.rarity.Equals("Legendary"))
                    {
                        index = Array.IndexOf(collectionTwo, "");
                        collectionTwo[index] = one.name;

                    }
                    output = output + Environment.NewLine + "Chose " + one.name;
                    Console.WriteLine("Chose " + one.name);
                    continue;
                }
                else if (!collectionTwo.Contains(one.name))
                {
                    int index = Array.IndexOf(collectionTwo, "");
                    collectionTwo[index] = one.name;
                    output = output + Environment.NewLine + "Chose " + one.name;
                    Console.WriteLine("Chose " + one.name);
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
                    output = output + Environment.NewLine + "Chose " + two.name;
                    Console.WriteLine("Chose " + two.name);
                    continue;
                }
                else if (!collectionTwo.Contains(two.name))
                {
                    int index = Array.IndexOf(collectionTwo, "");
                    collectionTwo[index] = two.name;
                    output = output + Environment.NewLine + "Chose " + two.name;
                    Console.WriteLine("Chose " + two.name);
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
                    output = output + Environment.NewLine + "Chose " + three.name;
                    Console.WriteLine("Chose " + three.name);
                    continue;
                }
                else if (!collectionTwo.Contains(three.name))
                {
                    int index = Array.IndexOf(collectionTwo, "");
                    collectionTwo[index] = three.name;
                    output = output + Environment.NewLine + "Chose " + three.name;
                    Console.WriteLine("Chose " + three.name);
                    continue;
                }

                output = output + Environment.NewLine + "I have all these cards!";
                switch (one.rarity)
                {
                    case "Common":
                        currentDust += commonSell;
                        output = output + Environment.NewLine + "Dusted a common for " + commonSell + ". I have " + currentDust + " dust.";
                        break;
                    case "Rare":
                        currentDust += rareSell;
                        output = output + Environment.NewLine + "Dusted a rare for " + rareSell + ". I have " + currentDust + " dust.";
                        break;
                    case "Epic":
                        currentDust += epicSell;
                        output = output + Environment.NewLine + "Dusted an epic for " + epicSell + ". I have " + currentDust + " dust.";
                        break;
                    case "Legendary":
                        currentDust += legendarySell;
                        output = output + Environment.NewLine + "Dusted a legendary for " + legendarySell + ". I have " + currentDust + " dust.";
                        break;
                    default:
                        Console.WriteLine("fug :D");
                        break;

                }

                var ll = from Card in cards
                              where Card.rarity.Equals("Legendary")
                              select Card;
                List<Card> legendList = ll.ToList();

                if (currentDust >= legendaryBuy) {
                    foreach (Card legendary in legendList)
                    {
                        if ((Array.IndexOf(collectionOne, legendary.name) == -1))
                        {
                            currentDust -= legendaryBuy;
                            int index = Array.IndexOf(collectionOne, "");
                            collectionOne[index] = legendary.name;
                            index = Array.IndexOf(collectionTwo, "");
                            collectionTwo[index] = legendary.name;
                            output = output + Environment.NewLine + "Crafted legendary " + legendary.name + ". I have " + currentDust + " now.";

                            if (currentDust < legendaryBuy)
                            {
                                break;
                            }
                        }
                    }
                }

                bool legendCheckIfDone = true;
                if (!allLegends) {
                    foreach (Card legendary in legendList)
                    {
                        if ((Array.IndexOf(collectionOne, legendary.name) == -1))
                        {
                            legendCheckIfDone = false;

                        }
                    }
                }
                
                if (legendCheckIfDone == true)
                {
                    allLegends = true;
                }

                if (allLegends)
                {
                    var el = from Card in cards
                             where Card.rarity.Equals("Epic")
                             select Card;
                    List<Card> epicList = el.ToList();

                    if (currentDust >= epicBuy) {
                        foreach (Card epic in epicList)
                        {
                            if ((Array.IndexOf(collectionOne, epic.name) == -1))
                            {
                                currentDust -= epicBuy;
                                int index = Array.IndexOf(collectionOne, "");
                                collectionOne[index] = epic.name;
                                output = output + Environment.NewLine + "Crafted epic " + epic.name + ". I have " + currentDust + " now.";

                            } else if ((Array.IndexOf(collectionTwo, epic.name) == -1))
                            {
                                currentDust -= epicBuy;
                                int index = Array.IndexOf(collectionTwo, "");
                                collectionTwo[index] = epic.name;
                                output = output + Environment.NewLine + "Crafted epic " + epic.name + ". I have " + currentDust + " now.";

                            }
                            if (currentDust < epicBuy)
                            {
                                break;
                            }
                        }
                    }

                    bool epicCheckIfDone = true;
                    if (!allEpics)
                    {
                        foreach (Card epic in epicList)
                        {
                            if ((Array.IndexOf(collectionOne, epic.name) == -1))
                            {
                                epicCheckIfDone = false;

                            }
                        }
                    }

                    if (epicCheckIfDone == true)
                    {
                        allEpics = true;
                    }
                }

                if (allLegends && allEpics)
                {
                    var rl = from Card in cards
                             where Card.rarity.Equals("Rare")
                             select Card;
                    List<Card> rareList = rl.ToList();

                    if (currentDust >= rareBuy) {
                        foreach (Card rare in rareList)
                        {
                            if ((Array.IndexOf(collectionOne, rare.name) == -1) && currentDust >= rareBuy)
                            {
                                currentDust -= rareBuy;
                                int index = Array.IndexOf(collectionOne, "");
                                collectionOne[index] = rare.name;
                                output = output + Environment.NewLine + "Crafted rare " + rare.name + ". I have " + currentDust + " now.";

                            }
                            if ((Array.IndexOf(collectionTwo, rare.name) == -1) && currentDust >= rareBuy)
                            {
                                currentDust -= rareBuy;
                                int index = Array.IndexOf(collectionTwo, "");
                                collectionTwo[index] = rare.name;
                                output = output + Environment.NewLine + "Crafted rare " + rare.name + ". I have " + currentDust + " now.";

                            }

                            if (currentDust < rareBuy)
                            {
                                break;
                            }
                        }
                    }

                    bool rareCheckIfDone = true;
                    if (!allRares)
                    {

                        foreach (Card rare in rareList)
                        {
                            if ((Array.IndexOf(collectionOne, rare.name) == -1))
                            {
                                rareCheckIfDone = false;

                            }
                        }
                    }

                    if (rareCheckIfDone == true)
                    {
                        allRares = true;
                    }
                }

                if (allLegends && allEpics && allRares)
                {
                    var cl = from Card in cards
                             where Card.rarity.Equals("common")
                             select Card;
                    List<Card> commonList = cl.ToList();

                    if (currentDust >= commonBuy) {
                        foreach (Card common in commonList)
                        {
                            if ((Array.IndexOf(collectionOne, common.name) == -1) && currentDust >= commonBuy)
                            {
                                currentDust -= commonBuy;
                                int index = Array.IndexOf(collectionOne, "");
                                collectionOne[index] = common.name;
                                output = output + Environment.NewLine + "Crafted common " + common.name + ". I have " + currentDust + " now.";

                            }
                            if ((Array.IndexOf(collectionTwo, common.name) == -1) && currentDust >= commonBuy)
                            {
                                currentDust -= commonBuy;
                                int index = Array.IndexOf(collectionTwo, "");
                                collectionTwo[index] = common.name;
                                output = output + Environment.NewLine + "Crafted common " + common.name + ". I have " + currentDust + " now.";

                            }

                            if (currentDust < commonBuy)
                            {
                                break;
                            }
                        }
                    }

                    bool commonCheckIfDone = true;
                    if (!allCommons)
                    {
                        foreach (Card common in commonList)
                        {
                            if ((Array.IndexOf(collectionOne, common.name) == -1))
                            {
                                commonCheckIfDone = false;

                            }
                        }
                    }

                    if (commonCheckIfDone == true)
                    {
                        allCommons = true;
                    }
                }

                //var colOne = from str in collectionOne
                //         where str.Equals("")
                //         select str;
                //var colTwo = from str in collectionTwo
                //             where str.Equals("")
                //             select str;

                //int emptySlots = colOne.Count() + colTwo.Count();

                if (Array.IndexOf(collectionOne, "") == -1 && Array.IndexOf(collectionTwo, "") == -1)
                {
                    kill = true;
                }
            }
            Console.WriteLine("i finished! i have " + currentDust + " dust and opened " + count + " boxes.");
            output = output + Environment.NewLine + Environment.NewLine + "i finished! i have " + currentDust + " dust and opened " + count + " boxes.";

            System.IO.File.WriteAllText(@"output.txt", output);
            Console.ReadLine();
        }

        private static void RandomBox(List<Card> cards, int numBoxes)
        {
            for (int i = 0; i < numBoxes; i++)
            {
                int rand = random.Next(10000);

                if (rand < legendary)
                {
                    OpenABox(cards, "Legendary");
                }
                else if (rand < epic)
                {
                    OpenABox(cards, "Epic");
                }
                else if (rand < rare)
                {
                    OpenABox(cards, "Rare");
                }
                else
                {
                    OpenABox(cards, "Common");
                }
                Console.ReadLine();
            }
        }

        private static List<Card> RandomBoxCards(List<Card> cards, int numBoxes)
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

        private static void OpenBoxes(List<Card> cards, int numBoxes)
        {
            RandomBox(cards, numBoxes);
        }

        private static List<Card> OpenABox(List<Card> cards, string type)
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
            //Console.WriteLine(type.ToUpper() + " BOX");
            //Console.WriteLine("(" + packCardOne.type + ") " + packCardOne.name + " | " + "(" + packCardTwo.type + ") " + packCardTwo.name + " | " + "(" + packCardThree.type + ") " + packCardThree.name);
            return new List<Card> { packCardOne, packCardTwo, packCardThree };
        }
    }
}
