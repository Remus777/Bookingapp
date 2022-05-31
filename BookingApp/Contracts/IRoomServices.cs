using BookingApp.Models.DataTrasnferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Contracts
{
    public interface IRoomServices  
    {
        List<RoomDTO> RoomsIndex();
        RoomDTO DetailsRoom(int id);
        int CreateRoom(RoomDTO model);
        int EditRoom(RoomDTO model);
        int DeleteRoom(int id);
    }
}
