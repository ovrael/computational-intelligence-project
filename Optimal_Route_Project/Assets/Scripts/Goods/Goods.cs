using UnityEngine;

namespace Assets.Scripts
{
    public enum GoodsType
    {
        Orange,
        Tuna,
        Uranium,
    }

    [ExecuteInEditMode]
    public class Goods
    {
        [SerializeField]
        private GoodsType goodsType;
        public GoodsType GoodsType
        {
            get { return goodsType; }
            private set { goodsType = value; }
        }

        [SerializeField]
        private int weight;
        public int Weight
        {
            get { return weight; }
            private set { weight = value; }
        }

        public Goods(GoodsType type, bool randomWeight = true)
        {
            GoodsType = type;
            Weight = 0;

            if (randomWeight)
                Weight = Random.Range(Config.GetInstance().MinGoodsWeight, Config.GetInstance().MaxGoodsWeight + 1);
        }

        public void LoadFromOther(Goods otherGoods)
        {
            if (GoodsType != otherGoods.GoodsType)
            {
                Debug.LogWarning($"Tried load {otherGoods.GoodsType} goods into {GoodsType} type");
                return;
            }

            Weight += otherGoods.Weight;
            otherGoods.Weight = 0;
        }

        public void LoadFromOther(Goods otherGoods, int weight)
        {
            if (GoodsType != otherGoods.GoodsType)
            {
                Debug.LogWarning($"Tried load {otherGoods.GoodsType} goods into {GoodsType} type.");
                return;
            }

            if (otherGoods.Weight < weight)
            {
                Debug.LogWarning($"Tried load {weight} weight but only {otherGoods.Weight} is available.");
                return;
            }

            if (weight < 0)
            {
                Debug.LogWarning($"Tried load negative weight:{weight}.");
                return;
            }

            Weight += weight;
            otherGoods.Weight -= weight;
        }
    }
}