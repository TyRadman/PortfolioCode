// Fill out your copyright notice in the Description page of Project Settings.


#include "Core/Managers/PlayerManager.h"

#include "Core/Systems/PlayerStats/PlayerStatsBonusData.h"


void UPlayerManager::InitData(UPlayerConfigData* ConfigData)
{
	_ConfigData = ConfigData;
	_CurrentStatsData = ConfigData->StatsData;
}

int UPlayerManager::GetInitialModPoints()
{
	return _CurrentStatsData.InitialModPoints;
}

FPlayerUpgradeSaveData UPlayerManager::GetPlayerUpgradeData()
{
	FPlayerUpgradeSaveData Data = FPlayerUpgradeSaveData();
	Data.StatPoints = _AvailableStats;
	Data.DamageStatPoints = _CurrentDamageStats;
	Data.HealthStatPoints = _CurrentHealthStats;
	Data.FlaskStatPoints = _CurrentFlaskStats;
	Data.IsDashPerkUpgraded = _IsDashPerkUpgraded;
	Data.IsRangedPerkUpgraded = _IsRangedPerkUpgraded;
	
	return Data;
}

void UPlayerManager::SetPlayerUpgradeData()
{
	//GetPlayerUpgradeData();
}


bool UPlayerManager::AddStatPoint()
{
	_AvailableStats++;
	return true;
}

bool UPlayerManager::RemoveStatPoint()
{
	if(_AvailableStats > 0)
	{
		_AvailableStats--;
	}

	if(_AvailableStats == 0)
	{
		return false;
	}
	
	return true;
}

int UPlayerManager::GetStatPoint()
{
	return _AvailableStats;
}


bool UPlayerManager::AddHealthStat()
{
	if(_AvailableStats >= _ConfigData->HealthStatData.UpgradeCost)
	{
		_AvailableStats -= _ConfigData->HealthStatData.UpgradeCost;
		_CurrentHealthStats++;
	}

	if(_AvailableStats == 0)
	{
		return false;
	}
	
	return true;
}

bool UPlayerManager::RemoveHealthStat()
{
	if(_CurrentHealthStats > 0)
	{
		_AvailableStats += _ConfigData->HealthStatData.UpgradeCost;
		_CurrentHealthStats--;
	}

	if(_CurrentHealthStats == 0)
	{
		return false;
	}
	
	return true;
}

int UPlayerManager::GetHealthStat()
{
	return _CurrentHealthStats;
}


bool UPlayerManager::AddDamageStat()
{
	if(_AvailableStats >= _ConfigData->DamageStatData.UpgradeCost)
	{
		_AvailableStats -= _ConfigData->DamageStatData.UpgradeCost;
		_CurrentDamageStats++;
	}

	if(_AvailableStats == 0)
	{
		return false;
	}
	
	return true;
}

bool UPlayerManager::RemoveDamageStat()
{
	if(_CurrentDamageStats > 0)
	{
		_AvailableStats += _ConfigData->DamageStatData.UpgradeCost;
		_CurrentDamageStats--;
	}

	if(_CurrentDamageStats == 0)
	{
		return false;
	}
	
	return true;
}

int UPlayerManager::GetDamageStat()
{
	return _CurrentDamageStats;
}

bool UPlayerManager::AddFlaskStat()
{
	if(_AvailableStats >= _ConfigData->FlaskStatData.UpgradeCost)
	{
		_AvailableStats -= _ConfigData->FlaskStatData.UpgradeCost;
		_CurrentFlaskStats++;
	}

	if(_AvailableStats == 0)
	{
		return false;
	}
	
	return true;
}

bool UPlayerManager::RemoveFlaskStat()
{
	if(_CurrentFlaskStats > 0)
	{
		_AvailableStats += _ConfigData->FlaskStatData.UpgradeCost;
		_CurrentFlaskStats--;
	}

	if(_CurrentFlaskStats == 0)
	{
		return false;
	}
	
	return true;
}

int UPlayerManager::GetFlaskStat()
{
	return _CurrentFlaskStats;
}

int UPlayerManager::GetTotalFlaskCount()
{
	return _CurrentFlaskStats + _ConfigData->FlaskStatData.BaseFlaskAmount;
}

float UPlayerManager::GetFlaskHealAmount()
{
	return ceilf(_ConfigData->FlaskStatData.HealFactor * GetHealthBonusData().FinalValue);
}

float UPlayerManager::GetFlaskHealPercent()
{
	return _ConfigData->FlaskStatData.HealFactor * 100;
}


bool UPlayerManager::AddActionStat()
{
	if(_AvailableStats >= _CurrentStatsData.ActionStatUpgradeCost)
	{
		_AvailableStats -= _CurrentStatsData.ActionStatUpgradeCost;
		_CurrentActionStats++;
	}

	if(_AvailableStats == 0)
	{
		return false;
	}
	
	return true;
}

