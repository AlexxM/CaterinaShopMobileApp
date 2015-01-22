using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Util;
namespace CaterinaShopMobileApp
{
	[Activity (Theme="@android:style/Theme.Light.NoTitleBar.Fullscreen", Label = "ShowDetailActivity")]			
	class ShowDetailActivity : Activity
	{
	
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);


			SetContentView (Resource.Layout.ShowDetail);
			string imagePath = this.Intent.GetStringExtra("imagePath");

			Bitmap b = MemoryCacheBitmap.GetInstance.GetDrawableFromQueue (imagePath);
			ScrollableImageView siv = FindViewById<ScrollableImageView> (Resource.Id.scrollableImageView);
			siv.SetImageBitmap (b);

		}

	}

}

