using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Algorim.CreoleWiki;

namespace Tests.Algorim.CreoleWiki
{
	[TestClass]
	public class CreoleParserTests
	{
		[TestMethod]
		public void Parse_Paragraphs()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"paragraph 1

paragraph 2
still paragraph 2

paragraph 3");

			Assert.AreEqual(@"<p>paragraph 1</p>
<p>paragraph 2
still paragraph 2</p>
<p>paragraph 3</p>", actual);
		}

		[TestMethod]
		public void Parse_Strong()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"not strong **strong ** not strong **strong");

			Assert.AreEqual(@"<p>not strong <strong>strong </strong> not strong <strong>strong</strong></p>", actual);
		}

		[TestMethod]
		public void Parse_Emphasis()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"not em //em // not em //em");

			Assert.AreEqual(@"<p>not em <em>em </em> not em <em>em</em></p>", actual);
		}

		[TestMethod]
		public void Parse_StrongEmphasis()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"not strong **//strong em//** not strong not em **//strong em**// just em");

			Assert.AreEqual(@"<p>not strong <strong><em>strong em</em></strong> not strong not em <strong><em>strong em</em></strong><em> just em</em></p>", actual);
		}

		[TestMethod]
		public void Parse_EmphasisLink()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"//test [[http://www.google.com]] test//");
			
			Assert.AreEqual(@"<p><em>test <a href=""http://www.google.com"" target=""_blank"">http://www.google.com</a> test</em></p>", actual);
		}

		[TestMethod]
		public void Parse_EmphasisUrls()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"Creole1.0 specifies that http://bar and ftp://bar should not render italic, something like foo://bar should render as italic.");

			Assert.AreEqual(@"<p>Creole1.0 specifies that http://bar and ftp://bar should not render italic, something like foo:<em>bar should render as italic.</em></p>", actual);
		}

		[TestMethod]
		public void Parse_LineBreak()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"first line\\second line
\\third line

another paragraph");

			Assert.AreEqual(@"<p>first line<br />
second line
<br />
third line</p>
<p>another paragraph</p>", actual);
		}

		[TestMethod]
		public void Parse_NoWiki()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"test here

{{{
**no wiki here**
}}}

**but wiki here**");

			Assert.AreEqual(@"<p>test here</p>
<pre>
**no wiki here**
</pre>
<p><strong>but wiki here</strong></p>", actual);
		}

		[TestMethod]
		public void Parse_Heading()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"= h1");

			Assert.AreEqual(@"<h1> h1</h1>", actual);
		}

		[TestMethod]
		public void Parse_Heading_WithEnding()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"====== h6 ======");

			Assert.AreEqual(@"<h6> h6 </h6>", actual);
		}

		[TestMethod]
		public void Parse_Heading_WithPartialEnding()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"====== h6 ===");

			Assert.AreEqual(@"<h6> h6 </h6>", actual);
		}

		[TestMethod]
		public void Parse_Heading_WithAdditions()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"===== h5 ======");

			Assert.AreEqual(@"<h5> h5 =</h5>", actual);
		}

		[TestMethod]
		public void Parse_Heading_WithManyAdditions()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"======== h6 ========");

			Assert.AreEqual(@"<h6>== h6 ==</h6>", actual);
		}

		[TestMethod]
		public void Parse_HorizontalLine()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"first paragraph
 ----- 
