using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortressPrime.BackEnd.Reporting.FProogle
{
	public class ParameterItem
	{
		public string Key { get; set; }
		public string Operator { get; set; }
		public string Value { get; set; }
	}

	public class KeyItem
	{
		public string fullName { get; set; }
		public Type type { get; set; }
		public string oper { get; set; }
		public bool bulk { get; set; }
		public Type treatAs { get; set; }
	}

	public class KeyShortcut
	{
		public string shortName { get; set; }
		public string fullName { get; set; }
		public bool standalone { get; set; }
	}
}
