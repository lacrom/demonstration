using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;
using System.Text.RegularExpressions;

namespace FortressPrime.BackEnd.Reporting.FProogle
{
	public class FProogle<T> : IFProogle<T>
	{
		private Dictionary<Type, Dictionary<string, string>> templatesForType;

		private List<KeyShortcut> shortKeys;

		private List<KeyItem> fullKeys;

		private Lua state = new Lua();

		public FProogle(Tuple<Dictionary<Type, Dictionary<string, string>>, List<KeyShortcut>, List<KeyItem>> settings)
		{
			this.templatesForType = settings.Item1;
			this.shortKeys = settings.Item2;
			this.fullKeys = settings.Item3;
		}

		public IEnumerable<T> filter(List<T> unfiltered, string searchString)
		{
			List<string> paramList = new List<string>();

            //remove additional spaces
			searchString = buttonUpSpaces(searchString);

			StringBuilder sBuilder = new StringBuilder(searchString);
			Match match;

			//get KVP where value is in quotes
			string keyQuotedValuePairs = "\\b" + getKVPKeys() + "\\b *" + getKVPOpers() + " *\"[\\w\\s]*[^\"\']*\"";

			do
			{
				match = Regex.Match(sBuilder.ToString(), keyQuotedValuePairs);
				if (match.Success)
				{
					paramList.Add(match.Value);
					sBuilder = sBuilder.Remove(match.Index, match.Length);
					sBuilder = sBuilder.Insert(match.Index, "{" + (paramList.Count() - 1) + "}");
				}
			} while (match.Success);

			//match string between quotes
			//string betweenQuotes = "[\"'].*[\"']";
			string betweenQuotes = "([\"\'])(?:(?=(\\\\?))\\2.)*?\\1";

			do
			{
				match = Regex.Match(sBuilder.ToString(), betweenQuotes);
				if (match.Success)
				{
					paramList.Add(match.Value);
					sBuilder = sBuilder.Remove(match.Index, match.Length);
					sBuilder = sBuilder.Insert(match.Index, "{" + (paramList.Count() - 1) + "}");
				}
			} while (match.Success);

            //get conditions with key and value
			string keyValuePairs = @"\b" + getKVPKeys() + @"\b *" + getKVPOpers() + @" *[\w.]+";

			do
			{
				match = Regex.Match(sBuilder.ToString(), keyValuePairs);
				if (match.Success)
				{
					paramList.Add(match.Value);
					sBuilder = sBuilder.Remove(match.Index, match.Length);
					sBuilder = sBuilder.Insert(match.Index, "{" + (paramList.Count() - 1) + "}");
				}
			} while (match.Success);

            //find conditions where only operator and value exists
			string generalConditionsWithOperator = @"" + getKVPOpers() + " *[\\w.\"\']+";

			do
			{
				match = Regex.Match(sBuilder.ToString(), generalConditionsWithOperator);
				if (match.Success)
				{
					paramList.Add(match.Value);
					sBuilder = sBuilder.Remove(match.Index, match.Length);
					sBuilder = sBuilder.Insert(match.Index, "{" + (paramList.Count() - 1) + "}");
				}
			} while (match.Success);

            //find conditions with word "no"
			string noConditions = @"\bno\b +\b" + getNoKeys() + @"\b";

			do
			{
				match = Regex.Match(sBuilder.ToString(), noConditions);
				if (match.Success)
				{
					paramList.Add(match.Value);
					sBuilder = sBuilder.Remove(match.Index, match.Length);
					sBuilder = sBuilder.Insert(match.Index, "{" + (paramList.Count() - 1) + "}");
				}
			} while (match.Success);

			string generalCondition = @"(?<!{)(?!})(?!or)(?!OR)(?!and)(?!AND)\b[\w.]+\b";

			do
			{
				match = Regex.Match(sBuilder.ToString(), generalCondition);
				if (match.Success)
				{
					paramList.Add(match.Value);
					sBuilder = sBuilder.Remove(match.Index, match.Length);
					sBuilder = sBuilder.Insert(match.Index, "{" + (paramList.Count() - 1) + "}");
				}
			} while (match.Success);

			string replaceSpaces = @"(?<!{)(?!})(?<!or)(?<!OR)(?<!and)(?<!AND) +(?!or)(?!OR)(?!and)(?!AND)";

			do
			{
				match = Regex.Match(sBuilder.ToString(), replaceSpaces);
				if (match.Success)
				{
					sBuilder = sBuilder.Remove(match.Index, match.Length);
					sBuilder = sBuilder.Insert(match.Index, " and ");
				}
			} while (match.Success);

			//parse all parameters
			var parsedParams = paramList.Select(item => parseParameter(item)).ToList();

			//detect mode
			bool allWithKeys = true;
			bool hasKeys = false;
			bool hasOpersWithoutKeys = false;

			foreach (var item in parsedParams)
			{
				if (item.Key != null && item.Key != "TradingConfiguration")
				{
					hasKeys = true;
				}
				else if (item.Key != "TradingConfiguration")
				{
					allWithKeys = false;
				}

				if (item.Operator != null && item.Key == null)
				{
					hasOpersWithoutKeys = true;
				}
			}

			if (allWithKeys)
			{
				//promode
				paramList = parsedParams.Select(item => transformParameter(item)).ToList();
				string outString = string.Format(sBuilder.ToString(), paramList.ToArray());
				var result = unfiltered.Where(item => evalItem(item, outString)).ToList();
				return result;
			}
			else if (hasKeys)
			{
				//ERROR: all keys shoud be specified
				throw new Exception("If you use shortcuts you should use it with every condition");
			} else if (hasOpersWithoutKeys) {
				string luastr = null;

				if (typeof(T) == typeof(BackEnd.Common.Beans.User.UserEntity))
				{
					var UCKey = parsedParams.Where(item => item.Key == "TradingConfiguration").FirstOrDefault();

					if (UCKey != null)
					{
						luastr += transformParameter(UCKey);
						searchString = searchString.Remove(searchString.IndexOf("UC"), 2).Trim();
						parsedParams.Remove(UCKey);
					}
				}
				
				var tmp = getSearchStringForSemiMode(parsedParams, searchString);

				if (luastr != null)
				{
					luastr += " and (" + tmp + ")";
				}
				else
				{
					luastr = tmp;
				}

				//semi mode
				string outString = string.Format(luastr, paramList.ToArray());
				var result = unfiltered.Where(item => evalItem(item, outString)).ToList();
				return result;
			}
			else
			{
				string luastr = null;

				//for user list only
				if (typeof(T) == typeof(BackEnd.Common.Beans.User.UserEntity))
				{
					var UCKey = parsedParams.Where(item => item.Key == "TradingConfiguration").FirstOrDefault();

					if (UCKey != null)
					{
						luastr += transformParameter(UCKey);
						searchString = searchString.Remove(searchString.IndexOf("UC"), 2).Trim();
					}
				}

				//simplest mode
				ParameterItem pi = new ParameterItem() {
					Value = searchString
				};

				var tmp = transformParameter(pi);

				if (luastr != null)
				{
					luastr += " and (" + tmp + ")";
				}
				else
				{
					luastr = tmp;
				}

				var result = unfiltered.Where(item => evalItem(item, luastr)).ToList();
				return result;
			}
		}

