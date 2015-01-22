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
using System.Collections.Concurrent;
using Android.Graphics.Drawables;
using Android.Graphics;
using System.Net;
using System.Threading.Tasks;
using System.IO;
namespace CaterinaShopMobileApp
{
	class MemoryCacheBitmap
	{

		public event Action<BitmapItem> DownloadBitmapComplited; 

		const int QUEUE_LENGTH = 10;

		private static MemoryCacheBitmap instance = new MemoryCacheBitmap();

		ConcurrentQueue<BitmapItem> _cq=new ConcurrentQueue<BitmapItem>();
	
		private MemoryCacheBitmap()
		{
		}

		public static MemoryCacheBitmap GetInstance
		{ 
			get{ return instance;}
		}

		public Bitmap  GetDrawableFromQueue(string imagePath)
		{
			foreach (var i in _cq)
			{
				if (i.ImagePath == imagePath)
				{
					return i.Bitmap;
				}
			}

			return null;

		}

		public void GetDrawableFromWebSiteAsync(string imagePath)
		{

			Task<BitmapItem> t = new Task<BitmapItem> (()=>{
					using(WebClient wc =new WebClient())
					using(Stream s = wc.OpenRead(new Uri(imagePath)))
					{
						BitmapFactory.Options opt = new BitmapFactory.Options();
						opt.InPreferredConfig = Bitmap.Config.Rgb565;
						Bitmap b =BitmapFactory.DecodeStream(s,null,opt);
						if(_cq.Count==QUEUE_LENGTH){
							BitmapItem lastItem;
							_cq.TryDequeue(out lastItem);
							lastItem.Bitmap.Dispose();
						}
						BitmapItem newItem = new BitmapItem(b,imagePath);
						_cq.Enqueue(newItem);
						return newItem;
					}	
			});
			t.ContinueWith ((d)=>{ 
				if(d.Exception==null)
				{
					DownloadBitmapComplited(d.Result);
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
			t.Start ();
		}
	}

	class BitmapItem
	{

		private Bitmap _b;
		private string _imagePath;

		public Bitmap Bitmap
		{
			get{ return _b;  }
		}

		public string ImagePath
		{
			get{return _imagePath;}
		}

		public BitmapItem(Bitmap d,string imagePath)
		{
			_b = d;
			_imagePath = imagePath;

		}

	}


}

