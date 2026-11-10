
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[RequireComponent(typeof(RectTransform))]
public class ResponsiveGrid : MonoBehaviour
{
    [SerializeField, Min(1)]
    private int columnas = 2;

    private GridLayoutGroup grid;
    private RectTransform rect;

    private void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        rect = GetComponent<RectTransform>();

        grid.constraint =
            GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columnas;
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
        int elementos = transform.childCount;
        int filas = Mathf.CeilToInt(
            (float)elementos / columnas
        );

        float ancho = rect.rect.width
            - grid.padding.left
            - grid.padding.right
            - grid.spacing.x * (columnas - 1);

        float alto = rect.rect.height
            - grid.padding.top
            - grid.padding.bottom
            - grid.spacing.y * (filas - 1);

        grid.cellSize = new Vector2(
            Mathf.Max(1, ancho / columnas),
            Mathf.Max(1, alto / Mathf.Max(1, filas))
        );
    }
}
