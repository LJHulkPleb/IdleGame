using UnityEngine;
using UnityEngine.UI;

public class InfoboxController : MonoBehaviour
{
    public RectTransform canvasRectTransform; // Assign your Canvas RectTransform here
    public RectTransform infoBoxRectTransform; // Assign your Infobox RectTransform here

    void Update()
    {
        ClampToTopWhenNeeded();
    }

    void ClampToTopWhenNeeded()
    {
        if (canvasRectTransform == null || infoBoxRectTransform == null)
            return;

        // Ensure the layout is recalculated if necessary
        LayoutRebuilder.ForceRebuildLayoutImmediate(infoBoxRectTransform);

        // Get the position of the Infobox in world coordinates
        Vector3[] corners = new Vector3[4];
        infoBoxRectTransform.GetWorldCorners(corners);

        // Top-left corner in world coordinates
        Vector3 topCorner = corners[1];

        // Convert the top corner to canvas local space
        Vector2 topCornerLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, topCorner, null, out topCornerLocalPos);

        // Calculate how far the Infobox is exceeding the top of the canvas
        float overflowHeight = topCornerLocalPos.y - canvasRectTransform.rect.height / 2;

        // If the Infobox is overflowing the top of the canvas
        if (overflowHeight > 0)
        {
            Vector3 newPosition = infoBoxRectTransform.localPosition;
            newPosition.y -= overflowHeight;
            infoBoxRectTransform.localPosition = newPosition;
        }
    }
}