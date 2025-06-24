using System;
using Presentation.Presenter;
using Presentation.Resources;
using UnityEngine;

namespace Presentation.Utilities
{
    public class ThemePalette
    {
        private ColorTheme _theme;

        public ThemePalette(ColorTheme theme)
        {
            _theme = theme;
        }

        public void SetTheme(ColorTheme theme)
        {
            _theme = theme;
            InAppContext.EventDispatcher.SendGlobalEvent(GlobalEvent.OnThemeUpdated);
        }
        
        public Color GetColor(ColorOf type)
        {
            return type switch {
                ColorOf.Original => new Color(1f, 1f, 1f),
                ColorOf.Primary => _theme.primary,
                ColorOf.Secondary => _theme.secondary,
                ColorOf.Tertiary => _theme.tertiary,
                ColorOf.Highlight => _theme.highlight,
                ColorOf.Accent => _theme.accent,
                ColorOf.Background => _theme.background,
                ColorOf.BackSecondary => _theme.backSecondary,
                ColorOf.BackTertiary => _theme.backTertiary,
                ColorOf.Surface => _theme.surface,
                ColorOf.Border => _theme.border,
                ColorOf.Notification => _theme.notification,
                ColorOf.OnNotification => _theme.onNotification,
                ColorOf.Success => _theme.success,
                ColorOf.OnSuccess => _theme.onSuccess,
                ColorOf.Info => _theme.info,
                ColorOf.OnInfo => _theme.onInfo,
                ColorOf.Warning => _theme.warning,
                ColorOf.OnWarning => _theme.onWarning,
                ColorOf.Danger => _theme.danger,
                ColorOf.OnDanger => _theme.onDanger,
                ColorOf.TextDefault => _theme.textDefault,
                ColorOf.TextSecondary => _theme.textSecondary,
                ColorOf.TextTertiary => _theme.textTertiary,
                ColorOf.TextDisabled => _theme.textDisabled,
                ColorOf.TextSaturday => _theme.textSaturday,
                ColorOf.TextHoliday => _theme.textHoliday,
                ColorOf.Link => _theme.textLink,
                _ => Color.magenta,
            };
        }

        public Color GetColor(ColorOf type, float alpha)
        {
            var color = GetColor(type);
            return new Color(color.r, color.g, color.b, alpha);
        }
        
        public ColorOf FindNearestColorType(Color targetColor)
        {
            const float distanceThreshold = 15f;
            var minDistanceSq = distanceThreshold * distanceThreshold;
            var nearestColorType = ColorOf.Custom;

            foreach (ColorOf type in Enum.GetValues(typeof(ColorOf)))
            {
                if (type is ColorOf.Custom) continue;

                var themeColor = GetColor(type);
                var distanceSq = CalculateColorDistanceSq(targetColor, themeColor);

                if (!(distanceSq < minDistanceSq)) continue;
                minDistanceSq = distanceSq;
                nearestColorType = type;
            }

            return nearestColorType;
        }

        private static float CalculateColorDistanceSq(Color color1, Color color2)
        {
            var dr = (color1.r - color2.r) * 255f;
            var dg = (color1.g - color2.g) * 255f;
            var db = (color1.b - color2.b) * 255f;

            return dr * dr + dg * dg + db * db;
        }
    }
}