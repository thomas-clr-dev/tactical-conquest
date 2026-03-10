using System;
using UnityEngine;

[Serializable]
public class TodoItem
{
    public string id;
    public string title;
    [TextArea(2, 5)]
    public string description;
    public bool isCompleted;
    public TodoPriority priority;
    public TodoCategory category;
    public string createdDate;
    public string dueDate;

    public TodoItem(string title)
    {
        this.id = Guid.NewGuid().ToString();
        this.title = title;
        this.description = "";
        this.isCompleted = false;
        this.priority = TodoPriority.Normal;
        this.category = TodoCategory.General;
        this.createdDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        this.dueDate = "";
    }
}

public enum TodoPriority
{
    Low,
    Normal,
    High,
    Urgent
}

public enum TodoCategory
{
    General,
    Bug,
    Feature,
    Optimization,
    Documentation,
    Art,
    Audio,
    Design
}

// Enums pour les filtres (avec option "All")
public enum TodoPriorityFilter
{
    All,
    Low,
    Normal,
    High,
    Urgent
}

public enum TodoCategoryFilter
{
    All,
    General,
    Bug,
    Feature,
    Optimization,
    Documentation,
    Art,
    Audio,
    Design
}
