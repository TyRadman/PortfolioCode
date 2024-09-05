// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "ActionStatData.generated.h"

USTRUCT(BlueprintType)
struct FActionStatData
{
	GENERATED_BODY()
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int UpgradeCost = 1;

	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int BaseActionPoints = 5;

	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	float BaseActionStatRegenRate = 0.2f;

	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	float PenaltyRegenRate = 5.0f;
};