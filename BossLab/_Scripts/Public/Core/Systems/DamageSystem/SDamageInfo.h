// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "EDamageResponse.h"
#include "UObject/NoExportTypes.h"
#include "EDamageType.h"
#include "SDamageInfo.generated.h"

/** Please add a struct description */
USTRUCT(BlueprintType)
struct FSDamageInfo
{
	GENERATED_BODY()
	
	/** Please add a variable description */
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	float Amount;

	/** Please add a variable description */
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	float DamageMultiplier = 1.0f;

	/** Please add a variable description */
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	EDamageType DamageType;

	/** Please add a variable description */
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	EDamageResponse DamageResponse;

	/** Please add a variable description */
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	bool ShouldDamageInvincible;

	/** Please add a variable description */
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	bool ShouldForceInterrupt;

	/** Please add a variable description */
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	bool CanParry;
};
