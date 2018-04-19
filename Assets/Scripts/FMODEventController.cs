using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FMOD works for every Platform, except WebGL
/// </summary>
public class FMODEventController : MonoBehaviour {

    private string eventRef;

    private FMOD.Studio.EventInstance eventInstance;

    // Dictionary is just as a list, but combines the first generic with the second like an adresse book
    private Dictionary<string, FMOD.Studio.ParameterInstance> paramDict;

    private void Awake()
    {
        // Instantiate the paramDict
        paramDict = new Dictionary<string, FMOD.Studio.ParameterInstance>();

        // Get the eventInstance
        eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventRef);

        // Get all the parameters and write them into the dictionary
        LoadParameters(eventInstance);
    }

    // Use this for initialization
    void Start () {
        eventInstance.start();
	}

    private void Update()
    {
        // Returns the pressed key as a string, for example if you press question mark, you get that ascii code
        //if(input.inputstring)

    }

    // Goes through every paramteter and loads them into the dictionary
    private void LoadParameters(FMOD.Studio.EventInstance eventInstance)
    {
        int paramCount;
        eventInstance.getParameterCount(out paramCount);

        for(int i = 0; i < paramCount; i++)
        {
            // Create the fields to get the instance and description in
            FMOD.Studio.ParameterInstance paramInstance;
            FMOD.Studio.PARAMETER_DESCRIPTION paramDescription;

            // Fill the fields
            eventInstance.getParameterByIndex(i, out paramInstance);
            paramInstance.getDescription(out paramDescription);

            // Put the name and instance into the dictionary
            paramDict.Add((string)paramDescription.name, paramInstance);

            Debug.LogFormat("Param : {0} | Type: {1} | Default Value: {2}", paramDescription.name, paramDescription.type.ToString(), paramDescription.defaultvalue);                 
        }
    }

    // Set the parameter with the given name to a specific value
    public void SetParameter(string paramName, float paramValue)
    {
        FMOD.Studio.ParameterInstance paramInstance;

        if(paramDict.TryGetValue(paramName, out paramInstance))
        {
            paramInstance.setValue(paramValue);
            Debug.LogFormat("Parameter {0} set to {1}.", paramName, paramValue);
        }
        else
        {
            Debug.LogErrorFormat("Parameter {0} not found!", paramName);
        }
    }
}
