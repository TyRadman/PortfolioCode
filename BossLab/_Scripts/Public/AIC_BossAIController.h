// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "AIController.h"
#include "BehaviorTree/BlackboardComponent.h"
#include "BehaviorTree/BlackboardData.h"
#include "BehaviourChance.h"
#include "AIC_BossAIController.generated.h"

/**
 * 
 */
UCLASS()
class BOSSLAB_API AAIC_BossAIController : public AAIController
{
	GENERATED_BODY()


private:
	// the name is from the blackboard
	FName M_BehaviourKey = FName(TEXT("ComboToPlay"));
	float M_HighestChance = 0.0f;
	float M_PreviousChance = 0.0f;
	float M_SelectedChance = 0.0f;
	int M_SelectedIndex = 0;
	

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Action Chances")
	TObjectPtr<UBlackboardComponent> BlackBoardComponent;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Action Chances")
	TObjectPtr<UBlackboardData> BlackBoardData;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Action Chances")
	TArray<UBehaviourChance*> BehaviorChances;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Action Chances")
	TArray<UBehaviourChance*> TempBehaviorChances;
	
	UFUNCTION(BlueprintCallable, BlueprintNativeEvent)
	void StartBoss();
	virtual void StartBoss_Implementation();
	
	UFUNCTION(BlueprintCallable)
	virtual void StartTheBoss();
	
	UFUNCTION(BlueprintCallable)
	void SetNextState(AttackRangeType rangeType);

	UFUNCTION(BlueprintCallable)
	void SetUpActionChances();
	
	UFUNCTION(BlueprintCallable)
	void AddActionChance(UBehaviourChance* newAction);
};
