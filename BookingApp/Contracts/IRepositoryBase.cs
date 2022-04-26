﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Contracts
{
    public interface IRepositoryBase<T> where T: class
    {
        ICollection<T> FindAll();
        T FindById(int id);
        T FindByIds(string id);
        bool isExists(int id);
        bool Create(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        bool Save();
    }
}
