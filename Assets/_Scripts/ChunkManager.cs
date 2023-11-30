using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOEvents;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] SOEvent startScalingEvent;
    [SerializeField] SOEvent startScalingDelayedEvent;
    [SerializeField] Vector2Vector2SOEvent populateChunkEvent;
    [SerializeField] Rigidbody2D playerBody;

    [SerializeField] Vector2 numChunksExtents = new Vector2(6, 4);
    [SerializeField] Vector2 chunkSize;
    [SerializeField] Vector2 centreChunk;
    [SerializeField] Vector2 previousCentreChunk;

    [SerializeField] float spawnDelayOnStart = 2f;

    private void Awake() 
    {
        startScalingEvent.AddListener(RunOnScaleIncrease);
        startScalingDelayedEvent.AddListener(RunOnScaleIncreaseDelayed);
    }

    private void RunOnScaleIncrease()
    {
        chunkSize *= ScaleManager.ScaleSizeFactorGlobal;
    }

    private void RunOnScaleIncreaseDelayed()
    {
        PopulateAllChunks();
    }

    private void Start() 
    {
        StartCoroutine(PopulateOnStart());
    }

    private IEnumerator PopulateOnStart()
    {
        yield return new WaitForSeconds(spawnDelayOnStart);
        PopulateAllChunks();
    }

    private void FixedUpdate() 
    {
        Vector2 newCentreChunk = GetCentreChunk(playerBody.transform.position);
        if (newCentreChunk != centreChunk) 
        {
            previousCentreChunk = centreChunk;
            centreChunk = newCentreChunk;
            PopulateOutskirtChunks();
        }
    }

    private void PopulateAllChunks()
    {
        for (int x = (int)(centreChunk.x - numChunksExtents.x); x < (int)(centreChunk.x + numChunksExtents.x + 1); x++)
        {
            for (int y = (int)(centreChunk.y - numChunksExtents.y); y < (int)(centreChunk.y + numChunksExtents.y + 1); y++)
            {
                if ( x == 0 && y == 0) continue;
                populateChunkEvent.Invoke(GetChunkCentre(x, y), chunkSize);
            }
        }
    }

    private void PopulateOutskirtChunks()
    {
        Vector2 deltaChunk = previousCentreChunk - centreChunk; 
        if (deltaChunk.x > 0) PopulateColumnChunks((int)(centreChunk.x - numChunksExtents.x));
        else if (deltaChunk.x < 0) PopulateColumnChunks((int)(centreChunk.x + numChunksExtents.x));

        if (deltaChunk.y > 0) PopulateRowChunks((int)(centreChunk.y - numChunksExtents.y));
        else if (deltaChunk.y < 0) PopulateRowChunks((int)(centreChunk.y + numChunksExtents.y));
    }

    private void PopulateRowChunks(int row)
    {
        for (int x = (int)(centreChunk.x - numChunksExtents.x); x < (int)(centreChunk.x + numChunksExtents.x + 1); x++) populateChunkEvent.Invoke(GetChunkCentre(x, row), chunkSize);
    }

    private void PopulateColumnChunks(int column)
    {
        for (int y = (int)(centreChunk.y - numChunksExtents.y); y < (int)(centreChunk.y + numChunksExtents.y + 1); y++) populateChunkEvent.Invoke(GetChunkCentre(column, y), chunkSize);
    }

    private Vector2 GetChunkCentre(int x, int y) => new Vector2(x * chunkSize.x, y * chunkSize.y);
    private Vector2 GetCentreChunk(Vector3 position)
    {
        Vector2 output = new Vector2(position.x / chunkSize.x, position.y / chunkSize.y);
        output += new Vector2(0.5f * Mathf.Sign(position.x), 0.5f * Mathf.Sign(position.y));
        return new Vector2((int)output.x, (int)output.y);
    }
}
