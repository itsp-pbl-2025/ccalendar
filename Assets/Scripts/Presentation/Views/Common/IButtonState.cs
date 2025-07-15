using R3;

namespace Presentation.Views.Common
{
    public interface IButtonState
    {
        public Observable<(ButtonSelectionState state, bool instant)> State { get; }
    }
}