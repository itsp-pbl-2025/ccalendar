using UnityEngine;

namespace Presentation.Utilities
{
    public static class VectorExtensions
    {
        public static Vector2 Abs(this Vector2 vec2)
        {
            return new Vector2(Mathf.Abs(vec2.x), Mathf.Abs(vec2.y));
        }

        public static Vector3 Abs(this Vector3 vec3)
        {
            return new Vector3(Mathf.Abs(vec3.x), Mathf.Abs(vec3.y), Mathf.Abs(vec3.z));
        }
    }
}