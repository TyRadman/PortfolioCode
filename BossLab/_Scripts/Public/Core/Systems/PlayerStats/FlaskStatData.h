// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "FlaskStatData.generated.h"

USTRUCT(BlueprintType)
struct FFlaskStatData
{
	GENERATED_BODY()
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int UpgradeCost = 1;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int BaseFlaskAmount = 1;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	float HealFactor = 0.25;
};
