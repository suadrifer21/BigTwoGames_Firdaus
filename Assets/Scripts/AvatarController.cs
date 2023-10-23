using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    public static AvatarController Instance;

    [SerializeField]
    private List<AvatarData> avatarDatas;
    private int selected;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void SetAvatar(int selected)
    {
        this.selected = selected;
    }

    public Sprite GetAvatarData(int requested, AvatarMode mode)
    {
        int index = (selected+requested)% 4;
        switch (mode)
        {
            case AvatarMode.Normal:
                return avatarDatas[index].avatarImage[0].sprite;
            case AvatarMode.Losing:
                return avatarDatas[index].avatarImage[1].sprite;
            case AvatarMode.Winning:
                return avatarDatas[index].avatarImage[2].sprite;
            default: 
                return null;
        }
        
    }

    
}
