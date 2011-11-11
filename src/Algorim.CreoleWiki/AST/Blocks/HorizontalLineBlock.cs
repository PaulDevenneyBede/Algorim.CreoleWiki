using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorim.CreoleWiki.AST.Blocks
{
	public class HorizontalLineBlock : BlockElement
	{
		public HorizontalLineBlock()
		{ }

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			writer.AppendRawLine("<hr />");
		}

		public static HorizontalLineBlock TryParse(CreoleReader reader)
		{
			var line = reader.PeekLine().Trim();

			if (!line.StartsWith("----"))
				return null;

			if (line.Any(c => c != '-'))
				return null;

			reader.SkipLine();
			return new HorizontalLineBlock();
		}
	}
}
