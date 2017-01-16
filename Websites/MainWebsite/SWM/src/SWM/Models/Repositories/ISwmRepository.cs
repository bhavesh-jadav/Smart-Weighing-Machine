using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWM.JsonModels;
using SWM.Models.ApiModels;
using SWM.ViewModels;

namespace SWM.Models.Repositories
{
    public interface ISwmRepository
    {
        List<ProductInfo> GetProductInfoByUserName(string userName);
        List<LocationInfo> GetLocationInfoByUserName(string userName);
        Task<SwmUser> GetUserByUserName(string userName);
        Task<SwmUser> GetUserByUserId(string id);
        string GetStateName(int stateId);
        string GetCountryName(int countryId);
        List<ProductInfo> GetProductInfoByUserNameAndLocation(string locationName, string userName);
        ProductInformation GetProductInformation(int productId);
        UserInformation GetUserInformationForAPI(string userName);
        Task<bool> CheckUserPassword(SwmUser user, string password);
        bool AddNewDataFromMachine(DataFromMachineModel data, string userId, int machineId);
        List<TableDataModel> GetDataForDataTable(SwmUser user);
        string[] GetSubscriptionTypes();
        Task<bool> AddNewUser(AddNewUserModel userModel);
        List<ShowUserModel> GetAllUsers();
        Task<bool> RemoveUser(RemoveUserModel userModel);
        bool AddNewLocation(AddNewLocationModel newLocation, string userId);
        UserDashboardModel GetDashBoardForUser(string userId);
    }
}
