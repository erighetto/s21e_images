using System.Net;
using System.Linq;
using System.Web;
using System.Collections.Specialized;

namespace S21eimagescollect.Service
{
    public class GoogleImages
    {
        public GoogleImages()
        {
        }

        public string FindImages(NameValueCollection options)
        {
            string query = ToQueryString(options);
            WebClient webClient = new WebClient();
            return webClient.DownloadString("https://www.googleapis.com/customsearch/v1" + query);     
        }

        private string ToQueryString(NameValueCollection nvc)
        {
            var array = (
                from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format(
            "{0}={1}",
            HttpUtility.UrlEncode(key),
            HttpUtility.UrlEncode(value))
                ).ToArray();
            return "?" + string.Join("&", array);
        }
    }
}
