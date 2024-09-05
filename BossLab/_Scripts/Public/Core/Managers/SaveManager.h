// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Core/Systems/SessionSave/SessionSaveData.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "SaveManager.generated.h"

UCLASS()
class BOSSLAB_API USaveManager : public UGameInstanceSubsystem
{
	GENERATED_BODY()
private:
	FSessionSaveData _CurrentSessionData; 

public:
	UFUNCTION(BlueprintCallable)
	void SaveGameSessionData(FSessionSaveData data);

	UFUNCTION(BlueprintCallable)
	FSessionSaveData GetGameSessionData();
};
