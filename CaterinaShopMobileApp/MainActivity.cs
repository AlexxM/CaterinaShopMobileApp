using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Java.Util;
using SupportFrag = Android.Support.V4.App.Fragment;
using SupportFragManager = Android.Support.V4.App.FragmentManager;
namespace CaterinaShopMobileApp
{
	[Activity(Label="@string/mainTitle", MainLauncher = true, Icon = "@drawable/shopIcon")]
    public partial class MainActivity : FragmentActivity,ActionBar.IOnNavigationListener,ViewPager.IOnPageChangeListener
    {
		string _host="www.katerina-serp.ru";
		string _query="route=feed/mobile_app";

		ViewPager _viewPager;
		MyFragmentPagerAdapter _pagerAdapter;
	    XMLFeedReaderAsync _xmlFeedAsync;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);  
			ActionBar bar = base.ActionBar;
			bar.NavigationMode = ActionBarNavigationMode.List;
			PagerTabStrip pts = FindViewById<PagerTabStrip>(Resource.Id.pagerTabStrip);
			pts.TabIndicatorColor=Android.Graphics.Color.ParseColor("#31c434");
			base.ActionBar.Title=Resources.GetString(Resource.String.actionBarTitle);
			string httpString = GetHttpRequestString ();
			_xmlFeedAsync = new XMLFeedReaderAsync (httpString,this);
			_xmlFeedAsync.Execute ();
			string s  = typeof(MainActivity).AssemblyQualifiedName;
		}

		private string GetHttpRequestString()
		{
			string lang = Resources.Configuration.Locale.Language;
			return "http://" + this._host + "?" + this._query + "&lang=" + lang;

		}
        
		//обработчик меню ActionBar (при нажатии на единственный пункт вызывается всплывающее меню - PopupMenu)
		public override bool OnMenuItemSelected (int featureId, IMenuItem item)
    	{
			if(item.ItemId==Resource.Id.itemPopupMenu)
			{
				PopupMenu popup=new PopupMenu(this,FindViewById(Resource.Id.itemPopupMenu));
				MenuInflater.Inflate(Resource.Menu.Popup_menu,popup.Menu);
				popup.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs e) => { 
					if(e.Item.ItemId == Resource.Id.itemShowShopWebSite)
					{
						string uriStr=GetHttpRequestString();
						uriStr = uriStr.Substring(0,uriStr.IndexOf('?'));
						Android.Net.Uri uri = Android.Net.Uri.Parse(uriStr);
						Intent intent  = new Intent(Intent.ActionView,uri);
						StartActivity(intent);
					}
					else if(e.Item.ItemId == Resource.Id.itemShowAbout)
					{
						Intent intent = new Intent(this,typeof(AboutAppActivity));
						StartActivity(intent);
					}
				};
				popup.Show ();
			}
			return true;
    	}

		//задание элементов меню actionBar
		public override bool OnCreateOptionsMenu(IMenu menu)
        {

			base.MenuInflater.Inflate(Resource.Menu.Main_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }


		//задание адаптера данных при выборе из списка actionbar
		public bool OnNavigationItemSelected(int pos,long id)
		{
			_pagerAdapter = new MyFragmentPagerAdapter (_xmlFeedAsync.productCategoryDict.ElementAt(pos).Value,SupportFragmentManager);
			_viewPager.Adapter = _pagerAdapter;
			_viewPager.Invalidate ();
			return true;
		}
		

		public void OnPageScrolled (int p0, float p1, int p2)
    	{
    		
    	}

    	public void OnPageSelected (int p0)
    	{
    		
    	}

		public void OnPageScrollStateChanged(int state)
		{

		}

		class MyFragmentPagerAdapter : FragmentStatePagerAdapter
		{
			private int _pageCount;
			private ProductItem[] _products;
			public MyFragmentPagerAdapter(ProductItem[] products, SupportFragManager fm) : base(fm)
			{
				_products=products;
				_pageCount = products.Length;
			}

			public override int Count {
				get {
					return _pageCount;
				}
			}

			public void SetProductsItems(ProductItem[] products)
			{
				_products = products;
				_pageCount = products.Length;
			}
		
			public override Java.Lang.ICharSequence GetPageTitleFormatted (int index)
			{
				return  new Java.Lang.String (_products[index].Name);
			}

			public override SupportFrag GetItem (int position)
			{
				return ViewPagerFragment.NewInstance (position,_products[position]);
			}

		}

    }
}

