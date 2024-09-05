// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "ResizableRadialIndicator.generated.h"

UCLASS()
class BOSSLAB_API AResizableRadialIndicator : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	AResizableRadialIndicator();

protected:
	virtual void BeginPlay() override;

public:
	virtual void Tick(float Deltatime) override;
	UFUNCTION(BlueprintCallable)
	void SetSize(float Size);
	UFUNCTION(BlueprintCallable)
	void StartFillingProcess(float Duration);
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	UStaticMeshComponent* InnerCircleMesh;
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	UStaticMeshComponent* OuterCircleMesh;
	UMaterialInstanceDynamic* CircleMaterial;
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FColor InnerCircleColor;
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FColor OuterCircleColor;

protected:

private:
	FName M_SizePropertyName = "Radius";
	FName M_ColorPropertyName = "FinalColor";
	float M_RadiusStartValue = 0.0f;
	float M_RadiusEndValue = 0.0f;
	float M_TimeElapsed = 0.0f;
	float M_SizingDuration = 0.0f;
	void UpdateRadius();
	bool M_Update = false;
};
