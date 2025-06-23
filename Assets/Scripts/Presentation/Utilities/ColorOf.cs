namespace Presentation.Utilities
{
    public enum ColorOf
    {
        Original,        // 画像などで利用する、「そのまま」の意味の色
        
        // 汎用的な役割の色
        Primary,         // ブランドのメインカラー
        Secondary,       // 補足的なブランドカラー
        Tertiary,        // さらに薄いブランドカラー
        Highlight,       // 強調・インタラクティブ要素のハイライト
        Accent,          // なんかに使う

        Background,      // 画面全体の背景色
        BackSecondary,   // 背景に重ねる背景色
        BackTertiary,    // さらに重ねる背景色
        Surface,         // カード、ダイアログなどのUI要素の背景色
        Border,          // 区切り線、枠線

        // 特定の状態を示す色
        Notification,    // 通知、未読
        OnNotification,
        Success,         // 成功メッセージ、完了
        OnSuccess,
        Info,            // 情報、通知
        OnInfo,
        Warning,         // 警告、注意
        OnWarning,
        Danger,           // エラー、危険
        OnDanger,

        // テキストの色（OnXxx と重複する場合もあるが、より汎用的なテキスト表現として）
        TextDefault,     // 通常のテキスト
        TextSecondary,   // 副次的なテキスト、キャプションなど
        TextTertiary,    // より薄いテキスト
        TextDisabled,    // 無効化されたテキスト
        TextSaturday,    // 土曜日の色
        TextHoliday,     // 日曜日/祝日の色
        Link,            // リンクの色
        
        Custom,          // カスタム色
    }
}