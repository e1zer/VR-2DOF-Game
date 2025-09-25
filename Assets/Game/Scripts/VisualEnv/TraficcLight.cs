using UnityEngine;

public class TraficcLight : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private Color green, red, yellow;
    [SerializeField]
    private MeshRenderer[] meshLamps;

    private int conterPreStart = 0;

    private void Awake()
    {
        gameManager.OnPreStart += PreStart;
        gameManager.OnStartGame += StartGame;
    }

    private void OnDestroy()
    {
        gameManager.OnPreStart -= PreStart;
        gameManager.OnStartGame -= StartGame;
    }

    private void PreStart()
    {
        ChangeColorLamp(conterPreStart, yellow);

        conterPreStart++;
    }

    private void StartGame()
    {
        for (int i = 0; i < meshLamps.Length; i++)
        {
            ChangeColorLamp(i, green);
        }
    }

    private void ChangeColorLamp(int index, Color color)
    {
        meshLamps[index].material.color = color;
        meshLamps[index].material.SetColor("_EmissionColor", color);
    }
}
