using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        LoadingScene,
        GameScene,
        LobbyScene,
        CharacterSelectScene
    }

    private static Scene targerScene;

    public static void LoadScene(Scene targerScene)
    {
        Loader.targerScene = targerScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadSceneNetwork(Scene targerScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targerScene.ToString(),LoadSceneMode.Single);
    }

    public static void LoaderCallBack()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Loader.targerScene.ToString());
    }
}
