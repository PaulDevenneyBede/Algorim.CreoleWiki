using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorim.CreoleWiki.AST.Inlines
{
	public class EmphasisInline : InlineElement
	{
		public EmphasisInline(string content)
		{
			this.content = content;
		}

		private string content;

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			writer.AppendRaw("<em>");
			writer.AppendRaw(parser.ParseInlines(content));
			writer.AppendRaw("</em>");
		}

		public static EmphasisInline TryParse(CreoleReader reader)
		{
			if (reader.Peek(2) != "//")
				return null;

			var index = reader.IndexOf("//", 2);
			if (index == -1)
				return null;

			reader.Skip(2);
			var content = reader.Read(index - 2);
			reader.Skip(2);

			return new EmphasisInline(content);
		}
	}
}
