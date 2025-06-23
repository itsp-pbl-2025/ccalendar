using UnityEngine;

namespace Presentation.Resources
{
    [CreateAssetMenu(fileName = "ColorTheme", menuName = "Context/ColorTheme", order = 1)]
    public class ColorTheme : ScriptableObject
    {
        [SerializeField] private string themeName;
        
        [SerializeField] public Color primary = new(87/256f, 204/256f, 153/256f);
        [SerializeField] public Color secondary = new(128/256f, 237/256f, 153/256f);
        [SerializeField] public Color tertiary = new(199/256f, 249/256f, 204/256f);
        [SerializeField] public Color highlight = new(56/256f, 163/256f, 165/256f);
        [SerializeField] public Color accent = new(34/256f, 87/256f, 122/256f);
        
        [SerializeField] public Color background = new(248/256f, 250/256f, 253/256f);
        [SerializeField] public Color backSecondary = new(235/256f, 237/256f, 240/256f);
        [SerializeField] public Color backTertiary = new(150/256f, 151/256f, 153/256f);
        [SerializeField] public Color surface = new(256/256f, 256/256f, 256/256f);
        [SerializeField] public Color border = new(32/256f, 32/256f, 32/256f);
        
        [SerializeField] public Color notification = new(242/256f, 100/256f, 81/256f);
        [SerializeField] public Color onNotification = new(256/256f, 251/256f, 253/256f);
        [SerializeField] public Color success = new(217/256f, 249/256f, 229/256f);
        [SerializeField] public Color onSuccess = new(67/256f, 148/256f, 108/256f);
        [SerializeField] public Color info = new(224/256f, 242/256f, 254/256f);
        [SerializeField] public Color onInfo = new(2/256f, 132/256f, 199/256f);
        [SerializeField] public Color warning = new(254/256f, 243/256f, 199/256f);
        [SerializeField] public Color onWarning = new(245/256f, 158/256f, 11/256f);
        [SerializeField] public Color danger = new(254/256f, 226/256f, 226/256f);
        [SerializeField] public Color onDanger = new(239/256f, 68/256f, 68/256f);
        
        [SerializeField] public Color textDefault = new(51/256f, 51/256f, 51/256f);
        [SerializeField] public Color textSecondary = new(88/256f, 88/256f, 89/256f);
        [SerializeField] public Color textTertiary = new(121/256f, 121/256f, 122/256f);
        [SerializeField] public Color textDisabled = new(102/256f, 102/256f, 102/256f);
        [SerializeField] public Color textSaturday = new(81/256f, 110/256f, 245/256f);
        [SerializeField] public Color textHoliday = new(243/256f, 121/256f, 92/256f);
        [SerializeField] public Color textLink = new(3/256f, 102/256f, 214/256f);
    }
}