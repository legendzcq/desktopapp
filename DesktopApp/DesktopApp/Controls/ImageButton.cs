using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DesktopApp.Controls
{
	public class ImageButton : Button
	{
		static ImageButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
		}

		public ImageButton()
		{
			Cursor = Cursors.Hand;
		}

		#region Dependency Properties

		public bool IsShowNotice
		{
			get { return (bool)GetValue(IsShowNoticeProperty); }
			set { SetValue(IsShowNoticeProperty, value); }
		}

		public static readonly DependencyProperty IsShowNoticeProperty =
			DependencyProperty.Register("IsShowNotice", typeof(bool), typeof(ImageButton),
			new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

		public double ImageWidth
		{
			get { return (double)GetValue(ImageWidthProperty); }
			set { SetValue(ImageWidthProperty, value); }
		}

		public static readonly DependencyProperty ImageWidthProperty =
			DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageButton),
			new FrameworkPropertyMetadata(27.0, FrameworkPropertyMetadataOptions.AffectsRender));

		public double ImageHeight
		{
			get { return (double)GetValue(ImageHeightProperty); }
			set { SetValue(ImageHeightProperty, value); }
		}

		public static readonly DependencyProperty ImageHeightProperty =
			DependencyProperty.Register("ImageHeight", typeof(double), typeof(ImageButton),
			new FrameworkPropertyMetadata(27.0, FrameworkPropertyMetadataOptions.AffectsRender));

		public string NormalImage
		{
			get { return (string)GetValue(NormalImageProperty); }
			set { SetValue(NormalImageProperty, value); }
		}

		public static readonly DependencyProperty NormalImageProperty =
			DependencyProperty.Register("NormalImage", typeof(string), typeof(ImageButton),
			new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender, ImageSourceChanged));

		public string HoverImage
		{
			get { return (string)GetValue(HoverImageProperty); }
			set { SetValue(HoverImageProperty, value); }
		}

		public static readonly DependencyProperty HoverImageProperty =
			DependencyProperty.Register("HoverImage", typeof(string), typeof(ImageButton),
			new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender, ImageSourceChanged));

		public string PressedImage
		{
			get { return (string)GetValue(PressedImageProperty); }
			set { SetValue(PressedImageProperty, value); }
		}

		public static readonly DependencyProperty PressedImageProperty =
			DependencyProperty.Register("PressedImage", typeof(string), typeof(ImageButton),
			new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender, ImageSourceChanged));

		public string DisabledImage
		{
			get { return (string)GetValue(DisabledImageProperty); }
			set { SetValue(DisabledImageProperty, value); }
		}

		public static readonly DependencyProperty DisabledImageProperty =
			DependencyProperty.Register("DisabledImage", typeof(string), typeof(ImageButton),
			new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender, ImageSourceChanged));

		private static void ImageSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			Application.GetResourceStream(new Uri("pack://application:,,," + (string)e.NewValue));
		}

		public Visibility BorderVisibility
		{
			get { return (Visibility)GetValue(BorderVisibilityProperty); }
			set { SetValue(BorderVisibilityProperty, value); }
		}

		public static readonly DependencyProperty BorderVisibilityProperty =
			DependencyProperty.Register("BorderVisibility", typeof(Visibility), typeof(ImageButton),
			new FrameworkPropertyMetadata(Visibility.Hidden, FrameworkPropertyMetadataOptions.AffectsRender));

		#endregion
	}
}
