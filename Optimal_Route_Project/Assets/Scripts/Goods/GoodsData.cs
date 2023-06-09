using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class GoodsData
    {
        public Dictionary<GoodsType, int> multipleGoods;

        public GoodsData()
        {
            multipleGoods = new Dictionary<GoodsType, int>
            {
                {GoodsType.Orange, 0 },
                {GoodsType.Tuna, 0 },
                {GoodsType.Uranium, 0 },
            };
        }

        public static GoodsData RandomPointGoods(bool haveIt = true)
        {
            Random rng = new Random();

            double firstStopPoint = rng.NextDouble();
            double secondStopPoint = rng.NextDouble() * (1 - firstStopPoint) + firstStopPoint;
            int goodsSum = rng.Next(100, 201);

            int orangeWeight = (int)(goodsSum * firstStopPoint);
            int tunaWeight = (int)(goodsSum * (secondStopPoint - firstStopPoint));
            int uraniumWeight = (int)(goodsSum * (1 - secondStopPoint));

            GoodsData pointGoods = new GoodsData
            {
                multipleGoods = new Dictionary<GoodsType, int>
                {
                    { GoodsType.Orange, orangeWeight },
                    { GoodsType.Tuna, tunaWeight },
                    { GoodsType.Uranium, uraniumWeight }
                }
            };

            if (haveIt == false)
            {
                foreach (var goods in pointGoods.multipleGoods)
                {
                    pointGoods.multipleGoods[goods.Key] *= -1;
                }
            }

            return pointGoods;
        }


        public int GetAllWeight()
        {
            return multipleGoods.Select(x => x.Value).Sum();
        }
    }
}
