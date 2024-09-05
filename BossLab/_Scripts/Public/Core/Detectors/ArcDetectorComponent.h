// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "DetectorSystem.h"
#include "Components/ActorComponent.h"
#include "CustomCollisionChannels.h"
#include "ArcDetectorComponent.generated.h"

#define DETECTOR_CONFIG "DetectorConfig"

UCLASS(ClassGroup=(Custom), meta=(BlueprintSpawnableComponent))
class BOSSLAB_API UArcDetectorComponent : public UDetectorSystem
{
	GENERATED_BODY()

public:
	// Sets default values for this component's properties
	UArcDetectorComponent();

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = DETECTOR_CONFIG)
	float Range = 100.f;
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = DETECTOR_CONFIG)
	float SectorAngle = 60.f;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = DETECTOR_CONFIG)
	bool EnableDebug = true;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = DETECTOR_CONFIG)
	TArray<TEnumAsByte<ECollisionChannel>> TargetCollisionChannels;
	// temp additions
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category=DETECTOR_CONFIG)
	FVector DetectionStartPoint;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category=DETECTOR_CONFIG)
	bool DetectFromActorLocation = true;

private:
	bool IsInSector(const FVector& PlayerPosition, const FVector& SectorCenter, float SectorAngle, float SectorRadius,
	                const FVector& EnemyPosition);

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType,
	                           FActorComponentTickFunction* ThisTickFunction) override;

	// UFUNCTION(BlueprintCallable)
	// void ActivateArc();
	UFUNCTION(BlueprintCallable)
	void ActivateArc(float _Range, float _Angle);
	// UFUNCTION(BlueprintCallable)
	// void ActivateArc(FVector Location);
};
