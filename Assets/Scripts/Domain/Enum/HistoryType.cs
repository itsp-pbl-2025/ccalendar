namespace Domain.Enum
{
    /// <summary>
    /// HistoryContainerで断片的なデータを保存する際に使うラベルenum。
    /// 保存の際はenumに結び付けられた値が主キーとなるため、宣言では必ず数を併記し、変更を行わないこと。
    /// </summary>
    public enum HistoryType : uint
    {
        // 0XX: Global Settings
        
        /// <summary>
        /// 前回起動時のゲームバージョン
        /// type: string
        /// </summary>
        PreviousAppVersion = 1,
        
        /// <summary>
        /// 使っているテーマ名
        /// type: string
        /// </summary>
        ThemeUsing = 2,
        
        // 1XX: Player Settings
        
        /// <summary>
        /// SEの再生設定
        /// type: boolean
        /// </summary>
        SFXMute = 101,
        
        /// <summary>
        /// BGMの再生設定
        /// type: boolean
        /// </summary>
        BGMMute = 102,
        
        /// <summary>
        /// SEの音量設定
        /// type: float
        /// </summary>
        SFXVolume = 103,
        
        /// <summary>
        /// BGMの音量設定
        /// type: float
        /// </summary>
        BGMVolume = 104,
        
        // 2XX~: Services
        
        /// <summary>
        /// HistoryServiceが過去に取得した休日一覧
        /// <para>type: Dictionary&lt;CCDateOnly, string&gt;</para>
        /// </summary>
        CachedHolidays = 201,
        
        // 1XXX: SceneSettings
        
        // 10XX: BaseScene
        
        // 11XX: TaskScene
        
        // 12XX: CalendarScene
        
        /// <summary>
        /// 表示するカレンダータイプの設定
        /// type: CalendarType
        /// </summary>
        PreviousCalendarType = 1201,
    }
}