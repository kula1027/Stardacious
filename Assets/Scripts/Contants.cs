public enum MonsterType{
	NotInitialized,
	Spider,
	Walker,
	Fly,
	Kitten
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
	MeteoBullet,
	GuidenceBullet,
	RecallBullet,
	EffectIce,
	EffectRush,
	EffectSlash,
	EffectPortalIn,
	EffectPortalOut
}

public enum ChIdx{
	NotInitialized,
	Heavy,
	Doctor,
	Esper
}

public static class CameraConst{
	public const int defaultCamSize = 13;
	public const float defaultLerpSpd = 0.3f;
}
	
public static class NetworkConst{
	public const int maxPlayer = 3;
	public const float chPosSyncTime = 0.05f;
	public const float projPosSyncTime = 0.1f;
}

public static class SceneName{
	public const string scNameInGame = "scIngame";
	public const string scNameAwake = "scAwake";
	public const string scNameServer = "scServer";
}

public static class MosnterConst{
	public const int monsterBulletDamage = 1;

	public class Spider{
		public const int maxHp = 150;
	}

	public class Walker{
		public const int maxHp = 300;
	}

	public class Fly{
		public const int maxHp = 150;
	}

	public class Kitten{
		public const int maxHp = 300;
	}

	public class Snake{
		public const int maxHp = 2000;
	}
}
	
public static class CharacterConst{
	public const float defaultRespawnTime = 10f;
	public const float incRespawnTimePerDeath = 2f;
	public const float maxRespawnTime = 20f;
	public const int maxHp = 2;

	public static float GetRespawnTime(int dieCount){
		float dieTime = CharacterConst.defaultRespawnTime  + CharacterConst.incRespawnTimePerDeath * dieCount;
		if(dieTime > maxRespawnTime)dieTime = maxRespawnTime;
		return dieTime;
	}

	public class Heavy{
		public const float coolDownSkill0 = 1f;//overcharge
		public const float coolDownSkill1 = 1f;//mine
		public const float coolDownSkill2 = 1f;//swap

		public const int damageShotgun = 220;
		public const int damageShotgunDistDec = 10;//거리 단위당 데미지 감소 정도
		public const int damageOvercharge = 50;
		public const int damageMine = 250;
		public const int forceMine = 800;
		public const int damageMinigun = 40;
		public const float rateMinigun = 0.1f;
	}

	public class Doctor{
		public const float coolDownSkill0 = 1f;
		public const float coolDownSkill1 = 1f;//ice shot
		public const float coolDownSkill2 = 1f;

		public const int damageEnergyBall = 30;
		public const int damageChaserBullet = 40;
		public const int freezeTime = 8;
	}

	public class Esper{
		public const float coolDownSkill0 = 1f;//rush
		public const float coolDownSkill1 = 1f;//shield
		public const float coolDownSkill2 = 1f;//recall

		public const float itvRush = 0.4f;

		public const int damageDash = 50;
		public const int damageNormal = 75;
		public const int damageJumpAttack = 75;
	
		public const int damageRush = 50;
		public const float speedRush = 1f;
		public const float timeRush = 0.33f;
	}
}

public enum GameState{
	Waiting,
	Ready,
	Playing,
	Empty
}
