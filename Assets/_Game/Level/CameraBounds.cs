using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    [SerializeField] private float wallThickness = 1f;

    private BoxCollider2D topWall;
    private BoxCollider2D bottomWall;
    private BoxCollider2D leftWall;
    private BoxCollider2D rightWall;

    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Start()
    {
        CreateWalls();
        UpdateWalls();

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    private void Update()
    {
        // Detect resolution/aspect-ratio changes
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            UpdateWalls();

            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    private void CreateWalls()
    {
        topWall = CreateWall("Wall_Top");
        bottomWall = CreateWall("Wall_Bottom");
        leftWall = CreateWall("Wall_Left");
        rightWall = CreateWall("Wall_Right");
    }

    private void UpdateWalls()
    {
        Camera cam = Camera.main;

        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        // Top
        topWall.transform.position =
            new Vector2(0, height + wallThickness / 2);

        topWall.size =
            new Vector2(width * 2 + wallThickness * 2, wallThickness);

        // Bottom
        bottomWall.transform.position =
            new Vector2(0, -height - wallThickness / 2);

        bottomWall.size =
            new Vector2(width * 2 + wallThickness * 2, wallThickness);

        // Right
        rightWall.transform.position =
            new Vector2(width + wallThickness / 2, 0);

        rightWall.size =
            new Vector2(wallThickness, height * 2 + wallThickness * 2);

        // Left
        leftWall.transform.position =
            new Vector2(-width - wallThickness / 2, 0);

        leftWall.size =
            new Vector2(wallThickness, height * 2 + wallThickness * 2);
    }

    private BoxCollider2D CreateWall(string name)
    {
        GameObject wall = new GameObject(name);
        wall.transform.parent = transform;

        return wall.AddComponent<BoxCollider2D>();
    }
}