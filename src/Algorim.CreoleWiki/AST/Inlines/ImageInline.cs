using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Algorim.CreoleWiki.AST.Inlines
{
	public class ImageInline : InlineElement
	{
		public ImageInline(string url, string alt)
		{
			this.url = url;
			this.alt = alt;
		}

		private string url;
		private string alt;

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			writer.AppendRaw(@"<img src=""{0}"" alt=""{1}"" />", HttpUtility.UrlPathEncode(url), HttpUtility.HtmlEncode(alt));
		}

		public static ImageInline TryParse(CreoleReader reader)
		{
			if (reader.Peek(2) != "{{")
				return null;

			var index = reader.IndexOf("}}", 2);
			if (index == -1)
				return null;

			reader.Skip(2);
			var content = reader.Read(index - 2);
			reader.Skip(2);

			index = content.IndexOf("|");
			if (index == -1)
				return new ImageInline(content, null);

			var url = content.Substring(0, index);
			var alt = content.Substring(index + 1, content.Length - index - 1);

			return new ImageInline(url, alt);
		}
	}
}
