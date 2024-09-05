// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "AIController.h"
#include "AIC_BossAIController.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "BossManager.generated.h"

/**
 * 
 */
UCLASS()
class BOSSLAB_API UBossManager : public UGameInstanceSubsystem
{
	GENERATED_BODY()
    
public:
	// Sets the boss actor
	UFUNCTION(BlueprintCallable, Category = "Boss")
	void SetBoss(AActor* NewBoss);

	// Gets the boss actor
	UFUNCTION(BlueprintCallable, Category = "Boss")
	AActor* GetBoss() const;
	
	UFUNCTION(BlueprintCallable, Category = "Boss")
	AAIC_BossAIController* GetBossAIController() const;


private:
	// Holds the boss actor
	UPROPERTY()
	AActor* Boss;
	AAIC_BossAIController* BossAIController;
};
