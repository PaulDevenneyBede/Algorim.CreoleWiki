using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorim.CreoleWiki.AST
{
	public abstract class CreoleElement
	{
		public abstract void Render(CreoleParser parser, CreoleWriter writer);
	}
}
