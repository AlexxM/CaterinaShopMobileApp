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
using Android.Support.V4.View;
namespace CaterinaShopMobileApp
{
	//класс анимации для элемента ViewPager
	class ViewPagerAnimation : Java.Lang.Object,ViewPager.IPageTransformer
	{
		private const float MIN_SCALE = 0.75f;
		public void TransformPage (View p0, float p1)
		{
			int pageWidth = p0.Width;
			int pageHeight = p0.Height;
			if (p1 < -1)
			{
				p0.Alpha = 0;
			} 
			else if(p1 <= 0)
			{
				p0.Alpha = 1;
				p0.TranslationX = 0;
				p0.ScaleX = 1;
				p0.ScaleY = 1;
			}
			else if (p1 <= 1)
			{
				float scale = MIN_SCALE + (1-MIN_SCALE)*(1-Math.Abs(p1));
				p0.ScaleX = scale;
				p0.ScaleY = scale;
				p0.Alpha = Math.Abs (1-p1);
				p0.TranslationX = pageWidth * -p1;
			}
			else
			{
				p0.Alpha = 0;
			}
		}
	}
}

