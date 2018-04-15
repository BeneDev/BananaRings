using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    GameObject player;

    [SerializeField] float speed = 1f;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newPos = player.transform.position;
        newPos.y = 0f;
        transform.position = Vector3.Lerp(transform.position, newPos, speed * Time.deltaTime);
	}
}
