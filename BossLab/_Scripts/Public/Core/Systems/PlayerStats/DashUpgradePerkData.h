// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "DashUpgradePerkData.generated.h"

USTRUCT(BlueprintType)
struct FDashUpgradePerkData
{
	GENERATED_BODY()
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int UnlockCost = 1;
};