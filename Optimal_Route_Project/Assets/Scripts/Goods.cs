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
        static readonly Random rng = new Random();

        [SerializeField]
        private int goodsType;
        public int GoodsType
        {
            get { return goodsType; }
            set { goodsType = value; }
        }

        [SerializeField]
        private int weight = rng.Next(100,200);
        private int Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        private void RandomType()
        {
            Array values = Enum.GetValues(typeof(GoodsType));
            GoodsType randomGoodsType = (GoodsType)values.GetValue(rng.Next(values.Length));
        }




    }



}