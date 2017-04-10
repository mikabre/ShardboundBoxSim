using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShardboundBoxSim
{

    class BoxResultSet
    {
        public List<Box> Boxes { get; private set; }
        public int LegendaryCount { get; private set; }
        public int EpicCount { get; private set; }
        public int RareCount { get; private set; }
        public int CommonCount { get; private set; }

        public BoxResultSet(List<Box> boxes)
        {
            Initialize(boxes);
        }

        private void Initialize(List<Box> boxes)
        {
            Boxes = boxes;
            var results = from Box in boxes
                          where Box.Rarity.Equals(Constants.Legendary())
                          select Box;
            LegendaryCount = results.Count();
            results = from Box in boxes
                      where Box.Rarity.Equals(Constants.Epic())
                      select Box;
            EpicCount = results.Count();
            results = from Box in boxes
                      where Box.Rarity.Equals(Constants.Rare())
                      select Box;
            RareCount = results.Count();
            results = from Box in boxes
                      where Box.Rarity.Equals(Constants.Common())
                      select Box;
            CommonCount = results.Count();
        }

        public BoxResultSet()
        {

        }

        public void Add(Box box)
        {
            Boxes.Add(box);
            Initialize(Boxes);
        }

        public void Add(List<Box> box)
        {
            Boxes.Concat(box);
            Initialize(Boxes);
        }
    }
}
