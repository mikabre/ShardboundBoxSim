using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShardboundBoxSim
{
    class GenerateCards
    {
        public static void Generate()
        {
            List<Card> cardList = new List<Card>();
            bool kill = false;

            while (kill == false)
            {
                Card card = new Card();
                Console.WriteLine("Card name: ");
                card.name = Console.ReadLine();
                Console.WriteLine("Card rarity Basic(1), Common(2), Rare(3), Epic(4), Legendary(5): ");
                string rarity = Console.ReadLine();
                switch (rarity)
                {
                    case "1":
                        card.rarity = "Basic";
                        break;
                    case "2":
                        card.rarity = "Common";
                        break;
                    case "3":
                        card.rarity = "Rare";
                        break;
                    case "4":
                        card.rarity = "Epic";
                        break;
                    case "5":
                        card.rarity = "Legendary";
                        break;
                    default:
                        card.rarity = "";
                        break;
                }

                Console.WriteLine("Card Type Neutral(1), Red(2), Orange(3), Yellow(4), Green(5), Blue(6), Purple(7): ");
                string type = Console.ReadLine();
                switch (type)
                {
                    case "1":
                        card.type = "Neutral";
                        break;
                    case "2":
                        card.type = "Red";
                        break;
                    case "3":
                        card.type = "Orange";
                        break;
                    case "4":
                        card.type = "Yellow";
                        break;
                    case "5":
                        card.type = "Green";
                        break;
                    case "6":
                        card.type = "Blue";
                        break;
                    case "7":
                        card.type = "Purple";
                        break;
                    default:
                        card.type = "";
                        break;
                }
                if (card.name != "" && card.rarity != "" && card.type != "")
                {
                    cardList.Add(card);
                }
                else
                {
                    kill = true;
                }
            }
            string output = JsonConvert.SerializeObject(cardList);
            Console.Write(output);
            System.IO.File.WriteAllText(@"json.txt", output);
            Console.ReadLine();
        }
    }
}
