﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Syncfusion.Maui.Scheduler;
using System.Diagnostics;
using TaskFlow.Model;
using TaskFlow.View;

namespace TaskFlow.ViewModel;

/// <summary>
/// ViewModel for logic of the <see cref="ToDoPage"/> View.
/// </summary>
public partial class ToDoViewModel : ObservableObject
{
    private readonly TodoModel _tm; // TodoModel

    [ObservableProperty]
    private ObservableCollection<TodoItem> todoItems;

    [ObservableProperty]
    private ObservableCollection<TodoItem> doneItems;

    [ObservableProperty]
    private IDictionary<string, string> sortItems;

    #region Constructor
    public ToDoViewModel()
    {
        _tm = App.TodoModel;
        TodoItems = new ObservableCollection<TodoItem>();
        DoneItems = new ObservableCollection<TodoItem>();
        SortItems = new Dictionary<string, string>();
        LoadSortDictionary();
        this.GenerateAppointments();
    }

    #endregion

    /// <summary>
    /// Loads todo items from the database and updates the <see cref="TodoItems"/> collection
    /// </summary>
    public async Task LoadTodoItems()
    {
        try
        {
            var itemsList = await Task.Run(() => _tm.GetData());

            if (itemsList != null && itemsList.Count > 0)
            {
                TodoItems.Clear();
                DoneItems.Clear();
                foreach (var item in itemsList)
                {
                    if(item.Completed == true)
                    {
                        DoneItems.Add(item);
                    } else
                    {
                        TodoItems.Add(item);
                    }

                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading todo items: {ex}");
        }

    }

    /// <summary>
    /// Loads the available sorting options for the todo list and updates the local SortItems dictionary.
    /// Each element in the dictionary consists of a key for its representation in the sort combo box, 
    /// and a value corresponding to the todo item property used for sorting.
    /// </summary>
    public void LoadSortDictionary()
    {
        SortItems = new Dictionary<string, string>()
        {
            { "Date created", null },  // set to null to clear the sort.
            { "Task Title", nameof(TodoItem.Title) },
            { "Deadline", nameof(TodoItem.DueDate) },
            { "Importance" , nameof(TodoItem.Importance )}
        };
    }

    /// <summary>
    /// Navigates to the <see cref="NewTodoPage"/> view
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    public async Task GoToNewTaskPage()
    {
        await Shell.Current.GoToAsync(nameof(NewTodoPage));
    }

    /// <summary>
    /// Updates a todo item's completion status in the database.
    /// </summary>
    /// <param name="todoItem">TodoItem to be updated</param>
    /// <param name="completed">New completion status of the todo item</param>
    public async void UpdateTodoCompletion(TodoItem todoItem, bool completed)
    {
        try
        {
            if (completed)
            {
                await Task.Delay(500);
                TodoItems.Remove(todoItem);
                DoneItems.Add(todoItem);
            }
            else
            {
                await Task.Delay(500);
                DoneItems.Remove(todoItem);
                TodoItems.Add(todoItem);
            }

            todoItem.Completed = completed;
            _tm.Insert(todoItem);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating todo item: {ex}");
        }

    }
    
    #region Properties
    public ObservableCollection<SchedulerAppointment> Events { get; set; }
    #endregion

    #region Method
    private void GenerateAppointments()
    {
        this.Events = new ObservableCollection<SchedulerAppointment>();

        //Adding the schedule appointments in the schedule appointment collection.
        this.Events.Add(new SchedulerAppointment
        {
            StartTime = DateTime.Now.Date.AddHours(10),
            EndTime = DateTime.Now.Date.AddHours(11),
            Subject = "Client Meeting",
            Background = new SolidColorBrush(Color.FromArgb("#FF8B1FA9")),
        });
    }

    #endregion
}
