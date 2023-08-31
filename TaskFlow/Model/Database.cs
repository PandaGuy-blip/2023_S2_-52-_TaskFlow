﻿using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace TaskFlow.Model
{
    /// <summary>
    /// Abstract class for creating and accessing database tables.
    /// </summary>
    /// <typeparam name="T">Data Object type to be the basis of the table</typeparam>
    public abstract class Database<T>
    {
        /// <summary>
        /// Connection for accessing the database and executing commands
        /// </summary>
        protected static SQLiteAsyncConnection dbConn;
        /// <summary>
        /// If true, the database has a updates not local to memory
        /// </summary>
        protected bool hasUpdates;
        /// <summary>
        /// The list of objects stored in the database's table
        /// </summary>
        protected List<T> data;

        /// <summary>
        /// Creates a new database object. Establishes a connection with the local DB.
        /// </summary>
        public Database()
        {
            dbConn = EstablishConnection();
            this.data = new List<T>();
            CreateTableAsync();
        }
        /// <summary>
        /// Establishes a new database connection in the current app's directory
        /// </summary>
        /// <returns>SQLiteAsyncConnection</returns>
        private SQLiteAsyncConnection EstablishConnection()
        {
            //Specify the store location of the database -> app data not cache
            string dbLocation =  Path.Combine(FileSystem.Current.AppDataDirectory, "taskflow.db");
            return new SQLiteAsyncConnection(dbLocation); //Create a new connection and return it
        }

        /// <summary>
        /// Code should be as follows:
        /// <code>
        /// protected override void CreateTableAsync()
        /// {
        ///     dbConn.CreateTableAsync&lt;Object&#62;();
        /// }
        /// </code>
        /// </summary>
        protected abstract void CreateTableAsync();

        /// <summary>
        /// Used to define the table that will be got from the database. Code as follows:
        /// <code>
        /// protected override List&lt;TodoItem&gt; GetDataAbstract()
        /// {
        ///     return dbConn.Table&lt;TodoItem&gt;().ToListAsync().Result;
        /// }
        /// </code>
        /// </summary>
        /// <returns>List&lt;T&gt;</returns>
        protected abstract List<T> GetDataAbstract();

        /// <summary>
        /// Returns the list of items that is currently stored. Will update items based on if the database
        /// has non present data.
        /// </summary>
        /// <returns>List&lt;T&gt; of all columns in the database</returns>
        public List<T> GetData()
        {
            if (this.hasUpdates)
                this.data = this.GetDataAbstract();

            this.hasUpdates = false;

            return this.data;
        }
        /// <summary>
        /// Inserts data into the database
        /// </summary>
        /// <param name="data">Object to be added</param>
        /// <returns>Number of columns affected</returns>
        public async void Insert(T data)
        {
            this.hasUpdates = true;
            await dbConn.InsertOrReplaceWithChildrenAsync(data);
        }

        /// <summary>
        /// Inserts a list of objects into the database
        /// </summary>
        /// <param name="data">List of data to add</param>
        /// <returns>Number of columns affected</returns>
        public async void InsertAll(List<T> data)
        {
            this.hasUpdates = true;
            await dbConn.InsertOrReplaceAllWithChildrenAsync(data);
        }

        /// <summary>
        /// Deletes the object from the database if it exists
        /// </summary>
        /// <param name="data">Object to be removed</param>
        /// <returns>Number of columns affected</returns>
        protected async void Delete(T data)
        {
            this.hasUpdates = true;
            await dbConn.DeleteAsync(data); 
        }

#if DEBUG
        /// <summary>
        /// Will clear the table from the database. Only functional when debugging, else will
        /// do nothing.
        /// </summary>
        /// <remarks>This method will not be included in release builds</remarks>
        protected void DeleteAllTableContent()
        {
            dbConn.DeleteAllAsync<T>();
        }
#endif

    }
}