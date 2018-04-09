using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISnake
{
    /// <summary>
    /// Set player was move out surface of hole
    /// </summary>
    void CanGoOut();

    /// <summary>
    /// Player was stand on hole
    /// </summary>
    void CantGoOut();
}