        //evaluate item using lua
		private bool evalItem(T item, string luastr)
		{
			state["item"] = item;

			luastr = "return " + luastr;

			try
			{
				bool res = (bool)state.DoString(luastr).ToList()[0];
				return res;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
		}

		private ParameterItem parseParameter(string p)
		{
			//GET KEYS
			string getKey = @"(\b" + getKVPKeys() + @"\b(?= *" + getKVPOpers() + @"))|((?<=\bno\b *)" + getKVPKeys() + @")";
			ParameterItem newParam = new ParameterItem();

			Match match = Regex.Match(p, getKey);
			if (match.Success)
			{
				newParam.Key = match.Value;
				p = p.Remove(match.Index, match.Length);
			}

			////GET OPERATORS	
			p = p.Trim();
			if (p == "no" && newParam.Key != null)
			{
				newParam.Operator = "no";
			}
			else
			{
				string getOp = getKVPOpers() + "(?= *(\\w|\"|\'))";

				match = Regex.Match(p, getOp, RegexOptions.IgnoreCase);
				if (match.Success)
				{
					newParam.Operator = match.Value;
					p = p.Remove(match.Index, match.Length);
				}

				p = p.Trim();

				if (!p.Contains('\"'))
				{
					p = p.Replace(" ", "");
				}

				if (p != "")
				{

					if (getStandaloneKeys().Contains(p))
					{
						newParam.Key = shortKeys.Where(item => item.shortName == p).FirstOrDefault().fullName;
						newParam.Operator = "=";
						newParam.Value = "null";
					}
					else
					{
						newParam.Value = p;
					}
				}
			}

			return newParam;
		}

        //transform parameter into string
		private string transformParameter(ParameterItem parsed)
		{
			string p = null;

			if (parsed.Key != null && parsed.Operator != null && (parsed.Value != null || parsed.Operator == "no"))
			{
				//replace with full name
				parsed.Key = shortKeys.Where(item => item.shortName == parsed.Key).FirstOrDefault().fullName;

				if (parsed.Key == "All" || parsed.Key == "Any")
				{
					p = makeParamOperValue(parsed);
				}
				else
				{
					p = makeParamTwo(parsed);
				}
			}
			else if (parsed.Value != null)
			{
				p = makeParamOperValue(parsed);
			}
			
			else
			{
				p = " false ";
			}

			return p;
		}

        //make string from parameter which contains BOTH KEY and VALUE
		private string makeParamTwo(ParameterItem p)
		{
			string template = null;

			if (getValueType(p) == typeof(string))
			{
				//delete quotes if present
				p.Value = p.Value.Replace("\"", "");
			}

            //if value is null then replace it with nil (lua specific)
			if (fullKeys.Where(item => item.oper == p.Operator && item.fullName == p.Key).Select(item => item.treatAs).FirstOrDefault() == typeof(string)
				&& fullKeys.Where(item => item.oper == p.Operator && item.fullName == p.Key).Select(item => item.type).FirstOrDefault() == typeof(int) && p.Value == "null")
			{
				p.Value = "nil";
                //find template for operator, key and type
				template = templatesForType[fullKeys.Where(item => item.oper == p.Operator && item.fullName == p.Key).Select(item => item.type).FirstOrDefault()][p.Operator];
			}
			else
			{
				template = templatesForType[fullKeys.Where(item => item.oper == p.Operator && item.fullName == p.Key).Select(item => item.treatAs).FirstOrDefault()][p.Operator];
			}

			if (template != null)
			{
				if (p.Value != null)
				{
					p.Value = p.Value.ToLower();
				}
				return string.Format(template, "item." + p.Key, p.Value);
			}
			else
			{
				return " false ";
			}
		}

        //make string from parameter which contains only operator and value
		private string makeParamOperValue(ParameterItem p)
		{
			p.Operator = p.Operator != null ? p.Operator : "=";

			Type type = getValueType(p);

			int paramCount = 0;
			string outString = "(";

			//if it is int then search in strings, ints, decimals
			if (type == typeof(int))
			{
				//find keys for type
				foreach (string Key in fullKeys.Where(item => (item.type == typeof(decimal) || item.type == typeof(int) || item.type == typeof(string)) && item.bulk == true && item.oper == p.Operator).Select(item => item.fullName).ToList())
				{

					ParameterItem pi = new ParameterItem()
					{
						Key = Key,
						Operator = p.Operator,
						Value = p.Value
					};

					if (paramCount++ > 0)
					{
						if (p.Key == "All")
						{
							outString += " and ";
						}
						else
						{
							outString += " or ";
						}
					}

					outString += makeParamTwo(pi);

				}
				outString += ")";

				if (paramCount == 0)
				{
					return " false ";
				}
				else
				{
					return outString;
				}
			}
			else
			{
				//find keys for type
				foreach (string Key in fullKeys.Where(item => item.type == type && item.bulk == true && item.oper == p.Operator).Select(item => item.fullName).ToList())
				{
					ParameterItem pi = new ParameterItem()
					{
						Key = Key,
						Operator = p.Operator,
						Value = p.Value
					};

					if (paramCount++ > 0)
					{
						if (p.Key == "All")
						{
							outString += " and ";
						}
						else
						{
							outString += " or ";
						}
					}

					//make special condition for empty strings (compare with "") instead of checking for inclusion
					if (type == typeof(string) && p.Value == "")
					{
						pi.Operator = "empty";
					}

					outString += makeParamTwo(pi);

				}
				outString += ")";

				if (paramCount == 0)
				{
					return " false ";
				}
				else
				{
					return outString;
				}
			}
       	}

        //detect type of passed value
		private Type getValueType(ParameterItem p)
		{
			Type type = null;
			try
			{
				int.Parse(p.Value);
				type = typeof(int);

				if (p.Operator != null && fullKeys.Where(item => item.oper == p.Operator && item.type == type && item.bulk == true).FirstOrDefault() == null)
				{
					throw new Exception("specified key is not suitable for bulk operations");
				}
			}
			catch (Exception)
			{
				try
				{
					decimal.Parse(p.Value);
					type = typeof(decimal);
				}
				catch (Exception)
				{
					try
					{
						bool.Parse(p.Value);
						type = typeof(bool);
					}
					catch (Exception)
					{
						type = typeof(string);
					}
				}
			}
			return type;
		}

		private string getSearchStringForSemiMode(List<ParameterItem> ps , string searchString)
		{
			string result = null;

			List<ParameterItem> stringParams = new List<ParameterItem>();
			List<ParameterItem> intParams = new List<ParameterItem>();
			List<ParameterItem> decimalParams = new List<ParameterItem>();
			List<ParameterItem> boolParams = new List<ParameterItem>();
			Dictionary<Type, List<ParameterItem>> dict = new Dictionary<Type, List<ParameterItem>>();

			foreach(var item in ps)
			{
				var type = getValueType(item);

				if (type == typeof(string))
				{
					stringParams.Add(item);
				}
				else if (type == typeof(int))
				{
					intParams.Add(item);
				}
				else if (type == typeof(decimal))
				{
					decimalParams.Add(item);
				}
				else if (type == typeof(bool))
				{
					boolParams.Add(item);
				}
			}

			if (decimalParams.Count() != 0)
			{
				dict[typeof(decimal)] = decimalParams;
			}
			if (stringParams.Count() != 0)
			{
				dict[typeof(string)] = stringParams;
			}
			if (intParams.Count() != 0)
			{
				dict[typeof(int)] = intParams;
			}
			if (boolParams.Count() != 0)
			{
				dict[typeof(bool)] = boolParams;
			}

			List<string> typeStrings = new List<string>();

			//get string params
			foreach (var type in dict.Keys)
			{
				//get keys for this type MAYBE INT AMBIGUITY SHOUD BE HERE
				var TypeKeys = fullKeys.Where(item => item.type == type).Select(item => item.fullName).Distinct().ToList();
				List<string> keyStrings = new List<string>();

				foreach (var keyName in TypeKeys)
				{
					List<string> opStrings = new List<string>();

					//get opers for current type and key
					var opersForTypeAndKey = fullKeys.Where(item => item.type == type && item.fullName == keyName).Select(item => item.oper).ToList();
					//where oper, parameter

					foreach(var p in dict[type]) {

						if (p.Operator == null)
						{
							p.Operator = "=";
						}

						if (opersForTypeAndKey.Contains(p.Operator)) {
							p.Key = keyName;
							opStrings.Add(makeParamTwo(p));
						}
					}

					if (opStrings.Count() != 0)
					{
						keyStrings.Add("(" + string.Join(" and ", opStrings) + ")");
					}
				}

				if (keyStrings.Count() != 0)
				{
					typeStrings.Add("(" + string.Join(" or ", keyStrings) + ")");
				}
			}

			if (typeStrings.Count() != 0)
			{
				result = string.Join(" and ", typeStrings);
			}

			ParameterItem pi = new ParameterItem()
			{
				Operator = "=",
				Value = searchString
			};

			//add string itself with OR
			if (result != null) {
				result = "(" + result + ") or " + makeParamOperValue(pi);
			}
			else {
				result = makeParamOperValue(pi);
			}

			return result;
		}

		private string getKVPOpers()
		{
			//get only key value pair operators and keys
			var opers = fullKeys.Where(item => item.oper != "no")
				.GroupBy(test => test.oper)
			   .Select(grp => grp.First().oper)
			   .ToList();

			return "(" + string.Join("|", opers) + ")";
		}

		private string getKVPKeys()
		{
			//get only key value pair operators and keys
			List<string> KVPKeys = new List<string>();
			foreach (var shortKey in shortKeys)
			{
				foreach (var fullKey in fullKeys)
				{
					if (fullKey.fullName == shortKey.fullName && fullKey.oper != "no" && !shortKey.standalone && !KVPKeys.Contains(shortKey.shortName))
					{
						KVPKeys.Add(shortKey.shortName);
					}
				}
			}

			return "(" + string.Join("|", KVPKeys) + ")";
		}

		private string getNoKeys()
		{
			//get only key value pair operators and keys
			List<string> noKeys = new List<string>();
			foreach (var shortKey in shortKeys)
			{
				foreach (var fullKey in fullKeys)
				{
					if (fullKey.fullName == shortKey.fullName && fullKey.oper == "no" && !shortKey.standalone)
					{
						noKeys.Add(shortKey.shortName);
					}
				}
			}

			return "(" + string.Join("|", noKeys) + ")";
		}

		private List<string> getStandaloneKeys()
		{
			//get only key value pair operators and keys
			List<string> standaloneKeys = new List<string>();
			foreach (var shortKey in shortKeys)
			{
				foreach (var fullKey in fullKeys)
				{
					if (fullKey.fullName == shortKey.fullName && shortKey.standalone)
					{
						standaloneKeys.Add(shortKey.shortName);
					}
				}
			}

			return standaloneKeys;
		}

		private string buttonUpSpaces(string str)
		{
			str = str.Trim();
			while (str.Contains("  "))
			{
				str = str.Replace("  ", " ");
			}

			return str;
		} 
	}
}
