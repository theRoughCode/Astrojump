using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    // Type of panels, their respective probabilities
    private string[] panelTypes = { "GreenPanel", "OrangePanel", "RedPanel", "BluePanel", "PurplePanel" };
    private float[] yLevel = { 0, 20, 70, 150, 300 };
    private float[,] panelProbs = {
        { 1f, 0f, 0f, 0f, 0f },
        { 0.7f, 0.3f, 0f, 0f, 0f },
        { 0.5f, 0.3f, 0.2f, 0f, 0f },
        { 0f, 0.4f, 0.4f, 0.2f, 0f },
        { 0f, 0.3f, 0.3f, 0.3f, 0.1f },
    };

    // Difficulty settings
    private float[] panelGaps = { 0.46f, 0.7f, 0.93f, 1.16f };

    // Array of panels spawned
    public Queue<GameObject> panels;

    // Holds last panel in queue
    private GameObject lastPanel;

    // Used to determine position of panels
    private Camera cam;

    // Starting y position of initial panel (in worlds coords)
    private float startY = -3.1f;

    // Render height for panels (in world coords) -> Screen.height
    private float renderHeight;

    // X boundaries for panels (in world coords)
    private float minX, maxX;

    // Use this for initialization
    void Start()
    {
        panels = new Queue<GameObject>();
        cam = Camera.main;

        GameObject startPanel = ObjectPool.instance.GetObjectForType("GreenPanel", false);
        Vector3 pos = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, 80, cam.nearClipPlane));
        pos.y = startY;
        startPanel.transform.position = pos;
        panels.Enqueue(startPanel);
        lastPanel = startPanel;

        // Calculate x boundaries
        float panelWidth = startPanel.GetComponent<Renderer>().bounds.size.x;
        pos = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        minX = pos.x + panelWidth / 2;
        pos = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, cam.nearClipPlane));
        maxX = pos.x - panelWidth / 2;

        // Calculate max y for rendering panels
        renderHeight = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, cam.nearClipPlane)).y -
            cam.ScreenToWorldPoint(Vector3.zero).y;
    }

    // Spawns panels from minY to maxY
    public void Spawn(int level)
    {
        // Convert gap to world coords
        float gap = panelGaps[level];
        float minY = lastPanel.transform.position.y + gap;
        for (float y = minY; y <= minY + renderHeight; y += gap)
        {
            string panelType = pickPanelType(y);
            GameObject panel = ObjectPool.instance.GetObjectForType(panelType, false);

            // Dimensions of panel
            Vector3 pos = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            pos.x = Random.Range(minX, maxX);
            pos.y = y;
            panel.transform.position = pos;

            // If orange panel, initialize
            if (panelType == "OrangePanel") panel.GetComponent<OrangePanel>().Initialize();
            else if (panelType == "PurplePanel") panel.GetComponent<PurplePanel>().Initialize(minX, maxX);

            panels.Enqueue(panel);
            lastPanel = panel;
        }
    }

    // Despawn panels that are below the screen
    public void Despawn()
    {
        float minY = cam.ScreenToWorldPoint(Vector3.zero).y;
        while (panels.Count > 0)
        {
            GameObject panel = panels.Peek();
            float panelHeight = panel.GetComponent<Renderer>().bounds.size.y;

            // If top of queue is visible, rest should still be visible
            if (panel.transform.position.y > minY - panelHeight / 2) break;

            panel = panels.Dequeue();
            // Return to pool
            ObjectPool.instance.PoolObject(panel);

            if (panel == lastPanel) lastPanel = null;
        }
    }

    public void Restart()
    {
        // Clear all panels
        while (panels.Count > 0)
        {
            GameObject panel = panels.Dequeue();
            ObjectPool.instance.PoolObject(panel);
        }
        // Spawn initial panel
        GameObject startPanel = ObjectPool.instance.GetObjectForType("GreenPanel", false);
        Vector3 pos = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, cam.nearClipPlane));
        pos.y = startY;
        startPanel.transform.position = pos;
        panels.Enqueue(startPanel);
        lastPanel = startPanel;
    }

    // Returns true if game should spawn the next batch of panels
    public bool CanSpawnNext()
    {
        // Calculate y threshold for spawning (half screen height above view)
        Vector3 pos = cam.ScreenToWorldPoint(new Vector3(0, Screen.height * 1.5f, cam.nearClipPlane));
        return (lastPanel.transform.position.y < pos.y);
    }

    // Randomly select a panel type to spawn
    string pickPanelType(float y)
    {
        // Determine which difficulty level to select panels from
        int level = 0;
        for (int i = 0; i < yLevel.Length; i++)
        {
            if (y >= yLevel[i]) level = i;
        }

        // Select random num and determine panel type
        float num = Random.Range(0f, 1f);
        int panelType = 0;
        num -= panelProbs[level, panelType];
        while (num > 0)
        {
            panelType++;
            num -= panelProbs[level, panelType];
        }

        return panelTypes[panelType];
    }
}
