// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "SDamageInfo.h"
#include "UObject/Interface.h"
#include "IDamageSystemInterface.generated.h"

// This class does not need to be modified.
UINTERFACE(BlueprintType)
class UIDamageSystemInterface : public UInterface
{
	GENERATED_BODY()
};

/**
 * 
 */
class BOSSLAB_API IIDamageSystemInterface
{
	GENERATED_BODY()

	// Add interface functions to this class. This is the class that will be inherited to implement this interface.
	/** Please add a function description */
public:
	UFUNCTION(BlueprintNativeEvent,BlueprintCallable)
	void TakeDamage(FSDamageInfo DamageInfo, bool& WasDamaged);
	UFUNCTION(BlueprintNativeEvent,BlueprintCallable)
	bool IsDead();
};
