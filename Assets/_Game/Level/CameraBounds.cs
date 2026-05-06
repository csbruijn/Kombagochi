using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    [SerializeField] private float wallThickness = 1f;

    void Start()
    {
        CreateWalls();
    }

    private void CreateWalls()
    {
        Camera cam = Camera.main;

        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        // (position, size)
        CreateWall("Wall_Top", new Vector2(0, height + wallThickness / 2), new Vector2(width * 2 + wallThickness * 2, wallThickness));
        CreateWall("Wall_Bottom", new Vector2(0, -height - wallThickness / 2), new Vector2(width * 2 + wallThickness * 2, wallThickness));
        CreateWall("Wall_Right", new Vector2(width + wallThickness / 2, 0), new Vector2(wallThickness, height * 2 + wallThickness * 2));
        CreateWall("Wall_Left", new Vector2(-width - wallThickness / 2, 0), new Vector2(wallThickness, height * 2 + wallThickness * 2));
    }

    private void CreateWall(string name, Vector2 position, Vector2 size)
    {
        GameObject wall = new GameObject(name);
        wall.transform.position = position;

        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = size;
    }
}