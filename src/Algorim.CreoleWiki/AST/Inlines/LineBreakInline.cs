using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorim.CreoleWiki.AST.Inlines
{
	public class LineBreakInline : InlineElement
	{
		public LineBreakInline()
		{ }

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			writer.AppendRawLine("<br />");
		}

		public static LineBreakInline TryParse(CreoleReader reader)
		{
			if (reader.Peek(2) != "\\\\")
				return null;

			reader.Skip(2);
			return new LineBreakInline();
		}
	}
}
