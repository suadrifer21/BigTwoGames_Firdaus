using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AvatarSelector : MonoBehaviour
{
    public void SelectAvatar(int index)
    {
        AvatarController.Instance.SetAvatar(index);
        SceneManager.LoadScene(1);
    }
}
