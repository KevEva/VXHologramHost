using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class EntityResponse
{

    public string CategoryKey;
    public string TextResponse;
    public string AnimationTrigger;
}

[Serializable]
public class LocationData
{
    public string Name;
    public Location Location;

}

public class Location {
    public double latitude;
    public double longitude;
    public boolean isStatic;
}

[Serializable]
public class EntityResponses
{

    public EntityResponse[] responses;
}

[Serializable]
public class LocationDataPoints
{
    public LocationData[] location;
}