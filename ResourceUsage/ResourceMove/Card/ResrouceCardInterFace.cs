using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

public interface ResrouceCardInterface
{
    void Init();
    void Draw();
    void Update();

    Dictionary<List<PathInfo>, int> PathDic();

    List<int> PathList();

    void OnPathClick(int pathId, string value);
}