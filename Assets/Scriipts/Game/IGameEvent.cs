using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameListener
{
    void Notify(GameEvent gameEvent, Component component);
}