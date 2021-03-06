﻿using Android.Content;
using QRCode.Controls;
using QRCode.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MyEditor), typeof(MyEditorRenderer))]
namespace QRCode.Droid
{
    class MyEditorRenderer : EditorRenderer
    {
        public MyEditorRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SetBackgroundColor(global::Android.Graphics.Color.Transparent);
                Control.SetPadding(0, 0, 0, 0);
                Control.TextAlignment = Android.Views.TextAlignment.Center;
                //Control.SetShadowLayer(10, 5, 5, global::Android.Graphics.Color.LightGray);
            }
        }
    }
}