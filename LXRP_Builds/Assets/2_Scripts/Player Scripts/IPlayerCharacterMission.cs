using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerCharacterMission
{
    void StartMission();

    void CompleteMission();

    void UpdateMission();

    bool IsMissionComplete();
}
