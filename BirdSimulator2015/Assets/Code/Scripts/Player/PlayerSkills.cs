using UnityEngine;
using System.Collections;
using BirdSimulator2015.Code.Scripts.Cam;

namespace BS2015.Code.Scripts.Player
{
	public class PlayerSkills : MonoBehaviour {

		const float BLINK_DISTANCE = 20.0f;
		const float DASH_FORCE = 100.0f;
		const float TAUNT_DISTANCE = 50.0f;

		const float BLINK_COOLDOWN = 3.0f;
		const float DASH_COOLDOWN = 3.0f;
		const float TAUNT_COOLDOWN = 3.0f;

		//Cooldowns
		private float blinkCooldown = 0.0f;
		private float dashCooldown = 0.0f;

		//cooldown for taunt/gust effect
		private float tauntCooldown = 0.0f;

		private ParticleSystem particleSystem;
		private Animator animator;

		void Awake()
		{
			animator = (Animator)GetComponent(typeof(Animator));
			if (!animator || !animator.runtimeAnimatorController)
			{
				Debug.LogError("fuck you");
			}

			//Get the particle system from the child object
			particleSystem = this.GetComponentInChildren<ParticleSystem> ();
		}
	
		public void Blink()
		{
			RaycastHit hit;
			if (!Physics.CapsuleCast (this.transform.position + (this.transform.right), this.transform.position - (this.transform.right), 5, this.transform.forward, out hit, BLINK_DISTANCE)) 
			{
				StartCoroutine (AnimateBlink());
				blinkCooldown = Time.time + BLINK_COOLDOWN;
			}
		}

		public void Dash()
		{
			this.rigidbody.velocity += this.transform.forward * DASH_FORCE;
			dashCooldown = Time.time + DASH_COOLDOWN;
		}

		public void Taunt()
		{
			if(Time.time > tauntCooldown)
			{
				tauntCooldown = Time.time + TAUNT_COOLDOWN;
				GameObject taunt_effect = Instantiate(Resources.Load("Player/TauntEffect"),this.transform.position, Quaternion.identity) as GameObject;
				Destroy (taunt_effect, 0.5f);
				//Call overlap shpere
				Collider[] colliders = Physics.OverlapSphere (this.transform.position, TAUNT_DISTANCE);
				if (colliders.Length > 0) 
				{
					for(int i = 0; i < colliders.Length; i++)
					{
						Tauntable[] tauntables = colliders[i].gameObject.GetComponentsInChildren<Tauntable>();

						foreach(Tauntable taunted in tauntables)
						{
							taunted.Taunted(this.gameObject);
						}
					}
				}

			}
		}

		public void GustOfWind()
		{
			if (Time.time > tauntCooldown) 
			{
				tauntCooldown = Time.time + TAUNT_COOLDOWN;
			}
		}
		void ToggleRenderer()
		{
			SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
			for(int i = 0; i < renderers.Length; i++)
			{
				renderers[i].enabled = !renderers[i].enabled;
			}
		}

		IEnumerator AnimateBlink() 
		{
			if (!particleSystem.isPlaying) 
			{
				particleSystem.Play();
				ToggleRenderer();
			}
			
			yield return new WaitForSeconds(0.2f);
			
			this.transform.position = this.transform.position + (this.transform.forward * BLINK_DISTANCE);
			
			particleSystem.Stop ();
			
			//Toggle back on the renderer
			ToggleRenderer();

		}
	}
}
