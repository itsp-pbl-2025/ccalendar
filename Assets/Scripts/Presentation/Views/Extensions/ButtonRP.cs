using System;
using Presentation.Views.Common;
using R3;
using UnityEngine.UI;

namespace Presentation.Views.Extensions
{
    public class ButtonRP : Button, IButtonState
    {
        // ReSharper disable once InconsistentNaming
        public ImageRx imageRx => _imageRx ??= image as ImageRx;
        private ImageRx _imageRx;
        
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