using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerCollision : MonoBehaviour
{
  public GameObject character1;
  public GameObject character2;
//  public GameObject character3;
  public static bool hitFinishLine;

  public GameObject camera;
  public MovePB moveHuman;
  public MoveChicken moveChicken;
  public MoveDragon moveDragon;

  public AudioClip gem_collect;
  public AudioClip chicken_squawk;
  public AudioClip potion_hit;

  void Start()
  {
    // Set the active player to player 2 for now for transform of spawn to work
    if (character1.activeSelf)
      transform.parent.gameObject.GetComponent<Spawn>().setActivePlayer(character1);
    else if (character2.activeSelf)
      transform.parent.gameObject.GetComponent<Spawn>().setActivePlayer(character2);
//    else if (character3.activeSelf)
//      transform.parent.gameObject.GetComponent<Spawn>().setActivePlayer(character3);
    hitFinishLine = false;
  }

  void OnCollisionEnter(Collision collision)
  {
    print($"collision occured with {collision.collider.name}");

    if (collision.collider.CompareTag("Gem")) {
             GetComponent<AudioSource>().clip = gem_collect;
             GetComponent<AudioSource>().Play();
    }
    
    if (collision.collider.CompareTag("Potion")) {

      collision.collider.gameObject.GetComponent<potionCollision>().Explode();
      GetComponent<AudioSource>().clip = potion_hit;
      GetComponent<AudioSource>().Play();

      if (character1.activeSelf) {
        print("Changing to character 2");
        GetComponent<AudioSource>().clip = chicken_squawk;
        GetComponent<AudioSource>().Play();
        character2.transform.position = character1.transform.position;
        character2.transform.rotation = character1.transform.rotation;
        character1.SetActive(false);
        character2.SetActive(true);
        transform.parent.gameObject.GetComponent<Spawn>().setActivePlayer(character2);
        camera.GetComponent<CameraController>().PlayerTransform = character2.transform.Find("Focus");

      } else if (character2.activeSelf) {
        print("Changing to character 3");
        character1.transform.position = character2.transform.position;
        character1.transform.rotation = character2.transform.rotation;
        character2.SetActive(false);
        character1.SetActive(true);
        transform.parent.gameObject.GetComponent<Spawn>().setActivePlayer(character1);
        camera.GetComponent<CameraController>().PlayerTransform = character1.transform.Find("Focus");
        }
//      else if (character3.activeSelf) {
//        print("Changing to character 1");
//        character1.transform.position = character3.transform.position;
//        character1.transform.rotation = character3.transform.rotation;
//        character3.SetActive(false);
//        character1.SetActive(true);
//        transform.parent.gameObject.GetComponent<Spawn>().setActivePlayer(character1);
//        camera.GetComponent<CameraController>().PlayerTransform = character1.transform.Find("Focus");
//      }
    }

    if (collision.collider.CompareTag("FinishLine")) {
      hitFinishLine = true;
    }
  }
}
