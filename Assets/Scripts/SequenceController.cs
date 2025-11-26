using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SequenceController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject playerCameraObject;

    [Header("Subsystems")]
    [SerializeField] private HorrorLightingController lights;
    [SerializeField] private AudioController music;
    [SerializeField] private MonsterController monsters;
    [SerializeField] private DoorRegistry doors;
    [SerializeField] private HorrorRadioController horrorRadioController;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Environment")]
    [SerializeField] private GameObject blood;

    private void Start()
    {
        SceneSetup();
        StartCoroutine(RunSequence());
    }

    private void SceneSetup()
    {
        ResetScene();
    }

    private void ResetScene()
    {
        music.StopMusic();
        lights.SetAllLightIntensity(0f);
        lights.SetPlayerLightSound(false, ResourceManager.Instance.lightBuzz);
        lights.SetAllPlayerLightColor(Color.white);
        monsters.dad.gameObject.transform.position = monsters.dadPos.transform.position;
        monsters.dad.gameObject.transform.localEulerAngles = monsters.dadPos.localEulerAngles;
        monsters.dad.SetAnimTrigger("Idle");
        monsters.giantHead.SetHappy(0f);
        blood.gameObject.SetActive(false);
    }

    // ============================================================
    // MAIN SEQUENCE
    // ============================================================

    private IEnumerator RunSequence()
    {
        if (backgroundMusic != null)
            music.PlayMusic(backgroundMusic, 0.6f);

        yield return Segment_Intro();
        yield return Segment_RadioTalk();
        yield return Segment_DadDoorPeek();
        yield return Segment_RadioDialogue();
        yield return Segment_DaughterReveal();
        yield return Segment_GiantHead();
        yield return Segment_FinalAttack();

        Debug.Log("Sequence Complete");
        ResetScene();

        // Optional delay before restart
        yield return new WaitForSeconds(1f);

        // Restart entire sequence
        StartCoroutine(RunSequence());
    }

    // ============================================================
    // SEGMENTS
    // ============================================================

    private IEnumerator Segment_Intro()
    {
        yield return lights.PlayerFlickerRoutine(2f);

        lights.SetAllPlayerLightColor(Color.white);
        lights.SetPlayerLightIntensity(1f);
        lights.SetPlayerLightSound(true, ResourceManager.Instance.lightBuzz);

        yield return null;
    }

    private IEnumerator Segment_RadioTalk()
    {
        //Need to play the radio audio clip 
        yield return WaitForLook(horrorRadioController.transform, 3f);

        yield return horrorRadioController.PlayRoutine(ResourceManager.Instance.radioStatic); 
        yield return horrorRadioController.PlayRoutine(ResourceManager.Instance.radioTalk); 
        yield return new WaitForSeconds(10.0f);
    }

    private IEnumerator Segment_DadDoorPeek()
    {
        // Rattle the side door
        doors.SideDoor.PlayOneShot(ResourceManager.Instance.doorRattle);
        yield return doors.SideDoor.RattleRoutine();
        yield return new WaitForSeconds(5f);

        // Dad peek
        lights.SetNPCLightIntensity(0, 1f);
        lights.SetNPCLightColor(0, Color.red);

        doors.DadDoor.PlayOneShot(ResourceManager.Instance.doorCreak);
        yield return doors.DadDoor.HalfOpenRoutine(0.8f, Ease.InOutQuad);
        yield return new WaitForSeconds(2f);

        // Dad growling loop
        monsters.dad.PlayLoop(ResourceManager.Instance.dadGrowling, 0.5f);

        // Wait until player looks at DadDoor for 3 sec
        yield return WaitForLook(doors.DadDoor.transform, 3f);

        // Transition out of growling
        monsters.dad.StopLoop();

        monsters.dad.PlayOneShot(ResourceManager.Instance.dadSeeYou);
        yield return new WaitForSeconds(ResourceManager.Instance.dadSeeYou.length + 1f);

        // Slam door shut
        yield return doors.DadDoor.SlamRoutine(0.5f, Ease.InBack);
        doors.DadDoor.PlayOneShot(ResourceManager.Instance.doorShut);
        lights.SetNPCLightIntensity(0, 0f);

        yield return new WaitForSeconds(5f);
    }

    private IEnumerator Segment_RadioDialogue()
    {
        // Sequence of radio lines
        yield return horrorRadioController.PlayRoutine(ResourceManager.Instance.radioStatic);
        yield return horrorRadioController.PlayRoutine(ResourceManager.Instance.daughterDadWhy);
        yield return horrorRadioController.PlayRoutine(ResourceManager.Instance.radioStatic);
        yield return horrorRadioController.PlayRoutine(ResourceManager.Instance.daughterNotADemon);

        yield return new WaitForSeconds(3f);
    }

    private IEnumerator Segment_DaughterReveal()
    {
        // Flicker + Reset
        yield return lights.PlayerFlickerRoutine(2f);

        lights.SetAllLightIntensity(0f);
        lights.SetPlayerLightSound(false, ResourceManager.Instance.lightBuzz);

        // Red light behind daughter
        lights.SetNPCLightIntensity(1, 1f);
        lights.SetNPCLightColor(1, Color.red);

        monsters.daughter.PlayLoop(ResourceManager.Instance.daughterCry, 0.5f);

        // Wait until player looks at DaughterDoor
        yield return WaitForLook(doors.DaughterDoor.transform, 3f);

        monsters.daughter.StopLoop();

        yield return new WaitForSeconds(0.5f);

        // Door opens to reveal daughter
        doors.DaughterDoor.PlayOneShot(ResourceManager.Instance.doorCreak);
        yield return doors.DaughterDoor.OpenRoutine(2.5f, Ease.InOutQuad);

        // Start scary ambience
        music.PlayMusic(ResourceManager.Instance.scaryAmbience, 0.2f);

        yield return new WaitForSeconds(7f);

        // Slam shut
        yield return doors.DaughterDoor.SlamRoutine(0.5f, Ease.InBack);
        doors.DaughterDoor.PlayOneShot(ResourceManager.Instance.doorShut);

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator Segment_GiantHead()
    {
        lights.SetPlayerLightIntensity(.7f);

        monsters.giantHead.StartTrackPlayer(playerCameraObject.transform);
        doors.SideDoor.StartRattleLoop();
        yield return WaitForLook(doors.SideDoor.transform, 3f);
        doors.SideDoor.StopRattleLoop();

        doors.SideDoor.PlayOneShot(ResourceManager.Instance.doorCreak);
        yield return doors.SideDoor.OpenRoutine(1f, Ease.OutQuad);

        yield return new WaitForSeconds(1f);

        music.StopMusic();

        monsters.giantHead.PlayOneShot(ResourceManager.Instance.headSomeone);
        yield return new WaitForSeconds(ResourceManager.Instance.headSomeone.length);

        yield return monsters.giantHead.SlideHappy(100f, 2f);

        yield return new WaitForSeconds(2f);

        // Slam shut
        yield return doors.SideDoor.SlamRoutine(0.5f, Ease.InBack);
        doors.SideDoor.PlayOneShot(ResourceManager.Instance.doorShut);
        monsters.giantHead.StopTrackPlayer();

        lights.SetAllLightIntensity(0f);
    }

    private IEnumerator Segment_FinalAttack()
    {
        monsters.dad.SetAnimTrigger("Bite");

        yield return new WaitForSeconds(4f);

        Vector3 basePos = playerCameraObject.transform.position +
                  playerCameraObject.transform.forward * 0.5f;

        basePos.y = monsters.dad.transform.position.y + 0.1f;

        monsters.dad.transform.position = basePos;

        Vector3 lookPos = playerCameraObject.transform.position;
        lookPos.y = monsters.dad.transform.position.y;
        monsters.dad.transform.LookAt(lookPos);

        yield return new WaitForEndOfFrame();

        Color mixed = Color.Lerp(Color.white, Color.red, 0.5f);
        lights.SetAllLightColor(mixed);
        lights.SetAllLightIntensity(.7f);

        monsters.dad.PlayLoop(ResourceManager.Instance.dadGrowling, 0.5f);

        blood.gameObject.SetActive(true);
        music.PlayMusic(ResourceManager.Instance.scaryAmbience, 0.5f);


        yield return new WaitForSeconds(30f);

        monsters.dad.StopLoop();
        monsters.dad.SetAnimTrigger("Walk");
        monsters.dad.gameObject.transform.position = monsters.dadPos.transform.position;
        monsters.dad.gameObject.transform.localEulerAngles = monsters.dadPos.localEulerAngles;
    }

    // ============================================================
    // UTILITIES
    // ============================================================

    private bool IsPlayerLookingAt(Transform target, float thresholdDot = 0.7f)
    {
        Vector3 forward = playerCameraObject.transform.forward;
        Vector3 toTarget = (target.position - playerCameraObject.transform.position).normalized;

        float dot = Vector3.Dot(forward, toTarget);

        //Debug.Log($"[LOOK CHECK] dot={dot:F3}, threshold={thresholdDot:F3}, result={(dot > thresholdDot)}");

        return dot > thresholdDot;
    }

    private IEnumerator WaitForLook(Transform target, float seconds)
    {
        float t = 0f;

        while (t < seconds)
        {
            if (IsPlayerLookingAt(target))
                t += Time.deltaTime;
            else
                t = 0f;

            yield return null;
        }
    }
}
