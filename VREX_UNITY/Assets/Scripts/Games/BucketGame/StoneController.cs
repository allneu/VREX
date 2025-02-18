using UnityEngine;

public class SandController : MonoBehaviour
{
    public GameObject bigSandPile;
    public GameObject sandTarget;
    public GameObject smallSandPile;

    public GameObject[] targetObjects;
    public int targetIndex;

    private void Start()
    {
        if (bigSandPile == null) Debug.LogError("SandPile object is not set for the Sand Controller.");
        if (sandTarget == null) Debug.LogError("SandTarget object is not set for the Sand Controller.");
        if (targetObjects.Length == 0) Debug.LogError("No target objects set for the Sand Controller.");
        targetIndex = 0;
    }

    public void SetNextTarget(int newIndex)
    {
        targetIndex = newIndex;
        if (targetObjects.Length == 0)
        {
            Debug.LogError("No target objects available to set the next target.");
            return;
        }

        var target = targetObjects[targetIndex % targetObjects.Length];
        if (target != null)
            sandTarget.transform.position = target.transform.position;
        else
            Debug.LogError("Target object is null.");
        bigSandPile.SetActive(true);
        smallSandPile.SetActive(false);
    }

    public void Hide()
    {
        bigSandPile.SetActive(true);
        smallSandPile.SetActive(false);
        sandTarget.SetActive(false);
    }

    public void Show()
    {
        bigSandPile.SetActive(true);
        smallSandPile.SetActive(true);
        sandTarget.SetActive(true);
    }
}