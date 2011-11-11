using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Algorim.CreoleWiki
{
	public class CreoleWriter
	{
		public CreoleWriter()
		{
			builder = new StringBuilder();
		}

		private StringBuilder builder;

		private string Normalize(string text)
		{
			return text.Replace("\r\n", "\n");
		}
		private string Format(string format, params object[] args)
		{
			return Normalize(string.Format(format, args));
		}

		public void Append(string text)
		{
			builder.Append(HttpUtility.HtmlEncode(Normalize(text)));
		}
		public void Append(string format, params object[] args)
		{
			builder.Append(HttpUtility.HtmlEncode(Format(format, args)));
		}

		public void AppendLine(string text)
		{
			Append(text + "\n");
		}
		public void AppendLine(string format, params object[] args)
		{
			Append(format + "\n", args);
		}

		public void AppendRaw(string text)
		{
			builder.Append(Normalize(text));
		}
		public void AppendRaw(string format, params object[] args)
		{
			builder.Append(Format(format, args));
		}

		public void AppendRawLine(string text)
		{
			AppendRaw(text + "\n");
		}
		public void AppendRawLine(string format, params object[] args)
		{
			AppendRaw(format + "\n", args);
		}

		public override string ToString()
		{
			return builder.ToString();
		}
	}
}
