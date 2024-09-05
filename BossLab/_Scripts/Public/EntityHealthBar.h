// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "Components/TextBlock.h"
#include "EntityTag.h"
#include "EntityHealthBar.generated.h"

class UProgressBar;

UCLASS()
class BOSSLAB_API UEntityHealthBar : public UUserWidget
{
	GENERATED_BODY()
	
public:
	UFUNCTION(BlueprintCallable, Category="Health Bar", meta = (Tooltip = "Updates the progress bar based on the value passed which should range from 0 to 100."))
	void UpdateHealthBar(float CurrentHP, float BaseHP);
	UFUNCTION(BlueprintCallable, Category="Health Bar")
	void ShowHealthBar();
	UFUNCTION(BlueprintCallable, Category="Health Bar")
	void HideHealthBar();
	
protected:
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	UProgressBar* HealthProgressBar;
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	UTextBlock* HealthAmountTextBlock;
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	EEntityTag OwnerEntity;
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	bool ShowTextBlock;
};
