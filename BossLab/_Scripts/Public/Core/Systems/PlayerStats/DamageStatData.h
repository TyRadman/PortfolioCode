// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "DamageStatData.generated.h"

USTRUCT(BlueprintType)
struct FDamageStatData
{
	GENERATED_BODY()
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int UpgradeCost = 1;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	float DamageBonusFactor = 1.05;
};