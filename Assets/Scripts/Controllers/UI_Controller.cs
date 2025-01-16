using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTransition;

public class UI_Controller : MonoBehaviour
{
    public TransitionSettings transitionSettings;
    public void LoadNewScene(int sceneNum)
    {
        TransitionManager.Instance().Transition(sceneNum, transitionSettings, 0);
    }
}
