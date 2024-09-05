// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Engine/DataAsset.h"
#include "Core/Projectile.h"
#include "Core/Systems/DamageSystem/SDamageInfo.h"
#include "Core/EProjectilePatternType.h"
#include "ShootingAttackData.generated.h"

/**
 * 
 */
UCLASS(BlueprintType)
class BOSSLAB_API UShootingAttackData : public UDataAsset
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="References")
	TSubclassOf<AProjectile> ProjectileReference;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Settings")
	FString DataName;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Settings")
	bool Additive = true;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Settings")
	float Duration = 1.0f;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Settings")
	int ShotsNumber = 3;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Settings")
	float ProjectileSpeed = 1200.0f;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Settings")
	FSDamageInfo DamageInfo;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Settings")
	UEProjectilePatternType PatternType = UEProjectilePatternType::Split;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Split Shot", meta = (EditCondition = "PatternType == UEProjectilePatternType::Split", EditConditionHides))
	float Angle = 90.0f;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Split Shot", meta = (EditCondition = "PatternType == UEProjectilePatternType::Split", EditConditionHides))
	int SplitsCount = 3;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Parallel Shot", meta = (EditCondition = "PatternType == UEProjectilePatternType::Parallel", EditConditionHides))
	float HorizontalDistance = 1000.0f;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Parallel Shot", meta = (EditCondition = "PatternType == UEProjectilePatternType::Parallel", EditConditionHides))
	int ParallelCount = 3;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Modifiers")
	bool IsAreaOfEffect;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Modifiers", meta = (EditCondition = "IsAreaOfEffect == true", EditConditionHides))
	float AreaOfEffectRadius = 0.0f;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Modifiers")
	bool IsHoming;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Modifiers", meta = (EditCondition = "IsHoming == true", EditConditionHides))
	float Accuracy = 0.0f;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Modifiers")
	bool IsRicochet;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Modifiers", meta = (EditCondition = "IsRicochet == true", EditConditionHides))
	int DeflectionsCount = 0;

public:
	UFUNCTION(BlueprintCallable)
	void ResetValues();
};
