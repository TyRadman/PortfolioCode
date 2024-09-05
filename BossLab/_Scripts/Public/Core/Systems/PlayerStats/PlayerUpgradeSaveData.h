// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PlayerUpgradeSaveData.generated.h"

USTRUCT(BlueprintType)
struct FPlayerUpgradeSaveData
{
	GENERATED_BODY()
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int StatPoints;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int DamageStatPoints;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int HealthStatPoints;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int FlaskStatPoints;
	
	// Add ability unlock flags here!
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	bool IsDashPerkUpgraded;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	bool IsRangedPerkUpgraded;
	
};