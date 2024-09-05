// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "ShootingAttackData.h"
#include "Components/ArrowComponent.h"
#include "Shooter.generated.h"


UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class BOSSLAB_API UShooter : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UShooter();
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;
	UFUNCTION(BlueprintCallable, Category="Shooter")
	void Shoot(UShootingAttackData* data = nullptr);
	UFUNCTION(BlueprintCallable, Category="Shooter")
	void SetShotData(UShootingAttackData* data);

protected:
	// Called when the game starts
	virtual void BeginPlay() override;
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	UArrowComponent* SpawnPoint;

public:	
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	UShootingAttackData* CurrentData;
	

private:
	bool M_IsShooting = false;
	float M_TimeElapsed = 0.0f;
	float M_TimeBetweenShots = 1.0f;
	int32 M_ProjectilesShotCount = 0;
	void ShootProjectileProcess();
	void SpawnSplitProjectiles();
	void SpawnParallelProjectiles();
	void SpawnSingleProjectile(float angle = 0.0f, FVector location = FVector::Zero());
};