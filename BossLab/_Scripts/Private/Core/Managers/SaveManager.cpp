// Fill out your copyright notice in the Description page of Project Settings.


#include "Core/Managers/SaveManager.h"

void USaveManager::SaveGameSessionData(FSessionSaveData data)
{
	_CurrentSessionData = data;
}

FSessionSaveData USaveManager::GetGameSessionData()
{
	return _CurrentSessionData;
}
