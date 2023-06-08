using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class GoodsData
    {
        private List<Goods> multipleGoods;
        public List<Goods> MultipleGoods { get => multipleGoods; set => multipleGoods = value; }

        public GoodsData()
        {
            MultipleGoods = new List<Goods>
            {
                new Goods(GoodsType.Orange, 0),
                new Goods(GoodsType.Tuna, 0),
                new Goods(GoodsType.Uranium, 0)
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
                MultipleGoods = new List<Goods>
                {
                    new Goods(GoodsType.Orange, orangeWeight),
                    new Goods(GoodsType.Tuna, tunaWeight),
                    new Goods(GoodsType.Uranium, uraniumWeight)
                }
            };

            if (haveIt == false)
            {
                foreach (var goods in pointGoods.MultipleGoods)
                {
                    goods.Weight *= -1;
                }
            }

            return pointGoods;
        }


        public int GetAllWeight()
        {
            return MultipleGoods.Select(x => x.Weight).Sum();
        }

        /// <summary>
        /// Takes goods FROM other and puts them into THIS object.
        /// </summary>
        /// <param name="otherGoodsData">Other goods data from whom we are taking goods</param>
        /// <param name="weights">
        /// How much we are taking them.
        /// If not specified or too short then we take max.
        /// </param>
        public void LoadFromOther(GoodsData otherGoodsData, params int[] weights)
        {
            for (int i = 0; i < MultipleGoods.Count; i++)
            {
                int weightToLoad = (i < weights.Length) ? weights[i] : otherGoodsData.MultipleGoods[i].Weight;
                MultipleGoods[i].LoadFromOther(otherGoodsData.MultipleGoods[i], weightToLoad);
            }
        }
    }
}
