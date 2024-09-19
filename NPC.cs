//Vladislava Simakov
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using UnityEngine.Rendering;
using WebSocketSharp;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UniVRM10;
using Unity.VisualScripting;
using System.IO;
using System.Linq;
public class NPC : MonoBehaviour
{
    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;
    public Animator animator;
    public Text output_text;
    public Vector3 avator_posi;
    public string responseFilePath;
    private float _timeUntilBored;
    private int _numberOfBoredAnimations;
    private bool _isBored;
    private float _idleTime;
    private int _boredAnimation;
    private EntityResponses _responses;
    private LocationDataPoints _location;

    public enum Direction
    {
        FRONT = 0,
        RIGHT = 90,
        BACK = 180,
        LEFT = 270
    }

    private void Start()
    {
        // replace this with your key 
        string subscriptionKey = Key.subscriptionKey;
        string region = Key.region;

        speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);
        speechConfig.SpeechSynthesisVoiceName = "en-US-AriaNeural";
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);

        // Create the Speech Synthesizer
        synthesizer = new SpeechSynthesizer(speechConfig, null);
        animator = GetComponent<Animator>();

        avator_posi = new Vector3(0, 0, 0);
        _boredAnimation = UnityEngine.Random.Range(1, 3 + 1);
        _boredAnimation = _boredAnimation * 2 - 1;
        //animator.SetFloat("BoredIdle", 0);
        animator.SetFloat("BoredIdle", 1.46f);

        //Load the Json responses and location
        string json = File.ReadAllText(Application.dataPath + "/ResponseDataFile.json");
        _responses = JsonUtility.FromJson<EntityResponses>(json);

        string json = File.ReadAllText(Application.dataPath + "/LocationData.json");
        _location = JsonUtility.FromJson<LocationDataPoints>(json);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
    /*
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo)
        {
            ResetIdle();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_isBored == false)
            {
                _idleTime += Time.deltaTime;

                if (_idleTime > _timeUntilBored && stateInfo.normalizedTime % 1 < 0.02f)
                {
                    _isBored = true;
                    _boredAnimation = UnityEngine.Random.Range(1, _numberOfBoredAnimations + 1);
                    _boredAnimation = _boredAnimation * 2 - 1;
                    animator.SetFloat("BoredIdle", _boredAnimation - 1);
                }
            }
            animator.SetFloat("BoredIdle", _boredAnimation, 0.2f, Time.deltaTime);
        }

        private void ResetIdle()
        {
            if (_isBored)
            {
                _boredAnimation--;
            }

            _isBored = false;
            _idleTime = 0;
        }

    */

    public LocationData GetLocationFromEntity(string name, LocationDataPoints locationWrapper)
    {
        // Search the responses list for the given category and get the releavent properties.
        LocationData lcObj = new List<LocationData>(locationWrapper.location).FirstOrDefault(resp => resp.name == userLocation);
        return locObj;
    }

    public EntityResponse GetResponseFromEntity(string category, EntityResponses responsesWrapper)
    {
        // Search the responses list for the given category and get the releavent properties.
        EntityResponse respObj = new List<EntityResponse>(responsesWrapper.responses).FirstOrDefault(resp => resp.CategoryKey == category);
        return respObj;
    }

    // Read the user entered result and find the approriate response
    public void ReadResult(ConversationResult res)
    {
        Debug.Log("read result has been called");
        string topIntent = res.result.prediction.topIntent;
        string response = "Please say again.";

        // Check if there is a result and if the top scoring intent is "TellMe"
        if (res != null && topIntent == "TellMe")
        {
            // Check the returned category, find the response, and apply properties.
            foreach (var entity in res.result.prediction.entities)
            {
                EntityResponse respObj = GetResponseFromEntity(entity.category, _responses);
                response = respObj.TextResponse;
                // if statement, if direction of entity exist in data base, point in direction else, do random animation
                animator.SetTrigger(respObj.AnimationTrigger + getRandomTrigger(1));
            }
            Debug.Log(response);
        }

        else if (res != null && topIntent == "Tell me")
        {
            response = "Lalalala";
            StartCoroutine(UpdateOutputText(response));
            HelloWorld.Instance.SynthesizeSpeech(response);
        }

        else
        {
            StartCoroutine(UpdateOutputText(response));
            HelloWorld.Instance.SynthesizeSpeech(response);
            Debug.LogError("here");
            Debug.Log(response);
        }
    }

    // Pinpoint location relative to set locations
    public void LocationResult()
    {
        // Get the lat and long.
        double latA = UserLat;
        double longA = UserLog;
        double latB = SetLat;
        double longB = SetLog;
        // GeoCoordinate the distance in metres between the two provided values
        var locA = new GeoCoordinate(latA, longA);
        var locB = new GeoCoordinate(latB, longB);
        double distance = locA.GetDistanceTo(locB); // metres
        Debug.Log(distance);
        return distance;
    }

    // Gettng degree bearing

    public static double Bearing(double UserLat, double UserLog, double SetLat, double SetLog)
    {
        //Converting lat and long from degress into radians
        double x = Math.Cos(DegreesToRadians(UserLat)) * Math.Sin(DegreesToRadians(SetLat)) - Math.Sin(DegreesToRadians(UserLat)) * Math.Cos(DegreesToRadians(SetLat)) * Math.Cos(DegreesToRadians(SetLog - UserLog));
        double y = Math.Sin(DegreesToRadians(SetLog - UserLog)) * Math.Cos(DegreesToRadians(SetLat));

        // Math.Atan2 can return negative value, 0 <= output value < 2*PI expected 
        return (Math.Atan2(y, x) + Math.PI * 2) % (Math.PI * 2);
    }
    //Transform value into degress
    public static double DegreesToRadians(double angle)
    {
        return angle * Math.PI / 180.0d;
    }

    // static double DegressBearing(double UserLat, double UserLog, double SetLat, double SetLog){
    //     var dLon = ToRad(SetLog-UserLog);
    //     var dPhi = Math.Log(
    //         Math.Tan(ToRad(SetLat)/2+Math.PI/4)/Math.Tan(ToRad(UserLat)/2+Math.PI/4));
    //     if (Math.Abs(dLon) > Math.PI)
    //         dLon = dLon > 0 ? -(2*Math.PI-dLon) : (2*Math.PI+dLon);
    //     return ToBearing(Math.Atan2(dLon, dPhi));
    // }
    // public static double ToRad(double degrees)
    // {
    //     return degrees * (Math.PI / 180);
    // }

    // public static double ToDegrees(double radians)
    // {
    //     return radians * 180 / Math.PI;
    // }

    // public static double ToBearing(double radians) 
    // {  
    //     // convert radians to degrees (as bearing: 0...360)
    //     return (ToDegrees(radians) +360) % 360;
    // }

    //added these
    public void ReadAIResult(string airesponse, ConversationResult res)
    {
        Debug.Log("read result has been called");
        animator.SetBool("IsTalking", false);

        if (res.result.prediction.topIntent == "TellMe")
        {
            animator.SetTrigger("IsTalking0");
            HelloWorld.Instance.SynthesizeSpeech(airesponse);

        }
        else if (res.result.prediction.topIntent == "Location")
        {
            animator.SetTrigger("IsTalking1");
            HelloWorld.Instance.SynthesizeSpeech(airesponse);

        }
        else
        {
            Debug.Log(airesponse);
            animator.SetTrigger("IsTalking0");
            StartCoroutine(UpdateOutputText(airesponse));
            HelloWorld.Instance.SynthesizeSpeech(airesponse);
        }
    }

    private IEnumerator UpdateOutputText(string message)
    {
        output_text.text = message;
        yield return null;
    }
    //added ai read result

    //choose a random talking animation for naturalistic motion
    public string getRandomTrigger(int option)
    {
        string idx;
        int[] myTalkingAnims = { 0, 1 };
        int[] myIdleAnims = { 1, 2, 3 };
        int[] array;
        array = option > 0 ? myTalkingAnims : myIdleAnims;
        int length = array.Length;
        System.Random rand = new System.Random();
        int index = rand.Next(length);
        int chosen_anim = array[index];
        idx = chosen_anim.ToString();
        return idx;

    }

    void getDirection(string directionText)
    {
        switch (directionText)
        {
            case "forward":
                animator.SetTrigger("Forward");
                break;
            case "behind":
                animator.SetTrigger("Behind");
                break;
            case "left":
                animator.SetTrigger("LeftTrigger");
                break;
            case "right":
                animator.SetTrigger("RightTrigger");
                break;
            case "somewhere":
                animator.SetTrigger("Somewhere");
                break;
        }

    }

    // point to direction relative to users
    void getEnumDirection(Direction to_point)
    {
        switch (to_point)
        {
            case Direction.FRONT:
                animator.SetTrigger("Forward");
                break;
            case Direction.BACK:
                animator.SetTrigger("Behind");
                break;
            case Direction.LEFT:
                animator.SetTrigger("RightTrigger");
                break;
            case Direction.RIGHT:
                animator.SetTrigger("LeftTrigger");
                break;
        }
    }

    IEnumerator TriggerAnimations()
    {
        animator.SetTrigger("WaveTrigger2");
        yield return new WaitForSeconds(9);
        animator.SetTrigger("LeftTrigger");
        yield return new WaitForSeconds(11);
        animator.SetTrigger("Behind");
        yield return new WaitForSeconds(7f);
        animator.SetTrigger("RightTrigger");
        yield return new WaitForSeconds(6);
        animator.SetTrigger("Forward");
    }
    Direction getBestDirection(Vector3 target_position)
    {
        // float degree_to_point = getDegree(target_position);

        // Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));

        // Direction best_direction = Direction.FRONT;
        // float lowest_dif = 360;
        // foreach (Direction dir in directions)
        // {
        //     // float angle_diff = Mathf.Abs((degree_to_point % 360) - (float)dir);

        //     float angle_diff = Mathf.Min(Mathf.Abs((degree_to_point % 360) - (float)dir), 360 - Mathf.Abs((degree_to_point % 360) - (float)dir));

        //     if (angle_diff < lowest_dif)
        //     {
        //         lowest_dif = angle_diff;
        //         best_direction = dir;
        //     }
        // }

        // Debug.Log("best_direction: " + best_direction + ", lowest_dif: " + lowest_dif);
        // return best_direction;

        //Converting lat and long from degress into radians
        double x = Math.Cos(DegreesToRadians(UserLat)) * Math.Sin(DegreesToRadians(SetLat)) - Math.Sin(DegreesToRadians(UserLat)) * Math.Cos(DegreesToRadians(SetLat)) * Math.Cos(DegreesToRadians(SetLog - UserLog));
        double y = Math.Sin(DegreesToRadians(SetLog - UserLog)) * Math.Cos(DegreesToRadians(SetLat));

        return (Math.Atan2(y, x) + Math.PI * 2) % (Math.PI * 2)

    }

    private float getDegree(Vector3 target_position)
    {
        Vector3 targetDir = target_position - avator_posi;
        targetDir.y = 0; // Optional: Keep rotation in the horizontal plane only

        // Calculate the rotation needed to look at the target direction
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);

        Vector3 eulerRotation = targetRotation.eulerAngles;
        float yRotation = eulerRotation.y;

        float yRotationNormalized = NormalizeAngle360(yRotation);

        Debug.Log("yRotationNormalized: " + yRotationNormalized);

        return yRotationNormalized;
    }

    public static double DegreesToRadians(double angle)
    {
        return angle * Math.PI / 180.0d;
    }

    private float NormalizeAngle360(float angle)
    {
        return (angle % 360 + 360) % 360;
    }
}
