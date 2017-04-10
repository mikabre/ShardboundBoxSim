using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShardboundBoxSim
{
    static class Constants
    {
        public static readonly Random random = new Random();
        const int common = 10000;
        const int rare = 5219;
        const int epic = 1230;
        const int legendary = 342;
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
}
}
