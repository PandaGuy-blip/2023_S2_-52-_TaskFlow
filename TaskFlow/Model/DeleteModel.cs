﻿using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Model
{
    public class DeleteModel : Database<DeleteHistoryList>, IDatabase<DeleteHistoryList>
    {
        /// <summary>
        /// Creates a new object for managing todo items in the database.
        /// <list type="">
        /// Initializes its abstract parent:
        /// <see cref="Database{T}"/>
        /// </list></summary>
        public DeleteModel() : base()
        {
            this.hasUpdates = true;
        }
        protected override void CreateTableAsync()
        {
            dbConn.CreateTables<TodoItem, DeleteHistoryList>();
        }

        protected override List<DeleteHistoryList> GetDataAbstract()
        {
            try
            {
                return dbConn.GetAllWithChildren<DeleteHistoryList>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Will insert a new item into the database based on the todo item.
        /// Used for setting up the delete time for an item.
        /// </summary>
        /// <param name="item"></param>
        public void SetupDeleteTime(TodoItem item)
        {
            var data = GetData();
            if (data.Any(x => x.Id == item.Id))
            {
                return;
            }

            DeleteHistoryList history = new DeleteHistoryList();
            history.todo = item.Id;
            history.deleteTime = DateTime.Now;
            this.Insert(history);
        }

        /// <summary>
        /// Removes an delete history entry from the database based on the input
        /// todo item.
        /// </summary>
        /// <param name="todo"></param>
        public void Delete(TodoItem todo)
        {
            foreach (var item in this.GetData())
            {
                if (item.todo == todo.Id)
                {
                    base.Delete(item);
                }
            }
        }

        /// <summary>
        /// When run will delete any items that have been in the trash for more than 30 days.
        /// </summary>
        public void AutoDelete()
        {
            TodoModel tm = new TodoModel();
            foreach (var item in this.GetData())
            {
                if (item.deleteTime < DateTime.Now - TimeSpan.FromDays(30))
                {
                    tm.Delete(item.todo);
                    this.Delete(item);
                }
            }
        }

        #region TestMethods
#if DEBUG
        //Debug methods to allow for testing of the model. Contains 1-1 working copies
        //of their respective non test method.

        public List<DeleteHistoryList> test_list = new List<DeleteHistoryList>();

        public DeleteModel(bool test) : base(test)
        {
        }

        public void Test_SetupDeleteTime(TodoItem item)
        {
            var data = Test_GetData();
            if (data.Any(x => x.Id == item.Id))
            {
                return;
            }

            DeleteHistoryList history = new DeleteHistoryList();
            history.todo = item.Id;
            history.deleteTime = DateTime.Now;
            Test_Insert(history);
        }
        public void Test_Delete(TodoItem todo)
        {
            foreach (var item in test_list)
            {
                if (item.todo == todo.Id)
                {
                    Test_Delete(item);
                    break;
                }
            }
        }
        public void Test_AutoDelete()
        {
            var data = Test_GetData();
            foreach (var item in data)
            {
                if (item.deleteTime < DateTime.Now - TimeSpan.FromDays(30))
                {
                    Test_Delete(item);
                }
            }
        }
        public List<DeleteHistoryList> Test_GetData()
        {
            return test_list.ToList();
        }
        public void Test_Insert(DeleteHistoryList data)
        {
            test_list.Add(data);
        }       
        public void Test_Delete(DeleteHistoryList data)
        {
            test_list.Remove(data);
        }
#endif
        #endregion
    }
}
