// Fill out your copyright notice in the Description page of Project Settings.


#include "ResizableRadialIndicator.h"

// Sets default values
AResizableRadialIndicator::AResizableRadialIndicator()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
}

// Called when the game starts or when spawned
void AResizableRadialIndicator::BeginPlay()
{
	Super::BeginPlay();

	if(InnerCircleMesh)
	{
		UMaterialInterface* material = InnerCircleMesh->GetMaterial(0);
		// GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, FString::Printf(TEXT("2) There is mesh")));

		if(material)
		{
			// GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, FString::Printf(TEXT("3) There is material")));
			CircleMaterial = UMaterialInstanceDynamic::Create(material, this);
			InnerCircleMesh->SetMaterial(0, CircleMaterial);
			// set the color
			CircleMaterial->SetVectorParameterValue(M_ColorPropertyName, InnerCircleColor);
		}
	}
	
	// set the color for the outer mesh
	if(OuterCircleMesh)
	{
		UMaterialInterface* material = OuterCircleMesh->GetMaterial(0);
		UMaterialInstanceDynamic* dynamicMaterial = nullptr;
		
		if(material)
		{
			dynamicMaterial = UMaterialInstanceDynamic::Create(material, this);
			OuterCircleMesh->SetMaterial(0, dynamicMaterial);
			dynamicMaterial->SetVectorParameterValue(M_ColorPropertyName, OuterCircleColor);
		}
	}
}

void AResizableRadialIndicator::Tick(float Deltatime)
{
	Super::Tick(Deltatime);

	if(M_Update)
	{
		UpdateRadius();
	}
}


void AResizableRadialIndicator::SetSize(float Size)
{
	FVector VectorSize(Size, Size, Size);
	SetActorScale3D(VectorSize);
}

void AResizableRadialIndicator::StartFillingProcess(float Duration)
{
	if(Duration > 0)
	{
		M_TimeElapsed = 0.0f;
		M_SizingDuration = Duration;
		M_RadiusStartValue = 0.0f;
		M_RadiusEndValue = 1.0f;
		M_Update = true;
	}
	else
	{
		GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Red, FString::Printf(TEXT("Failed to start the process")));
	}
}

void AResizableRadialIndicator::UpdateRadius()
{
	M_TimeElapsed += GetWorld()->GetDeltaSeconds();
	float progress = M_TimeElapsed / M_SizingDuration;

	if(progress >= 1.0f)
	{
		progress = 1.0f;
		M_Update = false;
	}

	float currentRadius = FMath::Lerp(M_RadiusStartValue, M_RadiusEndValue, progress);
	CircleMaterial->SetScalarParameterValue(FName(M_SizePropertyName), currentRadius);
}
