// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "ModsUpgradeSaveData.generated.h"

USTRUCT(BlueprintType)
struct FModsUpgradeSaveData
{
	GENERATED_BODY()
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int ModPoints;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	TArray<UDataAsset*> ModsList;
};