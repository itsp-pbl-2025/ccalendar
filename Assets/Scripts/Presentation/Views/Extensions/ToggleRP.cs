using Presentation.Views.Common;
using R3;
using UnityEngine.UI;

namespace Presentation.Views.Extensions
{
    public class ToggleRP : Toggle
    {
        // ReSharper disable once InconsistentNaming
        public ImageRx imageRx => _imageRx ??= image as ImageRx;
        private ImageRx _imageRx;
        
        private readonly ReactiveProperty<(ButtonSelectionState state, bool instant)> _state = new();
        public Observable<(ButtonSelectionState state, bool instant)> State => _state;
        
        private readonly ReactiveProperty<(bool toggled, bool instant)> _toggled = new();
        public Observable<(bool toggled, bool instant)> Toggled => _toggled;

        protected override void Awake()
        {
            base.Awake();
            
            onValueChanged.AddListener(toggled => { _toggled.Value = (toggled, toggleTransition is ToggleTransition.None); });
        }

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