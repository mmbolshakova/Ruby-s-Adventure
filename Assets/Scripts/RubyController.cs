using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
	public int maxHealth = 5;
	public int health { get { return currentHealth; } }
	int currentHealth;
	public float speed = 3.0f;
	public float timeInvincible = 2.0f;
	bool isInvincible;
	float invincibleTimer;
	Animator animator;
	Vector2 lookDirection = new Vector2(1, 0);
	public GameObject projectilePrefab;
	public AudioClip throwSound;
	public AudioClip hitSound;

	Rigidbody2D rigidbody2d;
	AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
		animator = GetComponent<Animator>();
		rigidbody2d = GetComponent<Rigidbody2D>();
		currentHealth = maxHealth;
		audioSource = GetComponent<AudioSource>();
	}
	public void PlaySound(AudioClip clip)
	{
		audioSource.PlayOneShot(clip);
	}

    // Update is called once per frame
    void Update()
    {
		
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		Vector2 move = new Vector2(horizontal, vertical);

		animator.SetFloat("Look X", lookDirection.x);
		animator.SetFloat("Look Y", lookDirection.y);
		animator.SetFloat("Speed", move.magnitude);

		if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
		{
			lookDirection.Set(move.x, move.y);
			lookDirection.Normalize();
		}
		animator.SetFloat("Look X", lookDirection.x);
		animator.SetFloat("Look Y", lookDirection.y);
		animator.SetFloat("Speed", move.magnitude);
		Vector2 position = rigidbody2d.position;
		position = position + move * speed * Time.deltaTime;
		rigidbody2d.MovePosition(position);

		if (isInvincible)
		{
			invincibleTimer -= Time.deltaTime;
			if (invincibleTimer < 0)
				isInvincible = false;
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			Launch();
			
		}

		if (Input.GetKeyDown(KeyCode.X))
		{
			RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
			if (hit.collider != null)
			{
				NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
				if (character != null)
				{
					character.DisplayDialog();
				}
			}
		}

	}

	public void ChangeHealth(int amount)
	{
		if (amount < 0)
		{
			if (isInvincible)
				return;
			isInvincible = true;
			invincibleTimer = timeInvincible;
			PlaySound(hitSound);
		}
		currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
		UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
	}

	void Launch()
	{
		GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

		Projectile projectile = projectileObject.GetComponent<Projectile>();
		projectile.Launch(lookDirection, 300);

		animator.SetTrigger("Launch");
		PlaySound(throwSound);
	}
}
