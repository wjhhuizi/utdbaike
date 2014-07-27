using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Web.SessionState;

namespace ccbs.Helpers
{
	public class SessionHelper
	{
		private static HttpSessionState Session
		{
			get
			{
				return HttpContext.Current.Session;
			}
		}

		/// <summary>
		/// Gets the session culture.
		/// This allow us to choose the right language for the pages
		/// </summary>
		public static CultureInfo Culture
		{
			get
			{
				return (CultureInfo) Session["Culture"];
			}
			set
			{
				Session["Culture"] = value;
			}
		}
	}
}