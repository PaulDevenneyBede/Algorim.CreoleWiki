using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorim.CreoleWiki.AST.Blocks
{
	public class ListBlock : BlockElement
	{
		public ListBlock(string markup)
		{
			this.markup = markup;
		}

		private string markup;

		private void RenderList(CreoleParser parser, CreoleWriter writer, CreoleReader reader, int level)
		{
			var indent = "";
			for (int i = 1; i < level * 2 - 1; i++)
				indent += "    ";

			while (!reader.EndOfMarkup)
			{
				var type = reader.Peek(level);
				if (type.Any(c => c != type[0]))
					break;
				type = type[0].ToString();

				if (type == "*") writer.AppendRawLine("{0}<ul>", indent);
				else if (type == "#") writer.AppendRawLine("{0}<ol>", indent);
				else throw new InvalidOperationException("Invalid list type");

				while (!reader.EndOfMarkup)
				{
					var nextType = reader.Peek(level);
					if (nextType.Any(c => c != type[0]))
						break;

					reader.Skip(level);
					var line = reader.ReadLine();
					var indentEndLi = false;

					writer.AppendRaw("{0}    <li>", indent);
					writer.AppendRaw(parser.ParseInlines(line).Trim());
					if (!reader.EndOfMarkup)
					{
						var next = reader.Peek(level + 1);
						if (next.Length == level + 1 && (!next.Any(c => c != '*') || !next.Any(c => c != '#')))
						{
							writer.AppendRawLine("");
							RenderList(parser, writer, reader, level + 1);
							indentEndLi = true;
						}
					}
					if (indentEndLi)
						writer.AppendRawLine("{0}    </li>", indent);
					else
						writer.AppendRawLine("</li>");
				}

				if (type == "*") writer.AppendRawLine("{0}</ul>", indent);
				else if (type == "#") writer.AppendRawLine("{0}</ol>", indent);
				else throw new InvalidOperationException("Invalid list type");
			}
		}

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			RenderList(parser, writer, new CreoleReader(markup), 1);
		}

		public static ListBlock TryParse(CreoleReader reader)
		{
			if (reader.NextWord != "*" && reader.NextWord != "#")
				return null;

			var markup = reader.ReadLineUntil(l => (l.StartsWith("*") || l.StartsWith("#")) ? -1 : 0, trimLines: true);

			return new ListBlock(markup);
		}
	}
}
