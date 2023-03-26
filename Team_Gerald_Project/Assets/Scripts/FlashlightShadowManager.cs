using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightShadowManager : MonoBehaviour
{
    [SerializeField] AimManager aimer;
    [SerializeField] Light2D light;
    [SerializeField] LayerMask shadowLayers;
    [SerializeField] float maxDistance;
    private List<ShadowCaster2D> scs = new List<ShadowCaster2D>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(ShadowCaster2D s in scs)
        {
            s.selfShadows = true;
        }
        scs.Clear();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, aimer.to.normalized, maxDistance, shadowLayers);
        if (hit.transform != null && hit.transform.gameObject.GetComponent<ShadowCaster2D>() != null) {
            hit.transform.gameObject.GetComponent<ShadowCaster2D>().selfShadows = false;
            scs.Add(hit.transform.gameObject.GetComponent<ShadowCaster2D>());
        }

        float angle = (Mathf.Atan2(aimer.to.normalized.y, aimer.to.normalized.x) * Mathf.Rad2Deg);

        float highAngle = angle + (0.5f * light.pointLightOuterAngle);
        float lowAngle = angle - (0.5f * light.pointLightOuterAngle);

        Vector2 highDir = new Vector2(Mathf.Cos(highAngle * Mathf.Deg2Rad), Mathf.Sin(highAngle * Mathf.Deg2Rad));
        Vector2 lowDir = new Vector2(Mathf.Cos(lowAngle * Mathf.Deg2Rad), Mathf.Sin(lowAngle * Mathf.Deg2Rad));

        hit = Physics2D.Raycast(transform.position, highDir, maxDistance, shadowLayers);
        if (hit.transform != null && hit.transform.gameObject.GetComponent<ShadowCaster2D>() != null)
        {
            hit.transform.gameObject.GetComponent<ShadowCaster2D>().selfShadows = false;
            scs.Add(hit.transform.gameObject.GetComponent<ShadowCaster2D>());
        }

        hit = Physics2D.Raycast(transform.position, lowDir, maxDistance, shadowLayers);
        if (hit.transform != null && hit.transform.gameObject.GetComponent<ShadowCaster2D>() != null)
        {
            hit.transform.gameObject.GetComponent<ShadowCaster2D>().selfShadows = false;
            scs.Add(hit.transform.gameObject.GetComponent<ShadowCaster2D>());
        }

    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (aimer.to.normalized * maxDistance));
        float angle = (Mathf.Atan2(aimer.to.normalized.y, aimer.to.normalized.x) * Mathf.Rad2Deg);

        float highAngle = angle + (0.5f * light.pointLightOuterAngle);
        float lowAngle = angle - (0.5f * light.pointLightOuterAngle);

        Vector2 highDir = new Vector2(Mathf.Cos(highAngle * Mathf.Deg2Rad), Mathf.Sin(highAngle * Mathf.Deg2Rad));
        Vector2 lowDir = new Vector2(Mathf.Cos(lowAngle * Mathf.Deg2Rad), Mathf.Sin(lowAngle * Mathf.Deg2Rad));
        Gizmos.DrawLine(transform.position, (Vector2) transform.position + (highDir * maxDistance));
        Gizmos.DrawLine(transform.position, (Vector2) transform.position + (lowDir * maxDistance));

    }
    
}
