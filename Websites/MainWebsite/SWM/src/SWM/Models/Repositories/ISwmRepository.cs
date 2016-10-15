using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWM.JsonModels;

namespace SWM.Models.Repositories
{
    public interface ISwmRepository
    {
        List<ProductInfo> GetProductInfoByUserName(string userName);
        List<LocationInfo> GetLocationInfoByUserName(string userName);
        Task<SwmUser> GetUserByUserName(string userName);
        string GetStateName(int stateId);
        string GetCountryName(int countryId);
        int GetPinNumber(int pinId);
        List<ProductInfo> GetProductInfoByUserNameAndLocation(string locationName, string userName);
        ProductInformation GetProductInformation(int productId);
        UserInformation GetUserInformationForAPI(string userName);
    }
}
