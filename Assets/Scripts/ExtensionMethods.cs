using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
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
