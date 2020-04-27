/*
 * Copyright (c) 2020 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Flora License, Version 1.1 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://floralicense.org/license/
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Tizen.Wearable.CircularUI.Forms
{
    /// <summary>
    /// The ContentPopup is a Popup, which allows you to customize the View to be displayed.
    /// </summary>
    /// <since_tizen> 4 </since_tizen>
    public class ContentPopup : Element, IDisposable
    {
        IContentPopupRenderer _renderer;
        TaskCompletionSource<bool> _tcsForDismiss;

        /// <summary>
        /// For internal use.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Func<IContentPopupRenderer> CreateRenderer { get; set; } = null;

        /// <summary>
        /// BindableProperty. Identifies the Content bindable property.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(ContentPopup), null, propertyChanged: (b, o, n) => ((ContentPopup)b).UpdateContent());

        /// <summary>
        /// BindableProperty. Identifies the IsShow bindable property.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(nameof(IsOpen), typeof(bool), typeof(ContentPopup), false, propertyChanged:(b, o, n) => ((ContentPopup)b).UpdateRenderer());

        /// <summary>
        /// BindableProperty. Identifies the RotaryFocusObject bindable property Key.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public static readonly BindableProperty RotaryFocusObjectProperty = BindableProperty.Create(nameof(RotaryFocusObject), typeof(IRotaryFocusable), typeof(ContentPopup), null);

        /// <summary>
        /// Occurs when the device's back button is pressed.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public event EventHandler BackButtonPressed;

        /// <summary>
        /// Occurs when the popup is dismissed.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public event EventHandler Dismissed;

        /// <summary>
        /// Gets or sets object of RotaryFocusObject to receive bezel action(take a rotary event).
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public IRotaryFocusable RotaryFocusObject
        {
            get => (IRotaryFocusable)GetValue(RotaryFocusObjectProperty);
            set => SetValue(RotaryFocusObjectProperty, value);
        }

        /// <summary>
        /// Gets or sets content view of the Popup.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public View Content
        {
            get { return (View)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the popup is shown.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Shows the popup.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public Task Open()
        {
            IsOpen = true;
            _tcsForDismiss = new TaskCompletionSource<bool>();
            return _tcsForDismiss.Task;
        }

        /// <summary>
        /// Dismisses the popup.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public void Dismiss()
        {
            IsOpen = false;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SendDismissed()
        {
            Dismissed?.Invoke(this, EventArgs.Empty);
            _tcsForDismiss?.TrySetResult(true);
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SendBackButtonPressed()
        {
            BackButtonPressed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Dispose the popup.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _renderer != null)
            {
                _renderer.Dispose();
                _renderer = null;
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (Content != null)
                SetInheritedBindingContext(Content, BindingContext);
        }

        void UpdateContent()
        {
            if (Content != null)
                OnChildAdded(Content);
        }

        void UpdateRenderer()
        {
            if (_renderer == null)
            {
                _renderer = CreateRenderer();
                _renderer.SetElement(this);
            }
        }
    }
}
