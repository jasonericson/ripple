using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickRaycast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                var rippler = hit.collider.GetComponentInParent<Rippler>();
                if (rippler)
                {
                    rippler.DoRippleFromWorldPos(hit.point, Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                }
            }
        }
    }
}
