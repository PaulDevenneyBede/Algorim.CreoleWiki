using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Algorim.CreoleWiki.AST.Blocks
{
	public class NoWikiBlock : BlockElement
	{
		public NoWikiBlock(string content)
		{
			this.content = content;
		}

		private string content;

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			writer.AppendRaw("<pre>");
			writer.Append(content);
			writer.AppendRawLine("</pre>");
		}

		public static NoWikiBlock TryParse(CreoleReader reader)
		{
			if (reader.Peek(3) != "{{{")
				return null;
			
			var content = reader.ReadLineUntil(l => l.StartsWith("}}}") ? 3 : -1);
			if (content != null)
				return new NoWikiBlock(content.Substring(3, content.Length - 6));

			return null;	
		}
	}
}
