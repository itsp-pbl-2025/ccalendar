namespace Presentation.Utilities
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
    }
}