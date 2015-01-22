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

	class ProductItem
	{
		string _name;
		string _price;
		string _imagePath;
		Tuple<string,string>[] _attributes;
		string[] _sizes;

		public string Name
		{
			get{ return _name;}
			set{ _name=value;}
		}

		public string Price
		{
			get{ return _price;}
			set{_price = value;}
		}

		public string ImagePath
		{
			get{return _imagePath;}
			set{_imagePath = value;}
		}

		public Tuple<string,string>[] Attributes
		{
			get{return _attributes;}
			set{_attributes = value;}
		}

		public string[] Sizes
		{
			get{return _sizes;}
			set{_sizes = value;}
		}

		public ProductItem()
		{
		}

		public ProductItem(string name,string price,string imagePath,Tuple<string,string>[] attributes,string[] sizes)
		{
			_name = name;
			_price = price;
			_imagePath = imagePath;
			_attributes = attributes;
			_sizes = sizes;
		}
	
	}
}

