using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    /// Returns true if given list contain given MirrorPair
    /// </summary>
    /// <param name="list">List to search</param>
    /// <param name="gmToCheck">MirrorPair to look for</param>
    /// <returns>True when MirrorPair found in list</returns>
    public static bool Contains(this List<MirrorPair> list, GameObject gmToCheck) {
        foreach(MirrorPair mirrorPair in list) {
            if(mirrorPair.source == gmToCheck) return true;
        }
        return false;
    }
    
    /*public static void Add(this List<MirrorPair>list, GameObject gmToAdd) {
        list.Add(new MirrorPair(gmToAdd));
    }*/
}
