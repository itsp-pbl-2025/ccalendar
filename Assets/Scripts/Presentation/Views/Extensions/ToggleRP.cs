using System;
using Presentation.Views.Common;
using R3;
using UnityEngine.UI;

namespace Presentation.Views.Extensions
{
    public class ToggleRP : Toggle, IButtonState
    {
        // ReSharper disable once InconsistentNaming
        public ImageRx imageRx => _imageRx ??= image as ImageRx;
        private ImageRx _imageRx;
        
        private readonly ReactiveProperty<(ButtonSelectionState state, bool instant)> _state = new();
        public Observable<(ButtonSelectionState state, bool instant)> State => _state;
        
        private readonly ReactiveProperty<(bool toggled, bool instant)> _toggled = new();
        public Observable<(bool toggled, bool instant)> Toggled => _toggled;
        
        private readonly ReactiveProperty<bool> _toggleIsOn = new();

        protected override void Awake()
        {
            base.Awake();
            
            _toggleIsOn.Subscribe(toggled => { _toggled.Value = (toggled, toggleTransition is ToggleTransition.None); }).AddTo(this);
        }

        private void Update()
        {
            _toggleIsOn.Value = isOn;
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