using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorim.CreoleWiki.AST.Blocks
{
	public class HeadingBlock : BlockElement
	{
		public HeadingBlock(int level, string content)
		{
			if (level < 1 || level > 6)
				throw new ArgumentOutOfRangeException("level");

			this.level = level;
			this.content = content;
		}

		private int level;
		private string content;

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			writer.AppendRaw("<h{0}>", level);
			writer.Append(content);
			writer.AppendRaw("</h{0}>", level);
		}

		public static HeadingBlock TryParse(CreoleReader reader)
		{
			var line = reader.PeekLine().Trim();

			if (!line.StartsWith("="))
				return null;

			var level = 0;
			for (level = 1; level < 6; level++)
				if (line[level] != '=') break;

			var endIndex = 0;
			for (endIndex = line.Length - 1; endIndex >= line.Length - level; endIndex--)
				if (line[endIndex] != '=') break;

			reader.SkipLine();

			var content = line.Substring(level, endIndex - (level - 1));

			return new HeadingBlock(level, content);
		}
	}
}
