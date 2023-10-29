using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Rippler : MonoBehaviour
{
    public List<RenderTexture> heightMaps;
    public Camera splashCam;
    public Renderer splashRenderer;
    public Camera heightCam;
    public Renderer heightRenderer;
    public Camera normalCam;
    public Renderer normalRenderer;

    private int heightState;
    private Renderer rippleRenderer;
    private float timeAccumulator = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rippleRenderer = GetComponentInChildren<Renderer>();
        
        var x = UnityEngine.Random.Range(0.0f, 1.0f);
        var y = UnityEngine.Random.Range(0.0f, 1.0f);
        var size = UnityEngine.Random.Range(0.0f, 1.0f);
        var strength = UnityEngine.Random.Range(0.2f, 1.0f);

        DoRipple(strength, new Vector2(x, y), size);
    }

    float MapRangeClamped(float value, float outRangeA, float outRangeB)
    {
        return Mathf.Lerp(outRangeA, outRangeB, Mathf.Clamp01(value));
    }

    void DoRipple(float strength, Vector2 position, float size)
    {
        var posColor = new Color(MapRangeClamped(position.x, 0.2f, 0.8f), MapRangeClamped(position.y, 0.2f, 0.8f), 0.0f, 0.0f);

        var sizeClamped = MapRangeClamped(strength, 0.02f, 0.035f);

        splashRenderer.material.SetColor("_ForcePosition", posColor);
        splashRenderer.material.SetFloat("_ForceSize", sizeClamped);
        splashRenderer.material.SetFloat("_ForceStrength", strength);

        splashCam.targetTexture = heightMaps[heightState];
        splashCam.Render();
    }

    // Update is called once per frame
    void Update()
    {
        timeAccumulator += Time.deltaTime;
        while (timeAccumulator >= 1.0f / 60.0f)
        {
            timeAccumulator -= 1.0f / 60.0f;

            heightState = (heightState + 1) % heightMaps.Count;

            var lastHeightIdx = (heightState - 1 + heightMaps.Count) % heightMaps.Count;
            heightRenderer.material.SetTexture("_PreviousHeight1", heightMaps[lastHeightIdx]);
            var lastLastHeightIdx = (heightState - 2 + heightMaps.Count) % heightMaps.Count;
            heightRenderer.material.SetTexture("_PreviousHeight2", heightMaps[lastLastHeightIdx]);

            heightCam.targetTexture = heightMaps[heightState];
            heightCam.Render();
        }
        
        normalRenderer.material.SetTexture("_Heightfield", heightMaps[heightState]);

        rippleRenderer.material.SetTexture("_Heightfield", heightMaps[heightState]);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var x = UnityEngine.Random.Range(0.0f, 1.0f);
            var y = UnityEngine.Random.Range(0.0f, 1.0f);
            var size = UnityEngine.Random.Range(0.0f, 1.0f);
            var strength = UnityEngine.Random.Range(0.2f, 1.0f);

            DoRipple(strength, new Vector2(x, y), size);
        }
    }
}
