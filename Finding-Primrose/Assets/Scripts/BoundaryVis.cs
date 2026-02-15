using UnityEngine;

public class BoundaryVisualizer : MonoBehaviour
{
    [Header("Boundary Settings")]
    public float leftBoundary = -2.5f;
    public float rightBoundary = 2.5f;
    public float boundaryLength = 30f; // How far forward/back the lines extend
    public float yHeight = 0.5f; // Height above ground

    [Header("Visual Settings")]
    public bool showBoundaries = true;
    public Color boundaryColor = Color.green;
    public float lineWidth = 0.1f;

    [Header("Optional: Different Boundaries Per Path")]
    public bool showMultiplePaths = false;
    public float frontPathZ = 0f;
    public float middlePathZ = 5f;
    public float backPathZ = 10f;

    private LineRenderer leftLine;
    private LineRenderer rightLine;
    
    // For multiple paths
    private LineRenderer[] pathLines;

    void Start()
    {
        CreateBoundaryLines();
    }

    void Update()
    {
        // Toggle visibility with 'B' key
        if (Input.GetKeyDown(KeyCode.B))
        {
            showBoundaries = !showBoundaries;
            UpdateVisibility();
        }

        // Update if boundaries change in Inspector
        if (leftLine != null)
        {
            UpdateBoundaryPositions();
        }
    }

    void CreateBoundaryLines()
    {
        if (!showMultiplePaths)
        {
            // Create simple left/right boundaries
            leftLine = CreateLine("Left Boundary", boundaryColor);
            rightLine = CreateLine("Right Boundary", boundaryColor);
        }
        else
        {
            // Create boundaries for each path
            pathLines = new LineRenderer[6]; // 3 paths x 2 boundaries each
            
            pathLines[0] = CreateLine("Front Left", Color.green);
            pathLines[1] = CreateLine("Front Right", Color.green);
            pathLines[2] = CreateLine("Middle Left", Color.yellow);
            pathLines[3] = CreateLine("Middle Right", Color.yellow);
            pathLines[4] = CreateLine("Back Left", Color.red);
            pathLines[5] = CreateLine("Back Right", Color.red);
        }

        UpdateBoundaryPositions();
        UpdateVisibility();
    }

    LineRenderer CreateLine(string name, Color color)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(transform);
        
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        
        return lr;
    }

    void UpdateBoundaryPositions()
    {
        if (!showMultiplePaths && leftLine != null)
        {
            // Simple boundaries that follow the camera/character
            float centerZ = transform.position.z;
            float halfLength = boundaryLength / 2f;

            // Left boundary
            leftLine.SetPosition(0, new Vector3(leftBoundary, yHeight, centerZ - halfLength));
            leftLine.SetPosition(1, new Vector3(leftBoundary, yHeight, centerZ + halfLength));

            // Right boundary
            rightLine.SetPosition(0, new Vector3(rightBoundary, yHeight, centerZ - halfLength));
            rightLine.SetPosition(1, new Vector3(rightBoundary, yHeight, centerZ + halfLength));
        }
        else if (showMultiplePaths && pathLines != null)
        {
            float pathLength = 8f; // Length of each path segment

            // Front path
            pathLines[0].SetPosition(0, new Vector3(leftBoundary, yHeight, frontPathZ - pathLength/2));
            pathLines[0].SetPosition(1, new Vector3(leftBoundary, yHeight, frontPathZ + pathLength/2));
            pathLines[1].SetPosition(0, new Vector3(rightBoundary, yHeight, frontPathZ - pathLength/2));
            pathLines[1].SetPosition(1, new Vector3(rightBoundary, yHeight, frontPathZ + pathLength/2));

            // Middle path
            pathLines[2].SetPosition(0, new Vector3(leftBoundary, yHeight, middlePathZ - pathLength/2));
            pathLines[2].SetPosition(1, new Vector3(leftBoundary, yHeight, middlePathZ + pathLength/2));
            pathLines[3].SetPosition(0, new Vector3(rightBoundary, yHeight, middlePathZ - pathLength/2));
            pathLines[3].SetPosition(1, new Vector3(rightBoundary, yHeight, middlePathZ + pathLength/2));

            // Back path
            pathLines[4].SetPosition(0, new Vector3(leftBoundary, yHeight, backPathZ - pathLength/2));
            pathLines[4].SetPosition(1, new Vector3(leftBoundary, yHeight, backPathZ + pathLength/2));
            pathLines[5].SetPosition(0, new Vector3(rightBoundary, yHeight, backPathZ - pathLength/2));
            pathLines[5].SetPosition(1, new Vector3(rightBoundary, yHeight, backPathZ + pathLength/2));
        }
    }

    void UpdateVisibility()
    {
        if (!showMultiplePaths && leftLine != null)
        {
            leftLine.enabled = showBoundaries;
            rightLine.enabled = showBoundaries;
        }
        else if (showMultiplePaths && pathLines != null)
        {
            foreach (var line in pathLines)
            {
                if (line != null)
                    line.enabled = showBoundaries;
            }
        }
    }

    void OnValidate()
    {
        // Update when values change in Inspector
        if (Application.isPlaying && leftLine != null)
        {
            UpdateBoundaryPositions();
            
            // Update colors
            if (!showMultiplePaths)
            {
                leftLine.startColor = leftLine.endColor = boundaryColor;
                rightLine.startColor = rightLine.endColor = boundaryColor;
            }
            
            // Update width
            if (leftLine != null)
            {
                leftLine.startWidth = leftLine.endWidth = lineWidth;
                rightLine.startWidth = rightLine.endWidth = lineWidth;
            }
        }
    }

    void OnGUI()
    {
        if (showBoundaries)
        {
            GUI.color = Color.cyan;
            GUI.Label(new Rect(10, 150, 300, 20), "Press 'B' to toggle boundaries");
        }
    }
}