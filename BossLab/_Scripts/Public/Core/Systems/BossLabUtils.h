// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "UObject/NoExportTypes.h"
#include "BossLabUtils.generated.h"

/**
 * 
 */
UCLASS()
class BOSSLAB_API UBossLabUtils : public UObject
{
GENERATED_BODY()
	
public:
	UFUNCTION(BlueprintCallable)
	static void Print(const FString message);
	// overloads are only for blueprints
	static void Print(const FString message, float duration);
	static void Print(const FString message, const FColor color);
	static void Print(const FString message, float duration, const FColor color);
};
