using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RecipeSystemScriptableObjects/New Album")]
public class Album : ScriptableObject
{
    [SerializeField] List<SongData> songDataList = new List<SongData>();
}
