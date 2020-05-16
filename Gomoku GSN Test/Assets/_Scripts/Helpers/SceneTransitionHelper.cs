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
    [SerializeField] bool hasCustomSplashScreen = false;
    [SerializeField] GameObject loadingDisplayCanvas;
    [SerializeField] Image loadingBar;
    [SerializeField] Text progressText;

    #endregion

    #region Private Properties
    private bool displayLoadingBar = false;
    private int currentTransitionIndex; // what animation has been selected to transition to/from a given scene

    #endregion

    new void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    void Awake()
    {
        //base.Awake();

        this.currentTransitionIndex = this.startingTransitionIndex;
        this.loadingDisplayCanvas.SetActive(false);

        foreach (var animator in this.transitionAnimators)
        {
            animator.gameObject.SetActive(false);
        }

    }

    // // // A method that gets the scene change request from other parts of the code
    // // public void ChangeScene<T>(T scene, int transitionIndex, bool showLoading = true)
    // // {
    // //     // Make sure the type of our generic T is one of the expected types
    // //     if (!(typeof(T) == typeof(int)) && !(typeof(T) == typeof(string)))
    // //     {
    // //         Debug.Log("Scenes can only be loaded byt name or by build index number");
    // //         return;
    // //     }

    // //     this.currentTransitionIndex = transitionIndex;
    // //     this.displayLoadingBar = showLoading;
    // //     StartCoroutine(TransitionSceneLoader(scene));
    // // }

    // ------------------------------
    // Changing from a generic public accesor to a strongly typed with reflexion 
    // so there's no hiccups when somebody not 100% familiar with how the scene manager works tries to send a type that is not supported
    // or rather, that he knows that there are basically only 2 types he can use to switch to a given scene
    // ------------------------------

    // A method that gets the scene change request from other parts of the code
    public void ChangeScene(int scene, int transitionIndex, bool showLoading = true)
    {
        this.currentTransitionIndex = transitionIndex;
        this.displayLoadingBar = showLoading;
        StartCoroutine(TransitionSceneLoader(scene));
    }

    // A method that gets the scene change request from other parts of the code
    public void ChangeScene(string scene, int transitionIndex, bool showLoading = true)
    {
        this.currentTransitionIndex = transitionIndex;
        this.displayLoadingBar = showLoading;
        StartCoroutine(TransitionSceneLoader(scene));
    }

    //  the coroutine that will handle the visuals of transitioning out of a scene
    private IEnumerator TransitionSceneLoader<T>(T scene)
    {

        // Make sure the type of our generic T is one of the expected types, which should be 100% of the times
        if (!(typeof(T) == typeof(int)) && !(typeof(T) == typeof(string)))
        {
            Debug.LogAssertion("Scenes can only be loaded by name (String) or by build index number (int) \n If you somehow got this error, you most likely modified something that you shouldn't on the public accesors for the helper or performed black magic");
            yield break;
        }

        this.loadingBar.fillAmount = 0;

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
            // // SceneManager.LoadScene(Convert.ToInt32(scene));
            loadSceneAsync = SceneManager.LoadSceneAsync(Convert.ToInt32(scene));

        }
        else if (typeof(T) == typeof(string))
        {
            // Switching to async loading so we can have a loading bar
            // // SceneManager.LoadScene(scene.ToString());
            loadSceneAsync = SceneManager.LoadSceneAsync(scene.ToString());
        }

        this.loadingDisplayCanvas.SetActive(this.displayLoadingBar);
        // // this.loadingBar.gameObject.GetComponent<CanvasGroup>().alpha = this.displayLoadingBar ? 1 : 0;

        Debug.Log(scene);
        Debug.Log(loadSceneAsync.progress);

        /* //TODO:
          It would probably be better if instead of directly settign the fill amount for the image, 
          to rather have a class that haddles what happens as the progress moves forwards, for instace, display text,
          or, change the background color, etc ...
        */


        while (!loadSceneAsync.isDone)
        {
            if (this.displayLoadingBar)
            {
                // Since the progress for an async operation goes from 0 to 0.9, let's "normalize" it to that it goes from 0 to 1
                float loadingProgress = Mathf.Clamp01(loadSceneAsync.progress / 0.9f);
                // Display the loading progress as a filled image
                this.loadingBar.fillAmount = loadingProgress;
            }

            Debug.Log(loadSceneAsync.progress);

            yield return null;
        }

        this.loadingDisplayCanvas.SetActive(false);
        // // this.transitionAnimators[this.currentTransitionIndex].SetTrigger("In");
    }

    // A delegate function that will handle what happens when a scene has been loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Instance != this)
            return;

        /* //TODO:
            It may be a good idea to create a class called "Transition Effect" or something like that, that handles
            things like:
            - transition type (is it a splash transition, is it a regular inbetween)

            (On the other hand, i may be over designing and over complicating this whole thing)
        */

        if (this.transitionAnimators[this.currentTransitionIndex] == null)
            return;

        // since this object should be created on the first scene of the build
        // if said scene is a splash screen, let's transition smothly from the unity splash into our custom splash
        // (It is assumed that the animator at position 0 is in fact the custom splash screen)
        if (this.hasCustomSplashScreen && scene.buildIndex == 0)
            this.transitionAnimators[this.currentTransitionIndex].gameObject.SetActive(true);

        // upon loading the scene, start the in or "enter scene" animation transition
        this.transitionAnimators[this.currentTransitionIndex].SetTrigger("In");
    }

}
