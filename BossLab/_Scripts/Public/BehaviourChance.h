// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Engine/DataAsset.h"
#include "BehaviorTree/BehaviorTree.h"
#include "AttackRangeType.h"
#include "BehaviourChance.generated.h"

/**
 * 
 */
UCLASS(BlueprintType)
class BOSSLAB_API UBehaviourChance : public UDataAsset
{
	GENERATED_BODY()

public:
	// UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (DisplayName = "Current Chance"))
	float CurrentChance = 0.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (DisplayName = "Chance"))
	float DefaultChance = 0.5f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	AttackRangeType RangeType = AttackRangeType::ShortRange;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TObjectPtr<UBehaviorTree> Behavior;
};
