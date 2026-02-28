using UnityEngine;

public class AreaBGMTrigger : MonoBehaviour
{
    public enum AreaType { Plain, Village, Forest, Castle }

    public AreaType area;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (BGMManager.Instance == null) return;

        switch (area)
        {
            case AreaType.Plain:
                BGMManager.Instance.PlayFieldBGM();
                break;
            case AreaType.Village:
                BGMManager.Instance.PlayVillageBGM();
                break;
            case AreaType.Forest:
                BGMManager.Instance.PlayForestBGM();
                break;
            case AreaType.Castle:
                BGMManager.Instance.PlayCastleBGM();
                break;
        }
    }
}