using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D), typeof(Receptor))]
	internal sealed class DestructiveObject : StateController, Receptor.IReceptor, IDamageable
	{
		[Header("Destructive Object")]
		[SerializeField, Tooltip("If there a object that will be instantiate after the destruction of this.")] private GameObject _hiddenObject;
		[SerializeField, Tooltip("The vitality of this object before it destruction.")] private short _vitality;
		[SerializeField, Tooltip("The amount of damage that this object have to receive real damage.")] private short _biggerDamage;
		[SerializeField, Tooltip("The amount of stun that this object can resists.")] private float _stunResistance;
		[SerializeField, Tooltip("If this object will be destructed on collision with another object.")] private bool _destroyOnCollision;
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveObject;
		[SerializeField, Tooltip("If this object will saved on destruction or not.")] private bool _saveOnDestruction;
		private new void Awake()
		{
			base.Awake();
			SaveController.Load(out SaveFile saveFile);
			if (this._saveObject && saveFile.generalObjects.Contains(this.gameObject.name))
				Destroy(this.gameObject, 0.001f);
		}
		public void Execute()
		{
			if (this._hiddenObject)
				Instantiate(this._hiddenObject, this.transform.position, this._hiddenObject.transform.rotation);
			this.SaveObject();
			Destroy(this.gameObject);
		}
		private void SaveObject()
		{
			SaveController.Load(out SaveFile saveFile);
			if (this._saveObject && !saveFile.generalObjects.Contains(this.gameObject.name))
			{
				saveFile.generalObjects.Add(this.gameObject.name);
				SaveController.WriteSave(saveFile);
			}
		}
		private void DestroyOnCollision()
		{
			if (this._destroyOnCollision)
			{
				if (this._hiddenObject)
					Instantiate(this._hiddenObject, this.transform.position, this._hiddenObject.transform.rotation);
				this.SaveObject();
				Destroy(this.gameObject);
			}
		}
		private void OnCollisionEnter2D(Collision2D collision) => this.DestroyOnCollision();
		private void OnTriggerEnter2D(Collider2D collision) => this.DestroyOnCollision();
		public bool Damage(ushort damage)
		{
			if (this._vitality <= 0f)
				return false;
			if (damage < this._biggerDamage)
				return false;
			this._vitality -= (short)damage;
			if (this._vitality <= 0f)
			{
				if (this._hiddenObject)
					Instantiate(this._hiddenObject, this.transform.position, this._hiddenObject.transform.rotation);
				this.SaveObject();
				Destroy(this.gameObject);
			}
			return true;
		}
		public void Stun(float stunStength, float stunTime) { }
	};
};
