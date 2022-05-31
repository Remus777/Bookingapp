using BookingApp.Data;
using BookingApp.Models.DataTrasnferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Contracts
{
    public interface IUsersServices
    {
        ClientWithRoleDTO UsersIndex(Client currentUser);
        ClientDetailsDTO DetailsUser(string id);
        ClientDTOnoID CreateUserGET();
        ClientEditDTO EditUserGET(string id);
        ClientPasswordDTO EditPasswordGET(string id);
        int CreateUserPOST(ClientDTOnoID model);
        int EditUserPOST(ClientEditDTO model);
        int EditPasswordPOST(ClientPasswordDTO model);
        int DeleteUser(string id);
    }
}
