using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Algorim.CreoleWiki.AST;
using Algorim.CreoleWiki.AST.Blocks;
using Algorim.CreoleWiki.AST.Inlines;

namespace Algorim.CreoleWiki
{
	public class CreoleParser
	{
		public CreoleParser()
		{
			BlockElements = new List<Func<CreoleReader, BlockElement>>();
			BlockElements.Add(NoWikiBlock.TryParse);
			BlockElements.Add(HeadingBlock.TryParse);
			BlockElements.Add(HorizontalLineBlock.TryParse);
			BlockElements.Add(ListBlock.TryParse);
			BlockElements.Add(TableBlock.TryParse);

			InlineElements = new List<Func<CreoleReader, InlineElement>>();
			InlineElements.Add(NoWikiInline.TryParse);
			InlineElements.Add(StrongInline.TryParse);
			InlineElements.Add(EmphasisInline.TryParse);
			InlineElements.Add(LineBreakInline.TryParse);
			InlineElements.Add(LinkInline.TryParse);
			InlineElements.Add(ImageInline.TryParse);
		}

		public List<Func<CreoleReader, BlockElement>> BlockElements { get; private set; }
		public List<Func<CreoleReader, InlineElement>> InlineElements { get; private set; }

		public string ParseInlines(string markup)
		{
			return ParseInlines(markup, new Func<CreoleReader, CreoleElement>[0]);
		}
		internal string ParseInlines(string markup, params Func<CreoleReader, CreoleElement>[] ignore)
		{
			var elements = new List<InlineElement>();

			// parse markup
			var reader = new CreoleReader(markup);

			var textBuilder = new StringBuilder();
			while (!reader.EndOfMarkup)
			{
				InlineElement inline = null;

				foreach (var inlineParser in InlineElements)
				{
					if (ignore.Contains(inlineParser))
						continue;

					inline = inlineParser(reader);
					if (inline == null)
						continue;

					break;
				}

				if (inline == null)
				{
					textBuilder.Append(reader.Read(1));
				}
				else
				{
					elements.AddRange(TextInline.Parse(textBuilder.ToString()));
					textBuilder.Clear();

					elements.Add(inline);
				}
			}
			if (textBuilder.Length > 0)
				elements.AddRange(TextInline.Parse(textBuilder.ToString()));

			// render output
			var writer = new CreoleWriter();

			foreach (var element in elements)
			{
				element.Render(this, writer);
			}

			return writer.ToString().Replace("\n", Environment.NewLine);
		}

		public string Parse(string markup)
		{
			return Parse(markup, new Func<CreoleReader, CreoleElement>[0]);
		}
		internal string Parse(string markup, params Func<CreoleReader, CreoleElement>[] ignore)
		{
			var elements = new List<BlockElement>();

			// parse markup
			var reader = new CreoleReader(markup);

			var paragraphBuilder = new StringBuilder();
			while (!reader.EndOfMarkup)
			{
				BlockElement block = null;

				foreach (var blockParser in BlockElements)
				{
					if (ignore.Contains(blockParser))
						continue;

					block = blockParser(reader);
					if (block == null)
						continue;

					break;
				}

				if (block == null)
				{
					paragraphBuilder.AppendLine(reader.ReadLine());
				}
				else
				{
					elements.AddRange(ParagraphBlock.Parse(paragraphBuilder.ToString()));
					paragraphBuilder.Clear();

					elements.Add(block);
				}
			}
			if (paragraphBuilder.Length > 0)
				elements.AddRange(ParagraphBlock.Parse(paragraphBuilder.ToString()));

			// render output
			var writer = new CreoleWriter();

			foreach (var element in elements)
			{
				element.Render(this, writer);
			}

			return writer.ToString().Replace("\n", Environment.NewLine).Trim();
		}
	}
}
