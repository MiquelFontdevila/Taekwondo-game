
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[RequireComponent(typeof(RectTransform))]
public class ResponsiveGrid : MonoBehaviour
{
    [SerializeField, Min(1)]
    private int columnes = 2;

    private GridLayoutGroup grid;
    private RectTransform rect;

    private void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        rect = GetComponent<RectTransform>();

        grid.constraint =
            GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columnes;
    }

    private void OnRectTransformDimensionsChange()
    {
        if (grid == null || rect == null)
            return;

        Ajustar();
    }

    private void OnEnable()
    {
        if (grid != null)
            Ajustar();
    }

    private void Ajustar()
    {
        int elements = transform.childCount;
        int files = Mathf.CeilToInt(
            (float)elements / columnes
        );

        float ample = rect.rect.width
            - grid.padding.left
            - grid.padding.right
            - grid.spacing.x * (columnes - 1);

        float alt = rect.rect.height
            - grid.padding.top
            - grid.padding.bottom
            - grid.spacing.y * (files - 1);

        grid.cellSize = new Vector2(
            Mathf.Max(1, ample / columnes),
            Mathf.Max(1, alt / Mathf.Max(1, files))
        );
    }
}
