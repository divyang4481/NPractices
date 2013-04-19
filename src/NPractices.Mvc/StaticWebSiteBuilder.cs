using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace NPractices.Mvc
{
    /// <summary>
    /// A builder that could output the whole mvc site to a static one
    /// </summary>
    public class StaticWebSiteBuilder
    {
        private readonly HttpContextBase _httpContext;
        private readonly DirectoryInfo _outputFolder;
        private readonly string _outputVirtualPath;
        private readonly HttpRequestBase _request;
        private readonly HttpServerUtilityBase _server;
        private readonly UrlHelper _urlHelper;

        public StaticWebSiteBuilder(HttpContextBase httpContext, UrlHelper urlHelper,
                                    string outputVirtualPath = "~/output")
        {
            _httpContext = httpContext;
            _urlHelper = urlHelper;
            _server = httpContext.Server;
            _request = httpContext.Request;
            _outputVirtualPath = outputVirtualPath;
            _outputFolder = new DirectoryInfo(_server.MapPath(_outputVirtualPath));
        }

        public string Build(string viewsRootVirtualPath = "~/Views", string imagesRootVirtualPath = "~/Content")
        {
            var sb = new StringBuilder();
            sb.AppendFormat("output: {0} \r\n", _outputFolder.FullName);

            //1. cleanup
            var watch = Stopwatch.StartNew();
            sb.AppendLine("1. cleanup output...");
            if (_outputFolder.Exists)
                _outputFolder.Delete(true);

            _outputFolder.Create();

            watch.Stop();
            sb.AppendFormat("1. cleanup output completed in {0:N0} ms \r\n", watch.ElapsedMilliseconds);

            //2. copy images
            watch.Restart();
            sb.AppendLine("2. copying images...");
            var srcContentFolder = new DirectoryInfo(_server.MapPath(imagesRootVirtualPath));
            var destContentFolder = _outputFolder.CreateSubdirectory(srcContentFolder.Name);
            CopyTo(srcContentFolder, destContentFolder.FullName, "*.png", "*.gif", "*.jpg");

            watch.Stop();
            sb.AppendFormat("2. copy images completed in {0:N0} ms \r\n", watch.ElapsedMilliseconds);

            //3. build bundles
            watch.Restart();
            sb.AppendLine("3. build bundles...");
            BuildBundles();
            watch.Stop();
            sb.AppendFormat("3. build bundles completed in {0:N0} ms \r\n", watch.ElapsedMilliseconds);


            //4. build views
            watch.Restart();
            sb.AppendLine("4. start building views...");
            BuildViews(viewsRootVirtualPath);
            watch.Stop();
            sb.AppendFormat("4. build views completed in {0:N0} ms \r\n", watch.ElapsedMilliseconds);

            return sb.ToString();
        }

        private void BuildBundles()
        {
            foreach (var bundle in BundleTable.Bundles.GetRegisteredBundles())
            {
                var bundleContext = new BundleContext(_httpContext, BundleTable.Bundles, bundle.Path);
                var f = new FileInfo(_server.MapPath(_outputVirtualPath + bundle.Path.TrimStart('~')));
                if (!f.Directory.Exists)
                    f.Directory.Create();
                using (var sw = f.CreateText())
                    sw.Write(bundle.Builder.BuildBundleContent(bundle, bundleContext,
                                                               bundle.GenerateBundleResponse(bundleContext).Files));
            }
        }

        private void BuildViews(string viewsVirtualPath)
        {
            var viewFiles = GetViewFiles(viewsVirtualPath).ToList();

            viewFiles.Where(z => !z.IsPartial).ToList().ForEach(BuildView);
        }

        private void BuildView(ViewFile viewFile)
        {
            string id = Path.GetFileNameWithoutExtension(viewFile.Name).ToLower();
            var f =
                new FileInfo(
                    Path.Combine(_outputFolder.FullName, Path.ChangeExtension(viewFile.Name, ".html"))
                        .ToLower());
            if (f.Exists)
                f.Delete();

            string url = _urlHelper.RouteUrl(new {id});
            string urlRoot = _urlHelper.RouteUrl(new {});
            Regex rxAbsPath = new Regex(string.Format("\"{0}(.+)\"", urlRoot));

            var u = new Uri(_request.Url, url);
            using (var res = WebRequest.Create(u).GetResponse())
            using (var sr = new StreamReader(res.GetResponseStream()))
            using (var sw = f.CreateText())
            {
                string html = sr.ReadToEnd();
                html = rxAbsPath.Replace(html, "\"$1\"");
                sw.Write(html);
            }
        }

        private IEnumerable<ViewFile> GetViewFiles(string viewsVirtualPath)
        {
            var dir = new DirectoryInfo(_server.MapPath(viewsVirtualPath));
            var views = (from f in dir.GetFiles("*.cshtml")
                         select new ViewFile
                                    {
                                        Name = f.Name,
                                        VirtualPath = viewsVirtualPath + "/" + f.Name,
                                        Content = File.ReadAllText(f.FullName)
                                    }).ToList();

            var subViews = from d in dir.GetDirectories()
                           let subPath = viewsVirtualPath + "/" + d.Name
                           from v in GetViewFiles(subPath)
                           select v;

            views.AddRange(subViews);

            return views;
        }

        private static void CopyTo(DirectoryInfo sourceFolder, string destFolder, params string[] searchPatterns)
        {
            var files = searchPatterns.SelectMany(sourceFolder.GetFiles).ToArray();
            if (files.Length > 0 && !Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            foreach (var file in files)
            {
                var dest = Path.Combine(destFolder, file.Name);
                file.CopyTo(dest, true);
            }
            var folders = sourceFolder.GetDirectories();
            foreach (var folder in folders)
            {
                var dest = Path.Combine(destFolder, folder.Name);
                CopyTo(folder, dest, searchPatterns);
            }
        }

        private class ViewFile
        {
            public string Name { get; set; }
            public string VirtualPath { get; set; }
            public string Content { get; set; }

            public bool IsPartial
            {
                get { return Name.StartsWith("_"); }
            }
        }
    }
}