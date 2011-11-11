using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorim.CreoleWiki.AST.Inlines
{
	public class NoWikiInline : InlineElement
	{
		public NoWikiInline(string content)
		{
			this.content = content;
		}

		private string content;

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			writer.AppendRaw("<code>");
			writer.Append(content);
			writer.AppendRawLine("</code>");
		}

		public static NoWikiInline TryParse(CreoleReader reader)
		{
			if (reader.Peek(3) != "{{{")
				return null;

			var index = reader.IndexOf("}}}", 2);
			if (index == -1)
				return null;

			reader.Skip(3);
			var content = reader.Read(index - 3);
			reader.Skip(3);

			return new NoWikiInline(content);
		}
	}
}
