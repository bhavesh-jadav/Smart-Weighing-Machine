﻿using System;
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
        List<ProductInfoModel> GetAllProductInformation();
        List<LocationInfoModel> GetLocationInfoByUserName(string userName);
        Task<SwmUser> GetUserByUserName(string userName);
        Task<SwmUser> GetUserByUserId(string id);
        List<ProductInfoModel> GetProductInfoByUserNameAndLocation(string locationName, string userName);
        Task<bool> CheckUserPassword(SwmUser user, string password);
        bool AddNewDataFromMachine(DataFromMachineModel data);
        List<TableDataModel> GetUserDataForDataTable(SwmUser user);
        string[] GetSubscriptionTypes();
        Task<bool> AddNewUser(AddNewUserModel userModel);
        List<ShowUserModel> GetAllUsersForAdmin();
        Task<bool> RemoveUser(RemoveUserModel userModel);
        bool AddNewLocation(AddNewLocationModel newLocation, string userId);
        bool AddNewProduct(string userId, AddNewProductModel newProduct);
        AdminDashboardModel GetDashBoardForAdmin();
        List<KeyValuePair<int, string>> GetProductNames(string userId);
        List<KeyValuePair<int, string>> GetLocationNames(string userId);
        List<ProductDataMonthWiseModel> GetProductDataMonthWise(string userName, int startMonth, int startYear, int endMonth, int endYear);
        List<DateTime> GetDateRangeOfUserData(string userName);
        PublicDashboardModel GetDashBoardForPublic();
        List<SearchUserModel> GetAllUsersForPublic(int pageNo);
        List<SearchUserModel> GetSearchResultForUserByFullName(string fullName);
        UserDetailsModel GetUserDetails(string subId);
        List<SearchUserModel> AdvanceSearchResults(AdvanceSearchModel parameters);
        string GetSubIdFromUserName(string userName);
        UserDetailsModel GetUserDetailsLight(string subId);
        dynamic test_method(string userName);
        int GetTotalUsers();
    }
}
