using UnityEngine;

namespace Presentation
{
    public enum SceneOf
    {
        Sample,
    }
    
    public static class SceneSettings
    {
        public static readonly Vector2 FixedResolution = new(1080, 2160);
        
        public const string SceneBase = "BaseScene";
        public const string SceneSample = "SampleScene";
    }
}