using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltController : MonoBehaviour {

    [SerializeField] float speed = 10f;
    public LayerMask collideLayers;

    private void Awake()
    {
        int enemyLayer = LayerMask.NameToLayer("Enemies");
        int obstacleLayer = LayerMask.NameToLayer("Obstacles");
        collideLayers = 1 << enemyLayer;
        LayerMask obstLayerMask = 1 << obstacleLayer;
        collideLayers = collideLayers | obstLayerMask;
    }

    // Update is called once per frame
    void Update () {
        transform.position += transform.forward * speed * Time.deltaTime;
        StartCoroutine(DestroyAfterTime(3f));
	}

    private void FixedUpdate()
    {
        if(Physics.Raycast(transform.position, transform.forward, 1.4f, collideLayers))
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawRay(transform.position, transform.forward * 1.4f);
    //}
}
