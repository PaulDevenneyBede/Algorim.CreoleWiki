using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Algorim.CreoleWiki
{
	public class CreoleReader
	{
		private static Regex NextWordRegex = new Regex(@"^\s*(?<word>[^\s]+)", RegexOptions.Compiled | RegexOptions.Multiline);
		private static Regex NextLineRegex = new Regex(@"(?<line>.*)(\n|$)", RegexOptions.Compiled | RegexOptions.Multiline);

		public CreoleReader(string markup)
		{
			this.markup = markup.Replace("\r\n", "\n");

			index = 0;
		}

		private string markup;
		private int index;
		private string nextWord;

		public int IndexOf(string value, int skip)
		{
			var findIndex = markup.IndexOf(value, index + skip);

			if (findIndex == -1)
				return -1;

			return findIndex - index;
		}

		public string Peek(int length)
		{
			return Peek(index, length);
		}

		public string Peek(int index, int length)
		{
			if (index < 0 || index >= markup.Length)
				throw new ArgumentOutOfRangeException("index");

			if (length <= 0)
				throw new ArgumentOutOfRangeException("length");

			var remaining = markup.Length - index;
			if (remaining <= 0)
				return string.Empty;

			return markup.Substring(index, Math.Min(remaining, length));
		}

		public string PeekWord()
		{
			var match = NextWordRegex.Match(PeekLine());

			if (match.Success && match.Index == 0)
				return match.Groups["word"].Value;

			return string.Empty;
		}

		public string PeekLine()
		{
			var match = NextLineRegex.Match(markup, index);

			if (match.Success && match.Index == index)
				return match.Groups["line"].Value;

			return string.Empty;
		}

		public string Read(int length)
		{
			if (length <= 0)
				throw new ArgumentOutOfRangeException("length");

			var result = Peek(length);

			if (result.Length > 0)
				Skip(result.Length);

			return result;
		}

		public string ReadWord()
		{
			var match = NextWordRegex.Match(PeekLine());

			if (match.Success && match.Index == 0)
			{
				if (match.Length > 0)
					Skip(match.Length);
				return match.Groups["word"].Value;
			}

			return string.Empty;
		}

		public string ReadLine()
		{
			var match = NextLineRegex.Match(markup, index);

			if (match.Success && match.Index == index)
			{
				if (match.Length > 0)
					Skip(match.Length);

				return match.Groups["line"].Value;
			}

			return string.Empty;
		}

		public string ReadLineUntil(Func<string, int> condition, bool trimLines = false)
		{
			var index = this.index;

			var result = new StringBuilder();
			while (!EndOfMarkup)
			{
				var match = NextLineRegex.Match(markup, index);

				if (match.Success && match.Index == index)
				{
					var line = trimLines ? match.Groups["line"].Value.Trim() : match.Groups["line"].Value;

					var conditionResult = -1;
					if ((conditionResult = condition(line)) < 0)
					{
						index += match.Length;
						result.AppendLine(line);
						continue;
					}

					if (conditionResult > 0)
					{
						conditionResult = Math.Min(conditionResult, line.Length);

						index += conditionResult;
						result.Append(line.Substring(0, conditionResult));
					}

					this.index = index;
					nextWord = null;
					return result.ToString().Replace("\r\n", "\n");
				}

				return null;
			}

			return null;
		}

		public string ReadToEnd()
		{
			if (EndOfMarkup)
				return string.Empty;

			var result = markup.Substring(index, Length - index);

			index += result.Length;

			return result;
		}

		public void Skip(int length)
		{
			if (length <= 0)
				throw new ArgumentOutOfRangeException("length");

			nextWord = null;

			index += length;
		}

		public void SkipWord()
		{
			var match = NextWordRegex.Match(PeekLine());

			if (match.Success && match.Index == 0)
				Skip(match.Length);
		}

		public void SkipLine()
		{
			var match = NextLineRegex.Match(markup, index);

			if (match.Success && match.Index == index)
			{
				if (match.Length > 0)
					Skip(match.Length);
			}
		}

		public bool HasPrefix(string prefix, int relativeIndex = 0)
		{
			if (prefix == null)
				throw new ArgumentNullException("prefix");
			
			if (prefix == string.Empty)
				return true;

			var index = this.index + relativeIndex;
			if (index > Length)
				throw new ArgumentOutOfRangeException("relativeIndex");

			if (index < prefix.Length)
				return false;

			for (int i = 0; i < prefix.Length; i++)
			{ 
				var markupIndex = index - (prefix.Length - i);

				var prefixChar = prefix[i];
				var markupChar = markup[markupIndex];

				if (prefixChar != markupChar)
					return false;
			}

			return true;
		}

		public int Position
		{
			get { return index; }
		}

		public int Length
		{
			get { return markup.Length; }
		}

		public bool EndOfMarkup
		{
			get { return index >= markup.Length; }
		}

		public string NextWord
		{
			get
			{
				if (nextWord == null)
					nextWord = PeekWord();

				return nextWord;
			}
		}

		public bool IsNewWord
		{
			get
			{
				return index == 0 || markup[index - 1].ToString().Trim().Length == 0;
			}
		}
	}
}
