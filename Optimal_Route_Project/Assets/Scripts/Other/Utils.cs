using UnityEngine;

namespace Assets.Scripts
{
    static class Utils
    {
        public static Color CreateColor(int r, int g, int b)
        {
            return new Color(r / 255f, g / 255f, b / 255f);
        }
    }
}
