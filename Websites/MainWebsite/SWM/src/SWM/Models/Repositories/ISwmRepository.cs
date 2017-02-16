using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWM.Models.ApiModels;
using SWM.ViewModels;

namespace SWM.Models.Repositories
{
    public interface ISwmRepository
    {
        List<ProductInfoModel> GetProductInfoByUserName(string userName);
        List<LocationInfoModel> GetLocationInfoByUserName(string userName);
        Task<SwmUser> GetUserByUserName(string userName);
        Task<SwmUser> GetUserByUserId(string id);
        string GetStateName(int stateId);
        string GetCountryName(int countryId);
        List<ProductInfoModel> GetProductInfoByUserNameAndLocation(string locationName, string userName);
        ProductInformation GetProductInformation(int productId);
        Task<bool> CheckUserPassword(SwmUser user, string password);
        bool AddNewDataFromMachine(DataFromMachineModel data);
        List<TableDataModel> GetDataForDataTable(SwmUser user);
        string[] GetSubscriptionTypes();
        Task<bool> AddNewUser(AddNewUserModel userModel);
        List<ShowUserModel> GetAllUsers();
        Task<bool> RemoveUser(RemoveUserModel userModel);
        bool AddNewLocation(AddNewLocationModel newLocation, string userId);
        UserDashboardModel GetDashBoardForUser(string userId);
        bool AddNewProduct(string userId, AddNewProductModel newProduct);
        AdminDashboardModel GetDashBoardForAdmin();
        List<KeyValuePair<int, string>> GetProductNames(string userId);
        List<KeyValuePair<int, string>> GetLocationNames(string userId);
        List<ProductDataMonthWiseModel> GetProductDataMonthWise(string userName, int startMonth, int startYear, int endMonth, int endYear);
        List<DateTime> GetUserMonths(string userName);
        PublicDashboardModel GetDashBoardForPublic();
        List<SearchUserModel> GetSearchResultForUserByFullName(string fullName);
        UserDetailsModel GetUserDetails(string subId);
        List<SearchUserModel> AdvanceSearchResults(AdvanceSearchModel parameters);
    }
}
