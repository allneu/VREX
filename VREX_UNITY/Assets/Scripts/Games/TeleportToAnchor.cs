using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class TeleportToAnchor
{
    private readonly XROrigin xrOrigin;

    public TeleportToAnchor(XROrigin xrOrigin)
    {
        this.xrOrigin = xrOrigin;
    }

    public void Teleport(List<Transform> anchors, int index)
    {
        if (index >= 0)
        {
            foreach (var anchor in anchors)
                if (anchor != null)
                    anchor.gameObject.SetActive(false);

            var targetAnchor = anchors[index % anchors.Count];

            Teleport(targetAnchor);
        }
        else
        {
            Debug.LogError("Invalid anchor index: " + index);
        }
    }

    private void Teleport(Transform anchor)
    {
        if (xrOrigin != null && anchor != null)
        {
            anchor.gameObject.SetActive(true);
            var childAnchor = anchor.Find("Anchor");
            if (childAnchor != null)
            {
                xrOrigin.transform.position = childAnchor.position;
                xrOrigin.transform.rotation = childAnchor.rotation;
            }
            else
            {
                Debug.LogError("Child object 'Anchor' not found.");
            }
        }
        else
        {
            Debug.LogError("XROrigin or anchor is null.");
        }
    }
}