using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public string Questname;
    [TextArea(3, 10)] public string Questdescription;
    public MessageSystemEvent EventThatStartsQuest;
    public List<Objective> Objectives;
}

[Serializable]
public class Objective
{
    public string ObjectiveName;
    [TextArea(3, 10)] public string ObjectiveDescription;
    public MessageSystemEvent EventThatFinishesObjective;
    public bool FireEventWhenObjectIsFinished;
    public MessageSystemEvent EventAfterObjectiveCompleted;
}
[Serializable]
public class QuestWithObjectiveIndex
{
    public Quest Quest;
    public int ObjectiveIndex;
    public QuestWithObjectiveIndex(Quest Quest)
    {
        this.Quest = Quest;
        this.ObjectiveIndex = 0;
    }
}