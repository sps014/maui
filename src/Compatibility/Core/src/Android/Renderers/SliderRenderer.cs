﻿using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Widget;
using Microsoft.Maui.Controls.Platform;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Android
{
	public class SliderRenderer : ViewRenderer<Slider, SeekBar>, SeekBar.IOnSeekBarChangeListener
	{
		double _max, _min;
		ColorStateList defaultprogresstintlist, defaultprogressbackgroundtintlist;
		ColorFilter defaultthumbcolorfilter;
		Drawable defaultthumb;
		PorterDuff.Mode defaultprogresstintmode, defaultprogressbackgroundtintmode;
		bool _isDisposed;

		public SliderRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}

		[PortHandler]
		double Value
		{
			get { return _min + (_max - _min) * (Control.Progress / 1000.0); }
			set { Control.Progress = (int)((value - _min) / (_max - _min) * 1000.0); }
		}

		[PortHandler]
		void SeekBar.IOnSeekBarChangeListener.OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
		{
			if (fromUser)
				((IElementController)Element).SetValueFromRenderer(Slider.ValueProperty, Value);
		}

		[PortHandler]
		void SeekBar.IOnSeekBarChangeListener.OnStartTrackingTouch(SeekBar seekBar)
		{
			((ISliderController)Element)?.SendDragStarted();
		}

		[PortHandler]
		void SeekBar.IOnSeekBarChangeListener.OnStopTrackingTouch(SeekBar seekBar)
		{
			((ISliderController)Element)?.SendDragCompleted();
		}

		protected override SeekBar CreateNativeControl()
		{
			return new FormsSeekBar(Context);
		}

		[PortHandler]
		protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				var seekBar = CreateNativeControl();
				SetNativeControl(seekBar);

				seekBar.Max = 1000;
				seekBar.SetOnSeekBarChangeListener(this);

				defaultthumbcolorfilter = seekBar.Thumb.GetColorFilter();
				defaultprogresstintmode = seekBar.ProgressTintMode;
				defaultprogressbackgroundtintmode = seekBar.ProgressBackgroundTintMode;
				defaultprogresstintlist = seekBar.ProgressTintList;
				defaultprogressbackgroundtintlist = seekBar.ProgressBackgroundTintList;
				defaultthumb = seekBar.Thumb;
			}

			Slider slider = e.NewElement;
			_min = slider.Minimum;
			_max = slider.Maximum;
			Value = slider.Value;
			UpdateSliderColors();
		}

		SeekBar NativeSeekbar
		{
			get { return Control; }
		}

		[PortHandler]
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.IsDisposed())
			{
				return;
			}

			base.OnElementPropertyChanged(sender, e);

			Slider view = Element;
			switch (e.PropertyName)
			{
				case "Maximum":
					_max = view.Maximum;
					break;
				case "Minimum":
					_min = view.Minimum;
					break;
				case "Value":
					if (Value != view.Value)
						Value = view.Value;
					break;
			}

			if (e.PropertyName == Slider.MinimumTrackColorProperty.PropertyName)
				UpdateMinimumTrackColor();
			else if (e.PropertyName == Slider.MaximumTrackColorProperty.PropertyName)
				UpdateMaximumTrackColor();
			else if (e.PropertyName == Slider.ThumbImageSourceProperty.PropertyName)
				UpdateThumbImage();
			else if (e.PropertyName == Slider.ThumbColorProperty.PropertyName)
				UpdateThumbColor();
		}

		private void UpdateSliderColors()
		{
			UpdateMinimumTrackColor();
			UpdateMaximumTrackColor();
			var thumbImage = Element.ThumbImageSource;
			if (thumbImage != null && !thumbImage.IsEmpty)
			{
				UpdateThumbImage();
			}
			else
			{
				UpdateThumbColor();
			}
		}

		[PortHandler]
		private void UpdateMinimumTrackColor()
		{
			if (Element != null)
			{
				if (Element.MinimumTrackColor == null)
				{
					Control.ProgressTintList = defaultprogresstintlist;
					Control.ProgressTintMode = defaultprogresstintmode;
				}
				else
				{
					Control.ProgressTintList = ColorStateList.ValueOf(Element.MinimumTrackColor.ToAndroid());
					Control.ProgressTintMode = PorterDuff.Mode.SrcIn;
				}
			}
		}

		[PortHandler]
		private void UpdateMaximumTrackColor()
		{
			if (Element != null)
			{
				if (Element.MaximumTrackColor == null)
				{
					Control.ProgressBackgroundTintList = defaultprogressbackgroundtintlist;
					Control.ProgressBackgroundTintMode = defaultprogressbackgroundtintmode;
				}
				else
				{
					Control.ProgressBackgroundTintList = ColorStateList.ValueOf(Element.MaximumTrackColor.ToAndroid());
					Control.ProgressBackgroundTintMode = PorterDuff.Mode.SrcIn;
				}
			}
		}

		[PortHandler]
		void UpdateThumbColor()
		{
			Control.Thumb.SetColorFilter(Element.ThumbColor, FilterMode.SrcIn, defaultthumbcolorfilter);
		}

		void UpdateThumbImage()
		{
			this.ApplyDrawableAsync(Slider.ThumbImageSourceProperty, Context, drawable =>
			{
				Control.SetThumb(drawable ?? defaultthumb);
			});
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);

			// Thumb only supported JellyBean and higher

			if (Control == null)
				return;

			SeekBar seekbar = Control;

			Drawable thumb = seekbar.Thumb;
			int thumbTop = seekbar.Height / 2 - thumb.IntrinsicHeight / 2;

			thumb.SetBounds(thumb.Bounds.Left, thumbTop, thumb.Bounds.Left + thumb.IntrinsicWidth, thumbTop + thumb.IntrinsicHeight);
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			_isDisposed = true;

			if (disposing)
			{
				if (Control.IsAlive())
				{
					Control.SetOnSeekBarChangeListener(null);
				}
			}

			base.Dispose(disposing);
		}
	}
}
