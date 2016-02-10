using Avalon.Text;
using FortressPrime.BackEnd.Common.Beans;
using FortressPrime.BackEnd.Common.Beans.IB;
using FortressPrime.BackEnd.Common.Beans.User;
using FortressPrime.BackEnd.Database;
using FortressPrime.BackEnd.Database.DL;
using FortressPrime.BackEnd.Reporting.Common;
using FortressPrime.BackEnd.Reporting.Management;
using FortressPrime.Web;
using FortressPrime.Web.AngularJS;
using FortressPrime.Web.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Models.Reports;

namespace WebSite.Areas.Incorp.Controllers.Management
{
	public class UserManagementController : AdminAreaBaseController
	{
		public UserManagementController(DataLayerFactory dataLayerFactory)
		{
			this.DLFactory = dataLayerFactory;
		}

		public DataLayerFactory DataLayerFactory{ get; set; }

		public FortressPrime.BackEnd.WcfService.ClientService BackEndWcfClient { get; set; }

		public ActionResult UserList()
		{
			return View();
		}

		public ActionResult UserEdit()
		{
			return View();
		}

		public ActionResult GetUserJson(int userID)
		{
			UserEntity userData = null;

			this.ExecuteInDBTransaction((dl) =>
			{
				userData = dl.UserDL.GetUserByID(userID);
			});

			return new JsonNetResult() { Data = userData, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
		}

		public ActionResult GetTradeGroups()
		{
			List<string> tradeGroups = null;

			this.ExecuteInDBTransaction((dl) =>
			{
				tradeGroups = dl.UserDL.GetTradeGroups();
			});

			return new JsonNetResult() { Data = tradeGroups, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
		}

		public ActionResult GetUserAgentAccount(int userID)
		{
			int? agentAccount = null;

			this.ExecuteInDBTransaction((dl) => {
				agentAccount = dl.UserDL.GetUserAgentAccount(userID);
			});

			return new JsonNetResult() { Data = agentAccount, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
		}

		public ActionResult GetUserTradingConfigurations(int userID)
		{
			List<UserTradingConfiguration> UserTradingConfigurations = new List<UserTradingConfiguration>();

			this.ExecuteInDBTransaction((dl) =>
			{
				UserTradingConfigurations = dl.UserDL.getUserTradingConfigurations(userID);
			});

			return new JsonNetResult() { Data = UserTradingConfigurations, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
		}

		public ActionResult GetIBRelationsForUserJson(int userID, int ibRelationType)
		{
			List<IBRelation> IBRelationList = new List<IBRelation>();

			this.ExecuteInDBTransaction((dl) =>
			{
				IBRelationList = dl.IBDL.getIBRelationListForUser(userID, (IBRelationType)ibRelationType);
			});

			return new JsonNetResult() { Data = IBRelationList, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };

		}
		public ActionResult GetRebatesType2ForUserJson(int userID)
		{
			List<SplitRebatesConfiguration> IBRelationList = new List<SplitRebatesConfiguration>();

			this.ExecuteInDBTransaction((dl) =>
			{
				IBRelationList = dl.IBDL.getSplitRebatesForUser(userID, 2 /*type*/);
			});

			return new JsonNetResult() { Data = IBRelationList, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };

		}

		public ActionResult GetRebatesType3ForUserJson(int userID)
		{
			List<SplitRebatesConfiguration> IBRelationList = new List<SplitRebatesConfiguration>();

			this.ExecuteInDBTransaction((dl) =>
			{
				IBRelationList = dl.IBDL.getSplitRebatesForUser(userID, 3 /*type*/);
			});

			return new JsonNetResult() { Data = IBRelationList, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };

		}

		public ActionResult SetSplitRebatesForUser(SplitRebatesConfiguration relation)
		{
			relation.DateTill = null;

			try
			{
				this.ExecuteInDBTransaction((dl) =>
				{
					dl.IBDL.setSplitRebatesForUser(relation);
					this.Logger.InfoFormat("User {0} saved rebate type2 configuration. Relation={1}", this.CurrentUserName, JsonSerializer.Serialize(relation));
				});

				return new JsonNetResult() { Data = 0, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
			catch (Exception e)
			{
				return new JsonNetResult() { Data = e.Message, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
		}

		public ActionResult SetType1Rebates(List<IBRelation> relations)
		{
			try
			{
				this.ExecuteInDBTransaction((dl) =>
				{
					dl.IBDL.setType1RebatesForUser(relations);
					this.Logger.InfoFormat("User {0} saved rebate type1 configuration. Relations={1}", this.CurrentUserName, JsonSerializer.Serialize(relations));
				});

				return new JsonNetResult() { Data = 0, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
			catch (Exception e)
			{
				return new JsonNetResult() { Data = e.Message, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
		}

		public ActionResult SaveSplitRebatesHistory(IList<SplitRebatesConfiguration> relations)
		{
			try
			{
				this.ExecuteInDBTransaction((dl) =>
				{
					dl.IBDL.saveSplitRebatesHistory(relations);
					this.Logger.InfoFormat("User {0} saved rebate split HISTORY. Relation={1}", this.CurrentUserName, JsonSerializer.Serialize(relations));
				});

				return new JsonNetResult() { Data = 0, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
			catch (Exception e)
			{
				return new JsonNetResult() { Data = e.Message, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
		}

		public ActionResult DeleteUserTradingConfiguration(int userTradingConfigurationID)
		{
			try
			{
				this.ExecuteInDBTransaction((dl) =>
				{
					dl.UserDL.removeUserTradingConfiguration(userTradingConfigurationID);

				});
				this.Logger.InfoFormat("User {0} deleted UserTradingConfiguration. userTradingConfigurationID ={1}", this.CurrentUserName, userTradingConfigurationID);
				return new JsonNetResult() { Data = 0, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
			catch (Exception e)
			{
				return new JsonNetResult() { Data = e.Message, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
		}

		public ActionResult UpdateUser(UserEntity user)
		{
			this.Logger.InfoFormat("User {0} Updated User. UserEntity={1}", this.CurrentUserName, JsonSerializer.Serialize(user));
			this.BackEndWcfClient.UpdateUser(user);
			return new EmptyResult();
		}

		public ActionResult SaveOrUpdateUserTradingConfiguration(UserTradingConfiguration utc)
		{
			UserTradingConfiguration result = null;

			bool overlap = false;
			if (utc.DateFrom > utc.DateTill)
			{
				throw new Exception("Date Till can not be before Date From");
			}

			//check for timeframe correctness
			try
			{
				this.ExecuteInDBTransaction((dl) =>
				{
					overlap = dl.UserDL.UserTradingConfigurationOverlap(utc);
				});

				if (overlap)
				{
					throw new Exception("This user has overlapping methodics");
				}

				this.ExecuteInDBTransaction((dl) =>
				{
					result = dl.UserDL.SaveOrUpdateUserTradingConfiguration(utc);
					this.Logger.InfoFormat("User {0} saved user trading configuration. UserEntity={1}", this.CurrentUserName, JsonSerializer.Serialize(utc));
				});
				if (result != null)
				{
					return new JsonNetResult() { Data = result, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
				}

				return new JsonNetResult() { Data = 0, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
			catch (Exception e)
			{
				return new JsonNetResult() { Data = e.Message, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
		}

		public ActionResult BulkConfigurationUpdate(IList<UserEntity> users, TradingConfiguration tc)
		{
			IList<UserTradingConfiguration> result = null;

			try
			{
				this.ExecuteInDBTransaction((dl) =>
				{
					result = dl.UserDL.bulkUserTradingConfigurationUpdate(users, tc);
					this.Logger.InfoFormat("User {0} called BulkConfigurationUpdate. users={1}, tc={2}", this.CurrentUserName, JsonSerializer.Serialize(users), JsonSerializer.Serialize(tc));
				});

				if (result != null)
				{
					return new JsonNetResult() { Data = result, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
				}

				return new JsonNetResult() { Data = 0, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
			catch (Exception e)
			{
				return new JsonNetResult() { Data = e.Message, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
		}

		public ActionResult GetUsersJson(TableDisplayParams tableParams)
		{
			var data = this.GetUsers(tableParams);
			return new JsonNetResult() { Data = data, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
		}

		public ActionResult GetUsersWithFilter(TableDisplayParams tableParams, string searchString)
		{
			var parameters = new UserManagement.SelectParameters();

			parameters.searchString = searchString;

			parameters.PageSort = PageSortReportParameters.CreateFrom(tableParams);

			try
			{
				var data = UserManagement.GetAllUsers(parameters);
				return new JsonNetResult() { Data = data, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
			catch (Exception e)
			{
				return new JsonNetResult() { Data = e.Message, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
			}
			
		}

		public ActionResult GetMostFrequentTradingConfigurations()
		{
			List<TradingConfiguration> result = null;

			this.ExecuteInDBTransaction((dl) =>
			{
				result = dl.TradingConfigurationDL.getMostFrequentTradingConfigurations();
			});
			return new JsonNetResult() { Data = result, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
		}

		public ActionResult DownloadUserListAsCSV()
		{
			var data = this.GetUsers(null);
			var header = Avalon.Text.CSVWriter.ToCSVString(System.Globalization.CultureInfo.CurrentCulture,
				"Login", "Name", "Trade Group", "Send Invoices to MT4", "VolumeFee/Majors", "VolumeFee/Minors", "VolumeFee/Metals");

			var csv = header + Avalon.Text.CSVWriter.ToCSVString(System.Globalization.CultureInfo.CurrentCulture, data.ResultsForPage, (w, u) =>
			{
				w.WriteLine(u.LoginName, u.UserName, u.TradeGroup
					, u.TradingConfiguration == null ? string.Empty : u.TradingConfiguration.SendInvoicesToExternal.ToString()
					, u.TradingConfiguration == null ? string.Empty : u.TradingConfiguration.UsdVolumeFeeRateFXMajor.ToString()
					, u.TradingConfiguration == null ? string.Empty : u.TradingConfiguration.UsdVolumeFeeRateFXMinor.ToString()
					, u.TradingConfiguration == null ? string.Empty : u.TradingConfiguration.UsdVolumeFeeRateMetals.ToString()
					, u.TradingConfiguration == null ? string.Empty : u.TradingConfiguration.UsdVolumeFeeRateCFD.ToString());
			});

			string fileName = string.Format("UserList-{0}.csv", DateTime.UtcNow.ToString("G"));

			byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(csv);
			return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
		}


		private ResultPage<FortressPrime.BackEnd.Common.Beans.User.UserEntity> GetUsers(TableDisplayParams tableParams)
		{
			var parameters = new UserManagement.SelectParameters();

			parameters.PageSort = PageSortReportParameters.CreateFrom(tableParams);

			return UserManagement.GetAllUsers(parameters);
		}
	}
}