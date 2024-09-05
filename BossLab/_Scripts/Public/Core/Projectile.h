// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Core/Systems/DamageSystem/IDamageSystemInterface.h"
#include "Core/Systems/DamageSystem/SDamageInfo.h"
#include "GameFramework/ProjectileMovementComponent.h"
#include "Projectile.generated.h"

UCLASS()
class BOSSLAB_API AProjectile : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	AProjectile();
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "AOE")
	bool IsAOE = false;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "AOE")
	float AreaOfEffectRadius = 10.0f;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "AOE", meta=(Tooltip = "The scale of the impact particles will be the radius of the AOE multiplied by this number."))
	float ImpactEffectScaleMultiplier = 0.3f;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Ricochet")
	bool IsRicochet = false;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Ricochet")
	int DeflectionsCount = 1;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Home")
	bool IsHoming = false;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Home")
	float Accuracy = 1.0f;

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Settings")
	FSDamageInfo DamageData;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Settings")
	float ProjectileSpeed = 500.0f;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Settings")
	UProjectileMovementComponent* ProjectileMovementReference;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "References")
	TSubclassOf<AActor> ImpactEffect;



public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;
	UFUNCTION(BlueprintCallable)
	void OnProjectileHit(UPrimitiveComponent* HitComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, FVector NormalImpulse, const FHitResult& Hit);

	UFUNCTION(BlueprintCallable)
	void SetProjectileSettings(UShootingAttackData* data, FRotator rotation);

private:
	void OnImpact(const FHitResult& Hit);
	void OnAOEImpact();
	void OnRicochetImpact(const FHitResult& Hit);
	void PerformHoming();
	AActor* M_HomingTarget;
	
};
