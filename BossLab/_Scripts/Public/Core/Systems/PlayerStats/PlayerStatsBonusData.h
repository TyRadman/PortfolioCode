// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PlayerStatsBonusData.generated.h"
/**
 * 
 */
// TODO: To deprecate
USTRUCT(BlueprintType)
struct FPlayerStatsBonusData
{
	GENERATED_BODY()
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	float BonusPercentValue;

	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	float FinalValue;

	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	float MultiplierValue;
};

