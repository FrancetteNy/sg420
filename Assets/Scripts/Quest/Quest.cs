using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public string Questname;
    [TextArea(3, 10)]
    public string Questdescription;
    public MessageSystemEvent EventThatStartsQuest;
    public List<Objective> Objectives;
}

[Serializable]
public class Objective
{
    [TextArea(3, 10)]
    public string ObjectiveDescription;
    public MessageSystemEvent EventThatFinishesObjective;
    public bool FireEventWhenObjectIsFinished;
    public MessageSystemEvent EventAfterObjectiveCompleted;
}