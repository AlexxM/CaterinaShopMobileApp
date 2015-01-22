using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
namespace CaterinaShopMobileApp
{
	public class ScrollableImageView : ImageView
	{
		private GestureDetector _gd;
		private OverScroller _scroller;
		private ScaleGestureDetector _sgd;

		private float _scaleFactor = 1;

		private readonly float _minScaleFactor;
		private readonly float _maxScaleFactor;
		
		readonly int _screenWidth;
		readonly int _screenHeight;



		private int _positionX;
		private int _positionY;
		
		//кол-во гориз. пикселей изобр за пределом видимости
		private int MaxHorizontal
		{
			get{ 
				int output = (int)(Drawable.Bounds.Width () * _scaleFactor - _screenWidth);
				if (output < 0)
				{
					return 0;
				}
				return output;
			}
		}

		//кол-во верт. пикселей изобр за пределом видимости
		private int MaxVertical
		{
			get
			{
				int output= (int)(Drawable.Bounds.Height() * _scaleFactor- _screenHeight);
				if (output < 0)
				{
					return 0;
				}
				return output;
			}
		}

		public ScrollableImageView(Context c,Android.Util.IAttributeSet attribute,int defStyle) : base(c,attribute,defStyle)
		{
			DisplayMetrics dm = Resources.DisplayMetrics;
			_screenWidth = dm.WidthPixels;
			_screenHeight = dm.HeightPixels;
			_gd = new GestureDetector (c,new GestureListener(this));
			_sgd = new ScaleGestureDetector (c,new ScaleGestureListener(this));
			_scroller = new OverScroller (c);

			//в xml разметке элемента можно определять коэф масштабирования
			TypedArray ta = c.ObtainStyledAttributes (attribute, Resource.Styleable.ScrollableImageView);
			if(_minScaleFactor==0)
				_minScaleFactor = ta.GetFloat (Resource.Styleable.ScrollableImageView_minScaleFactor, 0.5f);
			if (_minScaleFactor > 1)
				_scaleFactor = _minScaleFactor;
			if(_maxScaleFactor==0)
				_maxScaleFactor = ta.GetFloat (Resource.Styleable.ScrollableImageView_maxScaleFactor, 1.5f);

		}

		public ScrollableImageView(Context c) : this(c,null)
		{

		}

		public ScrollableImageView(Context c,float minScaleFactor,float maxScaleFactor) : this(c,null)
		{
			_minScaleFactor = minScaleFactor;
			_maxScaleFactor = maxScaleFactor;
		}

		public ScrollableImageView(Context c,IAttributeSet attribute) : this(c,attribute,0)
		{

		}

		protected override void OnDraw (Android.Graphics.Canvas canvas)
		{
			int offsetX=0;
			int offsetY=0;
			int currImageWidth = (int)(Drawable.Bounds.Width() * _scaleFactor);
			int currImageHeight = (int)(Drawable.Bounds.Height() * _scaleFactor);
			if (currImageWidth < _screenWidth)
				offsetX = _screenWidth / 2 - currImageWidth / 2;
			if (currImageHeight < _screenHeight)
				offsetY = _screenHeight / 2 - currImageHeight / 2;
			canvas.Translate (offsetX,offsetY);

			canvas.Scale (_scaleFactor, _scaleFactor);

			Drawable.Draw (canvas);
		}

		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged (w, h, oldw, oldh);
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			_sgd.OnTouchEvent (e);
			_gd.OnTouchEvent (e);
			return true;
		}

		public override void ComputeScroll ()
		{
			base.ComputeScroll ();

			if (_scroller.ComputeScrollOffset ())
			{
				_positionX = _scroller.CurrX;
				_positionY = _scroller.CurrY;
				ScrollTo (_positionX, _positionY);
			}
			else
			{
				_scroller.SpringBack (_positionX,_positionY,0,MaxHorizontal,0,MaxVertical);

			}

		}

		//корректировка новой позиции смещения для избежания выхода изобр за пределы экрана
		public void CheckNewScrollPosition(ref int dx,ref int dy)
		{
			int newPositionX = _positionX + dx;
			int newPositionY = _positionY + dy;

			if (newPositionX < 0)
			{
				dx = - _positionX;
			}
			else if (newPositionX > MaxHorizontal)
			{
				dx = MaxHorizontal - _positionX;
			}

			if (newPositionY < 0)
			{
				dy = - _positionY;
			}
			else if (newPositionY > MaxVertical)
			{
				dy = MaxVertical - _positionY;
			}
		}

		class ScaleGestureListener : ScaleGestureDetector.SimpleOnScaleGestureListener
		{
			ScrollableImageView _siv;

			public ScaleGestureListener(ScrollableImageView siv) : base()
			{
				_siv=siv;
			}

			public override bool OnScale (ScaleGestureDetector detector)
			{

				_siv._scaleFactor *= detector.ScaleFactor;
				_siv._scaleFactor = Math.Max(_siv._minScaleFactor,Math.Min (_siv._scaleFactor, _siv._maxScaleFactor));

				//dx и dy делят на 3 для плавного скрола при большом масштабировании
				int dx = (int)(detector.FocusX - _siv._screenWidth / 2)/3;
				int dy = (int)(detector.FocusY - _siv._screenHeight / 2)/3;

				_siv.CheckNewScrollPosition (ref dx,ref dy);
				_siv._scroller.StartScroll (_siv._positionX,_siv._positionY,dx,dy,0);

				_siv.ComputeScroll ();
				_siv.Invalidate ();

				return true;
			}
		}

		class GestureListener : GestureDetector.SimpleOnGestureListener
		{
			ScrollableImageView _siv;

			public GestureListener(ScrollableImageView siv) : base()
			{
				_siv=siv;

			}


			public override bool OnDown (MotionEvent e)
			{
				_siv._scroller.ForceFinished (true);
				_siv.Invalidate();
				return true;
			}

			public override bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
			{
				_siv._scroller.ForceFinished (true);
				_siv._scroller.Fling (_siv._positionX,_siv._positionY,(int)-velocityX,(int)-velocityY,0,_siv.MaxHorizontal,0,_siv.MaxVertical);
				_siv.Invalidate ();
				
				return true;
			}

			public override bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
			{
				int dx = (int)distanceX;
				int dy = (int)distanceY;
			
				_siv.CheckNewScrollPosition (ref dx,ref dy);
				_siv._scroller.StartScroll (_siv._positionX,_siv._positionY,dx,dy,0);
				_siv.Invalidate();

				return true;
			
			}

		}
	}
}

