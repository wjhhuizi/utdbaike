using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc.UI;
using System.IO;
using ccbs.Models;

namespace ccbs.Controllers
{
    public class ImageBrowserController : EditorFileBrowserController
    {

        private const int article_pic_width = 700;
        private const int article_pic_height = 1000;

        /// Gets the base paths from which content will be served.
        public override string[] ContentPaths
        {
            get
            {
                return new[] { "~/Photos/Baike", "~/Photos/News" };
            }
        }

        public override ActionResult Upload(string path, HttpPostedFileBase file)
        {

            var value = base.Upload(path, file);
            var physicalPath = Path.Combine(Server.MapPath(path), file.FileName);
            ImageOpt.ScaleImage(file, physicalPath, article_pic_width, article_pic_height);

            return value;
        }
    }
}
