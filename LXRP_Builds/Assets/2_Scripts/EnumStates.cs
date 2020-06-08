// Data storage script for all game Enumeration types and values
public enum EGameState
{
    BLANK,
    BEGIN,
    PLACED,
    LEVEL2_START,
    PLAYER_START,
    QUEST_START,
    QUEST_COMPLETE,
    PLAYER_COMPLETE,
    HIT_BY_CAR,
    HIT_BY_TRAIN,
    GAME_OVER
}

public enum EARState
{
    BLANK,
    TUTORIAL,
    PLACEMENT,
    HELP,
    PLACED
}

public enum ESpawnSelection
{
    PEDESTRIANS,
    VEHICLES,
    PLAYERS,
    RULES,
    DONUTS,
    QUESTION_CHARACTERS
}

public enum EMissionType
{
    GET_TO_STATION,
    FIND_CORRECT_RULES,
    ANSWER_QUESTIONS,
    COLLECT_HOTDOGS,
    COLLECT_DONUTS,
    SHOPPING,
    FIND_MISTAKES
}

public enum EScoreEvent
{
    GAME_START,
    ON_ROAD,
    AT_STATION,
    FOUND_PLAYER,
    CORRECT_QUESTION,
    CORRECT_TRUEORFALSE,
    HIT_BY_TRAIN
}