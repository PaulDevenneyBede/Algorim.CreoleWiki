using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorim.CreoleWiki.AST.Inlines
{
	public class StrongInline : InlineElement
	{
		public StrongInline(string content)
		{
			this.content = content;
		}

		private string content;

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			if (string.IsNullOrEmpty(content))
				return;

			writer.AppendRaw("<strong>");
			writer.AppendRaw(parser.ParseInlines(content));
			writer.AppendRaw("</strong>");
		}

		public static StrongInline TryParse(CreoleReader reader)
		{
			if (reader.Peek(2) != "**")
				return null;

			var index = reader.IndexOf("**", 2);

			reader.Skip(2);
			string content;
			if (index != -1)
			{
				content = reader.Read(index - 2);
				reader.Skip(2);
			}
			else
			{
				content = reader.ReadToEnd();
			}		

			return new StrongInline(content);
		}
	}
}
