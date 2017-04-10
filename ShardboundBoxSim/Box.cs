using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShardboundBoxSim
{
    class Box
    {
        Random random = Constants.Rand();
        public string Rarity { get; private set; }
        public List<Card> Cards { get; private set; }
        public Box(List<Card> allCards)
        {
            int rand = random.Next(Constants.CommonRarity());
            if (rand < Constants.LegendaryRarity())
            {
                Cards = Utils.OpenABox(allCards, Constants.Legendary());
                Rarity = Constants.Legendary();
            }
            else if (rand < Constants.EpicRarity())
            {
                Cards = Utils.OpenABox(allCards, Constants.Epic());
                Rarity = Constants.Epic();
            }
            else if (rand < Constants.RareRarity())
            {
                Cards = Utils.OpenABox(allCards, Constants.Rare());
                Rarity = Constants.Rare();
            }
            else
            {
                Cards = Utils.OpenABox(allCards, Constants.Common());
                Rarity = Constants.Common();
            }
        }
    }
}
