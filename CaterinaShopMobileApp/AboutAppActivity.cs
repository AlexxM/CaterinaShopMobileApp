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
	[Activity (Label = "@string/aboutAppTitle",Theme="@style/Theme.Holo.Light.Dialog.Green")]			
	public class AboutAppActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			AppHelpers.SetHoloTitleDivider (this.Window.DecorView, "#9931c434", "titleDivider");
			SetContentView (Resource.Layout.AboutApp);
		}
	}
}

