using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortressPrime.BackEnd.Reporting.FProogle
{
	public class FProogleSettings
	{
		private Dictionary<string, Dictionary<Type, Dictionary<string, string>>> templates;

		private Dictionary<string, List<KeyShortcut>> sKeys;

		private Dictionary<string, List<KeyItem>> fKeys;

		private static FProogleSettings instance;

		private FProogleSettings() { 
			//add settings here
			//TEMPLATES
			Dictionary<Type, Dictionary<string, string>> templatesForType = new Dictionary<Type, Dictionary<string, string>>();

			Dictionary<string, string> intOpers = new Dictionary<string, string>();
			intOpers["="] = @"{0}=={1}";
			intOpers["!="] = @"{0}~={1}";
			intOpers[">"] = @"{0}>{1}";
			intOpers["<"] = @"{0}<{1}";
			intOpers[">="] = @"{0}>={1}";
			intOpers["<="] = @"{0}<={1}";
			templatesForType[typeof(int)] = intOpers;

			Dictionary<string, string> decimalOpers = new Dictionary<string, string>();
			decimalOpers["="] = @"{0}=={1}";
			decimalOpers["!="] = @"{0}~={1}";
			decimalOpers[">"] = @"{0}>{1}";
			decimalOpers["<"] = @"{0}<{1}";
			decimalOpers[">="] = @"{0}>={1}";
			decimalOpers["<="] = @"{0}<={1}";
			templatesForType[typeof(decimal)] = decimalOpers;

			Dictionary<string, string> stringOpers = new Dictionary<string, string>();
			stringOpers["="] = @"string.find(string.lower({0}), '{1}')~=nil";
			stringOpers["-"] = @"string.find(string.lower({0}), '{1}')==nil";
			stringOpers["!="] = @"string.find(string.lower({0}), '{1}')==nil";
			stringOpers["empty"] = "'{0}'=='{1}'";
			templatesForType[typeof(string)] = stringOpers;

			Dictionary<string, string> objectOpers = new Dictionary<string, string>();
			objectOpers["="] = @"{0}==nil";
			objectOpers["!="] = @"{0}~=nil";
			objectOpers["no"] = @"{0}==nil";
			templatesForType[typeof(object)] = objectOpers;

			Dictionary<string, string> boolOpers = new Dictionary<string, string>();
			//boolOpers["="] = @"('{0}' == true and 'yes' or 'no')=='{1}'";
			boolOpers["="] = @"{0}=={1}";
			templatesForType[typeof(bool)] = boolOpers;

			templates = new Dictionary<string, Dictionary<Type, Dictionary<string, string>>>();
			templates["UserList"] = templatesForType;

			//SHORT KEYS
			List<KeyShortcut> shortKeys = new List<KeyShortcut>();
			shortKeys.Add(new KeyShortcut { shortName = "N", fullName = "UserName" });
			shortKeys.Add(new KeyShortcut { shortName = "G", fullName = "TradeGroup" });
			shortKeys.Add(new KeyShortcut { shortName = "L", fullName = "UserID" });
			shortKeys.Add(new KeyShortcut { shortName = "S", fullName = "TradingConfiguration.SendInvoicesToExternal" });
			shortKeys.Add(new KeyShortcut { shortName = "Mt", fullName = "TradingConfiguration.UsdVolumeFeeRateMetals" });
			shortKeys.Add(new KeyShortcut { shortName = "Mj", fullName = "TradingConfiguration.UsdVolumeFeeRateFXMajor" });
			shortKeys.Add(new KeyShortcut { shortName = "Mn", fullName = "TradingConfiguration.UsdVolumeFeeRateFXMinor" });
			shortKeys.Add(new KeyShortcut { shortName = "Name", fullName = "UserName" });
			shortKeys.Add(new KeyShortcut { shortName = "Login", fullName = "UserID" });
			shortKeys.Add(new KeyShortcut { shortName = "Group", fullName = "TradeGroup" });
			shortKeys.Add(new KeyShortcut { shortName = "Send", fullName = "TradingConfiguration.SendInvoicesToExternal" });
			shortKeys.Add(new KeyShortcut { shortName = "SendInvoicesToExternal", fullName = "TradingConfiguration.SendInvoicesToExternal" });
			shortKeys.Add(new KeyShortcut { shortName = "CA", fullName = "TradingConfiguration.CommissionAccount" });
			shortKeys.Add(new KeyShortcut { shortName = "CommissionAccount", fullName = "TradingConfiguration.CommissionAccount" });
			shortKeys.Add(new KeyShortcut { shortName = "unconfigured", fullName = "TradingConfiguration", standalone = true });
			shortKeys.Add(new KeyShortcut { shortName = "UC", fullName = "TradingConfiguration", standalone = true });
			shortKeys.Add(new KeyShortcut { shortName = "TradingConfiguration", fullName = "TradingConfiguration" });
			shortKeys.Add(new KeyShortcut { shortName = "TC", fullName = "TradingConfiguration" });
			shortKeys.Add(new KeyShortcut { shortName = "All", fullName = "All" });
			shortKeys.Add(new KeyShortcut { shortName = "ALL", fullName = "All" });
			shortKeys.Add(new KeyShortcut { shortName = "all", fullName = "All" });
			shortKeys.Add(new KeyShortcut { shortName = "Any", fullName = "Any" });
			shortKeys.Add(new KeyShortcut { shortName = "any", fullName = "Any" });
			shortKeys.Add(new KeyShortcut { shortName = "ANY", fullName = "Any" });

			sKeys = new Dictionary<string,List<KeyShortcut>>();
			sKeys["UserList"] = shortKeys;

			//FULL KEYS
			List<KeyItem> fullKeys = new List<KeyItem>();
			fullKeys.Add(new KeyItem { fullName = "UserName", type = typeof(string), oper = "=", bulk = true, treatAs = typeof(string) });
			fullKeys.Add(new KeyItem { fullName = "UserName", type = typeof(string), oper = "-",  bulk = true, treatAs = typeof(string) });
			fullKeys.Add(new KeyItem { fullName = "UserName", type = typeof(string), oper = "!=",  bulk = true, treatAs = typeof(string) });
			fullKeys.Add(new KeyItem { fullName = "UserName", type = typeof(string), oper = "empty",  bulk = true, treatAs = typeof(string) });

			fullKeys.Add(new KeyItem { fullName = "UserID", type = typeof(int), oper = "=", bulk = true, treatAs = typeof(string) });
			fullKeys.Add(new KeyItem { fullName = "UserID", type = typeof(int), oper = "!=", bulk = true, treatAs = typeof(string) });
			fullKeys.Add(new KeyItem { fullName = "UserID", type = typeof(int), oper = ">", bulk = false, treatAs = typeof(int) });
			fullKeys.Add(new KeyItem { fullName = "UserID", type = typeof(int), oper = "<", bulk = false, treatAs = typeof(int) });
			fullKeys.Add(new KeyItem { fullName = "UserID", type = typeof(int), oper = ">=",  bulk = false, treatAs = typeof(int) });
			fullKeys.Add(new KeyItem { fullName = "UserID", type = typeof(int), oper = "<=",  bulk = false, treatAs = typeof(int) });

			fullKeys.Add(new KeyItem { fullName = "TradeGroup", type = typeof(string), oper = "=", bulk = true, treatAs = typeof(string) });
			fullKeys.Add(new KeyItem { fullName = "TradeGroup", type = typeof(string), oper = "-", bulk = true, treatAs = typeof(string) });
			fullKeys.Add(new KeyItem { fullName = "TradeGroup", type = typeof(string), oper = "!=", bulk = true, treatAs = typeof(string) });
			fullKeys.Add(new KeyItem { fullName = "TradeGroup", type = typeof(string), oper = "empty", bulk = true, treatAs = typeof(string) });

			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.SendInvoicesToExternal", type = typeof(bool), oper = "=", bulk = true, treatAs = typeof(bool) });

			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateMetals", type = typeof(decimal), oper = "=", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateMetals", type = typeof(decimal), oper = "!=", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateMetals", type = typeof(decimal), oper = ">", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateMetals", type = typeof(decimal), oper = "<", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateMetals", type = typeof(decimal), oper = ">=", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateMetals", type = typeof(decimal), oper = "<=", bulk = true, treatAs = typeof(decimal) });

			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateFXMajor", type = typeof(decimal), oper = "=", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateFXMajor", type = typeof(decimal), oper = "=", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateFXMajor", type = typeof(decimal), oper = ">", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateFXMajor", type = typeof(decimal), oper = "<", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateFXMajor", type = typeof(decimal), oper = ">=", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateFXMajor", type = typeof(decimal), oper = "<=", bulk = true, treatAs = typeof(string) });

			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateFXMinor", type = typeof(decimal), oper = "=", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateFXMinor", type = typeof(decimal), oper = "=", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateFXMinor", type = typeof(decimal), oper = ">", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateFXMinor", type = typeof(decimal), oper = "<", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateFXMinor", type = typeof(decimal), oper = ">=", bulk = true, treatAs = typeof(decimal) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.UsdVolumeFeeRateFXMinor", type = typeof(decimal), oper = "<=", bulk = true, treatAs = typeof(decimal) });

			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.CommissionAccount", type = typeof(int), oper = "=",  bulk = false, treatAs = typeof(string) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.CommissionAccount", type = typeof(int), oper = "!=",  bulk = false, treatAs = typeof(string) });

			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration", type = typeof(Object), oper = "=",  bulk = false, treatAs = typeof(Object) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration", type = typeof(Object), oper = "!=", bulk = false, treatAs = typeof(Object) });

			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration", type = typeof(Object), oper = "no", bulk = false, treatAs = typeof(Object) });
			fullKeys.Add(new KeyItem { fullName = "TradingConfiguration.CommissionAccount", type = typeof(int), oper = "no", bulk = false, treatAs = typeof(Object) });

			fullKeys.Add(new KeyItem { fullName = "All", type = typeof(Object), oper = "=",  bulk = false, treatAs = typeof(Object) });
			fullKeys.Add(new KeyItem { fullName = "Any", type = typeof(Object), oper = "=",  bulk = false, treatAs = typeof(Object) });

			fKeys = new Dictionary<string, List<KeyItem>>();
			fKeys["UserList"] = fullKeys;
		}

		public static FProogleSettings Instance {
			get {
				if (instance == null)
				{
					instance = new FProogleSettings();
				}
				return instance;
			}
		}

		public Tuple<Dictionary<Type, Dictionary<string, string>>, List<KeyShortcut>, List<KeyItem>> getConfiguration(string key)
		{
			return Tuple.Create(templates[key], sKeys[key], fKeys[key]);
		}
	}
}