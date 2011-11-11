using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Algorim.CreoleWiki;

namespace Tests.Algorim.CreoleWiki
{
	[TestClass]
	public class CreoleReaderTests
	{
		[TestMethod]
		public void Peek()
		{
			var reader = new CreoleReader("1234567890");

			var actual = reader.Peek(5);

			Assert.AreEqual("12345", actual);
			Assert.AreEqual(0, reader.Position);
		}

		[TestMethod]
		public void PeekWord()
		{
			var reader = new CreoleReader("  123456 7890  ");

			var actual = reader.PeekWord();

			Assert.AreEqual("123456", actual);
			Assert.AreEqual(0, reader.Position);
		}

		[TestMethod]
		public void PeekLine()
		{
			var reader = new CreoleReader("  123456 \r\n 7890  ");

			var actual = reader.PeekLine();

			Assert.AreEqual("  123456 ", actual);
			Assert.AreEqual(0, reader.Position);
		}

		[TestMethod]
		public void Read()
		{
			var reader = new CreoleReader("1234567890");

			var actual = reader.Read(5);

			Assert.AreEqual("12345", actual);
			Assert.AreEqual(5, reader.Position);
		}

		[TestMethod]
		public void ReadWord()
		{
			var reader = new CreoleReader("  123456 7890  ");
			
			var actual = reader.ReadWord();

			Assert.AreEqual("123456", actual);
			Assert.AreEqual(8, reader.Position);
		}

		[TestMethod]
		public void ReadWord_With_NewLine()
		{
			var reader = new CreoleReader("  123\r\n 456 7890  ");

			var actual = reader.ReadWord();

			Assert.AreEqual("123", actual);
			Assert.AreEqual(5, reader.Position);
		}

		[TestMethod]
		public void ReadLine()
		{
			var reader = new CreoleReader("  123 \r\n 456 7890  ");

			var actual = reader.ReadLine();

			Assert.AreEqual("  123 ", actual);
			Assert.AreEqual(7, reader.Position);

			actual = reader.ReadLine();

			Assert.AreEqual(" 456 7890  ", actual);
			Assert.AreEqual(true, reader.EndOfMarkup);
		}

		[TestMethod]
		public void ReadLineUntil()
		{
			var reader = new CreoleReader("  123 \r\n 456 7890 \r\n asdfMARK \r\nMARK \r\n test \r\nMARK ");

			var actual = reader.ReadLineUntil(l => l.StartsWith("MARK") ? 2 : -1);

			Assert.AreEqual("  123 \n 456 7890 \n asdfMARK \nMA", actual);
			Assert.AreEqual(31, reader.Position);

			actual = reader.ReadLineUntil(l => l.StartsWith("MARK") ? 4 : -1);

			Assert.AreEqual("RK \n test \nMARK", actual);
			Assert.AreEqual(reader.Length - 1, reader.Position);
		}
	}
}
