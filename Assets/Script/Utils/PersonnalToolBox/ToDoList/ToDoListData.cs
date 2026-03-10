using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TodoListData", menuName = "Tools/Todo List Data")]
public class TodoListData : ScriptableObject
{
    public List<TodoItem> items = new List<TodoItem>();

    public void AddItem(TodoItem item)
    {
        items.Add(item);
    }

    public void RemoveItem(string id)
    {
        items.RemoveAll(item => item.id == id);
    }

    public void ToggleComplete(string id)
    {
        var item = items.Find(i => i.id == id);
        if (item != null)
        {
            item.isCompleted = !item.isCompleted;
        }
    }

    public void ClearCompleted()
    {
        items.RemoveAll(item => item.isCompleted);
    }

    public int GetActiveCount()
    {
        return items.FindAll(item => !item.isCompleted).Count;
    }

    public int GetCompletedCount()
    {
        return items.FindAll(item => item.isCompleted).Count;
    }
}