using System;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public enum GoodsType
    {
        Orange = 1, 
        Tuna = 2, 
        Uranium = 3,
    }

    [ExecuteInEditMode]
    public class Goods
    {

        [SerializeField]
        private int goodsType;
        public int GoodsType
        {
            get { return goodsType; }
            set { goodsType = value; }
        }

        [SerializeField]
        private int weight = UnityEngine.Random.Range(100, 200);
        private int Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        private void RandomType()
        {
            Array values = Enum.GetValues(typeof(GoodsType));
            GoodsType randomGoodsType = (GoodsType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }
    }
}