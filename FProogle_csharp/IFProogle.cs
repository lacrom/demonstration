using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortressPrime.BackEnd.Reporting.FProogle
{
	interface IFProogle<T>
	{
		IEnumerable<T> filter(List<T> inputList, string searchString);
	}
}