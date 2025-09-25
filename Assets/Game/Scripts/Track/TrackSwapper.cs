using UnityEngine;

public class TrackSwapper : MonoBehaviour
{
    [SerializeField]
    private Track[] tracks;

    private void Start()
    {
        int indexTrack = Random.Range(0, tracks.Length);

        tracks[indexTrack].SetTrack();
    }
}

[System.Serializable]
public class Track
{
    [SerializeField]
    private GameObject[] walls;

    public void SetTrack()
    {
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].SetActive(true);
        }
    }
}
