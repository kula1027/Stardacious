public enum MonsterType{
	NotInitialized,
	Spider,
	Walker,
	Fly
}

public enum ProjType{
	MiniGunBullet,
	HeavyMine,
	ChaserBullet,
	GuidanceDevice,
	BindBullet,
	EnergyBall,
	SpiderBullet,
	FlyBullet,
	WalkerBullet,
	RecallBullet,
	EffectIce,
	EffectPortalIn,
	EffectPortalOut
}

public enum ChIdx{
	NotInitialized,
	Heavy,
	Doctor,
	Esper
}
	
public static class NetworkConst{
	public const int maxPlayer = 3;
	public const float chPosSyncTime = 0.05f;
	public const float projPosSyncTime = 0.1f;
}
	
public enum GameState{
	Waiting,
	Ready,
	Playing,
	Empty
}
