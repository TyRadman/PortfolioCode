// Fill out your copyright notice in the Description page of Project Settings.


#include "EntityHealthBar.h"

#include "Components/ProgressBar.h"
#include "Components/TextBlock.h"

void UEntityHealthBar::UpdateHealthBar(float CurrentHP, float BaseHP)
{
	if(HealthProgressBar)
	{
		// update the progress bar value
		float progressBarValue = CurrentHP / BaseHP;
		HealthProgressBar->SetPercent(progressBarValue);

		// update the text element using the current and base HP
		FString currentHPString = FString::SanitizeFloat(CurrentHP, 0);
		FString baseHPString = FString::SanitizeFloat(BaseHP, 0);
		FString stringValue = FString::Printf(TEXT("%s/%s"), *currentHPString, *baseHPString);
		FText textValue = FText::FromString(stringValue);
		HealthAmountTextBlock->SetText(textValue);
	}
}


void UEntityHealthBar::ShowHealthBar()
{
	HealthAmountTextBlock->SetVisibility(ShowTextBlock? ESlateVisibility::Visible : ESlateVisibility::Collapsed);
	HealthProgressBar->SetVisibility(ESlateVisibility::Visible);
}

void UEntityHealthBar::HideHealthBar()
{
	HealthAmountTextBlock->SetVisibility(ESlateVisibility::Collapsed);
	HealthProgressBar->SetVisibility(ESlateVisibility::Collapsed);
}
