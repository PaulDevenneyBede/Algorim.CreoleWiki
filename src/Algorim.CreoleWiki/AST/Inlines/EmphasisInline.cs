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
			if (string.IsNullOrEmpty(content))
				return;

			writer.AppendRaw("<em>");
			writer.AppendRaw(parser.ParseInlines(content));
			writer.AppendRaw("</em>");
		}

		private static bool HasLinkPrefix(CreoleReader reader, int relativeIndex = 0)
		{
			return reader.HasPrefix("http:", relativeIndex) || reader.HasPrefix("https:", relativeIndex) || reader.HasPrefix("ftp:", relativeIndex);
		}
		public static EmphasisInline TryParse(CreoleReader reader)
		{
			if (reader.Peek(2) != "//" || HasLinkPrefix(reader))
				return null;

			int index = 0;
			while (true)
			{
				index = reader.IndexOf("//", index + 2);
				if (index == -1)
					break;

				if (!HasLinkPrefix(reader, index))
					break;
			}
			
			reader.Skip(2);
			string content;
			if (index != -1)
			{
				content = reader.Read(index - 2);
				reader.Skip(2);
			}
			else
			{
				content = reader.ReadToEnd();
			}

			return new EmphasisInline(content);
		}
	}
}
