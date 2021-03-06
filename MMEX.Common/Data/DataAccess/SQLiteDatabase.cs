﻿using MMEX.Common.Data.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MMEX.Common.Data.DataAccess
{
    public class SQLiteDatabase
    {
        static object locker = new object();

        SQLiteConnection database;

        public SQLiteDatabase(string DbPath)
        {
            // database = DependencyService.Get<ISQLite>().GetConnection();
            database = new SQLiteConnection(DbPath);
            database.CreateTable<Account>();
            database.CreateTable<Currency>();
            database.CreateTable<OptionDb>();
        }

        static readonly object Locker = new object();

        public IEnumerable<T> GetItems<T>() where T : Business.Contracts.IEntity, new()
        {
            lock (Locker)
            {
                return (from i in database.Table<T>() select i);
            }
        }

        public T GetItem<T> (int id) where T : Business.Contracts.IEntity, new()
        {
            lock (Locker)
            {
                return database.Table<T>().FirstOrDefault(x => x.Id == id);
            }
        }

        public int SaveItem<T> (T item) where T : Business.Contracts.IEntity
        {
            lock (Locker)
            {
                try
                {
                    if (item.Id != 0)
                    {
                        database.Update(item);
                        Debug.WriteLine("DATABASE UPDATE -> " + item.ToString() + " - " + item.Id);
                        return item.Id;
                    }
                    database.Insert(item);
                    Debug.WriteLine("DATABASE INSERT -> " + item.ToString() + " - " + item.Id);
                    return item.Id;
                }
                catch (Exception /*e*/)
                {
                    return 0;
                }
            }
        }

        public int DeleteItem<T> (int id) where T : Business.Contracts.IEntity, new()
        {
            lock (Locker)
            {
                Debug.WriteLine("DATABASE DELETE -> " + id.ToString());
                return database.Delete<T>(id);
            }
        }
    }
}
