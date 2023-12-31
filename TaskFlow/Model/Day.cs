﻿using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TaskFlow.Model
{
    [Table("Day")]
    public class Day
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Id for the Day
        [NotNull]
        public DateTime Date { get; set; } // The day on which the items are for
        [ManyToMany(typeof(TodoDayLink))]
        public List<TodoItem> TodoItem { get; set; } = new List<TodoItem>(); //The todo items that are assigned for today

        /// <summary>
        /// Create a new day which todo items can be added to. These items are to be carried out today.
        /// </summary>
        public Day()
        {
        }

        public void InitalizeDay()
        {
            this.Date = DateTime.Now.Date;
        }
    }
}
