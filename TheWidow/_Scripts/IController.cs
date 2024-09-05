using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController
{
    public void SetUp(IComponent component);
    public void Activate();
    public void Dispose();

}
