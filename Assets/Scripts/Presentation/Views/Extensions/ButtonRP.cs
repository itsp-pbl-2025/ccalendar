using R3;
using UnityEngine.UI;

namespace Presentation.Views.Extensions
{
    public class ButtonRP : Button
    {
        // ReSharper disable once InconsistentNaming
        public ImageRx imageRx => image as ImageRx;

        public enum ButtonSelectionState
        {
            /// <summary>
            /// The UI object can be selected.
            /// </summary>
            Normal,

            /// <summary>
            /// The UI object is highlighted.
            /// </summary>
            Highlighted,

            /// <summary>
            /// The UI object is pressed.
            /// </summary>
            Pressed,

            /// <summary>
            /// The UI object is selected
            /// </summary>
            Selected,

            /// <summary>
            /// The UI object cannot be selected.
            /// </summary>
            Disabled,
        }
        
        private readonly ReactiveProperty<(ButtonSelectionState state, bool instant)> _state = new();
        public Observable<(ButtonSelectionState state, bool instant)> State => _state;
        
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            _state.Value = (state switch {
                SelectionState.Normal => ButtonSelectionState.Normal,
                SelectionState.Highlighted => ButtonSelectionState.Highlighted,
                SelectionState.Pressed => ButtonSelectionState.Pressed,
                SelectionState.Selected => ButtonSelectionState.Selected,
                _ => ButtonSelectionState.Disabled,
            }, instant);
        }
    }
}