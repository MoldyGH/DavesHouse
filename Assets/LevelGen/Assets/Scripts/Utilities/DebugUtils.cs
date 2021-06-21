#define DEBUG

using System;
using UnityEngine;
using System.Diagnostics;
 
public class DaedalusDebugUtils
{
    [Conditional("DEBUG")]
    static public void Assert(bool condition)
    {
        if (!condition) throw new Exception();
    }
	
	[Conditional("DEBUG")]
	static public void Assert(bool condition, string message)
	{
		Assert(condition,message,null);
	}

	[Conditional("DEBUG")]
    static public void Assert(bool condition, string message, UnityEngine.Object context)
    {
		if (!condition) UnityEngine.Debug.LogWarning(message,context);
		Assert(condition);
    }
}