// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Core/Systems/Mods/ModsUpgradeSaveData.h"
#include "Core/Systems/PlayerStats/PlayerUpgradeSaveData.h"
#include "SessionSaveData.generated.h"

USTRUCT(BlueprintType)
struct FSessionSaveData
{
	GENERATED_BODY()
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int CurrentLevel;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	FPlayerUpgradeSaveData PlayerUpgradeData;

	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	FModsUpgradeSaveData ModsUpgradeData;
};