using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;

namespace Algorim.CreoleWiki.AST.Inlines
{
	public class LinkInline : InlineElement
	{
		private static Regex UrlRegex = new Regex(@"^(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#']*)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public LinkInline(string url, string content, bool parseContent = true)
		{
			this.url = url;
			this.content = content;
			this.parseContent = parseContent;
		}

		private string url;
		private string content;
		private bool parseContent;

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			var isExternal = url.StartsWith("http://") || url.StartsWith("https://");

			writer.AppendRaw("<a href=\"{0}\"{1}>", HttpUtility.UrlPathEncode(url), isExternal ? " target=\"_blank\"" : string.Empty);
			if (parseContent)
				writer.AppendRaw(parser.ParseInlines(content, TryParse));
			else
				writer.Append(content);
			writer.AppendRaw("</a>");
		}

		public static LinkInline TryParse(CreoleReader reader)
		{
			if (reader.Peek(2) == "[[")
			{
				var index = reader.IndexOf("]]", 2);
				if (index == -1)
					return null;

				reader.Skip(2);
				var data = reader.Read(index - 2);
				reader.Skip(2);

				index = data.IndexOf("|");
				if (index == -1)
					return new LinkInline(data, data, parseContent: false);

				var url = data.Substring(0, index);
				var content = data.Substring(index + 1, data.Length - index - 1);

				return new LinkInline(url, content);
			}
			else if (reader.IsNewWord && UrlRegex.IsMatch(reader.NextWord))
			{
				var url = reader.ReadWord();

				return new LinkInline(url, url, parseContent: false);
			}

			return null;
		}
	}
}
