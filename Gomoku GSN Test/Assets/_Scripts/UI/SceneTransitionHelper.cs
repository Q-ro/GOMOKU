/*
Author: Andres Mrad (Q-ro)
Creation Date : [ 2020/05 (May)/06 (Wed) ] @[ 16:11 ]
Description :  A general purpose helper to perform simple scene transition animations
*/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// A general purpose helper to perform simple scene transition animations
public class SceneTransitionHelper : Singleton<SceneTransitionHelper>
{
    #region Inspector Properties

    [SerializeField] int startingTransitionIndex; // the default or starting transition index for the scene loader to use
    [SerializeField] Animator[] transitionAnimators; // an array of the animator controllers for all the transition animations

    [SerializeField] bool displayLoadingBar = false;
    [SerializeField] GameObject loadingDisplayCanvas;
    [SerializeField] Image loadingBar;
    [SerializeField] Text progressText;

    #endregion

    #region Private Properties
    private int currentTransitionIndex; // what animation has been selected to transition to/from a given scene

    #endregion

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.currentTransitionIndex = this.startingTransitionIndex;
        this.loadingDisplayCanvas.SetActive(false);

        foreach (var animator in this.transitionAnimators)
        {
            animator.gameObject.SetActive(false);
        }
    }

    // A that gets the scene change request from other parts of the code
    public void ChangeScene<T>(T scene, int transitionIndex, bool showLoading = true)
    {
        // Make sure the type of our generic T is one of the expected types
        if (!(typeof(T) == typeof(int)) && !(typeof(T) == typeof(string)))
        {
            Debug.Log("Scenes can only be loaded byt name or by build index number");
            return;
        }

        this.currentTransitionIndex = transitionIndex;
        this.displayLoadingBar = showLoading;
        StartCoroutine(TransitionSceneLoader(scene));
    }

    //  the coroutine that will handle the visuals of transitioning out of a scene
    private IEnumerator TransitionSceneLoader<T>(T scene)
    {
        // Activate the animator container
        this.transitionAnimators[this.currentTransitionIndex].gameObject.SetActive(true);
        // Hide the loading bar container
        this.loadingDisplayCanvas.SetActive(false);

        // Trigger the out or "exit scene" animation transition
        this.transitionAnimators[this.currentTransitionIndex].SetTrigger("Out");
        // Get the length of the animation so we wait the appropiate amount of time before loading the next scene
        var waitTime = this.transitionAnimators[this.currentTransitionIndex].GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(waitTime);

        AsyncOperation loadSceneAsync = new AsyncOperation();
        // Parse the generic type T to either of the valid inputs for an scene to be loaded
        // and load the scene
        if (typeof(T) == typeof(int))
        {
            // Switching to async loading so we can have a loading bar
            // SceneManager.LoadScene(Convert.ToInt32(scene));
            loadSceneAsync = SceneManager.LoadSceneAsync(Convert.ToInt32(scene));

        }
        else if (typeof(T) == typeof(string))
        {
            // Switching to async loading so we can have a loading bar
            // SceneManager.LoadScene(scene.ToString());
            loadSceneAsync = SceneManager.LoadSceneAsync(scene.ToString());
        }

        this.loadingDisplayCanvas.SetActive(this.displayLoadingBar);
        // this.loadingBar.gameObject.GetComponent<CanvasGroup>().alpha = this.displayLoadingBar ? 1 : 0;

        Debug.Log(scene);

        this.loadingBar.fillAmount = 0;
        while (!loadSceneAsync.isDone)
        {
            if (this.displayLoadingBar)
            {
                float loadingProgress = Mathf.Clamp01(loadSceneAsync.progress / 0.9f);
                this.loadingBar.fillAmount = loadingProgress;
            }

            Debug.Log(loadSceneAsync.progress);
            yield return null;
        }
        this.loadingDisplayCanvas.SetActive(false);
        
        // this.transitionAnimators[this.currentTransitionIndex].SetTrigger("In");
    }

    // A delegate function that will handle what happens when a scene has been loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // upon loading the scene, start the in or "enter scene" animation transition
        this.transitionAnimators[this.currentTransitionIndex].SetTrigger("In");
    }

}