second paragraph
---- still second paragraph");

			Assert.AreEqual(@"<p>first paragraph</p>
<hr />
<p>second paragraph
---- still second paragraph</p>", actual);
		}

		[TestMethod]
		public void Parse_Link()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse("[[test page]]");

			Assert.AreEqual(@"<p><a href=""test%20page"">test page</a></p>", actual);
		}

		[TestMethod]
		public void Parse_Link_WithTitle()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse("[[test page|test page title]]");

			Assert.AreEqual(@"<p><a href=""test%20page"">test page title</a></p>", actual);
		}

		[TestMethod]
		public void Parse_Link_ExternalWithTitle()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse("[[http://www.google.com|Uncle google]]");

			Assert.AreEqual(@"<p><a href=""http://www.google.com"" target=""_blank"">Uncle google</a></p>", actual);
		}

		[TestMethod]
		public void Parse_Link_Free()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse("http://www.google.com");

			Assert.AreEqual(@"<p><a href=""http://www.google.com"" target=""_blank"">http://www.google.com</a></p>", actual);

			actual = parser.Parse("http:/www.google.com");

			Assert.AreEqual(@"<p>http:/www.google.com</p>", actual);
		}

		[TestMethod]
		public void Parse_Image()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse("{{/content/images/test.png}}");

			Assert.AreEqual(@"<p><img src=""/content/images/test.png"" alt="""" /></p>", actual);
		}

		[TestMethod]
		public void Parse_Image_WithTitle()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse("{{/content/images/test.png|title}}");

			Assert.AreEqual(@"<p><img src=""/content/images/test.png"" alt=""title"" /></p>", actual);
		}

		[TestMethod]
		public void Parse_Image_WithTitleQuotes()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse("{{/content/images/test.png|title'\"}}");

			Assert.AreEqual(@"<p><img src=""/content/images/test.png"" alt=""title&#39;&quot;"" /></p>", actual);
		}

		[TestMethod]
		public void Parse_List()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"# list 1
# list 2
* list 1
* list 2");

			Assert.AreEqual(@"<ol>
    <li>list 1</li>
    <li>list 2</li>
</ol>
<ul>
    <li>list 1</li>
    <li>list 2</li>
</ul>", actual);
		}

		[TestMethod]
		public void Parse_List_WithNesting()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"# list 1
## list 2
## list 2
#### list 3
# list 1");

			Assert.AreEqual(@"<ol>
    <li>list 1
        <ol>
            <li>list 2</li>
            <li>list 2
                <ol>
                    <li># list 3</li>
                </ol>
            </li>
        </ol>
    </li>
    <li>list 1</li>
</ol>", actual);
		}

		[TestMethod]
		public void Parse_List_WithMixing()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"# list 1
## list 2
## list 2
*** list 3
*** list 3
*#*** list 1
**** list 2
** list 2
## list 2
# list 1");

			Assert.AreEqual(@"<ol>
    <li>list 1
        <ol>
            <li>list 2</li>
            <li>list 2
                <ul>
                    <li>list 3</li>
                    <li>list 3</li>
                </ul>
            </li>
        </ol>
    </li>
</ol>
<ul>
    <li>#<strong>* list 1</strong>
        <ul>
            <li><strong> list 2</strong></li>
            <li>list 2</li>
        </ul>
        <ol>
            <li>list 2</li>
        </ol>
    </li>
</ul>
<ol>
    <li>list 1</li>
</ol>", actual);
		}

		[TestMethod]
		public void Parse_Table()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"|cell 1|**bold cell 2**
| cell 1 row 2 | cell 2 row 2 | //cell 3 row 2//");

			Assert.AreEqual(@"<table>
    <tr>
        <td>cell 1</td>
        <td><strong>bold cell 2</strong></td>
    </tr>
    <tr>
        <td>cell 1 row 2</td>
        <td>cell 2 row 2</td>
        <td><em>cell 3 row 2</em></td>
    </tr>
</table>", actual);
		}

		[TestMethod]
		public void Parse_Table_WithHeader()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"|= Header 1 |= Header 2
|cell 1|**bold cell 2**
| cell 1 row 2 | cell 2 row 2 | //cell 3 row 2//");

			Assert.AreEqual(@"<table>
    <thead>
        <tr>
            <th>Header 1</th>
            <th>Header 2</th>
        </tr>
    <thead>
    <tr>
        <td>cell 1</td>
        <td><strong>bold cell 2</strong></td>
    </tr>
    <tr>
        <td>cell 1 row 2</td>
        <td>cell 2 row 2</td>
        <td><em>cell 3 row 2</em></td>
    </tr>
</table>", actual);
		}

		[TestMethod]
		public void Parse_Table_WithHeader_WithEndings()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"|= Header 1 |= Header 2 |
|cell 1|**bold cell 2**|
| cell 1 row 2 | cell 2 row 2 | //cell 3 row 2//");

			Assert.AreEqual(@"<table>
    <thead>
        <tr>
            <th>Header 1</th>
            <th>Header 2</th>
        </tr>
    <thead>
    <tr>
        <td>cell 1</td>
        <td><strong>bold cell 2</strong></td>
    </tr>
    <tr>
        <td>cell 1 row 2</td>
        <td>cell 2 row 2</td>
        <td><em>cell 3 row 2</em></td>
    </tr>
</table>", actual);
		}

		[TestMethod]
		public void Parse_Table_WithEmptyCell()
		{
			var parser = new CreoleParser();

			var actual = parser.Parse(@"|cell 1|**bold cell 2**
|  | cell 2 row 2 | //cell 3 row 2//");

			Assert.AreEqual(@"<table>
    <tr>
        <td>cell 1</td>
        <td><strong>bold cell 2</strong></td>
    </tr>
    <tr>
        <td></td>
        <td>cell 2 row 2</td>
        <td><em>cell 3 row 2</em></td>
    </tr>
</table>", actual);
		}

		[TestMethod]
		public void Parse_ResolveLink()
		{
			var parser = new CreoleParser();

			parser.LinkResolver = l => "/test/" + l.Replace(" ", "-");

			var actual = parser.Parse(@"[[mighty link]]");

			Assert.AreEqual(@"<p><a href=""/test/mighty-link"">mighty link</a></p>", actual);
		}

		[TestMethod]
		public void Parse_ResolveImage()
		{
			var parser = new CreoleParser();

			parser.ImageResolver = l => "/images/" + l.Replace(" ", "-");

			var actual = parser.Parse(@"{{mighty image}}");

			Assert.AreEqual(@"<p><img src=""/images/mighty-image"" alt="""" /></p>", actual);
		}
	}
}
