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

namespace CaterinaShopMobileApp
{
	static class AppHelpers
	{
		public static void SetHoloTitleDivider(View v, string color, string idName,string packages="android")
		{
			int x = v.Resources.GetIdentifier(idName,"id",packages);
			View titleDivider = v.FindViewById (x);
			titleDivider.SetBackgroundColor (Android.Graphics.Color.ParseColor(color));
		}
	
	}
}

