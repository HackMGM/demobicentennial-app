using System;
using HtmlAgilityPack;

namespace MapsuiFormsSample.Helpers
{
    public class HtmlHelper : IHtmlHelper
    {
        public string ExtractText(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var text = htmlDoc.DocumentNode.InnerText;
            return text;
        }
    }
}
