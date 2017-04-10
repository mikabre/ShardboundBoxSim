using System;

namespace ShardboundBoxSim
{
    static class Constants
    {
        public static readonly Random random = new Random();
        // Setting various constants that are used regarding Rarity values. The first set
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

        // These texts are used often
        const string commonText = "Common";
        const string rareText = "Rare";
        const string epicText = "Epic";
        const string legendaryText = "Legendary";

        public static Random Rand(){ return random; }
        public static int CommonRarity() { return common; }
        public static int RareRarity(){ return rare; }
        public static int EpicRarity(){ return epic; }
        public static int LegendaryRarity() { return legendary; }
        public static string Common() { return commonText; }
        public static string Rare() { return rareText; }
        public static string Epic() { return epicText; }
        public static string Legendary() { return legendaryText; }
        public static int CommonSell() { return commonSell; }
        public static int CommonBuy() { return commonBuy; }
        public static int RareSell() { return commonSell; }
        public static int RareBuy() { return commonBuy; }
        public static int EpicSell() { return commonSell; }
        public static int EpicBuy() { return commonBuy; }
        public static int LegendarySell() { return commonSell; }
        public static int LegendaryBuy() { return commonBuy; }
    }
}
