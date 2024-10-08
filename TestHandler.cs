using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TestHandler : MonoBehaviour
{

    public NPC TestNPC;
    //Input fields
    public InputField inputField;
    public InputField keywordField;
    public InputField responseField;
    // Responses Database
    private EntityResponses _responses;
    //User input
    private string input;
    //Location Database
    private LocationDatas _location;


    void Start()
    {
        //Get responses from json
        string responseJson = File.ReadAllText(Application.dataPath + "/ResponseDataFile.json");
        _responses = JsonUtility.FromJson<EntityResponses>(responseJson);

        //Get Locations from json
        string locationJson = File.ReadAllText(Application.dataPath + "/LocationData.json");
        _location = JsonUtility.FromJson<LocationDatas>(locationJson);
    }
    public void ReadStringInput()
    {
        //Get Given input text, then find corrosponding category and display returns values.
        // To test ResponseDataFile
        // var e = TestNPC.GetResponseFromEntity(inputField.text, _responses);
        // keywordField.text = e.CategoryKey;
        // responseField.text = e.TextResponse;

        // Debug.Log(e.TextResponse);
        // Debug.Log(e.CategoryKey);
        // Debug.Log(e.AnimationTrigger);

        // To Test LocationData.
        var e = TestNPC.GetLocationFromEntity(inputField.text, _location);
        keywordField.text = e.name;
        responseField.text = e.description;

    }
    public void SaveToJson()
    {
        //Take given text from fields and save to json as new category.
        EntityResponse data = new EntityResponse();
        data.CategoryKey = keywordField.text;
        data.TextResponse = responseField.text;
        data.AnimationTrigger = "IsTalking";
        Array.Resize(ref _responses.responses, _responses.responses.Length + 1);
        _responses.responses[_responses.responses.Length - 1] = data;

        string responseJson = JsonUtility.ToJson(_responses, true);
        File.WriteAllText(Application.dataPath + "/ResponseDataFile.json", responseJson);
    }

}
