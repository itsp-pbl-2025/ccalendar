using System;
using UnityEngine;

namespace Presentation
{
    public enum SceneOf
    {
        Base,
        Sample,
        TaskPreview,
        Calendar,
    }
    
    public static class SceneSettings
    {
        public static readonly Vector2 FixedResolution = new(1080, 2160);

        public static string ToName(this SceneOf scene)
        {
            return scene switch
            {
                SceneOf.Base => "BaseScene",
                SceneOf.Sample => "SampleScene",
                SceneOf.TaskPreview => "TaskPreviewScene",
                SceneOf.Calendar => "CalendarScene",
                _ => string.Empty
            };
        }
    }
}