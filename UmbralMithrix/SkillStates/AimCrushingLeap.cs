using EntityStates.Huntress;
using KinematicCharacterController;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using EntityStates;

namespace UmbralMithrix
{
  public class AimCrushingLeap : BaseSkillState
  {
    public static GameObject areaIndicatorPrefab = ArrowRain.areaIndicatorPrefab;
    public float maxDuration = ModConfig.CrushingLeap.Value;
    public float stopwatch;
    private CharacterModel characterModel;
    private HurtBoxGroup hurtboxGroup;
    private GameObject areaIndicatorInstance;
    static Material tpMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Teleporters/matTeleporterRangeIndicator.mat").WaitForCompletion();
    static Material awShellExpolsionMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/artifactworld/matArtifactShellExplosionIndicator.mat").WaitForCompletion();

    public override void OnEnter()
    {
      base.OnEnter();
      Transform modelTransform = this.GetModelTransform();
      if ((bool)(Object)modelTransform)
      {
        this.characterModel = modelTransform.GetComponent<CharacterModel>();
        this.hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
      }
      if ((bool)(Object)this.characterModel)
        ++this.characterModel.invisibilityCount;
      if ((bool)(Object)this.hurtboxGroup)
        ++this.hurtboxGroup.hurtBoxesDeactivatorCounter;
      this.characterBody.AddBuff((BuffIndex)3);
      int num = (int)Util.PlaySound("Play_voidRaid_snipe_shoot_final", this.gameObject);
      this.gameObject.layer = LayerIndex.fakeActor.intVal;
      ((BaseCharacterController)this.characterMotor).Motor.RebuildCollidableLayers();
      this.characterMotor.velocity = Vector3.zero;
      ((BaseCharacterController)this.characterMotor).Motor.SetPosition(new Vector3(((Component)this.characterMotor).transform.position.x, ((Component)this.characterMotor).transform.position.y + 25f, ((Component)this.characterMotor).transform.position.z), true);
      if (!(bool)(Object)AimCrushingLeap.areaIndicatorPrefab)
        return;
      this.areaIndicatorInstance = Object.Instantiate<GameObject>(AimCrushingLeap.areaIndicatorPrefab);
      this.areaIndicatorInstance.transform.localScale = new Vector3(ArrowRain.arrowRainRadius, ArrowRain.arrowRainRadius, ArrowRain.arrowRainRadius);
      this.areaIndicatorInstance.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = tpMat;
    }

    private void UpdateAreaIndicator()
    {
      if (!(bool)(Object)this.areaIndicatorInstance)
        return;
      float num = 2000f;
      Ray aimRay = ((BaseState)this).GetAimRay();
      RaycastHit raycastHit;
      double maxDistance = (double)num;
      LayerIndex world = LayerIndex.world;
      int mask = (int)world.mask;
      if (!Physics.Raycast(aimRay, out raycastHit, (float)maxDistance, mask))
        return;
      this.areaIndicatorInstance.transform.position = raycastHit.point;
      this.areaIndicatorInstance.transform.up = raycastHit.normal;
    }

    public void HandleFollowupAttack()
    {
      this.characterMotor.Motor.SetPosition(new Vector3(this.areaIndicatorInstance.transform.position.x, this.areaIndicatorInstance.transform.position.y + 1f, this.areaIndicatorInstance.transform.position.z), true);
      this.outer.SetNextState(new ExitCrushingLeap());
    }

    public override void Update()
    {
      base.Update();
      this.UpdateAreaIndicator();
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      this.stopwatch += Time.fixedDeltaTime;
      if (this.stopwatch >= (this.maxDuration - 0.5f))
        this.areaIndicatorInstance.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = awShellExpolsionMat;
      if ((bool)(Object)this.characterMotor)
        this.characterMotor.velocity = Vector3.zero;
      if (!this.isAuthority || !(bool)(Object)this.inputBank || (double)this.fixedAge < (double)this.maxDuration && !((InputBankTest.ButtonState)this.inputBank.skill1).justPressed && !((InputBankTest.ButtonState)this.inputBank.skill4).justPressed)
        return;
      this.HandleFollowupAttack();
    }

    public override void OnExit()
    {
      if ((bool)(Object)this.characterModel)
        --this.characterModel.invisibilityCount;
      if ((bool)(Object)this.hurtboxGroup)
        --this.hurtboxGroup.hurtBoxesDeactivatorCounter;
      this.characterBody.RemoveBuff((BuffIndex)3);
      this.gameObject.layer = LayerIndex.defaultLayer.intVal;
      this.characterMotor.Motor.RebuildCollidableLayers();
      if ((bool)(Object)this.areaIndicatorInstance)
        EntityState.Destroy((Object)this.areaIndicatorInstance.gameObject);
      base.OnExit();
    }
  }
}
