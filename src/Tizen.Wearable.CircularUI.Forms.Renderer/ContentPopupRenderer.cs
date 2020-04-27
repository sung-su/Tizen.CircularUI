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

using ElmSharp.Wearable;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;
using ERotaryEventManager = ElmSharp.Wearable.RotaryEventManager;
using XForms = Xamarin.Forms.Forms;

namespace Tizen.Wearable.CircularUI.Forms.Renderer
{
    public class ContentPopupRenderer : IContentPopupRenderer
    {
        ElmSharp.Popup _popup;
        ContentPopup _element;
        IRotaryFocusable _currentRotaryFocusObject;

        public ContentPopupRenderer()
        {
            _popup = new ElmSharp.Popup(XForms.NativeParent);
            _popup.Style = "circle";

            _popup.BackButtonPressed += OnBackButtonPressed;
            _popup.Dismissed += OnDismissed;
        }

        public void SetElement(ContentPopup element)
        {
            if (element.Parent == null)
                element.Parent = Application.Current;
            element.PropertyChanged += OnElementPropertyChanged;
            _element = element;

            UpdateContent();
            UpdateIsOpen();
            UpdateRotaryFocusObject();
        }


        ~ContentPopupRenderer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void UpdateRotaryFocusObject()
        {
            if (_currentRotaryFocusObject == null)
            {
                _currentRotaryFocusObject = _element.RotaryFocusObject;
            }
            else
            {
                DeactivateRotaryWidget();
                _currentRotaryFocusObject = _element.RotaryFocusObject;
                ActivateRotaryWidget();
            }
        }

        protected void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ContentPopup.ContentProperty.PropertyName)
            {
                UpdateContent();
            }
            if (e.PropertyName == ContentPopup.IsOpenProperty.PropertyName)
            {
                UpdateIsOpen();
            }
            if (e.PropertyName == ContentPopup.RotaryFocusObjectProperty.PropertyName)
            {
                UpdateRotaryFocusObject();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_popup != null)
                {
                    _popup.BackButtonPressed -= OnBackButtonPressed;
                    _popup.Dismissed -= OnDismissed;
                    _popup.Unrealize();
                    _popup = null;
                }
                if (_element != null)
                {
                    _element.PropertyChanged -= OnElementPropertyChanged;
                    _element = null;
                }
            }
        }

        void OnBackButtonPressed(object sender, EventArgs e)
        {
            _element.SendBackButtonPressed();
        }

        void OnDismissed(object sender, EventArgs e)
        {
            _element.SendDismissed();
        }

        void UpdateContent()
        {
            if (_element.Content != null)
            {
                var renderer = Platform.GetOrCreateRenderer(_element.Content);
                (renderer as LayoutRenderer)?.RegisterOnLayoutUpdated();
                var native = renderer.NativeView;
                native.MinimumHeight = XForms.NativeParent.Geometry.Height;
                _popup.SetContent(native, false);
            }
            else
            {
                _popup.SetContent(null, false);
            }
        }

        void UpdateIsOpen()
        {
            if (_element.IsOpen)
                _popup?.Show();
            else
                _popup?.Hide();
        }

        IRotaryActionWidget GetRotaryWidget(IRotaryFocusable focusable)
        {
            var consumer = focusable as BindableObject;
            IRotaryActionWidget rotaryWidget = null;
            if (consumer != null)
            {
                var consumerRenderer = Platform.GetRenderer(consumer);
                rotaryWidget = consumerRenderer?.NativeView as IRotaryActionWidget;
            }
            return rotaryWidget;
        }

        void ActivateRotaryWidget()
        {
            if (_currentRotaryFocusObject is IRotaryEventReceiver)
            {
                ERotaryEventManager.Rotated += OnRotaryEventChanged;
            }
            else if (_currentRotaryFocusObject is IRotaryFocusable)
            {
                GetRotaryWidget(_currentRotaryFocusObject)?.Activate();
            }
        }
        void DeactivateRotaryWidget()
        {
            if (_currentRotaryFocusObject is IRotaryEventReceiver)
            {
                ERotaryEventManager.Rotated -= OnRotaryEventChanged;
            }
            else if (_currentRotaryFocusObject is IRotaryFocusable)
            {
                GetRotaryWidget(_currentRotaryFocusObject)?.Deactivate();
            }
        }
        void OnRotaryEventChanged(ElmSharp.Wearable.RotaryEventArgs e)
        {
            if (_currentRotaryFocusObject is IRotaryEventReceiver)
            {
                var receiver = _currentRotaryFocusObject as IRotaryEventReceiver;
                receiver.Rotate(new RotaryEventArgs { IsClockwise = e.IsClockwise });
            }
        }
    }
}
