using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorim.CreoleWiki.AST.Blocks
{
	public class ParagraphBlock : BlockElement
	{
		public ParagraphBlock(string content)
		{
			this.content = content;
		}

		private string content;

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			if (string.IsNullOrWhiteSpace(content))
				return;

			writer.AppendRaw("<p>");
			writer.AppendRaw(parser.ParseInlines(content));
			writer.AppendRawLine("</p>");
		}

		public static IEnumerable<ParagraphBlock> Parse(string paragraphs)
		{
			var reader = new CreoleReader(paragraphs);

			while (!reader.EndOfMarkup)
			{
				var content = reader.ReadLineUntil(l => l.Trim().Length <= 0 ? l.Length : -1);

				if (content == null)
					content = reader.ReadToEnd();
				else
					reader.SkipLine();

				if (content.Trim().Length <= 0)
					continue;

				yield return new ParagraphBlock(content.Trim());
			}
		}
	}
}
