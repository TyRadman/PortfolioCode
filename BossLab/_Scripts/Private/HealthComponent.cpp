// Fill out your copyright notice in the Description page of Project Settings.


#include "HealthComponent.h"
#include "EntityTag.h"
#include "EntityHealthBar.h"

// Sets default values for this component's properties
UHealthComponent::UHealthComponent()
{
	PrimaryComponentTick.bCanEverTick = true;
}

// Called when the game starts
void UHealthComponent::BeginPlay()
{
	Super::BeginPlay();
}

// Called every frame
void UHealthComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);
	
	OnHealthChanged.Broadcast(M_CurrentHP, M_BaseHP);
}

void UHealthComponent::TakeDamage(float DamageAmount)
{
	M_CurrentHP -= DamageAmount;
	OnHealthChanged.Broadcast(M_CurrentHP, M_BaseHP);
	
	if(M_CurrentHP <= 0)
	{
		M_CurrentHP = 0;
		OnDeath.Broadcast();
	}
}

void UHealthComponent::SetBaseHealth(float BaseHealth, bool ShouldSetCurrentHealth)
{
	M_BaseHP = BaseHealth;
	if(ShouldSetCurrentHealth)
	{
		M_CurrentHP = BaseHealth;
	}	
	OnHealthChanged.Broadcast(M_CurrentHP, M_BaseHP);
}

void UHealthComponent::AddToBaseHealth(float Amount)
{
	M_BaseHP += Amount;
	M_CurrentHP = M_BaseHP;
	OnHealthChanged.Broadcast(M_CurrentHP, M_BaseHP);
}

void UHealthComponent::AddToCurrentHealth(float Amount)
{
	M_CurrentHP = FMath::Clamp(M_CurrentHP + Amount, 0.0f, M_BaseHP);
	OnHealthChanged.Broadcast(M_CurrentHP, M_BaseHP);
}

void UHealthComponent::SetCurrentHealth(float CurrentHealth, float BaseHealth)
{
	M_BaseHP = BaseHealth;
	M_CurrentHP = FMath::Clamp(CurrentHealth, 0.0f, M_BaseHP);
	OnHealthChanged.Broadcast(M_CurrentHP, M_BaseHP);
}

float UHealthComponent::GetCurrentHealth()
{
	return M_CurrentHP;
}


float UHealthComponent::GetBaseHealth()
{
	return M_BaseHP;
}
