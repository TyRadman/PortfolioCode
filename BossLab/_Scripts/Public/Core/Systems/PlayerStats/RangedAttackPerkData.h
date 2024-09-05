// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "RangedAttackPerkData.generated.h"

USTRUCT(BlueprintType)
struct FRangedAttackPerkData
{
	GENERATED_BODY()
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int UnlockCost = 1;
};