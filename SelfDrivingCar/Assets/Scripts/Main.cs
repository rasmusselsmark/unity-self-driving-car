using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    IEnumerator Start()
    {
        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        UnloadScenes();
        LoadScenes(
            "Scenes/Modern City - Day Time",
            // "Scenes/SampleScene",
            "Scenes/UI");

        yield return null; // wait one frame for scenes to have been loaded

    }

    void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode sceneLoadMode)
    {
        print($"Scene loaded: {scene.name}");

        switch (scene.name)
        {
            case "Modern City - Day Time":
                SceneManager.SetActiveScene(scene);
                return;
            
            case "UI":
                // wire up events across subscenes
                var ui = GameObject.Find("UI").GetComponent<UI>();
                var car = GameObject.Find("Car").GetComponent<CarController>();
                car.SpeedChanged += ui.CarControllerOnSpeedChanged;
                
                return;
        }
    }

    void UnloadScenes()
    {
        var sceneCount = SceneManager.sceneCount;
        var activeScene = SceneManager.GetActiveScene();
        for (var i = sceneCount - 1; i >= 0; i--)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name != activeScene.name)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }

    void LoadScenes(params string[] scenes)
    {
        foreach (var scene in scenes)
        {
            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        }
    }
}
