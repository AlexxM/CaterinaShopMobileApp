using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Views.Animations;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace CaterinaShopMobileApp
{
	class ViewPagerFragment : Fragment,View.IOnTouchListener
	{
	   
		ProductItem _pi;

		long _firstTouchImageTime=0;

		public static ViewPagerFragment NewInstance(int pageNum,ProductItem pi)
		{
			ViewPagerFragment vpf = new ViewPagerFragment ();
			vpf._pi=pi;
			return vpf;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View vg = inflater.Inflate(Resource.Layout.ViewPagerFragment,null);
			SetProductData (vg);
			SetProductImage (vg);

			return vg;
		}

		private void SetProductData(View v)
		{
			TextView tv = v.FindViewById<TextView> (Resource.Id.lblPriceText);
			tv.Text = _pi.Price;

			v.FindViewById<TextView> (Resource.Id.lblSizesText).Text = String.Join (",", _pi.Sizes);
			foreach(var t in _pi.Attributes)
			{
				switch (t.Item1)
				{
				case "manufacture":
					v.FindViewById<TextView> (Resource.Id.lblManufactureText).Text = t.Item2;
					break;
				case "cloth":
					v.FindViewById<TextView>(Resource.Id.lblClothText).Text = t.Item2;
					break;
				case "color":
					v.FindViewById<TextView>(Resource.Id.lblColorText).Text = t.Item2;
					break; 
				}

			}

		}

		private void SetProductImage(View v)
		{
			Bitmap b = MemoryCacheBitmap.GetInstance.GetDrawableFromQueue (_pi.ImagePath);
			if (b != null) {
				ImageView iv = v.FindViewById<ImageView> (Resource.Id.imageView);
				iv.SetScaleType(ImageView.ScaleType.FitCenter);
				iv.SetImageBitmap (b);
				iv.SetOnTouchListener (this);
			}
			else
			{
				MemoryCacheBitmap.GetInstance.GetDrawableFromWebSiteAsync (_pi.ImagePath);
				MemoryCacheBitmap.GetInstance.DownloadBitmapComplited += OnDownloadDrawableComplited;
			}


		}
	

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			MemoryCacheBitmap.GetInstance.DownloadBitmapComplited -= OnDownloadDrawableComplited;
		}

		private void  OnDownloadDrawableComplited(BitmapItem bi)
		{
			if(View!=null && bi.ImagePath==_pi.ImagePath)
			{
				ImageView iv = View.FindViewById<ImageView> (Resource.Id.imageView);
				iv.SetScaleType(ImageView.ScaleType.FitCenter);
				iv.SetImageBitmap (bi.Bitmap);
				iv.SetOnTouchListener (this);

			}
		}

		public bool OnTouch (View v, MotionEvent e)
		{

			if (e.Action == MotionEventActions.Down)
			{

				if(e.EventTime - _firstTouchImageTime <= 300)
				{
					_firstTouchImageTime = 0;
					Intent intent = new Intent(this.Activity.BaseContext,typeof(ShowDetailActivity));
					intent.PutExtra ("imagePath", _pi.ImagePath);
					StartActivity(intent);
				}
				else
				{
					_firstTouchImageTime = e.EventTime;
				} 

			}

			return true;
		}



	}



}

