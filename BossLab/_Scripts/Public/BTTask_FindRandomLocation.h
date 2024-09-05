// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "BehaviorTree/BehaviorTreeComponent.h"
#include "BehaviorTree/BehaviorTreeTypes.h"
#include "BehaviorTree/Tasks/BTTask_BlackboardBase.h"
#include "GameFramework/Pawn.h"
#include "UObject/UObjectGlobals.h"
// #include "AIController.h"
#include "BTTask_FindRandomLocation.generated.h"

/**
 * 
 */
UCLASS()
class BOSSLAB_API UBTTask_FindRandomLocation : public UBTTask_BlackboardBase
{
	GENERATED_BODY()

// public:
// 	UBTTask_FindRandomLocation(FObjectInitializer const& ObjectInitializer);
// 	EBTNodeResult::Type ExecuteTask(UBehaviorTreeComponent& OwnerComp, uint8* NodeMemory) override;
// 	
//
// protected:
// 	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Search", meta = (AllowPrivateAccess = "true"))
// 	float P_SearchRadius = 1500.0f;
};
