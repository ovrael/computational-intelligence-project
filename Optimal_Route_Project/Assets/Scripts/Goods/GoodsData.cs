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
            MultipleGoods.Add(new Goods(GoodsType.Orange));
            MultipleGoods.Add(new Goods(GoodsType.Tuna));
            MultipleGoods.Add(new Goods(GoodsType.Uranium));
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
