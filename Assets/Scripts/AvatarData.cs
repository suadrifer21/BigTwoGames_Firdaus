using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Avatar", menuName = "Big Two/Avatar Data", order = 2)]
public class AvatarData : ScriptableObject
{
    //public string name;
    public List<AvatarImage> avatarImage;
}
[Serializable]
public class AvatarImage
{
    public AvatarMode mode;
    public Sprite sprite;
}
public enum AvatarMode
{
    Normal,
    Losing,
    Winning
}