bool UPlayerManager::RemoveActionStat()
{
	if(_CurrentActionStats > 0)
	{
		_AvailableStats += _CurrentStatsData.ActionStatUpgradeCost;
		_CurrentActionStats--;
	}

	if(_CurrentActionStats == 0)
	{
		return false;
	}
	
	return true;
}

// TODO: Deprecate these.
int UPlayerManager::GetBonusActionStat()
{
	return  _CurrentActionStats;
}

int UPlayerManager::GetTotalActionStat()
{
	return _CurrentStatsData.BaseActionPoints + _CurrentActionStats;
}

int UPlayerManager::GetBaseActionStats()
{
	return _ConfigData->ActionStatData.BaseActionPoints;
}

int UPlayerManager::GetActionStatRegenRate()
{
	return _ConfigData->ActionStatData.BaseActionStatRegenRate;
}


FPlayerStatsBonusData UPlayerManager::GetDamageBonusData(float BaseDamage)
{
	FPlayerStatsBonusData data =  FPlayerStatsBonusData();
	data.MultiplierValue = pow(_ConfigData->DamageStatData.DamageBonusFactor,_CurrentDamageStats);
	data.BonusPercentValue =  ceilf(100 * (data.MultiplierValue-1));
	data.FinalValue =   ceilf(BaseDamage * data.MultiplierValue);

	return data;
}

FPlayerStatsBonusData UPlayerManager::GetHealthBonusData()
{
	FPlayerStatsBonusData data =  FPlayerStatsBonusData();
	data.MultiplierValue = pow(_ConfigData->HealthStatData.HealthBonusFactor,_CurrentHealthStats);
	data.BonusPercentValue =  ceilf(100 * (data.MultiplierValue - 1));
	data.FinalValue =   ceilf(_ConfigData->HealthStatData.BaseHealth * data.MultiplierValue);

	return data;
}





bool UPlayerManager::AddComboStat()
{
	if(_AvailableStats > 0 && _CurrentComboStats < _CurrentStatsData.MaxComboStats)
	{
		_AvailableStats--;
		_CurrentComboStats++;
	}

	if(_AvailableStats == 0 || _CurrentComboStats == _CurrentStatsData.MaxComboStats)
	{
		return false;
	}
	
	return true;
}

bool UPlayerManager::RemoveComboStat()
{
	if(_CurrentComboStats > 0)
	{
		_AvailableStats++;
		_CurrentComboStats--;
	}

	if(_CurrentComboStats == 0)
	{
		return false;
	}
	
	return true;
}

int UPlayerManager::GetComboStat()
{
	return _CurrentComboStats;
}

bool UPlayerManager::EnableRangedAttack(bool Enable)
{
	if(Enable && _AvailableStats >= _ConfigData->RangedAttackPerkData.UnlockCost)
	{
		_IsRangedPerkUpgraded = true;
		_AvailableStats -= _ConfigData->RangedAttackPerkData.UnlockCost;
	}
	else if(_IsRangedPerkUpgraded)
	{
		_AvailableStats += _ConfigData->RangedAttackPerkData.UnlockCost;
		_IsRangedPerkUpgraded = false;
	}
	
	return _IsRangedPerkUpgraded;
}

bool UPlayerManager::IsRangedAttackEnabled()
{
	return _IsRangedPerkUpgraded;
}

bool UPlayerManager::EnableQuickerDash(bool Enable)
{
	if(Enable && _AvailableStats >= _ConfigData->DashUpgradePerkData.UnlockCost)
	{
		_IsDashPerkUpgraded = true;
		_AvailableStats -= _ConfigData->DashUpgradePerkData.UnlockCost;
	}
	else if(_IsQuickerDashUnlocked)
	{
		_AvailableStats += _ConfigData->DashUpgradePerkData.UnlockCost;
		_IsDashPerkUpgraded = false;
	}
	
	return _IsDashPerkUpgraded;
}

bool UPlayerManager::IsQuickerDashEnabled()
{
	return _IsDashPerkUpgraded;
}


void UPlayerManager::Reset()
{
	_AvailableStats = 0;
	_CurrentHealthStats = 0;
	_CurrentDamageStats = 0;
	_CurrentFlaskStats = 0;

	_IsDashPerkUpgraded = false;
	_IsRangedPerkUpgraded = false;
	

	// TODO: Deprecate below;
	_CurrentComboStats = 0;
	_CurrentActionStats = 0;
	_IsChargeAttackUnlocked = false;
	_IsQuickerDashUnlocked = false;
}




