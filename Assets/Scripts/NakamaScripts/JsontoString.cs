using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[System.Serializable]
public class Person
{
    public string name;
    public string id;
 
    public Person(string _name , string _id)
    {
        name = _name;
        id = _id;
 
    }
}

[System.Serializable]
public class PersonData
{
    public List<Person> client;
 
}


