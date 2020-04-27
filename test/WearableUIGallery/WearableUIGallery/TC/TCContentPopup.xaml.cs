/*
 * Copyright (c) 2018 Samsung Electronics Co., Ltd All Rights Reserved
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
using System.Threading.Tasks;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WearableUIGallery.TC
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TCContentPopupTest : ContentPage
    {
        public TCContentPopupTest()
        {
            InitializeComponent();
        }

        private async void OnContentPopupTest1Clicked(object sender, EventArgs e)
        {
            using (ContentPopup _popup = new ContentPopup())
            {
                var label = new Label
                {
                    Text = "This ContentPopup is dismissed as a back key.",
                    HorizontalTextAlignment = TextAlignment.Center,
                };

                _popup.BackButtonPressed += (s, ee) =>
                {
                    _popup?.Dismiss();
                    label1.Text = "Test1 Dismissed";
                };
                _popup.Content = label;

                //TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                //_popup.Dismissed += (s, evt) => tcs.SetResult(true);
                await _popup.Open();
            }
        }

        private async void OnContentPopupTest2Clicked(object sender, EventArgs e)
        {
            using (ContentPopup _popup = new ContentPopup())
            {
                var dismiss = new Button
                {
                    Text = "Dismiss",
                };
                dismiss.Clicked += (s, ee) =>
                {
                    _popup?.Dismiss();
                    label1.Text = "Test2 Dismissed";
                };
                var label = new Label
                {
                    Text = "This ContentPopup is dismissed as a below dismiss button.",
                    HorizontalTextAlignment = TextAlignment.Center,
                };

                var grid = new Grid();
                grid.HeightRequest = 360;
                grid.WidthRequest = 360;
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.Children.Add(label, 0, 1, 1, 3);
                grid.Children.Add(dismiss, 0, 1, 3, 4);

                _popup.Content = grid;

                await _popup.Open();
            }
        }
    }
}