using Syncfusion.Maui.DataSource;
using TaskFlow.Model;
using TaskFlow.ViewModel;
using TaskFlow.Comparers;

namespace TaskFlow.View;

public partial class ToDoPage : ContentPage
{
    public ToDoPage(ToDoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

    }

    /// <summary>
    /// Loads todo items from view model whenever page is about to appear on screen.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((ToDoViewModel)BindingContext).LoadTodoItems();

    }

    /// <summary>
    /// Handles the CheckBox checked changed event and updates associated todo todoItem's completion.
    /// </summary>
    /// <param name="sender">CheckBox that triggered the event</param>
    /// <param name="completed">New completion status of the todo todoItem</param>
    private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs completed)
    {
        var checkBox = (CheckBox)sender;
        var todoItem = checkBox.BindingContext as TodoItem;
        if (todoItem != null && completed.Value == true)
        {
            // Update completion status using ViewModel.
            await ((ToDoViewModel)BindingContext).UpdateTodoCompletion(todoItem, completed.Value);
        }
    }

    /// <summary>
    /// Handles the selection changed event of the sorting combo box. 
    /// Sorts the TodoList based on the selected sorting option.
    /// </summary>
    /// <param name="sender">The SfComboBox that triggered the event</param>
    /// <param name="e">The selected todo item from the combo box</param>
    private void SortByComboBox_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        // Check if a valid selection was made.
        if (e.CurrentSelection.FirstOrDefault() == null)
            return;

        // Extract the selected sorting option as a key-value pair.
        var selectedItem = (KeyValuePair<string,string>)e.CurrentSelection.FirstOrDefault();
        string selectedValue = selectedItem.Value;

        ClearSortAndGroup();

        if(selectedValue == null)
        {
            sortComboBox.SelectedItem = null;  // Reset the combo box selected sort option
        }
        else
        {
            ApplySortDescriptor(selectedValue, ListSortDirection.Ascending);

            if (selectedItem.Value == nameof(TodoItem.DueDate))
            {
                SetupGroupHeaderTemplate();
                SetupDueDateGrouping();
            }
        }
        
    }

    /// <summary>
    /// Clears any previous sorting and grouping for the todo list.
    /// </summary>
    private void ClearSortAndGroup()
    {
        TodoList.DataSource.SortDescriptors.Clear();
        TodoList.DataSource.GroupDescriptors.Clear();
    }

    /// <summary>
    /// Sets up the group header template for the todo list.
    /// </summary>
    private void SetupGroupHeaderTemplate()
    {
        TodoList.GroupHeaderTemplate = new DataTemplate(() =>
        {
            var grid = new Grid { Margin = 0};

            var label = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                TextColor = Colors.White,
                FontSize = 16,
                HeightRequest = 20
            };

            label.SetBinding(Label.TextProperty, "Key");

            grid.Children.Add(label);

            return grid;
        });
    }

    /// <summary>
    /// Sets up due date grouping for the todo list.
    /// </summary>
    private void SetupDueDateGrouping()
    {
        TodoList.DataSource.GroupComparer = new DueDateGroupComparer();
        TodoList.DataSource.GroupDescriptors.Add(new GroupDescriptor()
        {
            PropertyName = nameof(TodoItem.DueDate),
            KeySelector = (object obj) => GetDueDateGroupKey((TodoItem)obj)
        });
    }
    /// <summary>
    /// Gets the group key for a todo item based in its due date.
    /// </summary>
    /// <param name="todoItem">The todo item</param>
    /// <returns>The group key as a string representation of <see cref="DueDateGroup"/> enum.</returns>
    private string GetDueDateGroupKey(TodoItem todoItem)
    {
        var dueDate = todoItem.DueDate;

        DueDateGroup group = DueDateGroupExtension.GetDueDateGroup(dueDate);
        return group.ToFriendlyString();
    }

    /// <summary>
    /// Applies a new sort descriptor to the todo list.
    /// </summary>
    /// <param name="propertyName">The property to sort by.</param>
    /// <param name="direction">The sorting direction.</param>
    private void ApplySortDescriptor(string propertyName, ListSortDirection direction)
    {
        TodoList.DataSource.SortDescriptors.Add(new SortDescriptor()
        {
            PropertyName = propertyName,
            Direction = direction
        });
    }
}