using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This shows how utz builds up his code. The center of gravity pulls off the camera to make it look at interesting points. 
/// There should be a slider, which sets the amount. 
/// On 100 the camera is set fully to the center of gravity and on 0 it is completely set by the players transform.
/// </summary>
public class CameraGravityPoint : MonoBehaviour {

    #region Private Static Fields

    private static List<Vector3> gravityPointsList = new List<Vector3>();

    #endregion

    #region Exposed Static Properties

    public static bool HasCenterOfGravity { get { return gravityPointsList.Count > 0; } }
    public static Vector3 CenterOfGravity { get; private set; }

    #endregion

    #region Private Fields

    [SerializeField] int mass;

    #endregion

    #region Unity Messages

    private void OnEnable()
    {
        RegisterPoint();
        UpdateCenterOfGravity();
    }

    private void OnDisable()
    {
        UnregisterPoint();
        UpdateCenterOfGravity();
    }

    #endregion

    #region Private Functions

    private void UpdateCenterOfGravity()
    {

    }

    private void RegisterPoint()
    {

    }

    private void UnregisterPoint()
    {

    }

    #endregion

    #region Interface Functions

    public string GetData()
    {
        return "" + mass;
    }

    public void SetData(string data)
    {
        // Tries to go through the string and get an integer
        if(!int.TryParse(data, out mass))
        {
            Debug.LogError("Could not parse the mass out of data");
        }
    }

    #endregion
}
