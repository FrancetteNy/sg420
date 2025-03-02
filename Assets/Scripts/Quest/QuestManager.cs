using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    public List<Quest> LockedQuests;
    private Dictionary<Quest, UnityAction> _startQuestActions;
    private Dictionary<Objective, UnityAction> _objectiveActions;
    GameState _gameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameState = GameStateManagerSingleton.Instance.GameState;
        _startQuestActions = new Dictionary<Quest, UnityAction>();
        _objectiveActions = new Dictionary<Objective, UnityAction>();
        LockedQuests = Resources.LoadAll<Quest>("").ToList();
        foreach (Quest quest in _gameState.DoneQuestsList.List)
        {
            LockedQuests.Remove(quest);
        }
        foreach (Quest quest in _gameState.ActiveQuestsList.List)
        {
            StartQuest(quest);
        }
        foreach(Quest quest in LockedQuests)
        {
            UnityAction action = () => StartQuest(quest);
            _startQuestActions[quest] = action;
            MessageSystem.StartListening(quest.EventThatStartsQuest, action);
        }
    }
    private void StartQuest(Quest quest)
    {
        LockedQuests.Remove(quest);
        if (_startQuestActions.TryGetValue(quest, out var action)){
            MessageSystem.StopListening(quest.EventThatStartsQuest, action);
        }
        
        _startQuestActions.Remove(quest);
        if (!_gameState.ActiveQuestsList.List.Contains(quest))
        {
            _gameState.ActiveQuestsList.List.Add(quest);
        }
        

        if (quest.Objectives.Count > 0) {
            SetUpNextObjective(quest, 0);
        }
        else
        {
            Debug.LogError($"{quest.Questname} hat kein Objective");
        }
        
    }

    private void FinishObjective(Quest quest, int objectiveIndex)
    {
        Objective objective = quest.Objectives[objectiveIndex];
        MessageSystem.StopListening(objective.EventThatFinishesObjective, _objectiveActions[objective]);
        _objectiveActions.Remove(objective);
        if (objective.FireEventWhenObjectIsFinished)
        {
            MessageSystem.FireEvent(objective.EventAfterObjectiveCompleted);
        }
        if (quest.Objectives.Count >= objectiveIndex)
        {
            SetUpNextObjective(quest, objectiveIndex + 1);

        }
        else
        {
            _gameState.ActiveQuestsList.List.Remove(quest);
            _gameState.DoneQuestsList.List.Add(quest);
            print("quest finished");
        }
    }

    private void SetUpNextObjective(Quest quest, int objectiveIndex)
    {
        UnityAction action = () => FinishObjective(quest, objectiveIndex);
        _objectiveActions[quest.Objectives[objectiveIndex]] = action;
        MessageSystem.StartListening(quest.Objectives[objectiveIndex].EventThatFinishesObjective, action);
    }
}
