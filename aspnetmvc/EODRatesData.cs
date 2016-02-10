using FortressPrime.BackEnd.Common;
using FortressPrime.BackEnd.Common.IncomingCommands;
using FortressPrime.BackEnd.Database.Dbml;
using FortressPrime.BackEnd.Reporting.Common;
using FortressPrime.BackEnd.Reporting.Summary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FortressPrime.BackEnd.Database.DL;

namespace FortressPrime.BackEnd.Reporting.DataLayer
{
	public class EODRatesData : MsSqlDLBase, IEODRates
	{
		public EODRatesData(DataLayerFactory dataLayerFactory)
		{
			this.DLFactory = dataLayerFactory;
		}

		public DataLayerResult<Common.ResultPage<EODRates.Rate>> GetRatesForTradeDay(EODRates.ReportParameters reportParams)
		{
			try
			{
				IEnumerable<EODRates.Rate> result = null;
				int count = 0;

				this.ExecuteInDBTransaction((dl) =>
				{
					result = dl.ReportingDL.getEODRates(reportParams.ReportDay, reportParams.Currency).Select(item => ReadRate(item)).ToList();
				});

				count = result.Count();

				//do the paging
				if (reportParams.PageSort != null)
				{
					result = Sort(result, reportParams.PageSort).ToList();
					result = result.Skip((reportParams.PageSort.PageNo - 1) * reportParams.PageSort.PageSize).Take(reportParams.PageSort.PageSize).ToList();
				}

				return new DataLayerResult<ResultPage<EODRates.Rate>>(new ResultPage<EODRates.Rate>(result, null, count), DateTime.UtcNow);
			}
			catch (Exception e)
			{
				log4net.ILog logger = log4net.LogManager.GetLogger("DataLayer.EndOfDayTradeStateReportData");
				logger.Error(e.Message);
				throw e;
			}
		}

		private EODRates.Rate ReadRate(GetQuotesForDateResult dbRecord)
		{
			var result = new EODRates.Rate();

			result.SymbolName = dbRecord.SymbolName;
			result.Bid = dbRecord.Bid.GetValueOrDefault();
			result.Ask = dbRecord.Ask.GetValueOrDefault();
			result.UtcTimeStamp = dbRecord.UtcTimeStamp.GetValueOrDefault();
			result.Spread = dbRecord.Spread.GetValueOrDefault();
			result.TypeDescription = dbRecord.TypeDescription;

			return result;
		}

		private IEnumerable<EODRates.Rate> Sort(IEnumerable<EODRates.Rate> query, FortressPrime.BackEnd.Reporting.Common.PageSortReportParameters pageSort)
		{
			if (pageSort.OrderBy.FieldName == "Ask")
			{
				return Sort(query, r => r.Ask, pageSort.OrderBy.IsAsc);
			}

			if (pageSort.OrderBy.FieldName == "Bid")
			{
				return Sort(query, r => r.Bid, pageSort.OrderBy.IsAsc);
			}

			if (pageSort.OrderBy.FieldName == "SymbolName")
			{
				return Sort(query, r => r.SymbolName, pageSort.OrderBy.IsAsc);
			}

			if (pageSort.OrderBy.FieldName == "UtcTimeStamp")
			{
				return Sort(query, r => r.UtcTimeStamp, pageSort.OrderBy.IsAsc);
			}

			if (pageSort.OrderBy.FieldName == "Spread")
			{
				return Sort(query, r => r.Spread, pageSort.OrderBy.IsAsc);
			}

			if (pageSort.OrderBy.FieldName == "TypeDescription")
			{
				return Sort(query, r => r.TypeDescription, pageSort.OrderBy.IsAsc);
			}

			return query;
		}

		private IEnumerable<TSource> Sort<TSource, TKey>(IEnumerable<TSource> query, Func<TSource, TKey> keySelector, bool isAsc)
		{
			if (isAsc)
			{
				return query.OrderBy(keySelector);
			}

			return query.OrderByDescending(keySelector);
		}
	}
}
