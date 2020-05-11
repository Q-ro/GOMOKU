/*
Author: Andres Mrad (Q-ro)
Date: Wednesday 08/April/2020 @ 17:15:23
Description:  Loads a different scene 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    // Loads the scene with the given name
    public void LoadSceneByName(string scene)
    {
        // SceneManager.LoadScene (scene);
        SceneTransitionHelper.Instance.ChangeScene(scene,0);
    }
    public void LoadSceneByIndex(int scene)
    {
        // SceneManager.LoadScene (scene);
        SceneTransitionHelper.Instance.ChangeScene(scene,0);
    }
}