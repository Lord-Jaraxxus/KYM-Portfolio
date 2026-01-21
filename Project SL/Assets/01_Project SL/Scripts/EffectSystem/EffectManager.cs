using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    [System.Serializable]
    public class EffectData 
    {
        public string key;
        public float lifeTIme = 5f;
        public GameObject prefab;
    }

    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance;

        private void Awake() => Instance = this;
        private void OnDestroy() => Instance = null;

        [SerializeField] private List<EffectData> effectList = new List<EffectData>();

        public void SpawnEffect(string key, Vector3 position, Quaternion rotation)
        {
            var targetEffectData = effectList.Find(e => e.key == key);
            if (targetEffectData == null)
                return;

            var newEffect = Instantiate(targetEffectData.prefab, position, rotation);
            Destroy(newEffect, targetEffectData.lifeTIme);
        }
    }
}
