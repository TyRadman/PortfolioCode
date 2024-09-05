// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "HealthStatData.generated.h"

USTRUCT(BlueprintType)
struct FHealthStatData
{
	GENERATED_BODY()
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int UpgradeCost = 1;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	float BaseHealth = 100;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	float HealthBonusFactor = 1.17;
};