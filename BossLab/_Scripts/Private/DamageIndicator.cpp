// Fill out your copyright notice in the Description page of Project Settings.


#include "DamageIndicator.h"
#include "Engine/World.h"
#include "Kismet/GameplayStatics.h"
#include "Components/StaticMeshComponent.h"

UDamageIndicator::UDamageIndicator()
{
	PrimaryComponentTick.bCanEverTick = false;
	// FillAmountParameterName = "Percent"; // Name of the parameter in your material instance
}

// Called when the game starts
void UDamageIndicator::BeginPlay()
{
	Super::BeginPlay();
	SetDynamicMaterial();

	if(DynamicMaterialInstance)
	{
		DynamicMaterialInstance->SetVectorParameterValue(ColorPropertyName, IndicatorColor);
	}
	else
	{
		GEngine->AddOnScreenDebugMessage(-1, 10.f, FColor::Blue, FString::Printf(TEXT("Failed to set color")));
	}
}

void UDamageIndicator::SetDynamicMaterial()
{
	if (IndicatorMesh)
	{
		UMaterialInterface* Material = IndicatorMesh->GetMaterial(0);
		
		if (Material)
		{
			DynamicMaterialInstance = IndicatorMesh->CreateAndSetMaterialInstanceDynamicFromMaterial(0, Material);
			GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Red, FString::Printf(TEXT("Success")));
		}
		else
		{
			GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Red, FString::Printf(TEXT("Failed to assign Dynamic material")));
		}
	}
}


void UDamageIndicator::SetIndicatorDimensions(float Angle, float Size)
{
	if (DynamicMaterialInstance)
	{
		float NormalizedValue = FMath::Clamp(Angle / 360.0f, 0.0f, 1.0f);
		DynamicMaterialInstance->SetScalarParameterValue(FillAmountParameterName, NormalizedValue);
		// setting the size
		FVector vecotrOne(1.f, 1.f, 1.f);
		FVector sizeVector = vecotrOne * Size;
		IndicatorMesh->SetWorldScale3D(sizeVector);
	}
}

void UDamageIndicator::ShowIndicator()
{
	if (IndicatorMesh)
	{
		IndicatorMesh->SetVisibility(true);
	}
}

void UDamageIndicator::HideIndicator()
{
	if (IndicatorMesh)
	{
		IndicatorMesh->SetVisibility(false);
	}
}