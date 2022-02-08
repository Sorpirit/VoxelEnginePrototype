using UnityEngine;

namespace Helpers
{
    public static class MathHelper
    {
        public static Vector3Int ToInt(this Vector3 a) => new Vector3Int((int) a.x, (int) a.y, (int) a.z);
    }
}