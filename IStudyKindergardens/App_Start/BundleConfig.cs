using System.Web;
using System.Web.Optimization;

namespace IStudyKindergardens
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Styles

            bundles.Add(new StyleBundle("~/css/bootstrap").Include(
                        "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/css/allskins").Include(
                        "~/Content/dist/skins/_all-skins.min.css"));

            bundles.Add(new StyleBundle("~/css/theme").Include(
                        "~/Content/dist/AdminLTE.min.css",
                        "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/css/icheckSquareBlue").Include(
                        "~/Plugins/iCheck/square/blue.css"));

            bundles.Add(new StyleBundle("~/css/icheckFlatBlue").Include(
                        "~/Plugins/iCheck/flat/blue.css"));

            //Scripts

            bundles.Add(new ScriptBundle("~/js/jquery").Include(
                        "~/Plugins/jQuery/jQuery-2.1.3.min.js"));

            bundles.Add(new ScriptBundle("~/js/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/js/ajax").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/js/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/js/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/js/icheck").Include(
                        "~/Plugins/iCheck/icheck.min.js"));

            bundles.Add(new ScriptBundle("~/js/inputmask").Include(
                        "~/Plugins/input-mask/jquery.inputmask.js",
                        "~/Plugins/input-mask/jquery.inputmask.date.extensions.js",
                        "~/Plugins/input-mask/jquery.inputmask.extensions.js"
                ));
        }
    }
}
