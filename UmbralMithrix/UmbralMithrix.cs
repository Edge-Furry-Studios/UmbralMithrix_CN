using BepInEx;
using RoR2;
using RoR2.Skills;
using RoR2.Projectile;
using EntityStates.BrotherMonster;
using EntityStates.BrotherMonster.Weapon;
using R2API;
using System;
using System.Linq;
using System.Collections.Generic;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace UmbralMithrix
{
  [BepInPlugin("com.Nuxlar.UmbralMithrix", "UmbralMithrix", "1.8.5")]
  [BepInDependency("com.bepis.r2api.items", BepInDependency.DependencyFlags.HardDependency)]
  [BepInDependency("com.bepis.r2api.prefab", BepInDependency.DependencyFlags.HardDependency)]
  [BepInDependency("com.bepis.r2api.language", BepInDependency.DependencyFlags.HardDependency)]
  [BepInDependency("com.bepis.r2api.content_management", BepInDependency.DependencyFlags.HardDependency)]
  [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]

  public class UmbralMithrix : BaseUnityPlugin
  {
    bool hasfired;
    public int phaseCounter = 0;
    float elapsed = 0;
    float elapsedStorm = 0;
    bool shrineActivated = false;
    public static bool spawnedClone = false;
    public static ItemDef UmbralItem;
    IEnumerable<CharacterBody> mithies = null;

    static GameObject MagmaWorm = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaWormBody.prefab").WaitForCompletion();
    GameObject Throne = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/mdlBrotherThrone.fbx").WaitForCompletion();
    static GameObject ElectricWorm = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElectricWorm/ElectricWormBody.prefab").WaitForCompletion();
    GameObject Mithrix = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherBody.prefab").WaitForCompletion();
    GameObject Obelisk = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/mysteryspace/MSObelisk.prefab").WaitForCompletion();
    GameObject MithrixHurt = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherHurtBody.prefab").WaitForCompletion();
    GameObject BrotherHaunt = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BrotherHaunt/BrotherHauntBody.prefab").WaitForCompletion();
    SpawnCard MithrixCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Brother/cscBrother.asset").WaitForCompletion();
    SpawnCard MithrixHurtCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Brother/cscBrotherHurt.asset").WaitForCompletion();
    GameObject MithrixGlass = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/BrotherGlass/BrotherGlassBody.prefab").WaitForCompletion();
    SpawnCard MithrixGlassCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Junk/BrotherGlass/cscBrotherGlass.asset").WaitForCompletion();
    SpawnCard gildedWurmLazer = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/MagmaWorm/cscMagmaWorm.asset").WaitForCompletion();
    SpawnCard gildedWurmOrbs = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/ElectricWorm/cscElectricWorm.asset").WaitForCompletion();
    GameObject Exploder = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarExploder/LunarExploderBody.prefab").WaitForCompletion();
    GameObject LunarGolem = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarGolem/LunarGolemBody.prefab").WaitForCompletion();
    GameObject LunarWisp = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarWisp/LunarWispBody.prefab").WaitForCompletion();
    static GameObject exploderProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarExploder/LunarExploderShardProjectile.prefab").WaitForCompletion();
    static GameObject golemProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarGolem/LunarGolemTwinShotProjectile.prefab").WaitForCompletion();
    static SkillDef magmaWormBlink = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/MagmaWorm/MagmaWormBodyBlink.asset").WaitForCompletion();
    static SkillDef electricWormBlink = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/ElectricWorm/ElectricWormBodyBlink.asset").WaitForCompletion();
    public static GameObject auriPreSword = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanGoldPreFistProjectile.prefab").WaitForCompletion();
    public static GameObject auriSword = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanGoldFistEffect.prefab").WaitForCompletion();
    static GameObject vagrantOrb = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Vagrant/VagrantCannon.prefab").WaitForCompletion();
    static GameObject vagrantOrbGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Vagrant/VagrantCannonGhost.prefab").WaitForCompletion();
    static GameObject axe = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/moon/mdlBrotherAxe.prefab").WaitForCompletion();
    static GameObject halberd = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/moon/mdlBrotherHalberd.prefab").WaitForCompletion();
    static GameObject hammer = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/moon/mdlBrotherHammer.prefab").WaitForCompletion();
    static GameObject sword = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/moon/mdlBrotherSword.prefab").WaitForCompletion();
    static GameObject noblePhantasmAxe = PrefabAPI.InstantiateClone(axe, "NoblePhantasmAxe");
    static GameObject noblePhantasmHalberd = PrefabAPI.InstantiateClone(halberd, "NoblePhantasmHalberd");
    static GameObject noblePhantasmHammer = PrefabAPI.InstantiateClone(hammer, "NoblePhantasmHammer");
    static GameObject noblePhantasmSword = PrefabAPI.InstantiateClone(sword, "NoblePhantasmSword");
    static GameObject noblePhantasmGroundAxe = PrefabAPI.InstantiateClone(axe, "NoblePhantasmGroundAxe");
    static GameObject noblePhantasmGroundHalberd = PrefabAPI.InstantiateClone(halberd, "NoblePhantasmHalberd");
    static GameObject noblePhantasmGroundHammer = PrefabAPI.InstantiateClone(hammer, "NoblePhantasmGroundHammer");
    static GameObject noblePhantasmGroundSword = PrefabAPI.InstantiateClone(sword, "NoblePhantasmGroundSword");
    public static GameObject noblePhantasm = PrefabAPI.InstantiateClone(vagrantOrb, "NoblePhantasm");
    public static GameObject noblePhantasmGhost = PrefabAPI.InstantiateClone(vagrantOrbGhost, "NoblePhantasmGhost");
    public static List<GameObject> weaponsList = new List<GameObject>() { noblePhantasmAxe, noblePhantasmHalberd, noblePhantasmHammer, noblePhantasmSword };
    public static List<GameObject> weaponsGroundList = new List<GameObject>() { noblePhantasmGroundAxe, noblePhantasmGroundHalberd, noblePhantasmGroundHammer, noblePhantasmGroundSword };
    static GameObject voidling = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabBodyPhase3.prefab").WaitForCompletion();
    static Material preBossMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Brother/matBrotherPreBossSphere.mat").WaitForCompletion();
    static Material arenaWallMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon/matMoonArenaWall.mat").WaitForCompletion();
    static Material stealAuraMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Brother/matBrotherStealAura.mat").WaitForCompletion();
    static Mesh auriPsrMesh = auriSword.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>().mesh;
    static Material auriPsrMat = auriSword.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>().material;

    public void Awake()
    {
      ModConfig.InitConfig(Config);
      AddContent();
      CreateDoppelItem();
      On.RoR2.Run.Start += OnRunStart;
      On.EntityStates.EntityState.Update += SummonOnSprint;
      On.EntityStates.Interactables.MSObelisk.ReadyToEndGame.OnEnter += ReadyToEndGameOnEnter;
      On.EntityStates.Interactables.MSObelisk.ReadyToEndGame.FixedUpdate += ReadyToEndGameFixedUpdate;
      On.RoR2.Stage.Start += StageStart;
      On.RoR2.HealthComponent.TakeDamage += TakeDamage;
      On.RoR2.CharacterMaster.OnBodyStart += CharacterMasterOnBodyStart;
      On.EntityStates.BrotherHaunt.FireRandomProjectiles.OnEnter += FireRandomProjectilesOnEnter;
      On.EntityStates.BrotherHaunt.FireRandomProjectiles.FireProjectile += FireRandomProjectilesFireProjectile;
      On.EntityStates.BrotherMonster.SkyLeapDeathState.OnEnter += SkyLeapDeathStateOnEnter;
      On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.OnEnter += BrotherEncounterPhaseBaseStateOnEnter;
      On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += Phase1OnEnter;
      On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += Phase2OnEnter;
      On.EntityStates.Missions.BrotherEncounter.Phase3.OnEnter += Phase3OnEnter;
      On.EntityStates.Missions.BrotherEncounter.Phase4.OnEnter += Phase4OnEnter;
      On.EntityStates.Missions.BrotherEncounter.BossDeath.OnEnter += BossDeathOnEnter;
      On.EntityStates.FrozenState.OnEnter += FrozenStateOnEnter;
      On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += AddTimedBuff_BuffDef_float;
      On.EntityStates.BrotherMonster.SprintBash.OnEnter += SprintBashOnEnter;
      On.EntityStates.BrotherMonster.WeaponSlam.OnEnter += WeaponSlamOnEnter;
      On.EntityStates.BrotherMonster.WeaponSlam.FixedUpdate += WeaponSlamFixedUpdate;
      On.EntityStates.BrotherMonster.WeaponSlam.OnEnter += CleanupPillar;
      On.EntityStates.BrotherMonster.Weapon.FireLunarShards.OnEnter += FireLunarShardsOnEnter;
      On.EntityStates.BrotherMonster.UltChannelState.FireWave += UltChannelStateFireWave;
      On.EntityStates.BrotherMonster.SpellChannelEnterState.OnEnter += SpellChannelEnterStateOnEnter;
      On.EntityStates.BrotherMonster.SpellChannelExitState.OnEnter += SpellChannelExitStateOnEnter;
      On.EntityStates.BrotherMonster.StaggerEnter.OnEnter += StaggerEnterOnEnter;
      On.EntityStates.BrotherMonster.StaggerExit.OnEnter += StaggerExitOnEnter;
      On.EntityStates.BrotherMonster.StaggerLoop.OnEnter += StaggerLoopOnEnter;
      On.EntityStates.BrotherMonster.TrueDeathState.OnEnter += TrueDeathStateOnEnter;
      Mithrix.GetComponent<EntityStateMachine>().initialStateType = new EntityStates.SerializableEntityStateType(typeof(ThroneSpawnState));
    }

    // Mithrix text <color=#c6d5ff><size=120%>Mithrix: Die.</color></size>

    private void ArenaSetup()
    {
      // -88.4849 491.488 -0.3325 center orb thing
      GameObject arenaHolder = GameObject.Find("HOLDER: Final Arena");
      if (arenaHolder)
      {
        // Disable inner arena pillars and boulders
        Transform innerPillars = arenaHolder.transform.GetChild(0);
        innerPillars.gameObject.SetActive(false);
        Transform arenaBoulders = arenaHolder.transform.GetChild(6);
        arenaBoulders.gameObject.SetActive(false);
        // Add outer pillars and arches
        Transform outerPillars = arenaHolder.transform.GetChild(1);
        for (int i = 0; i < 8; i++)
          outerPillars.GetChild(i).gameObject.SetActive(true);
        for (int i = 8; i < 12; i++)
        {
          Transform weakPillar = outerPillars.GetChild(i);
          for (int idx = 0; idx < 4; idx++)
            weakPillar.GetChild(idx).gameObject.SetActive(true);
        }
      }
      // Activate the unused throne
      GameObject sceneInfo = GameObject.Find("SceneInfo");
      if (sceneInfo)
      {
        Transform missionController = sceneInfo.transform.GetChild(0);
        GameObject phase1Throne = missionController.transform.GetChild(3).GetChild(0).GetChild(3).gameObject;
        phase1Throne.SetActive(true);
      }
    }
    private void SetupNoblePhantasm()
    {
      foreach (GameObject weapon in weaponsList)
        weapon.transform.localScale = new Vector3(weapon.transform.localScale.x * 2, weapon.transform.localScale.y * 2, weapon.transform.localScale.z * 2);
      ProjectileController noblePhantasmController = noblePhantasm.GetComponent<ProjectileController>();
      noblePhantasm.AddComponent<ProjectileSteerTowardTarget>();
      noblePhantasm.GetComponent<ProjectileSteerTowardTarget>().rotationSpeed = 15;
      noblePhantasm.AddComponent<ProjectileDirectionalTargetFinder>();
      ProjectileDirectionalTargetFinder pdt = noblePhantasm.GetComponent<ProjectileDirectionalTargetFinder>();
      pdt.lookRange = 80;
      pdt.lookCone = 90;
      pdt.allowTargetLoss = true;
      auriPreSword.GetComponent<ProjectileController>().cannotBeDeleted = true;
      noblePhantasmController.ghostPrefab = noblePhantasmGhost;
      noblePhantasmController.GetComponent<ProjectileController>().cannotBeDeleted = true;
    }

    private void SetupGroundPhantasm()
    {
      ParticleSystemRenderer psr = auriSword.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>();
      psr.transform.localScale = new Vector3(10, 10, 10);
      foreach (GameObject weapon in weaponsGroundList)
      {
        MeshFilter weaponMeshFilter = weapon.GetComponent<MeshFilter>();
        weaponMeshFilter.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        CombineInstance combineInstance = new CombineInstance { mesh = weaponMeshFilter.sharedMesh, transform = weaponMeshFilter.transform.localToWorldMatrix };
        CombineInstance[] ciArr = new CombineInstance[] { combineInstance };
        weaponMeshFilter.mesh.CombineMeshes(ciArr, true);
      }
    }

    private void RevertMiscToVanilla()
    {
      // Revert Auri Sword Effect to vanilla
      ParticleSystemRenderer psr = auriSword.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>();
      psr.transform.localScale = new Vector3(1, 1, 1);
      psr.SetMeshes(new Mesh[] { auriPsrMesh }, 1);
      psr.material = auriPsrMat;

      // Revert Voidling to vanilla
      voidling.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void RevertToVanillaStats()
    {
      CharacterBody MithrixBody = Mithrix.GetComponent<CharacterBody>();
      CharacterBody MagmaWormBody = MagmaWorm.GetComponent<CharacterBody>();
      CharacterBody MithrixHurtBody = MithrixHurt.GetComponent<CharacterBody>();
      CharacterDirection MithrixDirection = Mithrix.GetComponent<CharacterDirection>();
      CharacterMotor MithrixMotor = Mithrix.GetComponent<CharacterMotor>();

      MithrixMotor.mass = 900;
      MithrixMotor.airControl = 0.25f;
      MithrixMotor.jumpCount = 1;

      MithrixBody.baseMaxHealth = 1000;
      MithrixBody.levelMaxHealth = 300;
      MithrixBody.baseDamage = 16;
      MithrixBody.levelDamage = 3.2f;

      // Mithrix Hurt
      MithrixHurtBody.baseMaxHealth = 1400;
      MithrixHurtBody.levelMaxHealth = 420;
      MithrixHurtBody.baseArmor = 20;
      MithrixHurtBody.baseDamage = 3;
      MithrixHurtBody.levelDamage = 0.6f;
      MithrixHurtBody.baseMoveSpeed = 4;
      // Mithrix Hurt

      MithrixBody.baseAttackSpeed = 1;
      MithrixBody.baseMoveSpeed = 15;
      MithrixBody.baseAcceleration = 45;
      MithrixBody.baseJumpPower = 25;
      MithrixDirection.turnSpeed = 270;

      MithrixBody.baseArmor = 20;

      ProjectileSteerTowardTarget component = FireLunarShards.projectilePrefab.GetComponent<ProjectileSteerTowardTarget>();
      component.rotationSpeed = 20;
      ProjectileDirectionalTargetFinder component2 = FireLunarShards.projectilePrefab.GetComponent<ProjectileDirectionalTargetFinder>();
      component2.lookRange = 80;
      component2.lookCone = 90;
      component2.allowTargetLoss = true;

      WeaponSlam.duration = 3.5f;
      HoldSkyLeap.duration = 3;
      ExitSkyLeap.waveProjectileCount = 12;
      ExitSkyLeap.recastChance = 0;
      UltChannelState.waveProjectileCount = 8;
      UltChannelState.maxDuration = 8;
      UltChannelState.totalWaves = 4;

      MagmaWormBody.baseMaxHealth = 2400;
      MagmaWormBody.levelMaxHealth = 720;
    }

    private void RevertToVanillaSkills()
    {
      SkillLocator SklLocate = Mithrix.GetComponent<SkillLocator>();
      SkillLocator skillLocator = MithrixHurt.GetComponent<SkillLocator>();
      SkillLocator MagmaWormSkillLocator = MagmaWorm.GetComponent<SkillLocator>();
      SkillLocator ElectricWormSkillLocator = ElectricWorm.GetComponent<SkillLocator>();
      // MithrixHurt
      SkillFamily fireLunarShardsHurt = skillLocator.primary.skillFamily;
      SkillDef fireLunarShardsHurtSkillDef = fireLunarShardsHurt.variants[0].skillDef;
      fireLunarShardsHurtSkillDef.baseRechargeInterval = 6;
      fireLunarShardsHurtSkillDef.baseMaxStock = 12;
      fireLunarShardsHurtSkillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(FireLunarShardsHurt));
      SkillFamily fistSlamHurt = skillLocator.secondary.skillFamily;
      SkillDef fistSlamHurtSkillDef = fistSlamHurt.variants[0].skillDef;
      fistSlamHurtSkillDef.baseRechargeInterval = 8;
      fistSlamHurtSkillDef.baseMaxStock = 1;
      fistSlamHurtSkillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(FistSlam));
      // Mithrix
      SkillFamily Hammer = SklLocate.primary.skillFamily;
      SkillDef HammerChange = Hammer.variants[0].skillDef;
      HammerChange.baseRechargeInterval = 4;
      HammerChange.baseMaxStock = 1;

      SkillFamily Bash = SklLocate.secondary.skillFamily;
      SkillDef BashChange = Bash.variants[0].skillDef;
      BashChange.baseRechargeInterval = 5;
      BashChange.baseMaxStock = 1;

      SkillFamily Dash = SklLocate.utility.skillFamily;
      SkillDef DashChange = Dash.variants[0].skillDef;
      DashChange.baseMaxStock = 2;
      DashChange.baseRechargeInterval = 3;

      SkillFamily Ult = SklLocate.special.skillFamily;
      SkillDef UltChange = Ult.variants[0].skillDef;
      UltChange.baseRechargeInterval = 30;
      UltChange.activationState = new EntityStates.SerializableEntityStateType(typeof(EnterSkyLeap));

      MagmaWormSkillLocator.utility.skillFamily.variants[0].skillDef = magmaWormBlink;
      SkillDef magmaWormBlinkDef = MagmaWormSkillLocator.utility.skillFamily.variants[0].skillDef;
      magmaWormBlinkDef.activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.MagmaWorm.BlinkState));
      magmaWormBlinkDef.baseRechargeInterval = 10f;
      magmaWormBlinkDef.beginSkillCooldownOnSkillEnd = false;
      magmaWormBlinkDef.interruptPriority = EntityStates.InterruptPriority.Skill;
      ElectricWormSkillLocator.utility.skillFamily.variants[0].skillDef = electricWormBlink;

      SkillDef electricWormBlinkDef = ElectricWormSkillLocator.utility.skillFamily.variants[0].skillDef;
      electricWormBlinkDef.activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.MagmaWorm.BlinkState));
      electricWormBlinkDef.baseRechargeInterval = 10f;
      electricWormBlinkDef.beginSkillCooldownOnSkillEnd = false;
      electricWormBlinkDef.interruptPriority = EntityStates.InterruptPriority.Skill;
    }

    private void AdjustBaseStats()
    {
      Logger.LogMessage("Adjusting Phase 1 Stats");
      int playerCount = PlayerCharacterMasterController.instances.Count;
      float hpMultiplier;
      float mobilityMultiplier;
      if (Run.instance.loopClearCount == 1)
      {
        hpMultiplier = (ModConfig.phase1BaseHPScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase1PlayerHPScaling.Value * playerCount);
        mobilityMultiplier = (ModConfig.phase1BaseMobilityScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase1PlayerMobilityScaling.Value * playerCount);
      }
      else
      {
        hpMultiplier = (ModConfig.phase1LoopHPScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase1PlayerHPScaling.Value * playerCount);
        mobilityMultiplier = (ModConfig.phase1LoopMobilityScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase1PlayerMobilityScaling.Value * playerCount);
      }
      CharacterBody MithrixBody = Mithrix.GetComponent<CharacterBody>();
      CharacterDirection MithrixDirection = Mithrix.GetComponent<CharacterDirection>();
      CharacterMotor MithrixMotor = Mithrix.GetComponent<CharacterMotor>();

      CharacterBody MithrixGlassBody = MithrixGlass.GetComponent<CharacterBody>();
      CharacterDirection MithrixGlassDirection = MithrixGlass.GetComponent<CharacterDirection>();
      CharacterMotor MithrixGlassMotor = MithrixGlass.GetComponent<CharacterMotor>();

      MithrixMotor.mass = ModConfig.mass.Value;
      MithrixMotor.airControl = ModConfig.aircontrol.Value;
      MithrixMotor.jumpCount = ModConfig.jumpcount.Value;

      MithrixGlassBody.baseDamage = ModConfig.basedamage.Value / 2;
      MithrixGlassBody.levelDamage = ModConfig.leveldamage.Value / 2;
      MithrixGlassMotor.airControl = ModConfig.aircontrol.Value;
      MithrixGlassDirection.turnSpeed = ModConfig.turningspeed.Value;

      MithrixBody.baseMaxHealth = ModConfig.basehealth.Value + (ModConfig.basehealth.Value * hpMultiplier);
      MithrixBody.levelMaxHealth = ModConfig.levelhealth.Value + (ModConfig.levelhealth.Value * hpMultiplier);
      MithrixBody.baseDamage = ModConfig.basedamage.Value;
      MithrixBody.levelDamage = ModConfig.leveldamage.Value;

      MithrixBody.baseAttackSpeed = ModConfig.baseattackspeed.Value;
      MithrixBody.baseMoveSpeed = ModConfig.basespeed.Value + (ModConfig.basespeed.Value * mobilityMultiplier);
      MithrixBody.baseAcceleration = ModConfig.acceleration.Value + (ModConfig.acceleration.Value * mobilityMultiplier);
      MithrixBody.baseJumpPower = ModConfig.jumpingpower.Value + (ModConfig.jumpingpower.Value * mobilityMultiplier);
      MithrixDirection.turnSpeed = ModConfig.turningspeed.Value + (ModConfig.turningspeed.Value * mobilityMultiplier);

      MithrixBody.baseArmor = ModConfig.basearmor.Value;

      ProjectileSteerTowardTarget component = FireLunarShards.projectilePrefab.GetComponent<ProjectileSteerTowardTarget>();
      component.rotationSpeed = ModConfig.ShardHoming.Value;
      ProjectileDirectionalTargetFinder component2 = FireLunarShards.projectilePrefab.GetComponent<ProjectileDirectionalTargetFinder>();
      component2.lookRange = ModConfig.ShardRange.Value;
      component2.lookCone = ModConfig.ShardCone.Value;
      component2.allowTargetLoss = true;

      WeaponSlam.duration = (3.5f / ModConfig.baseattackspeed.Value);
      HoldSkyLeap.duration = ModConfig.JumpPause.Value;
      ExitSkyLeap.waveProjectileCount = ModConfig.JumpWaveCount.Value;
      ExitSkyLeap.recastChance = ModConfig.JumpRecast.Value;
      UltChannelState.waveProjectileCount = (int)(ModConfig.UltimateWaves.Value);
      UltChannelState.maxDuration = ModConfig.UltimateDuration.Value;
      UltChannelState.totalWaves = ModConfig.UltimateCount.Value;
      ExitSkyLeap.cloneDuration = (int)Math.Round(ModConfig.SpecialCD.Value);
    }

    private void AdjustBaseSkills()
    {
      SkillLocator SklLocate = Mithrix.GetComponent<SkillLocator>();

      SkillFamily Hammer = SklLocate.primary.skillFamily;
      SkillDef HammerChange = Hammer.variants[0].skillDef;
      HammerChange.baseRechargeInterval = ModConfig.PrimCD.Value;
      HammerChange.baseMaxStock = ModConfig.PrimStocks.Value;

      SkillFamily Bash = SklLocate.secondary.skillFamily;
      SkillDef BashChange = Bash.variants[0].skillDef;
      BashChange.baseRechargeInterval = ModConfig.SecCD.Value;
      BashChange.baseMaxStock = ModConfig.SecStocks.Value;

      SkillFamily Dash = SklLocate.utility.skillFamily;
      SkillDef DashChange = Dash.variants[0].skillDef;
      DashChange.baseMaxStock = ModConfig.UtilStocks.Value;
      DashChange.baseRechargeInterval = ModConfig.UtilCD.Value;

      SkillFamily Ult = SklLocate.special.skillFamily;
      SkillDef UltChange = Ult.variants[0].skillDef;
      UltChange.baseRechargeInterval = ModConfig.SpecialCD.Value;
      UltChange.activationState = new EntityStates.SerializableEntityStateType(typeof(EnterCrushingLeap));
    }

    private void AdjustPhase2Stats()
    {
      Logger.LogMessage("Adjusting Phase 2 Stats");
      int playerCount = PlayerCharacterMasterController.instances.Count;
      float hpMultiplier;
      float mobilityMultiplier;
      if (Run.instance.loopClearCount == 1)
      {
        hpMultiplier = (ModConfig.phase2BaseHPScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase2PlayerHPScaling.Value * playerCount);
        mobilityMultiplier = (ModConfig.phase2BaseMobilityScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase2PlayerMobilityScaling.Value * playerCount);
      }
      else
      {
        hpMultiplier = (ModConfig.phase2LoopHPScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase2PlayerHPScaling.Value * playerCount);
        mobilityMultiplier = (ModConfig.phase2LoopMobilityScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase2PlayerMobilityScaling.Value * playerCount);
      }
      Mithrix.GetComponent<EntityStateMachine>().initialStateType = new EntityStates.SerializableEntityStateType(typeof(SkySpawnState));
      CharacterBody MithrixBody = Mithrix.GetComponent<CharacterBody>();
      CharacterDirection MithrixDirection = Mithrix.GetComponent<CharacterDirection>();

      MithrixBody.baseMaxHealth = (ModConfig.basehealth.Value + (ModConfig.basehealth.Value * hpMultiplier)) * 10;
      MithrixBody.levelMaxHealth = (ModConfig.levelhealth.Value + (ModConfig.levelhealth.Value * hpMultiplier)) * 10;

      MithrixBody.baseMoveSpeed = ModConfig.basespeed.Value + (ModConfig.basespeed.Value * mobilityMultiplier);
      MithrixBody.baseAcceleration = ModConfig.acceleration.Value + (ModConfig.acceleration.Value * mobilityMultiplier);
      MithrixBody.baseJumpPower = ModConfig.jumpingpower.Value + (ModConfig.jumpingpower.Value * mobilityMultiplier);
      MithrixDirection.turnSpeed = ModConfig.turningspeed.Value + (ModConfig.turningspeed.Value * mobilityMultiplier);

      WeaponSlam.duration = (3.5f / ModConfig.baseattackspeed.Value);
    }

    private void AdjustPhase3Stats()
    {
      Logger.LogMessage("Adjusting Phase 3 Stats");
      int playerCount = PlayerCharacterMasterController.instances.Count;
      float hpMultiplier;
      float mobilityMultiplier;
      if (Run.instance.loopClearCount == 1)
      {
        hpMultiplier = (ModConfig.phase3BaseHPScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase3PlayerHPScaling.Value * playerCount);
        mobilityMultiplier = (ModConfig.phase3BaseMobilityScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase3PlayerMobilityScaling.Value * playerCount);
      }
      else
      {
        hpMultiplier = (ModConfig.phase3LoopHPScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase3PlayerHPScaling.Value * playerCount);
        mobilityMultiplier = (ModConfig.phase3LoopMobilityScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase3PlayerMobilityScaling.Value * playerCount);
      }
      CharacterBody MithrixBody = Mithrix.GetComponent<CharacterBody>();
      CharacterDirection MithrixDirection = Mithrix.GetComponent<CharacterDirection>();

      MithrixBody.baseMaxHealth = ModConfig.enableSharedHP.Value ? ModConfig.basehealth.Value + (ModConfig.basehealth.Value * hpMultiplier) : (ModConfig.basehealth.Value + (ModConfig.basehealth.Value * hpMultiplier)) / 2;
      MithrixBody.levelMaxHealth = ModConfig.enableSharedHP.Value ? ModConfig.levelhealth.Value + (ModConfig.levelhealth.Value * hpMultiplier) : (ModConfig.levelhealth.Value + (ModConfig.levelhealth.Value * hpMultiplier)) / 2;

      MithrixBody.baseMoveSpeed = ModConfig.basespeed.Value + (ModConfig.basespeed.Value * mobilityMultiplier);
      MithrixBody.baseAcceleration = ModConfig.acceleration.Value + (ModConfig.acceleration.Value * mobilityMultiplier);
      MithrixBody.baseJumpPower = ModConfig.jumpingpower.Value + (ModConfig.jumpingpower.Value * mobilityMultiplier);
      MithrixDirection.turnSpeed = ModConfig.turningspeed.Value + (ModConfig.turningspeed.Value * mobilityMultiplier);

      WeaponSlam.duration = (3.5f / ModConfig.baseattackspeed.Value);
      UltChannelState.waveProjectileCount = ModConfig.UltimateWaves.Value;
      UltChannelState.maxDuration = ModConfig.UltimateDuration.Value;
    }

    private void AdjustPhase4Stats()
    {
      Logger.LogMessage("Adjusting Phase 4 Stats");
      int playerCount = PlayerCharacterMasterController.instances.Count;
      float hpMultiplier;
      if (Run.instance.loopClearCount == 1)
        hpMultiplier = (ModConfig.phase4BaseHPScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase4PlayerHPScaling.Value * playerCount);
      else
        hpMultiplier = (ModConfig.phase4LoopHPScaling.Value * Run.instance.loopClearCount) + (ModConfig.phase4PlayerHPScaling.Value * playerCount);
      CharacterBody MithrixHurtBody = MithrixHurt.GetComponent<CharacterBody>();

      if (ModConfig.doppelPhase4.Value)
      {
        MithrixHurtBody.baseMaxHealth = Run.instance.loopClearCount > 1 ? (ModConfig.basehealth.Value + (ModConfig.basehealth.Value * hpMultiplier)) * 5 : ModConfig.basehealth.Value + (ModConfig.basehealth.Value * hpMultiplier);
        MithrixHurtBody.levelMaxHealth = Run.instance.loopClearCount > 1 ? (ModConfig.levelhealth.Value + (ModConfig.levelhealth.Value * hpMultiplier)) * 5 : ModConfig.levelhealth.Value + (ModConfig.levelhealth.Value * hpMultiplier);
      }
      else
      {
        MithrixHurtBody.baseMaxHealth = ModConfig.basehealth.Value + (ModConfig.basehealth.Value * hpMultiplier);
        MithrixHurtBody.levelMaxHealth = ModConfig.levelhealth.Value + (ModConfig.levelhealth.Value * hpMultiplier);
      }

      MithrixHurtBody.baseMoveSpeed = 0;
      MithrixHurtBody.levelMoveSpeed = 0;
      MithrixHurtBody.baseDamage = ModConfig.basedamage.Value;
      MithrixHurtBody.levelDamage = ModConfig.leveldamage.Value;
      SkillLocator skillLocatorM = MithrixHurt.GetComponent<SkillLocator>();
      SkillFamily fireLunarShardsHurt = skillLocatorM.primary.skillFamily;
      SkillDef fireLunarShardsHurtSkillDef = fireLunarShardsHurt.variants[0].skillDef;
      fireLunarShardsHurtSkillDef.baseRechargeInterval = 7;
      fireLunarShardsHurtSkillDef.baseMaxStock = 1;
      SkillFamily fistSlam = skillLocatorM.secondary.skillFamily;
      SkillDef fistSlamSkillDef = fistSlam.variants[0].skillDef;
      fistSlamSkillDef.baseRechargeInterval = 8.5f;
      fistSlamSkillDef.baseMaxStock = 1;
    }

    private void AddContent()
    {
      // Add our new EntityStates to the game
      ContentAddition.AddEntityState<EnterCrushingLeap>(out _);
      ContentAddition.AddEntityState<AimCrushingLeap>(out _);
      ContentAddition.AddEntityState<ExitCrushingLeap>(out _);
      ContentAddition.AddEntityState<FireWurmLaser>(out _);
      ContentAddition.AddEntityState<FireWurmOrbs>(out _);
      ContentAddition.AddProjectile(noblePhantasm);
    }

    private void CreateDoppelItem()
    {
      LanguageAPI.Add("UMBRALMITHRIX_UMBRAL_ITEM", "起源");
      LanguageAPI.Add("UMBRALMITHRIX_UMBRAL_PICKUP", "仅米斯历克斯>:(");
      LanguageAPI.Add("UMBRALMITHRIX_UMBRAL_DESC", "有趣的本影皮肤");

      LanguageAPI.Add("UMBRALMITHRIX_UMBRAL_SUBTITLENAMETOKEN", "聚合体");
      LanguageAPI.Add("UMBRALMITHRIX_UMBRAL_MODIFIER", "本影");

      UmbralItem = ScriptableObject.CreateInstance<ItemDef>();
      UmbralItem.name = "UmbralMithrixUmbralItem";
      UmbralItem.deprecatedTier = ItemTier.NoTier;
      UmbralItem.nameToken = "UMBRALMITHRIX_UMBRAL_NAME";
      UmbralItem.pickupToken = "UMBRALMITHRIX_UMBRAL_PICKUP";
      UmbralItem.descriptionToken = "UMBRALMITHRIX_UMBRAL_DESC";
      // UmbralItem.tags = new ItemTag[] { ItemTag.WorldUnique, ItemTag.BrotherBlacklist, ItemTag.CannotSteal };
      ItemDisplayRule[] idr = new ItemDisplayRule[0];
      //ContentAddition.AddItemDef(UmbralItem);
      ItemAPI.Add(new CustomItem(UmbralItem, idr));

      On.RoR2.CharacterBody.GetSubtitle += (orig, self) =>
      {
        if (self.inventory && self.inventory.GetItemCount(UmbralItem) > 0 && self.name == "BrotherBody(Clone)")
        {
          return Language.GetString("UMBRALMITHRIX_UMBRAL_SUBTITLENAMETOKEN");
        }
        if (self.inventory && self.inventory.GetItemCount(UmbralItem) > 0 && (self.name == "ElectricWormBody(Clone)" || self.name == "MagmaWormBody(Clone)"))
        {
          return self.subtitleNameToken + Language.GetString("UMBRALMITHRIX_UMBRAL_SUBTITLENAMETOKEN");
        }
        return orig(self);
      };

      On.RoR2.Util.GetBestBodyName += (orig, bodyObject) =>
      {
        string toReturn = orig(bodyObject);
        CharacterBody cb = bodyObject.GetComponent<CharacterBody>();
        if (cb && cb.inventory && cb.inventory.GetItemCount(UmbralItem) > 0)
        {
          toReturn = Language.GetString("UMBRALMITHRIX_UMBRAL_MODIFIER") + toReturn; ;
        }
        return toReturn;
      };


      IL.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += (il) =>
      {
        ILCursor c = new ILCursor(il);
        c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "InvadingDoppelganger")
                    );
        c.Index += 2;
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Func<int, CharacterBody, int>>((vengeanceCount, self) =>
                {
                  int toReturn = vengeanceCount;
                  if (self.inventory)
                  {
                    toReturn += self.inventory.GetItemCount(UmbralItem);
                  }
                  return toReturn;
                });
      };

      IL.RoR2.CharacterModel.UpdateOverlays += (il) =>
      {
        ILCursor c = new ILCursor(il);
        c.GotoNext(
                   x => x.MatchLdsfld(typeof(RoR2Content.Items), "InvadingDoppelganger")
                  );
        c.Index += 2;
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Func<int, CharacterModel, int>>((vengeanceCount, self) =>
              {
                int toReturn = vengeanceCount;
                if (self.body && self.body.inventory)
                {
                  toReturn += self.body.inventory.GetItemCount(UmbralItem);
                }
                return toReturn;
              });
      };
    }

    private void StageStart(On.RoR2.Stage.orig_Start orig, RoR2.Stage self)
    {
      orig(self);
      if (self.sceneDef.cachedName == "moon2")
      {
        ArenaSetup();
        SpawnUmbralObelisk();
      }
    }

    private void SpawnUmbralObelisk()
    {
      // 1090.1f, -283.1f, 1138.6f
      GameObject obelisk = Instantiate(Obelisk, new Vector3(1090.1f, -283.1f, 1138.6f), Quaternion.identity);
      obelisk.GetComponent<PurchaseInteraction>().NetworkcontextToken = "挑衅米斯历克斯？";
      obelisk.name = "UmbralObelisk";
      obelisk.transform.eulerAngles = new Vector3(0.0f, 66f, 0.0f);
      NetworkServer.Spawn(obelisk);
    }

    private void ReadyToEndGameOnEnter(On.EntityStates.Interactables.MSObelisk.ReadyToEndGame.orig_OnEnter orig, EntityStates.Interactables.MSObelisk.ReadyToEndGame self)
    {
      if (self.gameObject.name == "UmbralObelisk")
      {
        self.purchaseInteraction = self.GetComponent<PurchaseInteraction>();
        shrineActivated = true;
        self.purchaseInteraction.Networkavailable = false;
        Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = $"<color=#8826dd>米斯历克斯正等待着...</color>" });
        self.GetComponent<ChildLocator>().FindChild(EntityStates.Interactables.MSObelisk.ReadyToEndGame.chargeupChildString).gameObject.SetActive(true);
        int num = (int)Util.PlaySound(EntityStates.Interactables.MSObelisk.ReadyToEndGame.chargeupSoundString, self.gameObject);
      }
      else
        orig(self);
    }

    private void ReadyToEndGameFixedUpdate(On.EntityStates.Interactables.MSObelisk.ReadyToEndGame.orig_FixedUpdate orig, EntityStates.Interactables.MSObelisk.ReadyToEndGame self)
    {
      if (self.gameObject.name == "UmbralObelisk")
      {
        if ((double)self.fixedAge < (double)EntityStates.Interactables.MSObelisk.ReadyToEndGame.chargeupDuration || self.ready)
          return;
      }
      else
        orig(self);
    }

    private void OnRunStart(On.RoR2.Run.orig_Start orig, Run self)
    {
      shrineActivated = false;
      RevertMiscToVanilla();
      RevertToVanillaStats();
      RevertToVanillaSkills();
      orig(self);
    }

    private void TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, DamageInfo damageInfo)
    {
      orig(self, damageInfo);
      if (shrineActivated && (bool)PhaseCounter.instance && ModConfig.enableSharedHP.Value)
      {
        if (self.body.name == "BrotherBody(Clone)" && PhaseCounter.instance.phase == 3)
        {
          if (mithies == null)
            mithies = Resources.FindObjectsOfTypeAll<CharacterBody>().Where(obj => obj.name == "BrotherBody(Clone)");
          foreach (CharacterBody mithy in mithies)
          {
            if (mithy.netId != self.netId)
              mithy.healthComponent.health = self.health;
          }
        }
      }
    }

    // Prevent freezing from affecting Mithrix after 10 stages or if the config is enabled
    private void FrozenStateOnEnter(On.EntityStates.FrozenState.orig_OnEnter orig, EntityStates.FrozenState self)
    {
      if (shrineActivated)
      {
        if ((self.characterBody.name == "BrotherBody(Clone)" || self.characterBody.name == "BrotherHurtBody(Clone)") && (Run.instance.loopClearCount >= 2 || ModConfig.debuffResistance.Value))
          return;
        if (self.characterBody.name == "BrotherBody(Clone)")
        {
          Ray aimRay = self.GetAimRay();
          for (int i = 0; i < ModConfig.SuperShardWeight.Value; i++)
          {
            Util.PlaySound(EntityStates.BrotherMonster.Weapon.FireLunarShards.fireSound, self.gameObject);
            ProjectileManager.instance.FireProjectile(FireLunarShards.projectilePrefab, aimRay.origin, Quaternion.LookRotation(aimRay.direction), self.gameObject, self.characterBody.damage * 0.1f / 12f, 0f, Util.CheckRoll(self.characterBody.crit, self.characterBody.master), DamageColorIndex.Default, null, -1f);
          }
        }
      }
      orig(self);
    }
    // Prevent tentabauble from affecting Mithrix after 10 stages or if the config is enabled
    private void AddTimedBuff_BuffDef_float(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
    {
      if (shrineActivated)
      {
        if ((self.name == "BrotherBody(Clone)" || self.name == "BrotherHurtBody(Clone)") && buffDef == RoR2Content.Buffs.Nullified && (Run.instance.loopClearCount >= 2 || ModConfig.debuffResistance.Value))
          return;
      }
      orig(self, buffDef, duration);
    }

    private void SummonOnSprint(On.EntityStates.EntityState.orig_Update orig, EntityStates.EntityState self)
    {
      orig(self);
      if (self.characterBody && shrineActivated && !spawnedClone)
      {
        if ((bool)PhaseCounter.instance)
        {
          if (self.characterBody.name == "MiniVoidRaidCrabBodyPhase3(Clone)" && PhaseCounter.instance.phase == 4)
          {
            elapsedStorm += Time.deltaTime;
            if (elapsedStorm >= 1f && self.gameObject.GetComponent<SphereZone>().Networkradius > 75)
            {
              elapsedStorm = elapsedStorm % 1f;
              self.gameObject.GetComponent<SphereZone>().Networkradius -= 2;
            }
          }
          if (self.characterBody.isSprinting && self.characterBody.name == "BrotherBody(Clone)" && PhaseCounter.instance.phase != 3)
          {
            elapsed += Time.deltaTime;
            if (elapsed >= 8f)
            {
              elapsed = elapsed % 8f;
              for (int i = 0; i < PlayerCharacterMasterController.instances.Count; i++)
              {
                DirectorPlacementRule placementRule = new DirectorPlacementRule();
                placementRule.placementMode = PlayerCharacterMasterController.instances[i].bodyMotor.isGrounded ? DirectorPlacementRule.PlacementMode.NearestNode : DirectorPlacementRule.PlacementMode.Direct;
                placementRule.minDistance = 3f;
                placementRule.maxDistance = 10f;
                placementRule.position = PlayerCharacterMasterController.instances[i].bodyMotor.isGrounded ? PlayerCharacterMasterController.instances[i].master.GetBody().corePosition : new Vector3(PlayerCharacterMasterController.instances[i].master.GetBody().corePosition.x + UnityEngine.Random.Range(3f, 20f), PlayerCharacterMasterController.instances[i].master.GetBody().corePosition.y, PlayerCharacterMasterController.instances[i].master.GetBody().corePosition.z + UnityEngine.Random.Range(3f, 20f));
                Xoroshiro128Plus rng = RoR2Application.rng;
                DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(MithrixGlassCard, placementRule, rng);
                directorSpawnRequest.summonerBodyObject = self.gameObject;
                directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)(spawnResult => spawnResult.spawnedInstance.GetComponent<Inventory>().GiveItem(RoR2Content.Items.HealthDecay, 2));
                DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
              }
            }
          }
        }
      }
    }

    private void FireRandomProjectilesOnEnter(On.EntityStates.BrotherHaunt.FireRandomProjectiles.orig_OnEnter orig, EntityStates.BrotherHaunt.FireRandomProjectiles self)
    {
      if (shrineActivated)
      {
        EntityStates.BrotherHaunt.FireRandomProjectiles.chargeRechargeDuration = 0.025f;
        EntityStates.BrotherHaunt.FireRandomProjectiles.chanceToFirePerSecond = 0.01f;
      }
      else
      {
        EntityStates.BrotherHaunt.FireRandomProjectiles.chargeRechargeDuration = 0.5f;
        EntityStates.BrotherHaunt.FireRandomProjectiles.chanceToFirePerSecond = 0.5f;
      }
      orig(self);
    }

    private void FireRandomProjectilesFireProjectile(On.EntityStates.BrotherHaunt.FireRandomProjectiles.orig_FireProjectile orig, EntityStates.BrotherHaunt.FireRandomProjectiles self)
    {
      if (shrineActivated)
      {
        // get random idx to grab a random player
        System.Random r = new System.Random();
        int rIdx = r.Next(0, PlayerCharacterMasterController.instances.Count - 1);
        PlayerCharacterMasterController player = PlayerCharacterMasterController.instances[rIdx];

        Vector3 position = new Vector3(player.body.footPosition.x, 490, player.body.footPosition.z) + new Vector3(UnityEngine.Random.Range(-50f, 50f), 0.0f, UnityEngine.Random.Range(-50f, 50f));

        GameObject prefab = UltChannelState.waveProjectileLeftPrefab;
        if ((double)UnityEngine.Random.value <= 0.5)
          prefab = UltChannelState.waveProjectileRightPrefab;

        int num = 360 / (ModConfig.UltimateWaves.Value / 2);
        Vector3 normalized = Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, Vector3.up).normalized;

        for (int index = 0; index < (ModConfig.UltimateWaves.Value / 2); ++index)
        {
          Vector3 forward = Quaternion.AngleAxis(num * (float)index, Vector3.up) * normalized;
          ProjectileManager.instance.FireProjectile(prefab, position, Util.QuaternionSafeLookRotation(forward), self.gameObject, self.characterBody.damage * UltChannelState.waveProjectileDamageCoefficient, UltChannelState.waveProjectileForce, Util.CheckRoll(self.characterBody.crit, self.characterBody.master));
        }
      }
      else
        orig(self);
    }

    private void CharacterMasterOnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
    {
      orig(self, body);
      if (shrineActivated)
      {
        if ((bool)PhaseCounter.instance)
        {
          if (PhaseCounter.instance.phase == 4 && (body.name == "ElectricWormBody(Clone)" || body.name == "MagmaWormBody(Clone)") && ModConfig.doppelPhase4.Value)
          {
            self.inventory.GiveItemString(UmbralItem.name);
            body.baseNameToken = "蠕虫";
            body.subtitleNameToken = "超载";

            if (body.name == "MagmaWormBody(Clone)")
            {
              CharacterBody electricWurm = Resources.FindObjectsOfTypeAll<CharacterBody>().Where(obj => obj.name == "ElectricWormBody(Clone)").First();
              if ((bool)electricWurm)
              {
                body.baseMaxHealth = electricWurm.baseMaxHealth;
                body.levelMaxHealth = electricWurm.levelMaxHealth;
              }
              SkillDef magmaWormBlinkDef = body.skillLocator.utility.skillFamily.variants[0].skillDef;
              magmaWormBlinkDef.activationState = new EntityStates.SerializableEntityStateType(typeof(FireWurmLaser));
              magmaWormBlinkDef.baseRechargeInterval = (float)ModConfig.WurmLaserCD.Value;
              magmaWormBlinkDef.beginSkillCooldownOnSkillEnd = true;
              magmaWormBlinkDef.interruptPriority = EntityStates.InterruptPriority.Death;
            }
            if (body.name == "ElectricWormBody(Clone)")
            {
              SkillDef electricWormBlinkDef = body.skillLocator.utility.skillFamily.variants[0].skillDef;
              electricWormBlinkDef.activationState = new EntityStates.SerializableEntityStateType(typeof(FireWurmOrbs));
              electricWormBlinkDef.baseRechargeInterval = (float)ModConfig.WurmOrbCD.Value;
              electricWormBlinkDef.beginSkillCooldownOnSkillEnd = true;
              electricWormBlinkDef.interruptPriority = EntityStates.InterruptPriority.Death;
            }
          }
          if ((PhaseCounter.instance.phase == 2 || PhaseCounter.instance.phase == 3) && (body.name == "LunarGolemBody(Clone)" || body.name == "LunarExploderBody(Clone)" || body.name == "LunarWispBody(Clone)"))
            body.healthComponent.Suicide();
          if (body.name == "BrotherBody(Clone)" && spawnedClone && PhaseCounter.instance.phase == 2)
            self.inventory.GiveItemString(UmbralItem.name);
          // Make Mithrix an Umbra
          if ((body.name == "BrotherBody(Clone)" && PhaseCounter.instance.phase == 3) || (body.name == "BrotherHurtBody(Clone)" || body.name == "BrotherGlassBody(Clone)"))
            self.inventory.GiveItemString(UmbralItem.name);
          if (body.name == "BrotherHurtBody(Clone)" && PhaseCounter.instance.phase == 4 && !ModConfig.doppelPhase4.Value)
          {
            body.inventory.GiveItem(UmbralItem);
            body.AddBuff(RoR2Content.Buffs.Immune);
            body.inventory.GiveItem(RoR2Content.Items.HealthDecay, 60);
            body.skillLocator.primary.skillFamily.variants[0].skillDef.interruptPriority = EntityStates.InterruptPriority.Skill;
            body.skillLocator.primary.skillFamily.variants[0].skillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(GateOfBabylon));
            body.skillLocator.secondary.skillFamily.variants[0].skillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(MightOfBabylon));
          }
        }
      }
    }

    private void SkyLeapDeathStateOnEnter(On.EntityStates.BrotherMonster.SkyLeapDeathState.orig_OnEnter orig, EntityStates.BrotherMonster.SkyLeapDeathState self)
    {
      if (shrineActivated)
      {
        if (self.characterBody.name == "BrotherGlassBody(Clone)")
        {
          self.DestroyModel();
          if (!NetworkServer.active)
            return;
          self.DestroyBodyAsapServer();
          return;
        }
        if (self.characterBody.name == "BrotherBody(Clone)" && self.characterBody.inventory.GetItemCount(UmbralItem) > 0)
          spawnedClone = false;
      }
      orig(self);
    }
    private void UltChannelStateFireWave(On.EntityStates.BrotherMonster.UltChannelState.orig_FireWave orig, EntityStates.BrotherMonster.UltChannelState self)
    {
      if (shrineActivated)
      {
        {
          // prevents pizza at Mithrix instead only following each player
          if (PhaseCounter.instance)
          {
            if (PhaseCounter.instance.phase == 3)
            {
              UltChannelState.waveProjectileCount = 0;
              int playerCount = PlayerCharacterMasterController.instances.Count;
              int pizzaLines = ModConfig.UltimateWaves.Value - 2;

              // dividing lines by player count so multiplayer doesn't have unavoidable pizza
              float num = 360f / pizzaLines;
              Vector3 normalized = Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, Vector3.up).normalized;
              GameObject prefab = UltChannelState.waveProjectileLeftPrefab;
              if ((double)UnityEngine.Random.value <= 0.5)
                prefab = UltChannelState.waveProjectileRightPrefab;

              // get random idx to grab a random player
              System.Random r = new System.Random();
              int rIdx = r.Next(0, playerCount - 1);
              PlayerCharacterMasterController player = PlayerCharacterMasterController.instances[rIdx];

              Vector3 position = new Vector3(player.body.footPosition.x, self.characterBody.footPosition.y, player.body.footPosition.z) + new Vector3(UnityEngine.Random.Range(-50f, 50f), 0.0f, UnityEngine.Random.Range(-50f, 50f));
              for (int index = 0; index < pizzaLines; ++index)
              {
                Vector3 forward = Quaternion.AngleAxis(num * (float)index, Vector3.up) * normalized;
                ProjectileManager.instance.FireProjectile(prefab, position, Util.QuaternionSafeLookRotation(forward), self.gameObject, self.characterBody.damage * UltChannelState.waveProjectileDamageCoefficient, UltChannelState.waveProjectileForce, Util.CheckRoll(self.characterBody.crit, self.characterBody.master));
              }
            }
          }
        }
      }
      orig(self);
    }

    // Phase 2 change to encounter spawns (Mithrix instead of Chimera)
    private void BrotherEncounterPhaseBaseStateOnEnter(On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState self)
    {
      if (shrineActivated)
      {
        phaseCounter++;
        // BrotherEncounterBaseState OnEnter
        self.childLocator = self.GetComponent<ChildLocator>();
        Transform child1 = self.childLocator.FindChild("ArenaWalls");
        Transform child2 = self.childLocator.FindChild("ArenaNodes");
        if ((bool)child1)
          child1.gameObject.SetActive(self.shouldEnableArenaWalls);
        if (!(bool)child2)
          return;
        child2.gameObject.SetActive(self.shouldEnableArenaNodes);
        // BrotherEncounterBaseState OnEnter
        if ((bool)PhaseCounter.instance)
        {
          phaseCounter = PhaseCounter.instance.phase;
          PhaseCounter.instance.GoToNextPhase();
        }
        if ((bool)self.childLocator)
        {
          self.phaseControllerObject = self.childLocator.FindChild(self.phaseControllerChildString).gameObject;
          if ((bool)self.phaseControllerObject)
          {
            self.phaseScriptedCombatEncounter = self.phaseControllerObject.GetComponent<ScriptedCombatEncounter>();
            self.phaseBossGroup = self.phaseControllerObject.GetComponent<BossGroup>();
            self.phaseControllerSubObjectContainer = self.phaseControllerObject.transform.Find("PhaseObjects").gameObject;
            self.phaseControllerSubObjectContainer.SetActive(true);
          }
          GameObject gameObject = self.childLocator.FindChild("AllPhases").gameObject;
          if ((bool)gameObject)
            gameObject.SetActive(true);
        }
        self.healthBarShowTime = Run.FixedTimeStamp.now + self.healthBarShowDelay;
        if ((bool)DirectorCore.instance)
        {
          foreach (Behaviour component in DirectorCore.instance.GetComponents<CombatDirector>())
            component.enabled = false;
        }
        if (!NetworkServer.active || self.phaseScriptedCombatEncounter == null)
          return;
        // Make Mithrix spawn for phase 2
        if (phaseCounter == 1)
        {
          Mithrix.transform.position = new Vector3(-88.5f, 491.5f, -0.3f);
          Mithrix.transform.rotation = Quaternion.identity;
          Transform explicitSpawnPosition = Mithrix.transform;
          ScriptedCombatEncounter.SpawnInfo spawnInfoMithrix = new ScriptedCombatEncounter.SpawnInfo
          {
            explicitSpawnPosition = explicitSpawnPosition,
            spawnCard = MithrixCard,
          };
          self.phaseScriptedCombatEncounter.spawns = new ScriptedCombatEncounter.SpawnInfo[] { spawnInfoMithrix };
        }
        if (phaseCounter == 2)
        {
          GameObject emptyGO = new GameObject();
          emptyGO.transform.position = new Vector3(-108.5f, 491.5f, -0.3f);
          emptyGO.transform.rotation = Quaternion.identity;
          Mithrix.transform.position = new Vector3(-88.5f, 491.5f, -0.3f);
          Mithrix.transform.rotation = Quaternion.identity;
          Transform explicitSpawnPosition1 = Mithrix.transform;
          Transform explicitSpawnPosition2 = emptyGO.transform;
          ScriptedCombatEncounter.SpawnInfo spawnInfoMithrix1 = new ScriptedCombatEncounter.SpawnInfo
          {
            explicitSpawnPosition = explicitSpawnPosition1,
            spawnCard = MithrixCard,
          };
          ScriptedCombatEncounter.SpawnInfo spawnInfoMithrix2 = new ScriptedCombatEncounter.SpawnInfo
          {
            explicitSpawnPosition = explicitSpawnPosition2,
            spawnCard = MithrixCard,
          };
          self.phaseScriptedCombatEncounter.spawns = new ScriptedCombatEncounter.SpawnInfo[] { spawnInfoMithrix1, spawnInfoMithrix2 };
        }
        if (phaseCounter == 3 && ModConfig.doppelPhase4.Value)
        {
          GameObject emptyGO = new GameObject();
          GameObject emptyGO2 = new GameObject();
          emptyGO.transform.position = new Vector3(-60f, 520f, -0.3f);
          emptyGO.transform.rotation = Quaternion.identity;
          emptyGO2.transform.position = new Vector3(-120f, 520f, -0.3f);
          emptyGO2.transform.rotation = Quaternion.identity;
          Transform explicitSpawnPosition1 = emptyGO.transform;
          Transform explicitSpawnPosition2 = emptyGO2.transform;
          ScriptedCombatEncounter.SpawnInfo spawnInfoWurm1 = new ScriptedCombatEncounter.SpawnInfo
          {
            explicitSpawnPosition = explicitSpawnPosition1,
            spawnCard = gildedWurmLazer,
          };
          ScriptedCombatEncounter.SpawnInfo spawnInfoWurm2 = new ScriptedCombatEncounter.SpawnInfo
          {
            explicitSpawnPosition = explicitSpawnPosition2,
            spawnCard = gildedWurmOrbs,
          };
          self.phaseScriptedCombatEncounter.spawns = new ScriptedCombatEncounter.SpawnInfo[] { spawnInfoWurm1, spawnInfoWurm2 };
        }
        self.phaseScriptedCombatEncounter.combatSquad.onMemberAddedServer += new Action<CharacterMaster>(self.OnMemberAddedServer);
      }
      else
        orig(self);
    }

    private void Phase1OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
    {
      if (shrineActivated)
      {
        spawnedClone = false;
        mithies = null;
        Logger.LogMessage("Accursing the King of Nothing");
        AdjustBaseSkills();
        AdjustBaseStats();
      }
      orig(self);
    }

    private void Phase2OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase2.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase2 self)
    {
      // Phase 2 Skip
      if (shrineActivated && ModConfig.phase2Skip.Value)
      {
        orig(self);
        self.PreEncounterBegin();
        self.outer.SetNextState((EntityStates.EntityState)new EntityStates.Missions.BrotherEncounter.Phase3());
      }
      // Regular Umbral Phase 2
      else if (shrineActivated)
      {
        self.KillAllMonsters();
        AdjustPhase2Stats();
        orig(self);
      }
      else
        orig(self);
    }

    private void Phase3OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase3.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase3 self)
    {
      if (shrineActivated)
      {
        self.KillAllMonsters();
        AdjustPhase3Stats();
      }
      orig(self);
    }

    private void Phase4OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase4.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase4 self)
    {
      if (shrineActivated)
      {
        AdjustPhase4Stats();

        if (!ModConfig.doppelPhase4.Value)
        {
          SetupGroundPhantasm();
          SetupNoblePhantasm();

          GameObject brotherHauntInstance = Instantiate(BrotherHaunt, new Vector3(-88.5f, 490f, -0.3f), Quaternion.identity);
          brotherHauntInstance.GetComponent<TeamComponent>().teamIndex = (TeamIndex)2;
          NetworkServer.Spawn(brotherHauntInstance);

          // Makes model invisible
          voidling.transform.GetChild(0).gameObject.SetActive(false);
          GameObject voidlingInstance = Instantiate(voidling, new Vector3(-88.5f, 520f, -0.3f), Quaternion.identity);
          voidlingInstance.GetComponent<TeamComponent>().teamIndex = (TeamIndex)2;
          SkillLocator voidlingSkillLocator = voidlingInstance.GetComponent<CharacterBody>().skillLocator;
          voidlingSkillLocator.primary = new GenericSkill();
          voidlingSkillLocator.secondary = new GenericSkill();
          voidlingSkillLocator.utility = new GenericSkill();
          voidlingSkillLocator.special = new GenericSkill();
          List<Material> materials = new List<Material> { preBossMat, arenaWallMat, stealAuraMat };
          Transform sphereIndicator = voidlingInstance.transform.GetChild(1).GetChild(0);
          sphereIndicator.GetComponent<MeshRenderer>().SetMaterials(materials);
          SphereZone sphere = voidlingInstance.GetComponent<SphereZone>();
          sphere.radius = 275;
          FogDamageController fogController = voidlingInstance.GetComponent<FogDamageController>();
          fogController.healthFractionPerSecond = 0.05f;
          fogController.healthFractionRampCoefficientPerSecond = 10f;
          NetworkServer.Spawn(voidlingInstance);
        }
      }
      orig(self);
    }

    private void BossDeathOnEnter(On.EntityStates.Missions.BrotherEncounter.BossDeath.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.BossDeath self)
    {
      orig(self);
      if (shrineActivated)
      {
        GameObject.Find("MiniVoidRaidCrabBodyPhase3(Clone)").GetComponent<HealthComponent>().Suicide();
        GameObject.Find("BrotherHauntBody(Clone)").GetComponent<HealthComponent>().Suicide();
        voidling.transform.GetChild(0).gameObject.SetActive(true);
      }
    }

    // Adds more projectiles to SprintBash in a cone shape
    private void SprintBashOnEnter(On.EntityStates.BrotherMonster.SprintBash.orig_OnEnter orig, SprintBash self)
    {
      if (shrineActivated)
      {
        if (self.isAuthority)
        {
          Ray aimRay = self.GetAimRay();
          for (int i = 0; i < ModConfig.SuperShardWeight.Value; i++)
          {
            Util.PlaySound(EntityStates.BrotherMonster.Weapon.FireLunarShards.fireSound, self.gameObject);
            ProjectileManager.instance.FireProjectile(FireLunarShards.projectilePrefab, aimRay.origin, Quaternion.LookRotation(aimRay.direction), self.gameObject, self.characterBody.damage * 0.1f / 12f, 0f, Util.CheckRoll(self.characterBody.crit, self.characterBody.master), DamageColorIndex.Default, null, -1f);
          }

          if ((bool)PhaseCounter.instance)
          {
            if (self.characterBody.name == "BrotherBody(Clone)")
            {
              if (PhaseCounter.instance.phase != 1 && self.characterBody.inventory.GetItemCount(UmbralItem) == 0)
              {
                Vector3 vector3 = Vector3.ProjectOnPlane(self.inputBank.aimDirection, Vector3.up);
                Vector3 footPosition = self.characterBody.footPosition;
                Vector3 forward = Quaternion.AngleAxis(0, Vector3.up) * vector3;
                ProjectileManager.instance.FireProjectile(WeaponSlam.waveProjectilePrefab, footPosition, Util.QuaternionSafeLookRotation(forward), self.gameObject, self.characterBody.damage * WeaponSlam.waveProjectileDamageCoefficient, WeaponSlam.waveProjectileForce, Util.CheckRoll(self.characterBody.crit, self.characterBody.master));
              }

              if (PhaseCounter.instance.phase != 3 && !spawnedClone)
              {
                DirectorPlacementRule placementRule = new DirectorPlacementRule();
                placementRule.placementMode = DirectorPlacementRule.PlacementMode.NearestNode;
                placementRule.minDistance = 3f;
                placementRule.maxDistance = 40f;
                placementRule.position = self.characterBody.footPosition;
                Xoroshiro128Plus rng = RoR2Application.rng;
                DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(MithrixGlassCard, placementRule, rng);
                directorSpawnRequest.summonerBodyObject = self.gameObject;
                directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)(spawnResult => spawnResult.spawnedInstance.GetComponent<Inventory>().GiveItem(RoR2Content.Items.HealthDecay, 4));
                DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
              }
            }
          }
        }
      }
      orig(self);
    }

    private void WeaponSlamOnEnter(On.EntityStates.BrotherMonster.WeaponSlam.orig_OnEnter orig, WeaponSlam self)
    {
      if (shrineActivated)
      {
        GameObject projectilePrefab = WeaponSlam.pillarProjectilePrefab;
        projectilePrefab.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
        projectilePrefab.GetComponent<ProjectileController>().ghostPrefab.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
        hasfired = false;
        if ((bool)PhaseCounter.instance)
        {

          if (self.characterBody.name == "BrotherBody(Clone)" && PhaseCounter.instance.phase != 3 && !spawnedClone)
          {
            DirectorPlacementRule placementRule = new DirectorPlacementRule();
            placementRule.placementMode = DirectorPlacementRule.PlacementMode.NearestNode;
            placementRule.minDistance = 3f;
            placementRule.maxDistance = 40f;
            placementRule.position = self.characterBody.footPosition;
            Xoroshiro128Plus rng = RoR2Application.rng;
            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(MithrixGlassCard, placementRule, rng);
            directorSpawnRequest.summonerBodyObject = self.gameObject;
            directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)(spawnResult => spawnResult.spawnedInstance.GetComponent<Inventory>().GiveItem(RoR2Content.Items.HealthDecay, 2));
            DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
          }
        }
      }
      orig(self);
    }

    private void WeaponSlamFixedUpdate(On.EntityStates.BrotherMonster.WeaponSlam.orig_FixedUpdate orig, EntityStates.BrotherMonster.WeaponSlam self)
    {
      if (shrineActivated)
      {
        if (self.isAuthority)
        {
          if (self.hasDoneBlastAttack)
          {
            Logger.LogDebug("blast attack done");
            if (self.modelTransform)
            {
              if (hasfired == false)
              {
                hasfired = true;
                Logger.LogDebug("modeltransformed");
                if (PhaseCounter.instance)
                {
                  int orbCount = (spawnedClone || PhaseCounter.instance.phase == 3) ? ModConfig.SlamOrbProjectileCount.Value / 2 : ModConfig.SlamOrbProjectileCount.Value;
                  float num = 360f / orbCount;
                  Vector3 xAxis = Vector3.ProjectOnPlane(self.characterDirection.forward, Vector3.up);
                  Transform transform2 = self.FindModelChild(WeaponSlam.muzzleString);
                  Vector3 position = transform2.position;
                  for (int i = 0; i < orbCount; i++)
                  {
                    Vector3 forward = Quaternion.AngleAxis(num * i, Vector3.up) * xAxis;
                    ProjectileManager.instance.FireProjectile(FistSlam.waveProjectilePrefab, position, Util.QuaternionSafeLookRotation(forward), self.gameObject, self.characterBody.damage * FistSlam.waveProjectileDamageCoefficient, FistSlam.waveProjectileForce, Util.CheckRoll(self.characterBody.crit, self.characterBody.master), DamageColorIndex.Default, null, -1f);
                  }
                }
              }
            }
          }
        }
      }
      orig(self);
    }

    private void FireLunarShardsOnEnter(On.EntityStates.BrotherMonster.Weapon.FireLunarShards.orig_OnEnter orig, FireLunarShards self)
    {
      if (shrineActivated)
      {
        if (self is FireLunarShardsHurt)
        {
          self.duration = FireLunarShards.baseDuration / self.attackSpeedStat;
          if (self.isAuthority)
          {
            Ray aimRay = self.GetAimRay();
            Transform modelChild = self.FindModelChild(FireLunarShards.muzzleString);
            if ((bool)(UnityEngine.Object)modelChild)
              aimRay.origin = modelChild.position;
            aimRay.direction = Util.ApplySpread(aimRay.direction, 0.0f, self.maxSpread, self.spreadYawScale, self.spreadPitchScale);
            for (int i = 0; i < ModConfig.SuperShardWeight.Value; i++)
            {
              int num = (int)Util.PlaySound(FireLunarShards.fireSound, self.gameObject);
              ProjectileManager.instance.FireProjectile(new FireProjectileInfo()
              {
                position = aimRay.origin,
                rotation = Quaternion.LookRotation(aimRay.direction),
                crit = self.characterBody.RollCrit(),
                damage = self.characterBody.damage * self.damageCoefficient,
                damageColorIndex = DamageColorIndex.Default,
                owner = self.gameObject,
                procChainMask = new ProcChainMask(),
                force = 0.0f,
                useFuseOverride = false,
                useSpeedOverride = false,
                target = (GameObject)null,
                projectilePrefab = FireLunarShards.projectilePrefab
              });
            }
          }
          self.PlayAnimation("Gesture, Additive", nameof(FireLunarShards));
          self.PlayAnimation("Gesture, Override", nameof(FireLunarShards));
          self.AddRecoil(-0.4f * FireLunarShards.recoilAmplitude, -0.8f * FireLunarShards.recoilAmplitude, -0.3f * FireLunarShards.recoilAmplitude, 0.3f * FireLunarShards.recoilAmplitude);
          self.characterBody.AddSpreadBloom(FireLunarShards.spreadBloomValue);
          EffectManager.SimpleMuzzleFlash(FireLunarShards.muzzleFlashEffectPrefab, self.gameObject, FireLunarShards.muzzleString, false);
        }
        else
        {
          if (!(self is FireLunarShardsHurt))
          {
            if (self.isAuthority)
            {
              Ray aimRay = self.GetAimRay();
              Transform transform = self.FindModelChild(FireLunarShards.muzzleString);
              if (transform)
              {
                aimRay.origin = transform.position;
              }
              FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
              fireProjectileInfo.position = aimRay.origin;
              fireProjectileInfo.rotation = Quaternion.LookRotation(aimRay.direction);
              fireProjectileInfo.crit = self.characterBody.RollCrit();
              fireProjectileInfo.damage = self.characterBody.damage * self.damageCoefficient;
              fireProjectileInfo.damageColorIndex = DamageColorIndex.Default;
              fireProjectileInfo.owner = self.gameObject;
              fireProjectileInfo.procChainMask = default(ProcChainMask);
              fireProjectileInfo.force = 0f;
              fireProjectileInfo.useFuseOverride = false;
              fireProjectileInfo.useSpeedOverride = false;
              fireProjectileInfo.target = null;
              fireProjectileInfo.projectilePrefab = FireLunarShards.projectilePrefab;

              for (int i = 0; i < ModConfig.LunarShardAdd.Value; i++)
              {
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                aimRay.direction = Util.ApplySpread(aimRay.direction, 0f, self.maxSpread * (1f + 0.45f * i), self.spreadYawScale * (1f + 0.45f * i), self.spreadPitchScale * (1f + 0.45f * i), 0f, 0f);
                fireProjectileInfo.rotation = Quaternion.LookRotation(aimRay.direction);
              }
            }
          }
          orig(self);
        }
      }
      else
        orig(self);
    }

    private void SpellChannelEnterStateOnEnter(On.EntityStates.BrotherMonster.SpellChannelEnterState.orig_OnEnter orig, SpellChannelEnterState self)
    {
      if (shrineActivated)
        self.outer.SetNextState(new EntityStates.BrotherMonster.SpellChannelExitState());
      else
        orig(self);
    }

    private void SpellChannelExitStateOnEnter(On.EntityStates.BrotherMonster.SpellChannelExitState.orig_OnEnter orig, SpellChannelExitState self)
    {
      if (shrineActivated)
      {
        SpellChannelExitState.lendInterval = 0.04f;
        SpellChannelExitState.duration = 2.5f;
      }
      orig(self);
    }

    private void StaggerEnterOnEnter(On.EntityStates.BrotherMonster.StaggerEnter.orig_OnEnter orig, StaggerEnter self)
    {
      if (shrineActivated)
        self.duration = 0.0f;
      orig(self);
    }

    private void StaggerExitOnEnter(On.EntityStates.BrotherMonster.StaggerExit.orig_OnEnter orig, StaggerExit self)
    {
      if (shrineActivated)
        self.duration = 0.0f;
      orig(self);
    }

    private void StaggerLoopOnEnter(On.EntityStates.BrotherMonster.StaggerLoop.orig_OnEnter orig, StaggerLoop self)
    {
      if (shrineActivated)
        self.duration = 0.0f;
      orig(self);
    }

    private void TrueDeathStateOnEnter(On.EntityStates.BrotherMonster.TrueDeathState.orig_OnEnter orig, TrueDeathState self)
    {
      if (shrineActivated)
        TrueDeathState.dissolveDuration = 3f;
      orig(self);
    }

    private void CleanupPillar(On.EntityStates.BrotherMonster.WeaponSlam.orig_OnEnter orig, EntityStates.BrotherMonster.WeaponSlam self)
    {
      if (shrineActivated)
      {
        GameObject projectilePrefab = EntityStates.BrotherMonster.WeaponSlam.pillarProjectilePrefab;
        projectilePrefab.transform.localScale = new Vector3(1f, 1f, 1f);
        projectilePrefab.GetComponent<ProjectileController>().ghostPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
      }
      orig(self);
    }
  }
}
