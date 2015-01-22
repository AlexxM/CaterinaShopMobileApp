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
using Android.Net;
using System.IO;
using System.Xml.Linq;
using System.Globalization;
using System.Net;
using Android.Support.V4.View;
namespace CaterinaShopMobileApp
{
	public partial class MainActivity{
    class XMLFeedReaderAsync : AsyncTask
	{
		System.Uri _uri;
		MainActivity _activity;
	    ProgressDialog _pg;
		public string lang=String.Empty;
		public Dictionary<string,ProductItem[]> productCategoryDict;

		public XMLFeedReaderAsync(string url,MainActivity activity)
		{

			_uri = new System.Uri (url);
			_activity = activity;
			
		
		

		}

		protected override void OnPreExecute ()
		{
				base.OnPreExecute ();
				_pg = new ProgressDialog(_activity);
				_pg.SetProgressStyle (ProgressDialogStyle.Spinner);
				_pg.SetMessage ( _activity.GetString (Resource.String.progressDlgMessage));
				_pg.Show();
		}

		private bool CheckInternetConnection()
		{
			ConnectivityManager cm = (ConnectivityManager)_activity.GetSystemService (Service.ConnectivityService);
			foreach(NetworkInfo ni in cm.GetAllNetworkInfo())
			{
				if (ni.GetState() == NetworkInfo.State.Connected)
				{
					return true;
				}

			}
			return false;

		}

		protected override Java.Lang.Object DoInBackground (params Java.Lang.Object[] @params)
		{
			try
			{
				if (!CheckInternetConnection ())
					throw new InvalidOperationException ("Нет подключения");
				using(WebClient wc = new System.Net.WebClient ())
				using(Stream s = wc.OpenRead (_uri))
				{
					using(StreamReader sr = new StreamReader (s))
					{
						string data = sr.ReadToEnd ();
					
						productCategoryDict = ParseXML (data);
						return data;
					}
				}
			}
			catch
			{
				return null;
			}
				
		}

		protected override void OnPostExecute (Java.Lang.Object result)
		{
			try
			{
				base.OnPostExecute (result);
				_pg.Hide();		
				if (result == null)
					throw new InvalidOperationException ("Не удалось получить xml список товара");
				ArrayAdapter<string> adapt =  new ArrayAdapter<string>(_activity,Android.Resource.Layout.SimpleSpinnerDropDownItem,this.productCategoryDict.Keys.Select((i)=> i).ToArray());
				_activity.ActionBar.SetListNavigationCallbacks (adapt,_activity);
				//можно перенести
				_activity._viewPager = _activity.FindViewById<ViewPager> (Resource.Id.pager);
				_activity._viewPager.SetPageTransformer(true,new ViewPagerAnimation());
			}
			catch
			{
					AlertDialog.Builder dlg = new AlertDialog.Builder(_activity);
					dlg.SetTitle (Resource.String.errorDlgTitle);
					dlg.SetCancelable (false);
					dlg.SetPositiveButton ("OK", (e,i) => { 
						_activity.Finish();
					});
					AlertDialog dialogView = dlg.Show ();
					AppHelpers.SetHoloTitleDivider (dialogView.Window.DecorView, "#9931c434", "titleDividerTop");
			}
		}
		
		private Dictionary<string,ProductItem[]> ParseXML(string inputXML)
		{
			XElement root = XElement.Parse (inputXML);
			lang = root.Attribute ("lang").Value;
			Dictionary<string,ProductItem[]> categoryDictonary = new Dictionary<string, ProductItem[]> ();
			foreach (XElement category in root.Elements())
			{
				int cntItem = category.Elements ().Count();
				ProductItem[] items = new ProductItem[cntItem];
				int i = 0;
				foreach (XElement productItem in category.Elements())
				{
					ProductItem item = new ProductItem ();
					item.Name = productItem.Attribute("name").Value;
					item.Price = productItem.Attribute ("price").Value;
					item.ImagePath = productItem.Element("image").Value;
					item.Sizes = productItem.Element ("sizes").Elements ().Select ((e)=>{ return e.Value;}).ToArray();
					item.Attributes  = productItem.Element ("attributes").Elements ().Select ((e)=>{return new Tuple<string,string>(e.Attribute("attribName").Value,e.Value);}).ToArray(); 
					items [i] = item;
					i++;
				}
				categoryDictonary.Add (category.Attribute("name").Value, items);
			}

			return categoryDictonary;

		}

	}
	}
}

