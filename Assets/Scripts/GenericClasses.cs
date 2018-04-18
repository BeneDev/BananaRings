using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericClasses : MonoBehaviour
{
    private void Start()
    {
        Generics<int> instance = new Generics<int>();
        instance.Print(2000);

        Generics<string> instanceString = new Generics<string>();
        instanceString.Print("Hello World");
    }
}

public class Generics<T> {

	public void Print(T data)
    {
        Debug.LogFormat("Type: {0} | Value: {1}", typeof(T).ToString(), data);
    }

    public T GetComponent<T>() where T : Component
    {
        T data;

        // Find the component

        return null; // Return T

    }

}
