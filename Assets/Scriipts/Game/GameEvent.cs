using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    private List<IGameListener> listeners = new List<IGameListener>();

    public string Title { get; private set; }

    public GameEvent(string title)
    {
        this.Title = title;
    }

    public void Attach(IGameListener listener)
    {
        listeners.Add(listener);
    }

    public void Notify(Component other)
    {
        foreach (IGameListener listener in listeners)
        {
            listener.Notify(this, other);
        }
    }
}
