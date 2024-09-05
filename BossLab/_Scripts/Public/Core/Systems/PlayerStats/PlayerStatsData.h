// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PlayerStatsData.generated.h"

/**
 * 
 */
USTRUCT(BlueprintType)
struct FPlayerStatsData
{
	GENERATED_BODY()
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	int MaxComboStats;

	// Stats Data

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category="StatData|BaseStats")
	float DamageBonusFactor = 1;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category="StatData|BaseStats")
	float HealthBonusFactor = 1;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category="StatData|BaseStats")
	float BaseHealth = 100;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category="StatData|BaseStats")
	float BaseActionPoints = 3;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category="StatData|BaseStats")
	float InitialModPoints = 10;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category="StatData|BaseStats")
	float InitialStatPoints = 10;

	// Cost Data
	
	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category="StatData|UpgradeCosts")
	float QuickerDashUnlockCost = 3;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category="StatData|UpgradeCosts")
	float ChargeAttackUnlockCost = 3;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category="StatData|UpgradeCosts")
	float HealthStatUpgradeCost = 1;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category="StatData|UpgradeCosts")
	float DamageStatUpgradeCost = 1;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category="StatData|UpgradeCosts")
	float ActionStatUpgradeCost = 1;
	
};
