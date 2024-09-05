// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "Materials/MaterialInstanceDynamic.h"
#include "DamageIndicator.generated.h"


UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class BOSSLAB_API UDamageIndicator : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UDamageIndicator();

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:    
	// Sets the angle of the indicator
	UFUNCTION(BlueprintCallable, Category = "Damage Indicator")
	void SetIndicatorDimensions(float Angle, float Size);

	// Displays the indicator
	UFUNCTION(BlueprintCallable, Category = "Damage Indicator")
	void ShowIndicator();

	// Hides the indicator
	UFUNCTION(BlueprintCallable, Category = "Damage Indicator")
	void HideIndicator();
	
	UFUNCTION(BlueprintCallable, Category = "Damage Indicator")
	void SetDynamicMaterial();

	// Reference to the mesh component to be used for the indicator
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Damage Indicator")
	UStaticMeshComponent* IndicatorMesh;
	// UPROPERTY(EditAnywhere)
	UMaterialInstanceDynamic* DynamicMaterialInstance;

protected:
	UPROPERTY(EditAnywhere)
	FColor IndicatorColor;

private:
	FName FillAmountParameterName = "Percent";
	FName ColorPropertyName = "FinalColor";

		
};
