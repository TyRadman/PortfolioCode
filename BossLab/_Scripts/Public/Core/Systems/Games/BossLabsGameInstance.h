// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Engine/GameInstance.h"
#include "BossLabsGameInstance.generated.h"

/**
 * 
 */
UCLASS(Blueprintable, Abstract)
class BOSSLAB_API UBossLabsGameInstance : public UGameInstance
{
	GENERATED_BODY()
	
public:
	virtual void Init() override;
	/** Opportunity for blueprints to handle the game instance being initialized after the subsystems have been initiatlized. */
	UFUNCTION(BlueprintImplementableEvent, meta=(DisplayName = "PostSubsystemInit"))
	void ReceiveInitPostSubsystem();
};

