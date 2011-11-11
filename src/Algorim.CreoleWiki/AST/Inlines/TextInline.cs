using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorim.CreoleWiki.AST.Inlines
{
	public class TextInline : InlineElement
	{
		public TextInline(string content)
		{
			this.content = content;
		}

		private string content;

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			writer.Append(content);
		}

		public static IEnumerable<TextInline> Parse(string text)
		{
			yield return new TextInline(text);
		}
	}
}
