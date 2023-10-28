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

    private int heightState;
    private Renderer rippleRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rippleRenderer = GetComponentInChildren<Renderer>();
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
        heightState = (heightState + 1) % heightMaps.Count;

        var lastHeightIdx = (heightState - 1 + heightMaps.Count) % heightMaps.Count;
        rippleRenderer.material.SetTexture("_PreviousHeight1", heightMaps[lastHeightIdx]);
        var lastLastHeightIdx = (heightState - 2 + heightMaps.Count) % heightMaps.Count;
        rippleRenderer.material.SetTexture("_PreviousHeight2", heightMaps[lastLastHeightIdx]);

        // rippleRenderer.material.SetTexture("_MainTex", heightMaps[lastHeightIdx]);

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
