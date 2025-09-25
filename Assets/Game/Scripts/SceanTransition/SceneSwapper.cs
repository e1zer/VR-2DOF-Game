using System.Collections.Generic;
using UnityEngine;

public enum TypeScene { Main, Game }
public class SceneSwapper : MonoBehaviour
{
    [SerializeField]
    private List<SceneEntry> scenes;
    [SerializeField]
    private float duration = 0.5f;

    private Dictionary<TypeScene, IScene> SceneMap = new Dictionary<TypeScene, IScene>();
    private IScene currentScene;

    private void Awake()
    {
        foreach (SceneEntry entry in scenes)
        {
            SceneMap[entry.type] = entry.scene;
        }
    }

    private void Start()
    {
        Show(TypeScene.Main);
    }

    public void Show(TypeScene type)
    {
        if (SceneMap.TryGetValue(type, out IScene nextScene))
        {
            if (currentScene == nextScene) return;

            var pageTransform = ((MonoBehaviour)nextScene).transform;
            pageTransform.SetAsLastSibling();

            nextScene.OnShown -= OnSceneShown;
            nextScene.OnShown += OnSceneShown;
            nextScene.SetVisible(true, duration);
        }
    }

    private void OnSceneShown(IScene newScene)
    {
        newScene.OnShown -= OnSceneShown;

        currentScene?.SetVisible(false, 0f);
        currentScene = newScene;
    }

    [System.Serializable]
    public struct SceneEntry
    {
        public TypeScene type;
        public SceneDefault scene;
    }
}
