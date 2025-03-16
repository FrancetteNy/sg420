using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    private List<QuestWithObjectiveIndex> _lockedQuests;
    private Dictionary<QuestWithObjectiveIndex, UnityAction> _startQuestActions;
    private Dictionary<Objective, UnityAction> _objectiveActions;
    GameState _gameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameState = GameStateManagerSingleton.Instance.GameState;
        _startQuestActions = new Dictionary<QuestWithObjectiveIndex, UnityAction>();
        _objectiveActions = new Dictionary<Objective, UnityAction>();
        _lockedQuests = Resources.LoadAll<Quest>("").Select((Quest quest) => new QuestWithObjectiveIndex(quest)).ToList();
        foreach (QuestWithObjectiveIndex quest in _gameState.DoneQuestsList.List)
        {
            _lockedQuests.Remove(quest);
        }
        foreach (QuestWithObjectiveIndex quest in _gameState.ActiveQuestsList.List)
        {
            StartQuest(quest);
        }
        foreach(QuestWithObjectiveIndex quest in _lockedQuests)
        {
            UnityAction action = () => StartQuest(quest);
            _startQuestActions[quest] = action;
            MessageSystem.StartListening(quest.Quest.EventThatStartsQuest, action);
        }
    }
    private void StartQuest(QuestWithObjectiveIndex quest)
    {
        _lockedQuests.Remove(quest);
        if (_startQuestActions.TryGetValue(quest, out var action)){
            MessageSystem.StopListening(quest.Quest.EventThatStartsQuest, action);
        }
        
        _startQuestActions.Remove(quest);
        if (!_gameState.ActiveQuestsList.List.Contains(quest))
        {
            _gameState.ActiveQuestsList.List.Add(quest);
        }
        

        if (quest.Quest.Objectives.Count > 0) {
            SetUpCurrentObjective(quest);
        }
        else
        {
            Debug.LogError($"{quest.Quest.Questname} hat kein Objective");
        }
        
    }

    private void FinishObjective(QuestWithObjectiveIndex quest)
    {
        Objective objective = quest.Quest.Objectives[quest.ObjectiveIndex];
        quest.ObjectiveProgress += 1;
        if (objective.RepeatsNeeded > quest.ObjectiveProgress)
        {
            MessageSystem.FireEvent(MessageSystemEvent.ObjectiveFinished);
            return;
        }
        quest.ObjectiveIndex += 1;
        MessageSystem.StopListening(objective.EventThatFinishesObjective, _objectiveActions[objective]);
        _objectiveActions.Remove(objective);
        if (objective.FireEventWhenObjectiveIsFinished)
        {
            MessageSystem.FireEvent(objective.EventAfterObjectiveCompleted);
        }
        if (quest.Quest.Objectives.Count > quest.ObjectiveIndex)
        {
            SetUpCurrentObjective(quest);
        }
        else
        {
            _gameState.ActiveQuestsList.List.Remove(quest);
            _gameState.DoneQuestsList.List.Add(quest);
            _gameState.Money += quest.Quest.MoneyReward;
            var notificationMessage = $"{quest.Quest.Questname} beendet.";
            if (quest.Quest.MoneyReward > 0)
            {
                notificationMessage += $" Du hast {quest.Quest.MoneyReward}€ verdient.";
            }
            UIEvents.AddNotification(new("Quest beendet", notificationMessage));
            GameState.UpdateHUD?.Invoke();
        }
        MessageSystem.FireEvent(MessageSystemEvent.ObjectiveFinished);
    }

    private void SetUpCurrentObjective(QuestWithObjectiveIndex quest)
    {
        UnityAction action = () => FinishObjective(quest);
        _objectiveActions[quest.Quest.Objectives[quest.ObjectiveIndex]] = action;
        MessageSystem.StartListening(quest.Quest.Objectives[quest.ObjectiveIndex].EventThatFinishesObjective, action);
    }
}
