// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#define HEALTH "Health"

#include "CoreMinimal.h"
#include "EntityTag.h"
#include "Components/ActorComponent.h"
#include "HealthComponent.generated.h"

class UEntityHealthBar;

DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FOnHealthChanged, float, CurrentHealth, float, BaseHealth);
DECLARE_DYNAMIC_MULTICAST_DELEGATE(FOnDeath);


UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class BOSSLAB_API UHealthComponent : public UActorComponent
{
	GENERATED_BODY()

private:
	UPROPERTY(EditAnywhere, Category = HEALTH)
	float M_BaseHP;
	UPROPERTY(EditAnywhere, Category=HEALTH)
	float M_CurrentHP;

	UPROPERTY(EditAnywhere, Category="References")
	UEntityHealthBar* M_HealthBar;
	
public:	
	// Sets default values for this component's properties
	UHealthComponent();
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	EEntityTag M_EntityTag;

	UPROPERTY(BlueprintAssignable, Category = "Events")
	FOnHealthChanged OnHealthChanged;

	UPROPERTY(BlueprintAssignable, Category = "Events")
	FOnDeath OnDeath;

	UFUNCTION(BlueprintCallable, Category=HEALTH_CATEGORY_NAME)
	void TakeDamage(float DamageAmount);
	UFUNCTION(BlueprintCallable, Category=HEALTH_CATEGORY_NAME)
	void SetBaseHealth(float BaseHealth, bool ShouldSetCurrentHealth = true);
	UFUNCTION(BlueprintCallable, Category=HEALTH_CATEGORY_NAME)
	void AddToBaseHealth(float Amount);
	UFUNCTION(BlueprintCallable, Category=HEALTH_CATEGORY_NAME)
	float GetBaseHealth();
	UFUNCTION(BlueprintCallable, Category=HEALTH_CATEGORY_NAME)
	void AddToCurrentHealth(float Amount);
	UFUNCTION(BlueprintCallable, Category=HEALTH_CATEGORY_NAME)
	void SetCurrentHealth(float CurrentHealth, float BaseHealth);
	UFUNCTION(BlueprintCallable, Category=HEALTH_CATEGORY_NAME)
	float GetCurrentHealth();

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

		
};
