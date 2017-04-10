using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Utils.OutputLog(Environment.NewLine + "This set of boxes had " + resultSet.LegendaryCount + " legendaries, " + resultSet.EpicCount + " epics, " + resultSet.RareCount + " rares, and " + resultSet.CommonCount + " commons.");
            return resultSet;
        }

        // A wrapper for RandomBox that lets you open sets of random boxes
        public static void ManyRandomBoxes(int setsTotal, List<Card> cards)
        {
            List<BoxResultSet> boxSet = new List<BoxResultSet>();
            int numBoxes = 0;

            Console.WriteLine("How many boxes?");
            try { numBoxes = Int32.Parse(Console.ReadLine()); }
            catch { IOException e; }
            { }

            for (int i = 0; i < setsTotal; i++)
            {
                boxSet.Add(RandomBoxes(cards, numBoxes));
            }
        }

        // Method for handling the text file and console output.
        public static void OutputLog(string text)
        {
            w.AutoFlush = true;
            w.WriteLine(text);
            Console.WriteLine(text);
        }
    }
}
