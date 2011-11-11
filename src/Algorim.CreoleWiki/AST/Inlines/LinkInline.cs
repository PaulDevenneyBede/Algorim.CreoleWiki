using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Algorim.CreoleWiki.AST.Inlines
{
	public class LinkInline : InlineElement
	{
		public LinkInline(string url, string content)
		{
			this.url = url;
			this.content = content;
		}

		private string url;
		private string content;

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			var isExternal = url.StartsWith("http://") || url.StartsWith("https://");

			writer.AppendRaw("<a href=\"{0}\"{1}>", HttpUtility.UrlPathEncode(url), isExternal ? " target=\"_blank\"" : string.Empty);
			writer.AppendRaw(parser.ParseInlines(content));
			writer.AppendRaw("</a>");
		}

		public static LinkInline TryParse(CreoleReader reader)
		{
			if (reader.Peek(2) != "[[")
				return null;

			var index = reader.IndexOf("]]", 2);
			if (index == -1)
				return null;

			reader.Skip(2);
			var data = reader.Read(index - 2);
			reader.Skip(2);

			index = data.IndexOf("|");
			if (index == -1)
				return new LinkInline(data, data);

			var url = data.Substring(0, index);
			var content = data.Substring(index + 1, data.Length - index - 1);

			return new LinkInline(url, content);
		}
	}
}
