using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Algorim.CreoleWiki.AST.Blocks
{
	public class TableBlock : BlockElement
	{
		private static Regex HeaderRegex = new Regex(@"|[^|]");

		public TableBlock(string[] headers, string[][] rows)
		{
			this.headers = headers;
			this.rows = rows;
		}

		private string[] headers;
		private string[][] rows;

		public override void Render(CreoleParser parser, CreoleWriter writer)
		{
			writer.AppendRawLine("<table>");
			if (headers != null)
			{
				writer.AppendRawLine("    <thead>");
				writer.AppendRawLine("        <tr>");
				foreach (var header in headers)
					writer.AppendRawLine("            <th>{0}</th>", parser.ParseInlines(header.Trim()));
				writer.AppendRawLine("        </tr>");
				writer.AppendRawLine("    <thead>");
			}
			foreach (var row in rows)
			{
				writer.AppendRawLine("    <tr>");
				for (var i = 0; i < row.Length; i++)
				{
					var cell = row[i];

					if (i == 0 && cell.StartsWith("="))
						writer.AppendRawLine("        <td>{0}</td>", parser.ParseInlines(cell.Substring(1).Trim()));
					else
						writer.AppendRawLine("        <td>{0}</td>", parser.ParseInlines(cell.Trim()));
				}
				writer.AppendRawLine("    </tr>");
			}
			writer.AppendRawLine("</table>");
		}

		public static TableBlock TryParse(CreoleReader reader)
		{
			if (reader.Peek(1) != "|")
				return null;

			bool isFirstLine = true;
			string[] headers = null;
			List<string[]> rows = new List<string[]>();
			while (!reader.EndOfMarkup)
			{
				var line = reader.PeekLine().TrimEnd();

				if (!line.StartsWith("|"))
					break;

				var split = line.Substring(1).Split(new char[] { '|' }, StringSplitOptions.None);
				if (string.IsNullOrEmpty(split.Last().Trim()))
					split = split.Take(split.Count() - 1).ToArray();

				if (isFirstLine && !split.Any(p => !p.StartsWith("=")))
					headers = split.Select(p => p.Substring(1)).ToArray();
				else
					rows.Add(split);

				reader.ReadLine();
				isFirstLine = false;
			}

			return new TableBlock(headers, rows.ToArray());
		}
	}
}
