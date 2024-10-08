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
public class EntityResponses
{

    public EntityResponse[] responses;
}

[Serializable]
public class LocationData
{
    public string name;
    public string note;
    public string description;
    public string coordinate_system;
    public Locations locations;
}

[Serializable]
public class Locations
{
    public double latitude;
    public double longitude;
    public bool isStatic;
    public string relative_to;
}
[Serializable]
public class LocationDatas
{
    public LocationData[] location;
}