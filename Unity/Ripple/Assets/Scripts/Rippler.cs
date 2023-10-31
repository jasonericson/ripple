using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using FMODUnity;

public class Rippler : MonoBehaviour
{
    public List<RenderTexture> heightMaps;
    public RenderTexture tempHeightMap;
    public Camera splashCam;
    public Renderer splashRenderer;
    public Camera heightCam;
    public Renderer heightRenderer;
    public FMODUnity.EventReference noteEvent;

    private int heightState;
    private Renderer rippleRenderer;
    private float timeAccumulator = 0.0f;

    void ClearRenderTexture(RenderTexture renderTexture)
    {
        var rt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = rt;
    }

    // Start is called before the first frame update
    void Start()
    {
        rippleRenderer = GetComponentInChildren<Renderer>();

        foreach (var heightMap in heightMaps)
        {
            ClearRenderTexture(heightMap);
        }
        ClearRenderTexture(tempHeightMap);
    }

    float MapRangeClamped(float value, float outRangeA, float outRangeB)
    {
        return Mathf.Lerp(outRangeA, outRangeB, Mathf.Clamp01(value));
    }

    public void DoRippleFromWorldPos(Vector3 worldPos, float strength, float size)
    {
        // @todo: if we move the plane, we'll have to factor in position
        var normalizedPos = new Vector2(worldPos.x * 0.5f / this.transform.localScale.x + 0.5f, worldPos.z * 0.5f / this.transform.localScale.z + 0.5f);
        DoRipple(0.5f, normalizedPos, 0.5f);
        
        // Play note
        var note = FMODUnity.RuntimeManager.CreateInstance(noteEvent);
        note.setParameterByName("Z", normalizedPos.y);
        note.setVolume(strength);
        note.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(worldPos));
        note.start();
        note.release();
    }

    private void DoRipple(float strength, Vector2 position, float size)
    {
        var posColor = new Color(position.x, position.y, 0.0f, 0.0f);

        // clamp size from 0-1 to 0.02-0.035
        var sizeClamped = MapRangeClamped(strength, 0.02f, 0.035f);

        // send values to splash material
        splashRenderer.material.SetColor("_ForcePosition", posColor);
        splashRenderer.material.SetFloat("_ForceSize", sizeClamped);
        splashRenderer.material.SetFloat("_ForceStrength", strength);
        Graphics.CopyTexture(heightMaps[heightState], tempHeightMap);
        splashRenderer.material.SetTexture("_Heightfield", tempHeightMap);

        // render splash material to texture
        splashCam.targetTexture = heightMaps[heightState];
        splashCam.Render();
    }

    // Update is called once per frame
    void Update()
    {
        // use timeAccumulator to update at a fixed rate (60 times/sec)
        timeAccumulator += Time.deltaTime;
        while (timeAccumulator >= 1.0f / 60.0f)
        {
            timeAccumulator -= 1.0f / 60.0f;

            // cycle through height maps
            heightState = (heightState + 1) % heightMaps.Count;

            // send last 2 height maps to heightfield material
            var lastHeightIdx = (heightState - 1 + heightMaps.Count) % heightMaps.Count;
            heightRenderer.material.SetTexture("_PreviousHeight1", heightMaps[lastHeightIdx]);
            var lastLastHeightIdx = (heightState - 2 + heightMaps.Count) % heightMaps.Count;
            heightRenderer.material.SetTexture("_PreviousHeight2", heightMaps[lastLastHeightIdx]);

            // render heightfield material to texture
            heightCam.targetTexture = heightMaps[heightState];
            heightCam.Render();
        }

        // send heightfield texture to rippler
        rippleRenderer.material.SetTexture("_Heightfield", heightMaps[heightState]);
    }
}
