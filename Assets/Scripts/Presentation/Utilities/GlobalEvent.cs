﻿namespace Presentation.Utilities
{
    public enum GlobalEvent
    {
        /// <summary>
        /// AppRunnerの読み込みが完了した直後
        /// </summary>
        OnAppLoaded,
        
        /// <summary>
        /// InAppContext.ThemeのColorThemeが変更された直後
        /// </summary>
        OnThemeUpdated,
        
        /// <summary>
        /// 新規タスクが作成された直後
        /// </summary>
        OnTaskCreated,
        
        /// <summary>
        /// CalendarSceneでScheduleが作成/変更された直後
        /// </summary>
        OnScheduleUpdated,
    }
}