using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    GameObject player;

    [SerializeField] float speed = 1f;
    [SerializeField] float expand = 1f;
    [SerializeField] float veloExpand = 0.35f;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newPos = player.transform.position;
        newPos += player.GetComponent<PlayerController>().ShootDirection * expand;
        newPos += player.GetComponent<PlayerController>().Velocity * veloExpand;
        transform.position = Vector3.Lerp(transform.position, newPos, speed * Time.deltaTime);
	}
}
