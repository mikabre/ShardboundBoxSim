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
                card.Name = Console.ReadLine();
                Console.WriteLine("Card rarity Basic(1), Common(2), Rare(3), Epic(4), Legendary(5): ");
                string rarity = Console.ReadLine();
                switch (rarity)
                {
                    case "1":
                        card.Rarity = "Basic";
                        break;
                    case "2":
                        card.Rarity = "Common";
                        break;
                    case "3":
                        card.Rarity = "Rare";
                        break;
                    case "4":
                        card.Rarity = "Epic";
                        break;
                    case "5":
                        card.Rarity = "Legendary";
                        break;
                    default:
                        card.Rarity = "";
                        break;
                }

                Console.WriteLine("Card Type Neutral(1), Red(2), Orange(3), Yellow(4), Green(5), Blue(6), Purple(7): ");
                string type = Console.ReadLine();
                switch (type)
                {
                    case "1":
                        card.Type = "Neutral";
                        break;
                    case "2":
                        card.Type = "Red";
                        break;
                    case "3":
                        card.Type = "Orange";
                        break;
                    case "4":
                        card.Type = "Yellow";
                        break;
                    case "5":
                        card.Type = "Green";
                        break;
                    case "6":
                        card.Type = "Blue";
                        break;
                    case "7":
                        card.Type = "Purple";
                        break;
                    default:
                        card.Type = "";
                        break;
                }
                if (card.Name != "" && card.Rarity != "" && card.Type != "")
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
